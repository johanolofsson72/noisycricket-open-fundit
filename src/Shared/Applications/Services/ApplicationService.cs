
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using HeyRed.Mime;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Hybrid;
using Shared.Data.DbContext;
using Shared.Documents.Services;
using Shared.Events.DTOs;
using Shared.Events.Entities;
using Shared.Events.Services;
using Shared.Notifications;
using Shared.Schemas.Entities;
using Shared.Users.Services;
using Telerik.Blazor.Components;
using Telerik.DataSource.Extensions;

namespace Shared.Applications.Services;

public class ApplicationService(IDbContextFactory<ApplicationDbContext> factory, IConfiguration configuration, HybridCache hybridCache, IEasyCachingProvider cache, IHttpClientFactory httpClientFactory, EventService eventService, UserService userService)
{
    
    // This is for testing purposes
    private readonly List<string> _tags = ["Applications"];
    private readonly HybridCacheEntryOptions _entryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(1),
        LocalCacheExpiration = TimeSpan.FromMinutes(1)
    };
    
    public async Task<ApplicationDto> TestGetApplicationAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            return await hybridCache.GetOrCreateAsync(
                $"TestGetApplicationAsync-{applicationId}", 
                async cancel =>
                {
                    var application = await context.Applications
                        .AsNoTracking()
                        .Where(x => x.Id == applicationId)
                        .FirstOrDefaultAsync(cancellationToken: cancel) ?? throw new Exception("Application not found");
                    
                    return application.ToDto();
                },
                _entryOptions,
                _tags,
                ct
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new ApplicationDto();
        }
    }
    public async Task<bool> TestUpdateApplicationAsync(int applicationId, string title, CancellationToken ct)
    {
        try
        {
            await hybridCache.RemoveByTagAsync(_tags[0], ct);
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            return await context.Applications
                .Where(x => x.Id == applicationId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.Title, title), cancellationToken: ct) > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task TestJson()
    {
        try
        {
            var timer = new Stopwatch();
            timer.Start();
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            await using var conn = new SqliteConnection(context.Database.GetConnectionString());
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Applications AS a SET Title = 'Bob' WHERE (a.Id = 1)";
            //cmd.CommandText = "UPDATE Applications AS a SET RequiredDocuments = '[{\"Id\":0,\"RequiredDocumentId\":3,\"RequiredDocumentIdentifier\":1},{\"Id\":0,\"RequiredDocumentId\":10,\"RequiredDocumentIdentifier\":2},{\"Id\":0,\"RequiredDocumentId\":12,\"RequiredDocumentIdentifier\":3},{\"Id\":0,\"RequiredDocumentId\":14,\"RequiredDocumentIdentifier\":4},{\"Id\":0,\"RequiredDocumentId\":16,\"RequiredDocumentIdentifier\":5},{\"Id\":0,\"RequiredDocumentId\":17,\"RequiredDocumentIdentifier\":6},{\"Id\":0,\"RequiredDocumentId\":21,\"RequiredDocumentIdentifier\":7},{\"Id\":0,\"RequiredDocumentId\":22,\"RequiredDocumentIdentifier\":8},{\"Id\":0,\"RequiredDocumentId\":75,\"RequiredDocumentIdentifier\":9},{\"Id\":0,\"RequiredDocumentId\":78,\"RequiredDocumentIdentifier\":10},{\"Id\":0,\"RequiredDocumentId\":79,\"RequiredDocumentIdentifier\":11},{\"Id\":0,\"RequiredDocumentId\":81,\"RequiredDocumentIdentifier\":12},{\"Id\":0,\"RequiredDocumentId\":82,\"RequiredDocumentIdentifier\":13},{\"Id\":0,\"RequiredDocumentId\":83,\"RequiredDocumentIdentifier\":14},{\"Id\":0,\"RequiredDocumentId\":84,\"RequiredDocumentIdentifier\":15},{\"Id\":0,\"RequiredDocumentId\":85,\"RequiredDocumentIdentifier\":16},{\"Id\":0,\"RequiredDocumentId\":86,\"RequiredDocumentIdentifier\":17},{\"Id\":0,\"RequiredDocumentId\":87,\"RequiredDocumentIdentifier\":18},{\"Id\":0,\"RequiredDocumentId\":88,\"RequiredDocumentIdentifier\":19},{\"Id\":0,\"RequiredDocumentId\":89,\"RequiredDocumentIdentifier\":20},{\"Id\":0,\"RequiredDocumentId\":90,\"RequiredDocumentIdentifier\":21},{\"Id\":0,\"RequiredDocumentId\":100,\"RequiredDocumentIdentifier\":22},{\"Id\":0,\"RequiredDocumentId\":112,\"RequiredDocumentIdentifier\":22}]' WHERE (a.Id = 1)";
            var result = await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"Time ADO: {timer.ElapsedMilliseconds}ms");
            
            timer = new Stopwatch();
            timer.Start();
            var application = await context.Applications
                .Where(x => x.Id == 1)
                .FirstOrDefaultAsync() ?? throw new Exception("Application not found");
            application.Title = "Fett";
            await context.SaveChangesAsync();
            Console.WriteLine($"Time EFCORE: {timer.ElapsedMilliseconds}ms");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    // This is for testing purposes
    
    public async Task<Result<ReplyApplicationDto, Exception>> ReplyApplicationByIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_ReplyApplicationByIdAsync_{applicationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var application = await GetReplyApplicationById(context, applicationId) ?? throw new Exception("Application not found");
                    return application.ToDto();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<SlimApplicationDto, Exception>> SlimApplicationByIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_SlimApplicationByIdAsync_{applicationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var application = await GetSlimApplicationById(context, applicationId) ?? throw new Exception("Application not found");
                    return application.ToDto();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<CounterApplicationDto, Exception>> CounterApplicationByIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_CounterApplicationByIdAsync_{applicationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var application = await GetCounterApplicationById(context, applicationId) ?? throw new Exception("Application not found");
                    return application.ToDto();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<EconomyApplicationDto, Exception>> EconomyApplicationByIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_EconomyApplicationByIdAsync_{applicationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var application = await GetEconomyApplicationById(context, applicationId) ?? throw new Exception("Application not found");
                    return application.ToDto();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<MediumApplicationDto, Exception>> MediumApplicationByIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_MediumApplicationByIdAsync_{applicationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var application = await GetMediumApplicationById(context, applicationId) ?? throw new Exception("Application not found");
                    return application.ToDto();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<DocumentApplicationDto, Exception>> DocumentApplicationByIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_DocumentApplicationByIdAsync_{applicationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var application = await GetDocumentApplicationById(context, applicationId) ?? throw new Exception("Application not found");
                    return application.ToDto();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<MiniApplicationDto, Exception>> MiniApplicationByIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_MiniApplicationByIdAsync_{applicationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var application = await GetMiniApplicationById(context, applicationId) ?? throw new Exception("Application not found");
                    return application.ToDto();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<(int ApplicationId, string TempPath), Exception>> CreateClientApplicationAsync(CreateClientApplicationDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.OrganizationId == 0) throw new Exception("OrganizationId is required");
            if (dto.ApplicantId == 0) throw new Exception("ApplicantId is required");
            if (dto.SchemaId == 0) throw new Exception("SchemaId is required");

            // Generate Number
            var number = await GetGeneratedClientNumberAsync(ct);
            
            // Create TempPath
            var tempPath = InitializeTempFileDirectory();
            
            // Get Project Managers for this specific schema
            var projectManagersResult = await userService.ProjectManagersAsync(dto.SchemaId, new CancellationToken());
            if (!projectManagersResult.IsOk) throw new Exception(projectManagersResult.Error.ToString());
            if (!projectManagersResult.Value.Any()) throw new Exception($"There are no Project Managers for schema: {dto.SchemaId}");
            
            // Set Project Manager for this application
            var projectManagerUser = projectManagersResult.Value.First();
            var projectManager = new ApplicationContact()
            {
                ContactIdentifier = projectManagerUser.Id,
                Name = projectManagerUser.FullName,
                Email = projectManagerUser.Email ?? "",
                PhoneNumber = projectManagerUser.PhoneNumbers.FirstOrDefault()?.Number ?? ""
            };
            
            // Get Organization
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var organizationResult = await context.Organizations
                .AsNoTracking()
                .Where(x => x.Id == dto.OrganizationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Organization not found");
            
            // Set Organization
            var organization = new ApplicationContact()
            {
                ContactIdentifier = organizationResult.Id,
                Name = organizationResult.Name,
                Email = organizationResult.Mail,
                PhoneNumber = organizationResult.PhoneNumbers.Any() ? organizationResult.PhoneNumbers.First().Number : ""
            };
            
            // Get Applicant
            var applicantUser = await context.Users
                .AsNoTracking()
                .Where(x => x.Id == dto.ApplicantId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Applicant not found");

            // Set Applicant
            var applicant = new ApplicationContact()
            {
                ContactIdentifier = applicantUser.Id,
                Name = applicantUser.FullName,
                Email = applicantUser.Email ?? "",
                PhoneNumber = applicantUser.PhoneNumbers.Any() ? applicantUser.PhoneNumbers.First().Number : ""
            };
            
            // Get Schemas
            var schema = await context.Schemas
                .AsNoTracking()
                .Where(x => x.Id == dto.SchemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");

            // Set Controls, Events, Progress and Required Documents
            var controls = schema.Controls.ToApplicationControl();
            var events = schema.Events.ToApplicationEvent();
            var progress = schema.Progress.ToApplicationProgress();
            var requiredDocuments = schema.RequiredDocuments.ToApplicationRequiredDocument();
            var application = new Application
            {
                StatusId = 2,
                ParentId = 0,
                Organization = organization,
                Applicant = applicant,
                ProjectManager = projectManager,
                SchemaId = schema.Id,
                SchemaNames = schema.Names,
                SchemaClaimTag = schema.ClaimTag,
                CreatedDate = DateTime.UtcNow,
                Number = number,
                Controls = controls,
                Events = events,
                Progress = progress,
                RequiredDocuments = requiredDocuments,
                Audits =
                [
                    new ApplicationAudit()
                    {
                        Id = 1,
                        Event = "Application Created",
                        Fields = [number],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "ApplicationService.CreateClientApplicationAsync"
                    }
                ]
            };

            // Create Application
            await context.Applications.AddAsync(application, ct);
            await context.SaveChangesAsync(ct);
            
            // Set Application State
            var applicationState = new ApplicationState
            {
                OrganizationId = dto.OrganizationId,
                UserId = dto.ApplicantId,
                ApplicationId = application.Id,
                SchemaId = dto.SchemaId,
                SchemaNames = schema.Names,
                TempPath = tempPath,
                CreatedDate = DateTime.UtcNow
            };
            await context.ApplicationStates.AddAsync(applicationState, ct);
            await context.SaveChangesAsync(ct);
            
            // Clear Cache
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.ApplicationStates.ToDescriptionString(), ct);
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            return (application.Id, tempPath);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<TriggerEventDto, Exception>> DeliverClientApplicationAsync(int applicationId, DeliverNewApplicationDto dto, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            // Save Application
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.StatusId = 3;
            application.DeliveryDate = DateTime.UtcNow;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                    new ApplicationAudit()
                    {
                        Id = application.Audits.Count + 1,
                        Event = "Application Delivered",
                        Fields = [applicationId.ToString(), application.Title, application.Number],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "ApplicationService.DeliverClientApplicationAsync"
                    });
            
            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            // Save Documents And Delete Temp Folder
            var documents = new List<int>();
            foreach (var control in application.Controls.Where(x => x.ControlTypeId == 13).ToList())
            {
                control.Value = Regex.Unescape(control.Value);
                if (string.IsNullOrWhiteSpace(control.Value)) continue;
               
                var files = JsonSerializer.Deserialize<List<UploadFileInfo>>(control.Value);
                if (files is null) continue;

                var destinationPath = Path.Combine(configuration["DocumentPhysicalRoot"]!,
                    "app",
                    application.Id.ToString(),
                    "att",
                    control.Id.ToString());
                
                var dir = new DirectoryInfo(destinationPath);
                if (!dir.Exists) dir.Create();
                Console.WriteLine("Destination Path created: " + destinationPath);

                var documentType = DocumentType(control.ToDto());
                
                if (documentType == 0) continue;

                foreach (var file in files)
                {
                    var filePath = Path.Combine(dto.TempPath, file.Name);
                    var sourceFile = new System.IO.FileInfo(filePath);
                    if (!sourceFile.Exists) continue;
                        
                    // create document
                    var document = new Document
                    {
                        ApplicationId = application.Id,
                        StatusId = 2,
                        RequirementTypeId = documentType,
                        DeliveryTypeId = 2,
                        FileName = sourceFile.Name,
                        MimeType = MimeTypesMap.GetMimeType(sourceFile.Name),
                        Extension = sourceFile.Extension,
                        Path = "",
                        Phrases = "",
                        Summarize = "",
                        Binary = [],
                        Metadata = [],
                        CreatedDate = DateTime.UtcNow,
                        IsDelivered = true,
                        IsSigned = false,
                        IsCertified = false,
                        IsLocked = false
                    };
                    await context.Documents.AddAsync(document, ct);
                    await context.SaveChangesAsync(ct);
                    
                    dir = new DirectoryInfo(Path.Combine(destinationPath, document.Id.ToString()));
                    if (!dir.Exists) dir.Create();
                    var destinationFile = Path.Combine(destinationPath, document.Id.ToString(), sourceFile.Name);
                    
                    sourceFile = sourceFile.CopyTo(destinationFile, true);
                    if (!sourceFile.Exists) throw new FileNotFoundException();
                    
                    await context.Documents
                        .Where(x => x.Id == document.Id)
                        .ExecuteUpdateAsync(x => x
                            .SetProperty(p => p.Path, destinationFile), cancellationToken: ct);
                    Console.WriteLine("Destination Path updated: " + destinationFile);
                    
                    documents.Add(document.Id);
                }
            }
            
            // Clear Temporary Files
            ClearTemporaryFilesFolder(dto.TempPath);
            
            // Delete Application State
            await context.ApplicationStates
                .Where(x => x.OrganizationId == dto.OrganizationId)
                .Where(x => x.UserId == dto.UserId)
                .Where(x => x.ApplicationId == applicationId)
                .ExecuteDeleteAsync(ct);
            
            // Reset Caches
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.ApplicationStates.ToDescriptionString(), ct);
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Documents.ToDescriptionString(), ct);            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            
            // Execute First Event
            var eventResult = await eventService.TriggerNextEventById(applicationId, 1, ct);
            if (!eventResult.IsOk) throw new Exception(eventResult.Error.ToString());
            var triggerEvent = eventResult.Value;
            
            // Start All Aggregations
            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);
            foreach (var document in documents)
            {
                await AggregateDocumentAsync(document, ct);
            }

            return triggerEvent.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateClientApplicationAsync(int applicationId, DeliverNewApplicationDto dto, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            // Save Application
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.UpdatedDate = DateTime.UtcNow;
            application.DeliveryDate = application.DeliveryDate;
            application.Audits.Add(
                    new ApplicationAudit()
                    {
                        Id = application.Audits.Count + 1,
                        ApplicationAuditIdentifier = application.Audits.Count + 1,
                        Event = "Application Updated",
                        Fields = [applicationId.ToString(), application.Title, application.Number],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "ApplicationService.UpdateClientApplicationAsync"
                    });
            
            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            // Save Documents And Delete Temp Folder
            var documents = new List<int>();
            foreach (var control in application.Controls.Where(x => x.ControlTypeId == 13).ToList())
            {
                control.Value = Regex.Unescape(control.Value);
                if (string.IsNullOrWhiteSpace(control.Value)) continue;
               
                var files = JsonSerializer.Deserialize<List<UploadFileInfo>>(control.Value);
                if (files is null) continue;

                var controlId = control.Id.ToString();
                var destinationPath = Path.Combine(configuration["DocumentPhysicalRoot"]!,
                    "app",
                    application.Id.ToString(),
                    "att",
                    controlId);
                var dir = new DirectoryInfo(destinationPath);
                if (!dir.Exists) dir.Create();

                var documentType = DocumentType(control.ToDto());
                
                if (documentType == 0) continue;

                foreach (var file in files)
                {
                    var filePath = Path.Combine(dto.TempPath, file.Name);
                    var sourceFile = new System.IO.FileInfo(filePath);
                    if (!sourceFile.Exists) continue;
                        
                    // create document
                    var document = new Document
                    {
                        ApplicationId = application.Id,
                        StatusId = 2,
                        RequirementTypeId = documentType,
                        DeliveryTypeId = 2,
                        FileName = sourceFile.Name,
                        MimeType = MimeTypesMap.GetMimeType(sourceFile.Name),
                        Extension = sourceFile.Extension,
                        Path = "",
                        Phrases = "",
                        Summarize = "",
                        Binary = [],
                        Metadata = [],
                        CreatedDate = DateTime.UtcNow,
                        IsDelivered = true,
                        IsSigned = false,
                        IsCertified = false,
                        IsLocked = false
                    };
                    await context.Documents.AddAsync(document, ct);
                    await context.SaveChangesAsync(ct);
                    
                    dir = new DirectoryInfo(Path.Combine(destinationPath, document.Id.ToString()));
                    if (!dir.Exists) dir.Create();
                    var destinationFile = Path.Combine(destinationPath, document.Id.ToString(), sourceFile.Name);
                    
                    sourceFile = sourceFile.CopyTo(destinationFile, true);
                    if (!sourceFile.Exists) throw new FileNotFoundException();
                    
                    await context.Documents.ExecuteUpdateAsync(x => x
                        .SetProperty(p => p.Path, destinationFile), cancellationToken: ct);
                    
                    documents.Add(document.Id);
                }
            }
            ClearTemporaryFilesFolder(dto.TempPath);
            
            // Reset Caches
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            
            // Start All Aggregations
            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);
            foreach (var document in documents)
            {
                await AggregateDocumentAsync(document, ct);
            }

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> UpdateApplicationControlAsync(int applicationId, int controlId, string value, bool updateHeader, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            if (controlId == 0) throw new Exception("controlId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            var control = application.Controls.FirstOrDefault(x => x.Id == controlId) ?? throw new Exception("Control not found");
            
            control.Value = value;
            
            application.UpdatedDate = DateTime.UtcNow;
            
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = $"Application Control: {controlId} Updated to: {value}",
                    Fields = [applicationId.ToString(), controlId.ToString(), value],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.UpdateClientApplicationControlAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            
            if (updateHeader) await AggregateApplicationAsync(application.Id, NotificationType.ProjectsHeader, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> UpdateEconomyApplicationControlAsync(int applicationId, int controlId, string value, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            if (controlId == 0) throw new Exception("controlId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            var control = application.Controls.FirstOrDefault(x => x.Id == controlId) ?? throw new Exception("Control not found");
            
            control.Value = value;
            
            if (control.UniqueId.ToString().ToLower().StartsWith("01000001"))
            {
                _ = decimal.TryParse(value, out var result);
                application.OurContribution = result;
            }
            
            application.UpdatedDate = DateTime.UtcNow;
            
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = $"Application Control: {controlId} Updated to: {value}",
                    Fields = [applicationId.ToString(), controlId.ToString(), value],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.UpdateClientApplicationControlAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            
            await AggregateApplicationAsync(application.Id, NotificationType.Economy, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateMediumApplicationAndControlAsync(MediumApplicationDto mediumApplication, int controlId, string value, bool updateHeader, CancellationToken ct)
    {
        try
        {
            if (controlId == 0) throw new Exception("controlId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == mediumApplication.Id)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.ProjectId = mediumApplication.ProjectId;
            application.ProjectNumber = mediumApplication.ProjectNumber;
            application.StatusId = mediumApplication.StatusId;
            application.SchemaId = mediumApplication.SchemaId;
            application.SchemaClaimTag = mediumApplication.SchemaClaimTag;
            application.Title = mediumApplication.Title;
            application.CreatedDate = mediumApplication.CreatedDate;
            application.DeliveryDate = mediumApplication.DeliveryDate;
            application.ProjectManager = mediumApplication.ProjectManager.ToEntity();
            application.ProductionManager = mediumApplication.ProductionManager.ToEntity();
            application.ContractManager = mediumApplication.ContractManager.ToEntity();
            application.DistributionManager = mediumApplication.DistributionManager.ToEntity();
            application.FinanceManager = mediumApplication.FinanceManager.ToEntity();
            application.ScriptManager = mediumApplication.ScriptManager.ToEntity();
            application.Controls = mediumApplication.Controls.Select(x => x.ToEntity()).ToList();
                
            var control = application.Controls.FirstOrDefault(x => x.Id == controlId) ?? throw new Exception("Control not found");
            
            control.Value = value;
            
            application.UpdatedDate = DateTime.UtcNow;
            
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = $"Application Control: {controlId} Updated to: {value}",
                    Fields = [mediumApplication.Id.ToString(), controlId.ToString(), value],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.UpdateMediumApplicationAndControlAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            
            if (updateHeader) await AggregateApplicationAsync(application.Id, NotificationType.ProjectsHeader, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateEconomyApplicationAndControlAsync(EconomyApplicationDto economyApplication, CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == economyApplication.Id)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.MilestonePayoutTotalAmount = economyApplication.MilestonePayoutTotalAmount;
            application.EarlierSupportTotalAmount = economyApplication.EarlierSupportTotalAmount;
            application.InternalBudgets = economyApplication.InternalBudgets.Select(x => x.ToEntity()).ToList();
            application.Controls = economyApplication.Controls.Select(x => x.ToEntity()).ToList();
            application.UpdatedDate = DateTime.UtcNow;
            
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = $"Application Economy Updated",
                    Fields = [economyApplication.Id.ToString()],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.UpdateEconomyApplicationAndControlAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            
            await AggregateApplicationAsync(application.Id, NotificationType.ProjectsHeader, ct);
            
            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> ConnectApplicationToProjectAsync(int applicationId, ConnectApplicationToProjectDto dto, CancellationToken ct)
    {
        await using var context = await factory.CreateDbContextAsync(ct);
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            if (applicationId == 0) throw new Exception("applicationId är obligatoriskt");
            if (dto.OrganizationId == 0) throw new Exception("organizationId är obligatoriskt");
            if (string.IsNullOrWhiteSpace(dto.ProjectNumber)) throw new Exception("ProjectNumber är obligatoriskt");

            // Kontrollera om projektet redan finns
            var project = await context.Projects
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Number == dto.ProjectNumber, ct);

            // Om projektet inte finns, skapa det
            if (project == null)
            {
                project = new Project
                {
                    Organization = new ProjectOrganization { OrganizationIdentifier = dto.OrganizationId },
                    StatusId = 4,
                    Number = dto.ProjectNumber,
                    Title = dto.Titles,
                    Applications = [],
                    ApplicationCount = 0,
                    UpdatedDate = DateTime.UtcNow,
                    CreateDate = DateTime.UtcNow
                };

                context.Projects.Add(project);
                await context.SaveChangesAsync(ct);

                if (project.Id == 0) throw new Exception("Projektet kunde inte skapas!");
                
                Console.WriteLine($"Project {dto.ProjectNumber} did not exist so we created: " + project.Id);
            }
            else
            {
                Console.WriteLine($"Project {dto.ProjectNumber} exist and has project id: {project.Id}");
            }

            // Hämta applikationen
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Applikationen hittades inte!");
            
            Console.WriteLine($"Updating application {applicationId}");
            Console.WriteLine($"ProjectId: {project.Id}");
            Console.WriteLine($"ProjectNumber: {project.Number}");
            Console.WriteLine($"ProjectTitle: {project.Title}");
            Console.WriteLine($"ProjectStatusId: {project.StatusId}");

            // Uppdatera applikationens data  978AC998
            var controls = application.Controls.ToList();
            var control = controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("00000001"));

            if (control is not null)
            {
                control.Value = dto.SelectedCurrency;
            }
            
            var productionYear = controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("978ac998"));

            if (productionYear is not null)
            {
                productionYear.Value = DateTime.Now.Year.ToString();
            }

            application.ProjectManager = dto.ProjectManager.ToEntity();
            application.ProjectId = project.Id;
            application.StatusId = 4;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = $"Application Connected to Project: {project.Id}",
                    Fields = new List<string> { applicationId.ToString(), project.Id.ToString() },
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.ConnectApplicationToProjectAsync"
                });

            context.Applications.Update(application);
            
            // Uppdatera application information i projektet
            var projectApplications = project.Applications.ToList();
            projectApplications.Add(new ProjectApplication()
            {
                ApplicationIdentifier = application.Id,
                ApplicationStatusId = application.StatusId,
                ApplicationSchemaId = application.SchemaId,
                ApplicationSchemaNames = application.SchemaNames,
                ApplicationTitle = application.Title,
                ApplicationProducer = application.Producer,
                ApplicationApplicant = application.Applicant,
                ApplicationProjectManager = application.ProjectManager,
                ApplicationProductionManager = application.ProductionManager,
                ApplicationFinanceManager = application.FinanceManager,
                ApplicationScriptManager = application.ScriptManager,
                ApplicationDistributionManager = application.DistributionManager,
                ApplicationContractManager = application.ContractManager,
                ApplicationCreatedDate = application.CreatedDate
            });
            
            project.Applications = projectApplications;
            project.ApplicationCount = projectApplications.Count;
            
            Console.WriteLine($"Updating project {project.Id}");
            Console.WriteLine($"ApplicationCount: {project.ApplicationCount}");
            
            // Spara ändringar i transaktionen
            await context.SaveChangesAsync(ct);

            if (application.Id == 0)
            {
                throw new InvalidOperationException("Applikationen har inget Id.");
            }

            if (application.ProjectId == 0)
            {
                throw new InvalidOperationException("Applikationen har inget projektId.");
            }

            // Bekräfta transaktionen
            await transaction.CommitAsync(ct);
            Console.WriteLine($"Application {applicationId} connected to project {project.Id} sucessfully!");

            // Kör aggregeringar
            await AggregateProjectAsync(project.Id, ct);
            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);
            
            // Rensa cache
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            return true;
        }
        catch (Exception ex)
        {
            // Om något går fel, rulla tillbaka transaktionen
            await transaction.RollbackAsync(ct);
            Console.WriteLine($"Error connecting application {applicationId} to project: {ex.Message}");
            return ex;
        }
    }
    
    public async Task AddApplicationAuditAsync(int applicationId, string title, List<string> fields, string executedBy, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = title,
                    Fields = fields,
                    Executed = DateTime.UtcNow,
                    ExecutedBy = executedBy
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            var message = ex.Message;
        }
    }

    public async Task<Result<ApplicationDto, Exception>> CreateApplicationAsync(CreateApplicationDto dto, CancellationToken ct, bool aggregate = true)
    {
        try
        {
            if (dto.OrganizationId == 0) throw new Exception("OrganizationId is required");
            if (dto.ProjectManagerId == 0) throw new Exception("ProjectManagerId is required");
            if (dto.ApplicantId == 0) throw new Exception("ApplicantId is required");
            if (dto.SchemaId == 0) throw new Exception("SchemaId is required");

            var number = await GetGeneratedClientNumberAsync(ct);
            
            // Organizations
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var organizationBase = await context.Organizations
                .AsNoTracking()
                .Where(x => x.Id == dto.OrganizationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Organization not found");

            var organization = new ApplicationContact()
            {
                ContactIdentifier = organizationBase.Id,
                Name = organizationBase.Name,
                Email = organizationBase.Mail,
                PhoneNumber = organizationBase.PhoneNumbers.Any() ? organizationBase.PhoneNumbers.First().Number : ""
            };
            
            // Applicant
            var applicantUser = await context.Users
                .AsNoTracking()
                .Where(x => x.Id == dto.ApplicantId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Applicant not found");

            var applicant = new ApplicationContact()
            {
                ContactIdentifier = applicantUser.Id,
                Name = applicantUser.FullName,
                Email = applicantUser.Email ?? "",
                PhoneNumber = applicantUser.PhoneNumbers.Any() ? applicantUser.PhoneNumbers.First().Number : ""
            };
            
            // Project Manager
            var projectManagerUser = await context.Users
                .AsNoTracking()
                .Where(x => x.Id == dto.ProjectManagerId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Project manager not found");

            var projectManager = new ApplicationContact()
            {
                ContactIdentifier = projectManagerUser.Id,
                Name = projectManagerUser.FullName,
                Email = projectManagerUser.Email ?? "",
                PhoneNumber = projectManagerUser.PhoneNumbers.FirstOrDefault()?.Number ?? ""
            };
            
            // Schemas, Controls, Events, Progress and Required Documents
            var schema = await context.Schemas
                .AsNoTracking()
                .Where(x => x.Id == dto.SchemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");

            var controls = schema.Controls.ToApplicationControl();
            var events = schema.Events.ToApplicationEvent();
            var progress = schema.Progress.ToApplicationProgress();
            var requiredDocuments = schema.RequiredDocuments.ToApplicationRequiredDocument();

            var application = new Application
            {
                StatusId = 2,
                ParentId = dto.ParentId,
                Organization = organization,
                Applicant = applicant,
                ProjectManager = projectManager,
                SchemaId = schema.Id,
                SchemaNames = schema.Names,
                SchemaClaimTag = schema.ClaimTag,
                CreatedDate = DateTime.UtcNow,
                Number = number,
                Controls = controls,
                Events = events,
                Progress = progress,
                RequiredDocuments = requiredDocuments,
                Audits =
                [
                    new ApplicationAudit()
                    {
                        Id = 1,
                        ApplicationAuditIdentifier = 1,
                        Event = "Application Created",
                        Fields = [number],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "ApplicationService.CreateApplicationAsync"
                    }
                ]
            };

            await context.Applications.AddAsync(application, ct);
            await context.SaveChangesAsync(ct);

            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            
            if (aggregate) await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);
            
            return application.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> UpdateApplicationAsync(int applicationId, UpdateApplicationDto dto, CancellationToken ct, bool aggregate = true)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            if (dto.SchemaId == 0) throw new Exception("SchemaId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.ParentId = dto.ParentId;
            application.ProjectId = dto.ProjectId;
            application.ProjectNumber = dto.ProjectNumber;
            application.StatusId = dto.StatusId;
            application.SchemaId = dto.SchemaId;
            application.Organization = dto.Organization.ToEntity();
            application.Applicant = dto.Applicant.ToEntity();
            application.Producer = dto.Producer.ToEntity();
            application.ProjectManager = dto.ProjectManager.ToEntity();
            application.ProductionManager = dto.ProductionManager.ToEntity();
            application.ContractManager = dto.ContractManager.ToEntity();
            application.Title = dto.Title;
            application.Number = dto.Number;
            application.InternalBudgets = dto.InternalBudgets.Select(x => x.ToEntity()).ToList();
            application.InternalBudgetsTotalAmount = dto.InternalBudgetsTotalAmount;
            application.MilestonePayoutTotalAmount = dto.MilestonePayoutTotalAmount;
            application.EarlierSupportTotalAmount = dto.EarlierSupportTotalAmount;
            application.InternalBudgetsApproved = dto.InternalBudgetsApproved;
            application.Controls = dto.Controls.Select(x => x.ToEntity()).ToList();
            application.BudgetAmount = dto.BudgetAmount;
            application.AppliedAmount = dto.AppliedAmount;
            application.OurContribution = dto.OurContribution;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                    new ApplicationAudit()
                    {
                        Id = application.Audits.Count + 1,
                        ApplicationAuditIdentifier = application.Audits.Count + 1,
                        Event = "Application Updated",
                        Fields = [applicationId.ToString(), dto.Title, dto.Number],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "ApplicationService.UpdateApplicationAsync"
                    });
            application.DeliveryDate = dto.DeliveryDate;
            
            // Update control values
            var applicationDeliveryDate = dto.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("10000001"));
            if (applicationDeliveryDate is not null) applicationDeliveryDate.Value = application.DeliveryDate.ToString("yyyy-MM-dd");

            var applicationProducer = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("eba1414f"));
            if (applicationProducer is not null && applicationProducer.Value.Length > 3)
            {
                applicationProducer.Value = Regex.Unescape(applicationProducer.Value);
                var tmp = JsonSerializer.Deserialize<List<ListboxNameEmailPhonenumberGenderDto>>(applicationProducer.Value);
                if (tmp != null && tmp.Count != 0)
                {
                    application.Producer = new ApplicationContact()
                    {
                        ContactIdentifier = 0,
                        Name = tmp.First().Name,
                        Email = tmp.First().Email,
                        PhoneNumber = tmp.First().Phonenumber
                    };
                }
            }
            
            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            if (aggregate) await AggregateApplicationAsync(application.Id, NotificationType.ApplicationOverview, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> SetApplicationDecisionDateAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            var avtal = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("839e5feb"));
            if (avtal is not null) avtal.Value = DateTime.UtcNow.ToString("yyyy-MM-dd");
            
            var spend1 = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("fa929fa7"));
            if (spend1 is not null && application.OurContribution > 0) spend1.Value = application.OurContribution.ToString(CultureInfo.InvariantCulture);
            
            var spend2 = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("025127d7"));
            if (spend2 is not null && application.OurContribution > 0) spend2.Value = (application.OurContribution * 0,85).ToString();

            application.UpdatedDate = DateTime.UtcNow;
            
            application.DecisionDate = DateTime.UtcNow;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                    new ApplicationAudit()
                    {
                        Id = application.Audits.Count + 1,
                        ApplicationAuditIdentifier = application.Audits.Count + 1,
                        Event = $"Application Decision Made {DateTime.UtcNow:yyyy-MM-dd}",
                        Fields = [applicationId.ToString(), DateTime.UtcNow.ToString("yyyy-MM-dd")],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "ApplicationService.SetApplicationDecisionDateAsync"
                    });
            
            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);  

            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> UpdateApplicationControlsAsync(int applicationId, UpdateApplicationControlsDto dto, CancellationToken ct, bool aggregate = true)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            var applicationTitle = dto.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00001001"))?.Value;
            if (applicationTitle != null) application.Title = applicationTitle;
            
            var applicationBudget = dto.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00010001"))?.Value;
            _ = decimal.TryParse(applicationBudget, out var applicationBudgetD);
            if (applicationBudget != null && applicationBudgetD > 0) application.BudgetAmount = applicationBudgetD;
            
            var applicationApplied = dto.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00000002"))?.Value;
            _ = decimal.TryParse(applicationApplied, out var applicationAppliedD);
            if (applicationApplied != null && applicationAppliedD > 0) application.AppliedAmount = applicationAppliedD;
            
            var applicationOur = dto.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("01000001"))?.Value;
            _ = decimal.TryParse(applicationOur, out var applicationOurD);
            if (applicationOur != null && applicationOurD > 0) application.OurContribution = applicationOurD;
            
            var applicationProcent = dto.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00100001"));
            if (applicationProcent is not null && applicationBudgetD > 0 && applicationOurD > 0) applicationProcent.Value = (applicationOurD / applicationBudgetD * 100).ToString();
            
            var applicationProducer = dto.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("eba1414f"));
            if (applicationProducer is not null && applicationProducer.Value.Length > 3)
            {
                applicationProducer.Value = Regex.Unescape(applicationProducer.Value);
                var tmp = JsonSerializer.Deserialize<List<ListboxNameEmailPhonenumberGenderDto>>(applicationProducer.Value);
                if (tmp != null && tmp.Count != 0)
                {
                    application.Producer = new ApplicationContact()
                    {
                        ContactIdentifier = 0,
                        Name = tmp.First().Name,
                        Email = tmp.First().Email,
                        PhoneNumber = tmp.First().Phonenumber
                    };
                }
            }

            application.Controls = dto.Controls.Select(x => x.ToEntity()).ToList();
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Application Controls Updated",
                    Fields = [applicationId.ToString(), applicationTitle, applicationBudget, applicationApplied, applicationOur, applicationProcent?.Value!],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.UpdateApplicationControlsAsync"
                });

            context.Applications.Update(application);

            if (application.ProjectId > 0)
            {
                var project = await context.Projects
                    .Where(x => x.Id == application.ProjectId)
                    .FirstOrDefaultAsync(ct) ?? throw new Exception("Project not found");

                if (applicationTitle != null && project.Title.First() != applicationTitle)
                {
                    project.Title.Insert(0, applicationTitle);
                    context.Projects.Update(project);
                }
            }
            
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            if (aggregate) await AggregateApplicationAsync(application.Id, NotificationType.ProjectsHeader, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> ImportStartedApplicationAsync(int applicationId, CancellationToken ct, bool aggregate = true)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.StatusId = 3;
            application.UpdatedDate = DateTime.UtcNow;
            application.DeliveryDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Application Imported",
                    Fields = [applicationId.ToString(), application.StatusId.ToString()],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.ImportStartedApplicationAsync"
                });

            var applicationTitle = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00001001"))?.Value;
            if (applicationTitle != null) application.Title = applicationTitle;
            
            // Update control values
            var applicationDeliveryDate = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("10000001"));
            if (applicationDeliveryDate is not null) applicationDeliveryDate.Value = application.DeliveryDate.ToString("yyyy-MM-dd");

            var applicationProducer = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("eba1414f"));
            if (applicationProducer is not null && applicationProducer.Value.Length > 3)
            {
                applicationProducer.Value = Regex.Unescape(applicationProducer.Value);
                var tmp = JsonSerializer.Deserialize<List<ListboxNameEmailPhonenumberGenderDto>>(applicationProducer.Value);
                if (tmp != null && tmp.Count != 0)
                {
                    application.Producer = new ApplicationContact()
                    {
                        ContactIdentifier = 0,
                        Name = tmp.First().Name,
                        Email = tmp.First().Email,
                        PhoneNumber = tmp.First().Phonenumber
                    };
                }
            }
            
            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            if (aggregate) await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteApplicationAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.StatusId = 19;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Application Deleted",
                    Fields = [applicationId.ToString(), application.StatusId.ToString()],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.DeleteApplicationAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);

            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            
            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<ApplicationDto, Exception>> ApplicationByIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_ApplicationByIdAsync_{applicationId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var application = await GetApplicationById(context, applicationId) ?? throw new Exception("Application not found");
                    return application.ToDto(); 
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> ResetApplicationAsync(int projectId, int applicationId, CancellationToken ct)
    {
        try
        {
            if (projectId == 0) throw new Exception("projectId is required");
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            // clear project
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var project = await context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == projectId, ct) ?? throw new Exception("Project not found");
            
            // clear messages
            _ = await context.Messages
                .Where(x => x.ApplicationId == applicationId)
                .ExecuteDeleteAsync(ct);
            
            // clear documents that are not attachments
            _ = await context.Documents
                .Where(x => x.ApplicationId == applicationId && x.RequirementTypeId < 10000)
                .ExecuteDeleteAsync(ct);
            
            // clear status
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");

            foreach (var ev in application.Events.Where(x => x.Id > 1))
            {
                ev.StatusId = 0;
            }
            
            application.StatusId = 3;
            application.ProjectId = 0;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Application Reset",
                    Fields = [applicationId.ToString(), projectId.ToString()],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.ResetApplicationAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);

            if (project.Applications.Count < 2)
            {
                _ = await context.Projects
                    .Where(x => x.Id == projectId)
                    .ExecuteDeleteAsync(ct) > 0;
            }
            
            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> ResetApplicationNewEventCounterAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.NewEventCounter = 0;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Application Reset New Event Counter",
                    Fields = [applicationId.ToString()],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.ResetApplicationNewEventCounterAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> ResetApplicationNewAuditCounterAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.NewAuditCounter = 0;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Application Reset New Message Counter",
                    Fields = [applicationId.ToString()],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.ResetApplicationNewMessageCounterAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> SetApplicationAsCompletedAsync(int projectId, int applicationId, CancellationToken ct)
    {
        try
        {
            if (projectId == 0) throw new Exception("projectId is required");
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            application.StatusId = 5;
            application.UpdatedDate = DateTime.UtcNow;
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Application Set As Completed",
                    Fields = [applicationId.ToString()],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.SetApplicationAsCompletedAsync"
                });

            context.Applications.Update(application);
            await context.SaveChangesAsync(ct);
            
            _ = await context.Projects
                .Where(x => x.Id == projectId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(c => c.StatusId, 5), ct);
            
            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> SetApplicationAsFullyFinancedAsync(int projectId, int applicationId, CancellationToken ct)
    {
        try
        {
            await Task.Delay(0, ct);
            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> SetPublic360MeetingApprovedAsync(int projectId, int applicationId, DateTime meetingDate, CancellationToken ct)
    {
        try
        {
            if (projectId == 0) throw new Exception("projectId is required");
            if (applicationId == 0) throw new Exception("applicationId is required");
            if (meetingDate < DateTime.UtcNow) throw new Exception("meetingDate is required");
            
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var application = await context.Applications
                .AsTracking()
                .Where(x => x.Id == applicationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Application not found");
            
            var applicationPublic360Meeting = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("01001001"));
            if (applicationPublic360Meeting is not null) applicationPublic360Meeting.Value = meetingDate.ToString();
            
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Application Set Public 360 Meeting Approved",
                    Fields = [applicationId.ToString(), meetingDate.ToString()],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "ApplicationService.SetPublic360MeetingApprovedAsync"
                });
            
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);

            await AggregateApplicationAsync(application.Id, NotificationType.Default, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> SetContractProcessAsCompletedAsync(int projectId, int applicationId, CancellationToken ct)
    {
        try
        {
            await Task.Delay(0, ct);
            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<List<ApplicationTabItemDto>, Exception>> ApplicationTabItemsAsync(int projectId, int applicationId, CancellationToken ct)
    {
        try
        {
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_ApplicationTabItemsAsync", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var applications = GetApplicationTabItems(context, projectId, applicationId) ?? throw new Exception("Applications not found");
                    return await applications.ToListAsync(cancellationToken: ct);
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<ApplicationSummaryDto>, Exception>> AllApplicationsSummaryAsync()
    {
        try
        {
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString());
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_GetApplicationsSummaryAsync", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var applications = GetApplicationsSummary(context) ?? throw new Exception("Applications not found");
                    return await applications.ToListAsync();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")));

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<GridApplicationDto>, Exception>> GridApplicationByStatusAsync(int statusId)
    {
        try
        {
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Applications.ToDescriptionString()}_GridApplicationByStatusAsync_{statusId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var applications = GetApplicationGridItemsByStatus(context, statusId) ?? throw new Exception("Applications not found");
                    return (await applications.ToListAsync()).ToList();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")));

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<ClientApplicationsDto, Exception>> ClientApplicationsAsync(int organizationId, int userId, int index)
    {
        try
        {
            if (organizationId == 0) throw new Exception("OrganizationId is required");
            if (userId == 0) throw new Exception("userId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.ApplicationStates.ToDescriptionString()}_ClientApplicationsAsync_{organizationId}_{userId}_{index}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
                    var clientSchemas = GetClientSchemas(context, index) ?? throw new Exception("Schemas not found");
                    var clientApplicationStates = GetClientApplicationStates(context, organizationId, userId, index) ?? throw new Exception("ApplicationStates not found");
                    var clientApplications = new ClientApplicationsDto()
                    {
                        ClientSchemas = await clientSchemas.ToListAsync(),
                        ClientApplicationStates = await clientApplicationStates.ToListAsync()
                    };
                    
                    return clientApplications;
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")));

            return cacheResult.Value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> DeleteApplicationStateAsync(int organizationId, int userId, int applicationId, CancellationToken ct)
    {
        try
        {
            if (organizationId == 0) throw new Exception("applicationStateId is required");
            if (userId == 0) throw new Exception("userId is required");
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.ApplicationStates.ToDescriptionString(), ct);

            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            return await context.ApplicationStates
                .Where(x => x.OrganizationId == organizationId && x.UserId == userId && x.ApplicationId == applicationId)
                .ExecuteDeleteAsync(ct) > 0;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private string InitializeTempFileDirectory()
    {
        var tmpBaseFolder = configuration["DocumentTempFolder"]!;
        var tmpBaseFolderDirectory = new DirectoryInfo(tmpBaseFolder);
        var tmp = "tmp" +
                  DateTime.UtcNow.Year +
                  DateTime.UtcNow.Month +
                  DateTime.UtcNow.Day +
                  DateTime.UtcNow.Hour +
                  DateTime.UtcNow.Minute +
                  DateTime.UtcNow.Second;

        if (tmpBaseFolderDirectory.Exists)
        {
            tmpBaseFolderDirectory.CreateSubdirectory(tmp);
        }
        
        return Path.Combine(tmpBaseFolder, tmp);
    }
    
    private static int DocumentType(ApplicationControlDto control)
    {
        if (control.UniqueId.ToString().EndsWith("000")) return 0; 
        
        var requirementString = control.UniqueId.ToString().Substring(control.UniqueId.ToString().Length - 3, 3);
        _ = int.TryParse(requirementString, out var requirementId);
        return requirementId;
    }
    
    private static void ClearTemporaryFilesFolder(string tempPath)
    {
        var dir = new DirectoryInfo(tempPath);

        if (!dir.Exists) return;

        foreach(var fi in dir.GetFiles())
        {
            fi.Delete();
        }

        foreach (var di in dir.GetDirectories())
        {
            ClearTemporaryFilesFolder(di.FullName);
            di.Delete();
        }

        dir.Delete();
    }
    
    private async Task<string> GetGeneratedClientNumberAsync(CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var exist = await GetGeneratedClientNumber(context);

            if (!exist) return "000001";

            var item = context.Applications.Count() + 1;

            return $"{item:000000}";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    private async Task AggregateApplicationAsync(int applicationId, NotificationType notificationType, CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"<= AggregateApplicationAsync Starting: {applicationId}");
            if (applicationId == 0) return;

            var emptyPayload = new {};
            var httpClient = httpClientFactory.CreateClient("api");

            var url = $"api/v1/jobs/aggregate/applications/{applicationId}/{notificationType}";
            Console.WriteLine($"<= AggregateApplicationAsync URL: {httpClient.BaseAddress}{url}");
            Console.WriteLine($"<= AggregateApplicationAsync httpClient BaseAddress: {httpClient.BaseAddress}");

            var response = await httpClient.PostAsJsonAsync(url, emptyPayload, ct);

            Console.WriteLine($"<= AggregateApplicationAsync Before EnsureSuccessStatusCode: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"<= AggregateApplicationAsync After EnsureSuccessStatusCode: StatusCode={response.StatusCode}");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"<= AggregateApplicationAsync Exception: {ex}");
        }
    }
    
    private async Task AggregateDocumentAsync(int documentId, CancellationToken ct)
    {
        try
        {
            var emptyPayload = new {};
            var httpClient = httpClientFactory.CreateClient("api");
            var response = await httpClient.PostAsJsonAsync($"api/v1/jobs/aggregate/documents/" + documentId, emptyPayload, ct);

            response.EnsureSuccessStatusCode();
        }
        catch
        {
            
        }
    }

    
    private async Task AggregateProjectAsync(int projectId, CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"<= AggregateProjectAsync Starting: {projectId}");
            if (projectId == 0) return;

            var emptyPayload = new {};
            var httpClient = httpClientFactory.CreateClient("api");

            var url = $"api/v1/jobs/aggregate/projects/{projectId}";
            Console.WriteLine($"<= AggregateProjectAsync URL: {httpClient.BaseAddress}{url}");
            Console.WriteLine($"<= AggregateProjectAsync httpClient BaseAddress: {httpClient.BaseAddress}");

            var response = await httpClient.PostAsJsonAsync(url, emptyPayload, ct);

            Console.WriteLine($"<= AggregateProjectAsync Before EnsureSuccessStatusCode: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"<= AggregateProjectAsync After EnsureSuccessStatusCode: StatusCode={response.StatusCode}");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"<= AggregateProjectAsync Exception: {ex}");
        }
    }
    
    private static readonly Func<ApplicationDbContext, Task<bool>> GetGeneratedClientNumber = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetGeneratedClientNumber")
                .Any());
    
    private static readonly Func<ApplicationDbContext, int, Task<Application?>> GetApplicationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetApplicationById")
                .FirstOrDefault(x => x.Id == applicationId));
    
    private static readonly Func<ApplicationDbContext, int, Task<SlimApplication?>> GetSlimApplicationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetSlimApplicationById")
                .Where(x => x.Id == applicationId)
                .Select(x => new SlimApplication()
                {
                    Id = x.Id,
                    SchemaId = x.SchemaId,
                    OrganizationId = x.Organization.ContactIdentifier,
                    Title = x.Title,
                    Controls = x.Controls
                })
                .FirstOrDefault());
    
    private static readonly Func<ApplicationDbContext, int, Task<CounterApplication?>> GetCounterApplicationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetCounterApplicationById")
                .Where(x => x.Id == applicationId)
                .Select(x => new CounterApplication()
                {
                    Id = x.Id,
                    OurContribution = x.OurContribution,
                    NewEventCounter = x.NewAuditCounter,
                    NewAuditCounter = x.NewAuditCounter
                })
                .FirstOrDefault());
    
    private static readonly Func<ApplicationDbContext, int, Task<EconomyApplication?>> GetEconomyApplicationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetEconomyApplicationById")
                .Where(x => x.Id == applicationId)
                .Select(x => new EconomyApplication()
                {
                    Id = x.Id,
                    EarlierSupportTotalAmount = x.EarlierSupportTotalAmount,
                    MilestonePayoutTotalAmount = x.MilestonePayoutTotalAmount,
                    OurContribution = x.OurContribution,
                    InternalBudgetsApproved = x.InternalBudgetsApproved,
                    InternalBudgets = x.InternalBudgets,
                    Controls = x.Controls
                })
                .FirstOrDefault());
    
    private static readonly Func<ApplicationDbContext, int, Task<MediumApplication?>> GetMediumApplicationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetMediumApplicationById")
                .Where(x => x.Id == applicationId)
                .Select(x => new MediumApplication()
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    ProjectNumber = x.ProjectNumber,
                    StatusId = x.StatusId,
                    SchemaId = x.SchemaId,
                    SchemaClaimTag = x.SchemaClaimTag,
                    CreatedDate = x.CreatedDate,
                    DeliveryDate = x.DeliveryDate,
                    ProjectManager = x.ProjectManager,
                    ProductionManager = x.ProductionManager,
                    ContractManager = x.ContractManager,
                    DistributionManager = x.DistributionManager,
                    FinanceManager = x.FinanceManager,
                    ScriptManager = x.ScriptManager,
                    Controls = x.Controls
                })
                .FirstOrDefault());
    
    private static readonly Func<ApplicationDbContext, int, Task<DocumentApplication?>> GetDocumentApplicationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetDocumentApplicationById")
                .Where(x => x.Id == applicationId)
                .Select(x => new DocumentApplication()
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    SchemaId = x.SchemaId,
                    Controls = x.Controls,
                    DecisionDate = x.DecisionDate,
                    SchemaNames = x.SchemaNames,
                    ProjectNumber = x.ProjectNumber,
                    Producer = x.Producer,
                    Organization = x.Organization,
                    ProjectManager = x.ProjectManager,
                    ProductionManager = x.ProductionManager,
                    ScriptManager = x.ScriptManager
                })
                .FirstOrDefault());
    
    private static readonly Func<ApplicationDbContext, int, Task<MiniApplication?>> GetMiniApplicationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetMiniApplicationById")
                .Where(x => x.Id == applicationId)
                .Select(x => new MiniApplication()
                {
                    Id = x.Id,
                    OurContribution = x.OurContribution
                })
                .FirstOrDefault());
    
    private static readonly Func<ApplicationDbContext, int, Task<ReplyApplication?>> GetReplyApplicationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetReplyApplicationById")
                .Where(x => x.Id == applicationId)
                .Select(x => new ReplyApplication()
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    StatusId = x.StatusId,
                    SchemaId = x.SchemaId,
                    ProjectNumber = x.ProjectNumber,
                    Title = x.Title,
                    ProjectManager = x.ProjectManager,
                    Organization = x.Organization
                })
                .FirstOrDefault());
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<ApplicationSummaryDto>> GetApplicationsSummary = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetApplicationsSummary")
                .Where(x => x.StatusId != 19)
                .Select(x => new ApplicationSummaryDto{ Id = x.Id, Name = x.Title }));
    
    private static readonly Func<ApplicationDbContext, int, int, IAsyncEnumerable<ApplicationTabItemDto>> GetApplicationTabItems = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int projectId, int applicationId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetApplicationTabItems")
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.Id)
                .Select(x => new ApplicationTabItemDto{ Id = x.Id, SchemaNames = x.SchemaNames, Selected = (x.Id == applicationId), DeletedOrDenied = (new List<int> {19, 6}.Contains(x.StatusId))}));
    
    
    private static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<GridApplicationDto>> GetApplicationGridItemsByStatus = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int statusId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetApplicationGridItemsByStatus")
                .Where(x => x.StatusId == statusId)
                .Select(x => new GridApplicationDto()
                {
                    Id = x.Id,
                    Title = x.Controls.FirstOrDefault(c => c.UniqueId.ToString().StartsWith("00001001"))!.Value,
                    UpdatedDate = x.UpdatedDate,
                    SchemaNames = x.SchemaNames,
                    OrganizationName = x.Organization.Name
                }));
    
    private static readonly Func<ApplicationDbContext, int, int, int, IAsyncEnumerable<ClientApplicationStateDto>> GetClientApplicationStates = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int organizationId, int userId, int index) => 
            context.ApplicationStates
                .AsNoTracking()
                .TagWith("GetClientApplicationStates")
                .Where(x => x.OrganizationId == organizationId && x.UserId == userId && x.ApplicationId > 0)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new ClientApplicationStateDto(){ Id = x.Id, ApplicationId = x.ApplicationId, Name = x.SchemaNames[index], CreatedDate = x.CreatedDate, TempPath = x.TempPath }));
    
    private static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<ClientSchemaDto>> GetClientSchemas = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int index) => 
            context.Schemas
                .AsNoTracking()
                .TagWith("GetClientSchemas")
                .Where(x => x.Enabled == true)
                .Select(x => new ClientSchemaDto(){ Id = x.Id, Name = x.Names[index]}));

}