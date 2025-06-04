using System.Net;
using System.Text.RegularExpressions;
using Shared.Data.DbContext;
using Shared.Notifications;
using Shared.OpenAi.Entities;
using Shared.Users.Enums;
using SmartComponents.LocalEmbeddings;
using ProjectDto = Shared.Projects.DTOs.ProjectDto;

namespace Shared.Projects.Services;

public static class Engine
{
    public static List<(int ProjectId, EmbeddingF32 Embedding)>? SearchIndex;
}

public class ProjectService(IDbContextFactory<ApplicationDbContext> factory, IConfiguration configuration, IHttpClientFactory httpClientFactory, LocalEmbedder embedder, IEasyCachingProvider cache)
{
    private async Task AggregateAsync(int projectId, int applicationId, NotificationType notificationType, CancellationToken ct)
    {
        try
        {
            var emptyPayload = new {};
            var httpClient = httpClientFactory.CreateClient("api");
            
            var responseProject = await httpClient.PostAsJsonAsync($"/api/v1/jobs/aggregate/projects/" + projectId, emptyPayload, ct);
            responseProject.EnsureSuccessStatusCode();
            
            if (applicationId > 0)
            {
                var responseApplication = await httpClient.PostAsJsonAsync($"/api/v1/jobs/aggregate/applications/" + applicationId + "/" + notificationType, emptyPayload, ct);
                responseApplication.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"ex: {ex}");
        }
    }

    public async Task<Result<ProjectDto, Exception>> CreateProjectAsync(CreateProjectDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.OrganizationId == 0) throw new Exception("organizationId is required");
            if (string.IsNullOrWhiteSpace(dto.Number)) throw new Exception("number is required");

            var project = new Project
            {
                Organization = new ProjectOrganization{ OrganizationIdentifier = dto.OrganizationId },
                StatusId = 4,
                Number = dto.Number,
                Title = dto.Titles,
                Applications = [],
                ApplicationCount = 0,
                UpdatedDate = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow
            };

            await using var context = await factory.CreateDbContextAsync(ct);
            await context.Projects.AddAsync(project, ct);
            await context.SaveChangesAsync(ct);
            
            // Reset Caches
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            await AggregateAsync(project.Id, 0, NotificationType.Default, ct);

