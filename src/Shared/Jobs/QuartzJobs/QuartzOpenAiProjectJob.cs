using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Quartz;
using Shared.Data.DbContext;
using Shared.Global.Entities;
using Shared.OpenAi.Entities;
using SmartComponents.LocalEmbeddings;
using Telerik.Blazor.Components;

namespace Shared.Jobs.QuartzJobs;

public class QuartzOpenAiProjectJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzOpenAiProject", "SingleJob");

    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        Console.WriteLine("<= QuartzOpenAiProjectJob starting...");
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var embedder = scope.ServiceProvider.GetRequiredService<LocalEmbedder>();
            var projectService = scope.ServiceProvider.GetRequiredService<ProjectService>();
            var data = jobExecutionContext.JobDetail.JobDataMap;
            var projectId = data.GetInt("projectId");

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzOpenAiProjects started with projectId: {projectId}" + $", at: {DateTime.UtcNow:hh:mm:ss}");
        
            // Delete this project from OpenAiProjects
            await context.OpenAiProjects.Where(x => x.ProjectId == projectId).ExecuteDeleteAsync();
            
            // Get all applications for this project
            var applications = (GetProjectApplications(context, projectId)).ToList();
            
            // Get all statuses
            var statuses = (GetStatuses(context)).ToList();
            
            // Generate OpenAi keys from all applications
            foreach (var application in applications)
            {
                var project = GetAiProject(context, application.ProjectId);
                
                if (project == null) continue;
                
                project.ProjectTitles.Add(application.Title);
                
                foreach (var title in project.ProjectTitles.Where(x => x.Length > 0))
                {
                    var projectTitle = WebUtility.HtmlDecode(title);
                    var projectNumber = project.ProjectNumber;
                    var projectManagerName = application.ProjectManager.Name;
                    var applicantName = application.Applicant.Name;
                    var producerName = application.Producer.Name;
                    var companyName = project.Organization.OrganizationName;
                    var applicationId = application.Id;
                    var schemaNames = application.SchemaNames;
                    var statusNames = statuses.FirstOrDefault(x => x.Id == application.StatusId)?.Names ?? [];
                    var appliedAmount = application.AppliedAmount;
                    var budgetAmount = application.BudgetAmount;
                    var ourContribution = application.OurContribution;
                    var datas = GenerateDataObjectFromApplication(application);
                    var expireDate = DateTime.Now.AddMinutes(5);
                    
                    var openAiProject = new OpenAiProject()
                    {
                        ProjectId = projectId,
                        ProjectTitle = projectTitle,
                        ProjectNumber = projectNumber,
                        ProjectManagerName = projectManagerName,
                        ApplicantName = applicantName,
                        ProducerName = producerName,
                        CompanyName = companyName,
                        ApplicationId = applicationId,
                        SchemaNames = schemaNames,
                        StatusNames = statusNames,
                        AppliedAmount = appliedAmount,
                        BudgetAmount = budgetAmount,
                        OurContribution = ourContribution,
                        Data = datas,
                        ExpireDate = expireDate
                    };
                    
                    context.OpenAiProjects.Add(openAiProject);
                }
            }
            
            // Save changes
            await context.SaveChangesAsync();
            
            // Create embedding data
            await CreateEmbeddingDataAsync(context, embedder);
            
            // Reset index
            await projectService.ResetIndex();
            
            Console.WriteLine($"<= QuartzOpenAiProjects succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzOpenAiProjects failed " + ex);
        }
    }
    
    private static async Task CreateEmbeddingDataAsync(ApplicationDbContext context, LocalEmbedder embedder)
    {
        var count = 0;
        while (true)
        {
            var projects = await context.OpenAiProjects
                .AsTracking()
                .Where(x => x.Embedding == null)
                .OrderBy(x => x.Id)
                .Take(100)
                .ToListAsync();

            if (projects.Count == 0) break;

            Parallel.ForEach(projects, project =>
            {
                var data = $"1: '{project.ProjectTitle}' " +
                           $"2: '{project.ProjectNumber}' " +
                           $"3: '{project.ProjectManagerName}' " +
                           $"4: '{project.ApplicantName}' " +
                           $"5: '{project.ProducerName}' " +
                           $"6: '{project.CompanyName}' " +
                           $"7: '{string.Join(", ", project.SchemaNames)}'" +
                           $"8: '{string.Join(", ", project.StatusNames)}'";
                
                data = project.Data
                    .Where(x => 
                        x.Value.Contains("System.Collections.Generic") == false && 
                        x.Value.Length > 2 && 
                        x.Value.IsDate() == false
                        ).Aggregate(data, (current, item) => current + (", " + item.Value));
                data = Regex.Unescape(Regex.Replace(data, @"\b(\w+)\s+\1\b", "$1"));
                
                Console.WriteLine($"data: {data}");
                project.Embedding = embedder.Embed<EmbeddingF32>(data).Buffer.ToArray();
            });

            await context.SaveChangesAsync();
            count += projects.Count;
            Console.WriteLine($@"Added embedding to {count} projects.");
        }
    }
    private List<OpenAiProjectData> GenerateDataObjectFromApplication(Application application)
    {
        var openAiProjectData = new List<OpenAiProjectData>();
        
        openAiProjectData.Add(
            new OpenAiProjectData()
            {
                Key = "producer-producent",
                Value = application.Producer.Name
            });

        foreach (var control in application.Controls.ToList())
        {
            control.Value = Regex.Unescape(control.Value);
            if (control.Value == "") continue;
            
            var key = control.Labels.First();

            var value = control.ControlTypeName switch
            {
                "Upload" => JsonSerializer.Deserialize<List<UploadFileInfo>>(control.Value)?
                    .Aggregate(string.Empty, (current, file) => current + ("Filename: " + file.Name + "; ")),
                "Listbox-Name-Email-Age" => JsonSerializer.Deserialize<List<ListboxNameEmailAgeDto>>(control.Value)?
                    .Aggregate(string.Empty, (current, item) => current + ("Name: " + item.Name + ", Email:" + item.Email + ", Age: " + item.Age + "; ")),
                "Listbox-Days-Location" => "Days: " + JsonSerializer.Deserialize<ListboxDaysLocationDto>(control.Value)?.Days +
                                           ", Location: " + JsonSerializer.Deserialize<ListboxDaysLocationDto>(control.Value)?.Location,
                "Listbox-Name-PricesReceived-Attended" => JsonSerializer
                    .Deserialize<List<ListboxNamePricesReceivedAttendedDto>>(control.Value)?
                    .Aggregate(string.Empty, (current, item) => current + ("Name: " + item.Name + ", Prices: " + item.Prices + ", Attended: " + item.Attended + "; ")),
                "Listbox-Name-Email-Phonenumber-Gender" => JsonSerializer
                    .Deserialize<List<ListboxNameEmailPhonenumberGenderDto>>(control.Value)?
                    .Aggregate(string.Empty, (current, item) => current + ("Name: " + item.Name + ", Email: " + item.Email + ", Phone: " + item.Phonenumber + ", Gender: " + item.Gender + "; ")),
                "Listbox-Name-Gender" => JsonSerializer.Deserialize<List<ListboxNameGenderDto>>(control.Value)?
                    .Aggregate(string.Empty, (current, item) => current + ("Name: " + item.Name + ", Gender: " + item.Gender + "; ")),
                _ => control.Value
            };
            
            if (value != null && value.EndsWith("; ")) value = value.Substring(0, value.Length - 2);

            if (value != null)
                openAiProjectData.Add(
                    new OpenAiProjectData()
                    {
                        Key = key,
                        Value = value
                    });
        }
        
        openAiProjectData.Add(new OpenAiProjectData() { Key = "Skapad", Value = application.CreatedDate.ToString("yyyy-MM-dd") });
        openAiProjectData.Add(new OpenAiProjectData() { Key = "Uppdaterad", Value = application.UpdatedDate.ToString("yyyy-MM-dd") });
        
        return openAiProjectData;
    }

    private string CreateUploadFileInfoObject(string value)
    {
        var uploadFileInfo = JsonSerializer.Deserialize<List<UploadFileInfo>>(value);

        if (uploadFileInfo != null)
        {
            var ret = uploadFileInfo.Aggregate("; ", (current, file) => current + ("Filename: " + file.Name));

            return ret;
        }

        return string.Empty;
    }

    private void GenerateOpenAiKeysFromAllApplications(IEnumerable<Application> applications, IConfiguration configuration)
    {
        var keys = new Dictionary<string, string>();
        foreach (var application in applications)
        {
            foreach (var control in application.Controls)
            {
                var value = control.ControlTypeName switch
                {
                    "Upload" => JsonSerializer.Serialize(new List<UploadFileInfo> { new() { Name = "file.txt", Extension = ".txt" }, new() { Name = "another.pdf", Extension = ".pdf" } }),
                    "Listbox-Name-Email-Age" => JsonSerializer.Serialize(new List<ListboxNameEmailAgeDto> { new() { Name = "John Doe", Email = "john.doe@gmail.com", Age = 55 }, new() { Name = "Jane Doe", Email = "jane.doe@gmail.com", Age = 67 } }),
                    "Listbox-Days-Location" => JsonSerializer.Serialize(new List<ListboxDaysLocationDto> { new() { Days = 13, Location = "New York" }, new() { Days = 6, Location = "Los Angeles" } }),
                    "Listbox-Name-PricesReceived-Attended" => JsonSerializer.Serialize(new List<ListboxNamePricesReceivedAttendedDto> { new() { Name = "Cannes", Prices = "fina priset", Attended = true }, new() { Name = "Filmfestivalen", Prices = "", Attended = false } }),
                    "Listbox-Name-Email-Phonenumber-Gender" => JsonSerializer.Serialize(new List<ListboxNameEmailPhonenumberGenderDto> { new() { Name = "John Doe", Email = "john.doe@gmail.com", Phonenumber = "12345678", Gender = "man" }, new() { Name = "Jane Doe", Email = "jane.doe@gmail.com", Phonenumber = "87654321", Gender = "kvinna" } }),
                    "Listbox-Name-Gender" => JsonSerializer.Serialize(new List<ListboxNameGenderDto> { new() { Name = "John Doe", Gender = "man" }, new() { Name = "Jane Doe", Gender = "kvinna" }}),
                    _ => "string"
                };

                if (control.Labels.Count > 0 && !string.IsNullOrEmpty(value) && !keys.ContainsKey(control.Labels.First()))
                    keys.Add(control.Labels.First(), value);
            }
        }
        
        // GeneratedKeysPath
        var keyFile = configuration.GetValue<string>("OpenAi:GeneratedKeysPath")!;
        var sb = new StringBuilder();
        foreach (var item in keys)
        {
            if (sb.Length > 0)
            {
                sb.Append(',');
            }
            sb.Append('"');
            sb.Append(item.Key);
            sb.Append('"');
            sb.Append(':');
            sb.Append('"');
            sb.Append(item.Value);
            sb.Append('"');
        }
        var text = sb.ToString();
        File.WriteAllText(keyFile, text);
    }
    private class AiProject
    {
        public int ProjectId { get; set; }
        public List<string> ProjectTitles { get; set; } = [];
        public string ProjectNumber { get; set; } = string.Empty;
        public ProjectOrganization Organization { get; set; } = new ();
    }
    
    private static readonly Func<ApplicationDbContext, int, IEnumerable<Application>> GetProjectApplications = 
        EF.CompileQuery((ApplicationDbContext context, int projectId) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetProjectApplications")
                .Where(x => x.ProjectId == projectId));
    
    private static readonly Func<ApplicationDbContext, IEnumerable<Application>> GetApplications = 
        EF.CompileQuery((ApplicationDbContext context) => 
            context.Applications
                .AsNoTracking()
                .TagWith("GetApplications")
                .Where(x => x.Id != 0));
    
    private static readonly Func<ApplicationDbContext, IEnumerable<Status>> GetStatuses =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Statuses
                .AsNoTracking()
                .TagWith("GetStatuses"));
    
    private static readonly Func<ApplicationDbContext, int, AiProject?> GetAiProject =
        EF.CompileQuery((ApplicationDbContext context, int projectId) =>
            context.Projects
                .AsNoTracking()
                .TagWith("GetAiProject")
                .Where(x => x.Id == projectId)
                .Select(x => new AiProject()
                {
                    ProjectId = x.Id,
                    ProjectNumber = x.Number,
                    ProjectTitles = x.Title,
                    Organization = x.Organization
                }).FirstOrDefault());
}