            return project.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateProjectHeaderAsync(ProjectHeaderDto dto, CancellationToken ct)
    {
        try
        {
            if (dto is null) throw new Exception("data is required");
            if (dto.Id == 0) throw new Exception("id is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var project = await context.Projects
                .AsTracking()
                .Where(x => x.Id == dto.Id)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Project not found");

            if (project.Title.Count > 0 && project.Title.First() != dto.Title)
            {
                project.Title.Insert(0, dto.Title);
                context.Projects.Update(project);
                await context.SaveChangesAsync(ct);
            }
            
            var organization = await context.Organizations
                .AsTracking()
                .Where(x => x.Id == project.Organization.OrganizationIdentifier)
                .FirstOrDefaultAsync(ct) ?? new Organization();

            if (organization.Id > 0)
            {
                organization.Name = dto.OrganizationName.Trim();
                
                var addresses = organization.Addresses.ToList();
                
                if (addresses.Count < 1)
                {
                    addresses.Add(new OrganizationAddress()
                    {
                        AddressIdentifier = 1,
                        Line1 = dto.OrganizationAddress.Trim(),
                        PostalCode = dto.OrganizationPostalCode.Trim(),
                        City = dto.OrganizationCity.Trim(),
                        Country = dto.OrganizationCountry.Trim()
                    });
                }
                else
                {
                    addresses[0].Line1 = dto.OrganizationAddress.Trim();
                    addresses[0].PostalCode = dto.OrganizationPostalCode.Trim();
                    addresses[0].City = dto.OrganizationCity.Trim();
                    addresses[0].Country = dto.OrganizationCountry.Trim();
                }
                
                organization.Addresses = addresses;
                
                context.Organizations.Update(organization);
                await context.SaveChangesAsync(ct);
            }
            
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.ProjectId == project.Id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(ct) ?? new Application();
            
            if (application.Id > 0)
            {
                var applicationTitle = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00001001"))?.Value;
                if (applicationTitle != null)
                {
                    var oldTitles = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("10000000"))?.Value;
                    if (oldTitles != null && dto.Title != applicationTitle)
                    {
                        application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("10000000"))!.Value = applicationTitle + ", " + oldTitles;
                    }
                    
                    application.Title = dto.Title;
                    application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00001001"))!.Value = dto.Title;
                }
                
                var producer = application.Producer;
                var applicant = application.Applicant;
                var projectManager = application.ProjectManager;
                
                producer.Name = dto.ProducerName != producer.Name ? dto.ProducerName.Trim() : producer.Name.Trim();
                producer.Email = dto.ProducerEmail != producer.Email ? dto.ProducerEmail.ToLower() : producer.Email.Trim();
                producer.PhoneNumber = dto.ProducerPhoneNumber != producer.PhoneNumber ? dto.ProducerPhoneNumber.Trim() : producer.PhoneNumber.Trim();
                
                applicant.Name = dto.ApplicantName != applicant.Name ? dto.ApplicantName.Trim() : applicant.Name.Trim();
                applicant.Email = dto.ApplicantEmail != applicant.Email ? dto.ApplicantEmail.ToLower() : applicant.Email.Trim();
                applicant.PhoneNumber = dto.ApplicantPhoneNumber != applicant.PhoneNumber ? dto.ApplicantPhoneNumber.Trim() : applicant.PhoneNumber.Trim();
                
                projectManager.Name = dto.ProjectManagerName != projectManager.Name ? dto.ProjectManagerName.Trim() : projectManager.Name.Trim();
                projectManager.Email = dto.ProjectManagerEmail != projectManager.Email ? dto.ProjectManagerEmail.ToLower() : projectManager.Email.Trim();
                projectManager.PhoneNumber = dto.ProjectManagerPhoneNumber != projectManager.PhoneNumber ? dto.ProjectManagerPhoneNumber.Trim() : projectManager.PhoneNumber.Trim();

                producer.ContactIdentifier = await AddOrUpdateUser(producer, ct);
                applicant.ContactIdentifier = await AddOrUpdateUser(applicant, ct);
                projectManager.ContactIdentifier = await AddOrUpdateUser(projectManager, ct);
                
                application.Producer = producer;
                application.Applicant = applicant;
                application.ProjectManager = projectManager;
                
                context.Applications.Update(application);
                await context.SaveChangesAsync(ct);
                
                Console.WriteLine($@"Producer: {application.Producer.ContactIdentifier} name: {application.Producer.Name} email: {application.Producer.Email} phone: {application.Producer.PhoneNumber}");
                Console.WriteLine($@"Applicant: {application.Applicant.ContactIdentifier} name: {application.Applicant.Name} email: {application.Applicant.Email} phone: {application.Applicant.PhoneNumber}");
                Console.WriteLine($@"ProjectManager: {application.ProjectManager.ContactIdentifier} name: {application.ProjectManager.Name} email: {application.ProjectManager.Email} phone: {application.ProjectManager.PhoneNumber}");
                
            }
            
            // Reset Caches
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);
            
            
            Console.WriteLine($"[UpdateProjectHeaderAsync] ProjectId: {dto.Id}");

            await AggregateAsync(project.Id, application.Id, NotificationType.ApplicationOverview, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<ProjectHeaderDto, Exception>> ProjectHeaderByIdAsync(int projectId, CancellationToken ct)
    {
        try
        {
            if (projectId == 0) throw new Exception("projectId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Projects.ToDescriptionString()}_ProjectHeaderByIdAsync_{projectId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var project = context.Projects
                        .AsNoTracking()
                        .TagWith("GetProjectHeaderById")
                        .FirstOrDefault(x => x.Id == projectId);
                    
                    if (project == null) throw new Exception("Project not found");

                    return new ProjectHeaderDto()
                    {
                        Id = project.Id,
                        Number = project.Number,
                        Title = project.Title.Count != 0 ? project.Title.First() : string.Empty,
                        OrganizationName = project.Organization.OrganizationName,
                        OrganizationAddress = project.Organization.OrganizationAddress,
                        OrganizationPostalCode = project.Organization.OrganizationPostalCode,
                        OrganizationCity = project.Organization.OrganizationCity,
                        OrganizationCountry = project.Organization.OrganizationCountry,
                        OrganizationUrl = project.Organization.OrganizationUrl,
                        ProducerName = project.Applications.Count != 0 ? project.Applications.Last().ApplicationProducer.Name : string.Empty,
                        ProducerPhoneNumber = project.Applications.Count != 0 ? project.Applications.Last().ApplicationProducer.PhoneNumber : string.Empty,
                        ProducerEmail = project.Applications.Count != 0 ? project.Applications.Last().ApplicationProducer.Email : string.Empty,
                        ApplicantName = project.Applications.Count != 0 ? project.Applications.Last().ApplicationApplicant.Name : string.Empty,
                        ApplicantPhoneNumber = project.Applications.Count != 0 ? project.Applications.Last().ApplicationApplicant.PhoneNumber : string.Empty,
                        ApplicantEmail = project.Applications.Count != 0 ? project.Applications.Last().ApplicationApplicant.Email : string.Empty,
                        ProjectManagerName = project.Applications.Count != 0 ? project.Applications.Last().ApplicationProjectManager.Name : string.Empty,
                        ProjectManagerPhoneNumber = project.Applications.Count != 0 ? project.Applications.Last().ApplicationProjectManager.PhoneNumber : string.Empty,
                        ProjectManagerEmail = project.Applications.Count != 0 ? project.Applications.Last().ApplicationProjectManager.Email : string.Empty,
                        Logo = project.Organization.OrganizationLogo
                    };
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<ProjectDto, Exception>> ProjectByProjectNumberAsync(string projectNumber, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(projectNumber)) throw new Exception("projectNumber is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Projects.ToDescriptionString()}_ProjectByProjectNumberAsync_{projectNumber}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var project = await GetProjectByProjectNumber(context, projectNumber) ?? throw new Exception("Project not found");
                    return project.ToDto();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<List<ProjectDto>, Exception>> ProjectsByOrganizationIdAsync(int organizationId, CancellationToken ct)
    {
        try
        {
            if (organizationId == 0) throw new Exception("organizationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Projects.ToDescriptionString()}_ProjectsByOrganizationIdAsync_{organizationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var projects = await GetProjectsByOrganizationId(context, organizationId).ToListAsync(ct);
                    return projects.ToDto().ToList();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;

        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<List<ProjectSearchResultDto>> OpenAiSearchAsync(string query, int maxResults, CancellationToken ct)
    {
        try
        {
            var cacheValue = await cache.GetAsync(
                $"{CacheKeyPrefix.Projects.ToDescriptionString()}_OpenAiSearchAsync_{query}_{maxResults}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var sqlQuery = "";
                    var cacheResult = await context.OpenAiCacheItems.FirstOrDefaultAsync(x => x.Query.ToLower() == query.ToLower(), cancellationToken: ct);
                    if (cacheResult != null)
                    {
                        cacheResult.ReturnCount++;
                        await context.SaveChangesAsync(ct);
                        
                        sqlQuery = cacheResult.Answer;
                    }
                
                    if (sqlQuery == "")
                    {
                        var endpoint = configuration["OpenAi:Endpoint"];

                        if (string.IsNullOrWhiteSpace(endpoint))
                        {
                            throw new InvalidOperationException("❌ Konfigurationsvärdet 'OpenAi:Endpoint' är tomt eller saknas.");
                        }

                        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
                        {
                            throw new InvalidOperationException($"❌ Ogiltig URI i 'OpenAi:Endpoint': '{endpoint}'");
                        }
                        var client = httpClientFactory.CreateClient("openai");
                        var response = await client.PostAsJsonAsync($"/api/v1/translateSql", new TranslateSql() { Question = query }, cancellationToken: ct);
                        response.EnsureSuccessStatusCode();

                        var result = await response.Content.ReadAsStringAsync(ct);

                        context.OpenAiCacheItems.Add(new OpenAiCache()
                        {
                            Query = query,
                            Answer = result.Trim(),
                            ReturnCount = 1,
                            ExpireDate = DateTime.UtcNow.AddYears(50)
                        });
                        await context.SaveChangesAsync(ct);
                        
                        sqlQuery = result.Trim();
                    }
                    
                    if (sqlQuery == "") return [];
                    
                    var resultX = new List<int>();
                    await using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        Console.WriteLine("🔍 Kör SQL från OpenAI:");
                        Console.WriteLine(sqlQuery);
                        Console.WriteLine("---------------------------");

                        command.CommandText = sqlQuery;
                        command.CommandType = CommandType.Text;

                        await context.Database.OpenConnectionAsync(cancellationToken: ct);

                        await using var reader = await command.ExecuteReaderAsync(ct);
                        int rowCount = 0;

                        while (await reader.ReadAsync(ct))
                        {
                            rowCount++;
                            var value = reader.GetValue(0);
                            var fieldType = reader.GetFieldType(0);

                            Console.WriteLine($"🔸 Rad {rowCount}: Typ = {fieldType}, Värde = {value}");

                            if (value is int id)
                            {
                                resultX.Add(id);
                            }
                            else if (int.TryParse(value?.ToString(), out var parsed))
                            {
                                resultX.Add(parsed);
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ Kunde inte konvertera värde till int: {value} ({value?.GetType()})");
                            }
                        }

                        Console.WriteLine($"📥 Antal ID:n extraherade: {resultX.Count}");
                        Console.WriteLine($"📋 Lista med ID:n: {string.Join(", ", resultX)}");

                        await reader.CloseAsync();
                    }

                    // Kontroll om vi fick något vettigt tillbaka
                    if (resultX.Count < 1)
                    {
                        Console.WriteLine("⚠️ Inga projekt-ID:n hittades i SQL-responsen.");
                        return [];
                    }

                    var openAiProjects = new List<OpenAiProject>();

                    var projects = await context.OpenAiProjects
                        .AsNoTracking()
                        .Where(x => resultX.Contains(x.Id))
                        .ToDictionaryAsync(x => x.Id, cancellationToken: ct);

                    Console.WriteLine($"📦 Antal projekt laddade från DB: {projects.Count}");

                    foreach (var id in resultX)
                    {
                        if (!projects.TryGetValue(id, out var proj))
                        {
                            Console.WriteLine($"❌ Projekt-ID {id} saknas i dictionaryn.");
                            continue;
                        }

                        Console.WriteLine($"✅ Lägger till projekt-ID {id} - {proj.ProjectTitle}");
                        openAiProjects.Add(proj);
                    }

                    
                    //openAiProjects = resultX.Select(x => projects[x]).ToList();
                    
                    var gridItems = new List<ProjectSearchResultDto>();
                    foreach (var openAiProject in openAiProjects.DistinctBy(x => x.ProjectId))
                    {
                        var applications = await ProjectApplicationSearchResult(context, openAiProject.ProjectId).ToListAsync(cancellationToken: ct);
                        
                        var titles = await GetProjectTitleById(context, openAiProject.ProjectId);
                        var title = titles is not null ? titles.FirstOrDefault() : openAiProject.ProjectTitle;
                        
                        gridItems.Add(new ProjectSearchResultDto()
                        {
                            Id = openAiProject.ProjectId,
                            Number = openAiProject.ProjectNumber,
                            Title = Regex.Unescape(title),
                            Applications = applications.ToProjectApplicationSearchResultDto().ToList(),
                            OrganizationName = openAiProject.CompanyName,
                            ProjectManager = openAiProject.ProjectManagerName
                        });
                    }

                    return gridItems;
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheValue.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return [];
        }
    }
    
    public async Task<List<ProjectSearchResultDto>> SemanticSearchAsync(string query, int maxResults, CancellationToken ct)
    {
        try
        {
            var cacheValue = await cache.GetAsync(
                $"{CacheKeyPrefix.Projects.ToDescriptionString()}_SemanticSearchAsync_{query}_{maxResults}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var openAiProjects = new List<OpenAiProject>();
        
                    if (query == "latest")
                    {
                        openAiProjects = await (GetLatestSearch(context, maxResults).ToListAsync());
                    }
                    else
                    {
                        var (closestIds, projects) = await FindClosest(query, maxResults);

                        try
                        {
                            openAiProjects = closestIds.Select(x => projects[x]).ToList();
                        }
                        catch
                        {
                            (closestIds, projects) = await FindClosest(query, maxResults);
                
                            openAiProjects = closestIds.Select(x => projects[x]).ToList();
                        }
                    }
        
                    var gridItems = new List<ProjectSearchResultDto>();
                    foreach (var openAiProject in openAiProjects.DistinctBy(x => x.ProjectId))
                    {
                        var applications = await ProjectApplicationSearchResult(context, openAiProject.ProjectId).ToListAsync(cancellationToken: ct);
                
                        if (applications.Count < 1) continue;
                
                        var index = 0;
                        foreach (var application in applications)
                        {
                            application.Index = index;
                            index++;
                        }
                        
                        var titles = await GetProjectTitleById(context, openAiProject.ProjectId);
                        var title = titles is not null ? titles.FirstOrDefault() : openAiProject.ProjectTitle;
                
                        gridItems.Add(new ProjectSearchResultDto()
                        {
                            Id = openAiProject.ProjectId,
                            Number = openAiProject.ProjectNumber,
                            Title = Regex.Unescape(title),
                            Applications = applications.ToProjectApplicationSearchResultDto().ToList(),
                            OrganizationName = openAiProject.CompanyName,
                            ProjectManager = openAiProject.ProjectManagerName,
                            LatestCreatedDate = applications.Max(x => x.CreatedDate)
                        });
                    }
        
                    return gridItems;
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheValue.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"ex: {ex}");
            return [];
        }
    }

    public async Task<List<ProjectSearchResultDto>> IdSearchAsync(List<int> projectIds, CancellationToken ct)
    {
        try
        {
            var cacheValue = await cache.GetAsync(
                $"{CacheKeyPrefix.Projects.ToDescriptionString()}_IdSearchAsync_{string.Join("_", projectIds)}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var openAiProjects = await (GetIdSearch(context, projectIds).ToListAsync());
                    var gridItems = new List<ProjectSearchResultDto>();
                    foreach (var openAiProject in openAiProjects.DistinctBy(x => x.ProjectId))
                    {
                        var applications = await ProjectApplicationSearchResult(context, openAiProject.ProjectId).ToListAsync();
                
                        if (applications.Count < 1) continue;
                
                        var index = 0;
                        foreach (var application in applications)
                        {
                            application.Index = index;
                            index++;
                        }
                        
                        var titles = await GetProjectTitleById(context, openAiProject.ProjectId);
                        var title = titles is not null ? titles.FirstOrDefault() : openAiProject.ProjectTitle;
                
                        gridItems.Add(new ProjectSearchResultDto()
                        {
                            Id = openAiProject.ProjectId,
                            Number = openAiProject.ProjectNumber,
                            Title = title ?? openAiProject.ProjectTitle,
                            Applications = applications.ToProjectApplicationSearchResultDto().ToList(),
                            OrganizationName = openAiProject.CompanyName,
                            ProjectManager = openAiProject.ProjectManagerName,
                            LatestCreatedDate = applications.Max(x => x.CreatedDate)
                        });
                    }
        
                    return gridItems;
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheValue.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"ex: {ex}");
            return [];
        }
    }

    
    
    
    private async Task<int> AddOrUpdateUser(ApplicationContact person, CancellationToken ct)
    {
        try
        {
            var userId = 0;
            var name = person.Name.Trim().Split(' ');
            var email = person.Email.Trim();

            if (!email.IsValidEmailAddress()) return person.ContactIdentifier;

            await using var context = await factory.CreateDbContextAsync(ct);
            var user = await context.Users
                .AsTracking()
                .Where(x => x.Email!.ToLower() == email.ToLower())
                .FirstOrDefaultAsync(ct);

            if (user != null)
            {
                user.Email = email;
                user.FirstName = name.Length > 0 ? name[0].Trim() : person.Name.Trim();
                user.LastName = name.Length > 1 ? name[1].Trim() : "";
                user.UserName = email;
                user.NormalizedEmail = email.ToUpper();
                user.NormalizedUserName = email.ToUpper();
                user.PhoneNumber = person.PhoneNumber.Trim();
                
                context.Users.Update(user);
                await context.SaveChangesAsync(ct);
                
                userId = user.Id;
            }
            else
            {
                user = new User()
                {
                    StatusId = 2,
                    Type = UserType.Client,
                    Email = email,
                    EmailConfirmed = false, 
                    FirstName = name.Length > 0 ? name[0] : person.Name,
                    LastName = name.Length > 1 ? name[1] : "",
                    UserName = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(), 
                    PhoneNumber = person.PhoneNumber.Trim(),
                    Organizations = [],
                    SecurityStamp = Guid.NewGuid().ToString(),
                    VisibleApplicationTypes = [],
                    LastLoginDate = [],
                    LastProject = [],
                    Statistics = [] 
                };
                
                context.Users.Add(user);
                await context.SaveChangesAsync(ct);
                
                userId = user.Id;
            }

            return userId;
        }
        catch
        {
            return 0;
        }
    }

    private async Task<(List<int> closestIds, Dictionary<int, OpenAiProject> projects)> FindClosest(string query, int maxResults)
    {
        Engine.SearchIndex = await PrepareIndexAsync();
        
        var searchEmbedding = embedder.Embed<EmbeddingF32>(query);

        await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
        var closestIds = LocalEmbedder.FindClosest(searchEmbedding, Engine.SearchIndex, maxResults).ToList();
        var projects = await context.OpenAiProjects
            .AsNoTracking()
            .Where(x => closestIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);
        //Console.WriteLine();
        //Console.WriteLine("closestIds");
        var openAiProjects = closestIds.Select(x => projects[x]).ToList();
        foreach (var project in openAiProjects.DistinctBy(x => x.ProjectId))
        {
            //Console.WriteLine($"project: {project.ProjectId} title: {project.ProjectTitle}");
        }
        //Console.WriteLine();
        
        return (closestIds, projects);
    }

    private async Task<List<(int ProjectId, EmbeddingF32 Embedding)>> PrepareIndexAsync()
    {
        await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
        var projects = GetPrepareIndex(context);
        return (await projects.ToListAsync()).Select(x => (x.Id, new EmbeddingF32(x.Embedding))).ToList();;
    }
    
    public async Task ResetIndex()
    {
        Engine.SearchIndex = null;
        await PrepareIndexAsync();
    }
    
    private class EmbeddingItem
    {
        public int Id { get; set; }
        public byte[]? Embedding { get; set; }
    }
    
    public async Task<string> GeneratedProjectNumberAsync(CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            var exist = await context.Applications.AsNoTracking().AnyAsync(ct);

            if (!exist) return $"FIV{DateTime.UtcNow.Year}-00001";
            
            var item = context.Projects
                .AsNoTracking()
                .Max(p => p.Id + 1);

            return $"FIV{DateTime.UtcNow.Year}-" + $"{item:00000}";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }


    
    private static readonly Func<ApplicationDbContext, string, Task<Project?>> GetProjectByProjectNumber = 
        EF.CompileAsyncQuery((ApplicationDbContext context, string projectNumber) => 
            context.Projects
                .AsNoTracking()
                .TagWith("GetProjectByProjectNumber")
                .FirstOrDefault(x => x.Number == projectNumber));
    
    private static readonly Func<ApplicationDbContext, int, Task<ProjectHeaderDto?>> GetProjectHeaderById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int projectId) => 
            context.Projects
                .AsNoTracking()
                .TagWith("GetProjectHeaderById")
                .Where(x => x.Id == projectId)
                .Select(x => new ProjectHeaderDto()
                {
                    Id = x.Id,
                    Number = x.Number,
                    Title = x.Title.First(),
                    OrganizationName = x.Organization.OrganizationName,
                    OrganizationAddress = x.Organization.OrganizationAddress,
                    OrganizationPostalCode = x.Organization.OrganizationPostalCode,
                    OrganizationCity = x.Organization.OrganizationCity,
                    OrganizationCountry = x.Organization.OrganizationCountry,
                    OrganizationUrl = x.Organization.OrganizationUrl,
                    ProducerName = x.Applications.Last().ApplicationProducer.Name,
                    ProducerPhoneNumber = x.Applications.Last().ApplicationProducer.PhoneNumber,
                    ProducerEmail = x.Applications.Last().ApplicationProducer.Email,
                    ApplicantName = x.Applications.Last().ApplicationApplicant.Name,
                    ApplicantPhoneNumber = x.Applications.Last().ApplicationApplicant.PhoneNumber,
                    ApplicantEmail = x.Applications.Last().ApplicationApplicant.Email,
                    ProjectManagerName = x.Applications.Last().ApplicationProjectManager.Name,
                    ProjectManagerPhoneNumber = x.Applications.Last().ApplicationProjectManager.PhoneNumber,
                    ProjectManagerEmail = x.Applications.Last().ApplicationProjectManager.Email,
                    Logo = x.Organization.OrganizationLogo
                }).FirstOrDefault());

    
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<EmbeddingItem>> GetPrepareIndex = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.OpenAiProjects
                .AsNoTracking()
                .TagWith("GetPrepareIndex")
                .Where(x => x.Embedding != null)
                .Select(x => new EmbeddingItem { Id = x.Id, Embedding = x.Embedding }));


    private static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<OpenAiProject>> GetLatestSearch = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int maxResults) => 
            context.OpenAiProjects
                .AsNoTracking()
                .TagWith("GetLatestSearch")
                .OrderByDescending(x => x.ProjectNumber)
                .Take(maxResults));


    private static readonly Func<ApplicationDbContext, List<int>, IAsyncEnumerable<OpenAiProject>> GetIdSearch =
        EF.CompileAsyncQuery((ApplicationDbContext context, List<int> projectIds) =>
            context.OpenAiProjects
                .AsNoTracking()
                .TagWith("GetIdSearch")
                .Where(x => projectIds.Contains(x.ProjectId)));


    private static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<Project>> GetProjectsByOrganizationId =
        EF.CompileAsyncQuery((ApplicationDbContext context, int organizationId) =>
            context.Projects
                .AsNoTracking()
                .TagWith("GetProjectsByOrganizationId")
                .Where(x => x.Organization.OrganizationIdentifier == organizationId));


    private static readonly Func<ApplicationDbContext, int, Task<List<string>?>> GetProjectTitleById =
        EF.CompileAsyncQuery((ApplicationDbContext context, int projectId) =>
            context.Projects
                .AsNoTracking()
                .TagWith("GetProjectTitleById")
                .Where(x => x.Id == projectId)
                .Select(x => x.Title)
                .FirstOrDefault());
    

    private static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<SearchResult>> ProjectApplicationSearchResult = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int projectId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("ProjectApplicationSearchResult")
                .Where(x => x.ProjectId == projectId)
                .Select(x => new SearchResult
                {
                    Id = x.Id,
                    StatusId = x.StatusId,
                    Title = x.Title,
                    CreatedDate = x.CreatedDate,
                    SchemaNames = x.SchemaNames,
                    OrganizationName = x.Organization.Name,
                    Index = 0,
                    Controls = x.Controls
                }));
    
    
    
}
  

