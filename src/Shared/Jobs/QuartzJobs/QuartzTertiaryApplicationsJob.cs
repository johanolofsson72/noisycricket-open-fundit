using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Quartz;
using Shared.Data.DbContext;
using Shared.Notifications;
using Shared.Users.Enums;

namespace Shared.Jobs.QuartzJobs;

public class QuartzTertiaryApplicationsJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzTertiaryApplications", "SingleJob");
    
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var cache = scope.ServiceProvider.GetRequiredService<IEasyCachingProvider>();
            var statusLate = new List<int> { 6, 19, 20 };

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzPrimaryApplications started" + $", at: {DateTime.UtcNow:hh:mm:ss}");
        
            // Get Application
            var applications = await context.Applications
                .AsTracking()
                .Where(x => statusLate.Contains(x.StatusId))
                .ToListAsync() ?? throw new Exception($"Applications not found");
            
            var schemaNames = GetSchemaNames(context).ToList();
            var projectNumbers = GetProjectNumbers(context).ToList();
            var organizations = GetOrganizations(context).ToList();
            var applicationsToUpdate = new List<Application>();
            
            foreach (var application in applications)
            {
                // Construct Values
                var organization = organizations.FirstOrDefault(x => x.Id == application.Organization.ContactIdentifier) ?? new OrganizationSummary();
                var producer = new UserSummary();
                if (application.Producer.Name.Length > 0)
                {
                    application.Producer.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.Producer);
                    producer = GetUser(context, application.Producer.ContactIdentifier);
                }
                else
                {
                    var applicationProducer = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("eba1414f"));
                    if (applicationProducer is not null && applicationProducer.Value.Length > 3)
                    {
                        applicationProducer.Value = Regex.Unescape(applicationProducer.Value);
                        var tmp = JsonSerializer.Deserialize<List<ListboxNameEmailPhonenumberGenderDto>>(applicationProducer.Value);
                        if (tmp != null && tmp.Count != 0)
                        {
                            var user = await context.Users
                                .AsNoTracking()
                                .Where(x => x.Email == tmp.First().Email)
                                .Where(x => x.Organizations.Any(o => o.OrganizationIdentifier == organization.Id))
                                .FirstOrDefaultAsync();
                        
                            if (user is not null)
                            {
                                producer = new UserSummary()
                                {
                                    Id = user.Id,
                                    FullName = user.FullName,
                                    Email = user.Email ?? "",
                                    PhoneNumber = user.PhoneNumbers.FirstOrDefault()?.Number ?? ""
                                };
                            }
                            else
                            {
                                producer = new UserSummary()
                                {
                                    Id = 0,
                                    FullName = tmp.First().Name,
                                    Email = tmp.First().Email,
                                    PhoneNumber = tmp.First().Phonenumber
                                };
                            }
                        }
                    }
                }
                var applicant = new UserSummary();
                if (application.Applicant.Name.Length > 0)
                {
                    application.Applicant.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.Applicant);
                    applicant = GetUser(context, application.Applicant.ContactIdentifier);
                }
                var projectManager = new UserSummary();
                if (application.ProjectManager.Name.Length > 0)
                {
                    application.ProjectManager.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.ProjectManager);
                    projectManager = GetUser(context, application.ProjectManager.ContactIdentifier);
                }
                var productionManager = new UserSummary();
                if (application.ProductionManager.Name.Length > 0)
                {
                    application.ProductionManager.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.ProductionManager);
                    productionManager = GetUser(context, application.ProductionManager.ContactIdentifier);
                }
                var financeManager = new UserSummary();
                if (application.FinanceManager.Name.Length > 0)
                {
                    application.FinanceManager.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.FinanceManager);
                    financeManager = GetUser(context, application.FinanceManager.ContactIdentifier);
                }
                var scriptManager = new UserSummary();
                if (application.ScriptManager.Name.Length > 0)
                {
                    application.ScriptManager.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.ScriptManager);
                    scriptManager = GetUser(context, application.ScriptManager.ContactIdentifier);
                }
                var distributionManager = new UserSummary();
                if (application.DistributionManager.Name.Length > 0)
                {
                    application.DistributionManager.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.DistributionManager);
                    distributionManager = GetUser(context, application.DistributionManager.ContactIdentifier);
                }
                var contractManager = new UserSummary();
                if (application.ContractManager.Name.Length > 0)
                {
                    application.ContractManager.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.ContractManager);
                    contractManager = GetUser(context, application.ContractManager.ContactIdentifier);
                }
                var earlierSupportTotalAmount = 0;
                var internalBudgetsTotalAmount = application.InternalBudgets.Sum(x => x.Amount);
                var internalBudgetsApproved = application.InternalBudgets.All(x => x.ApprovedDate > DateTime.MinValue);
                    
                // Set Values
                application.Organization.Name = organization.Name;
                application.ProjectNumber = projectNumbers.FirstOrDefault(x => x.Item1 == application.ProjectId).Item2;
                application.SchemaNames = schemaNames.FirstOrDefault(x => x.Item1 == application.SchemaId).Item2.ToList();
                
                if (producer.Id > 0)
                {
                    application.Producer = new ApplicationContact()
                    {
                        ContactIdentifier = producer.Id,
                        Name = producer.FullName,
                        Email = producer.Email,
                        PhoneNumber = producer.PhoneNumber
                    };
                }
                else
                {
                    application.Producer = new ApplicationContact()
                    {
                        ContactIdentifier = producer.Id,
                        Name = producer.FullName,
                        Email = producer.Email,
                        PhoneNumber = producer.PhoneNumber
                    };
                    application.Producer.ContactIdentifier = await CheckAndAddPersonIfIdIsGreaterThanZero(context, organization, application.Producer);
                }
                if (applicant.Id > 0) application.Applicant = new ApplicationContact()
                {
                    ContactIdentifier = applicant.Id,
                    Name = applicant.FullName,
                    Email = applicant.Email,
                    PhoneNumber = applicant.PhoneNumber
                };
                if (projectManager.Id > 0) application.ProjectManager = new ApplicationContact()
                {
                    ContactIdentifier = projectManager.Id,
                    Name = projectManager.FullName,
                    Email = projectManager.Email,
                    PhoneNumber = projectManager.PhoneNumber
                };  
                if (productionManager.Id > 0) application.ProductionManager = new ApplicationContact()
                {
                    ContactIdentifier = productionManager.Id,
                    Name = productionManager.FullName,
                    Email = productionManager.Email,
                    PhoneNumber = productionManager.PhoneNumber
                };
                if (financeManager.Id > 0) application.FinanceManager = new ApplicationContact()
                {
                    ContactIdentifier = financeManager.Id,
                    Name = financeManager.FullName,
                    Email = financeManager.Email,
                    PhoneNumber = financeManager.PhoneNumber
                };
                if (scriptManager.Id > 0) application.ScriptManager = new ApplicationContact()
                {
                    ContactIdentifier = scriptManager.Id,
                    Name = scriptManager.FullName,
                    Email = scriptManager.Email,
                    PhoneNumber = scriptManager.PhoneNumber
                };
                if (distributionManager.Id > 0) application.DistributionManager = new ApplicationContact()
                {
                    ContactIdentifier = distributionManager.Id,
                    Name = distributionManager.FullName,
                    Email = distributionManager.Email,
                    PhoneNumber = distributionManager.PhoneNumber
                };
                if (contractManager.Id > 0) application.ContractManager = new ApplicationContact()
                {
                    ContactIdentifier = contractManager.Id,
                    Name = contractManager.FullName,
                    Email = contractManager.Email,
                    PhoneNumber = contractManager.PhoneNumber
                };
                application.EarlierSupportTotalAmount = earlierSupportTotalAmount;
                application.InternalBudgetsTotalAmount = internalBudgetsTotalAmount;
                application.InternalBudgetsApproved = internalBudgetsApproved;
                
                var applicationTitle = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00001001"))?.Value;
                if (applicationTitle != null) application.Title = applicationTitle;
                
                var applicationBudget = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00010001"))?.Value;
                _ = decimal.TryParse(applicationBudget, out var applicationBudgetD);
                if (applicationBudget != null && applicationBudgetD > 0) application.BudgetAmount = applicationBudgetD;
                
                var applicationApplied = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00000002"))?.Value;
                _ = decimal.TryParse(applicationApplied, out var applicationAppliedD);
                if (applicationApplied != null && applicationAppliedD > 0) application.AppliedAmount = applicationAppliedD;
                
                var applicationOur = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("01000001"))?.Value;
                _ = decimal.TryParse(applicationOur, out var applicationOurD);
                if (applicationOur != null && applicationOurD > 0) application.OurContribution = applicationOurD;
                
                var applicationProcent = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00100001"));
                if (applicationProcent is not null && applicationBudgetD > 0 && applicationOurD > 0) applicationProcent.Value = (applicationOurD / applicationBudgetD * 100).ToString();

                var applicationDeliveryDate = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("10000001"));
                if (applicationDeliveryDate is not null) applicationDeliveryDate.Value = application.DeliveryDate.ToString("yyyy-MM-dd");
                
                // Save Application
                applicationsToUpdate.Add(application);
                
            }
            
            // Save Applications to database
            context.Applications.UpdateRange(applicationsToUpdate);
            await context.SaveChangesAsync();

            var projectsToUpdate = new List<Project>();
            foreach (var application in applications)
            {
                // Check if Project Exists
                if (application.ProjectId <= 0) continue;
                
                // Get Project Applications
                var projectApplications = GetProjectApplications(context, application.ProjectId).ToList();
                    
                // Get Project
                var project = await context.Projects
                    .AsTracking()
                    .FirstOrDefaultAsync(x => x.Id == application.ProjectId) ?? throw new Exception($"Project: {application.ProjectId} not found for application: {application.Id}");
                    
                // Set Project Values
                project.Applications = projectApplications;
                project.ApplicationCount = projectApplications.Count;
                
                var applicationTitle = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00001001"))?.Value;
                if (applicationTitle != null) application.Title = applicationTitle;

                if (applicationTitle != null && project.Title.First() != applicationTitle)
                {
                    project.Title.Insert(0, applicationTitle);
                }
                    
                var applicationEarlierTitle = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("10000000"))?.Value;
                if (applicationEarlierTitle != null)
                {
                    project.Title.Add(applicationEarlierTitle);
                }
                    
                // Save Project
                projectsToUpdate.Add(project);
            }
            
            // Save Projects to database
            context.Projects.UpdateRange(projectsToUpdate);
            await context.SaveChangesAsync();

            // Clear Cache
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString());

            Console.WriteLine($"<= QuartzTertiaryApplications" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzTertiaryApplications failed " + ex);
        }
    }
    
    private (string, string) SplitPerson(string person)
    {
        try
        {
            var names = person.Split(' ');
            return (names[0], names.Length > 1 ? names[1] : "");
        }
        catch
        {
            return (person, "");
        }
    }

    private async Task<int> CheckAndAddPersonIfIdIsGreaterThanZero(ApplicationDbContext context, OrganizationSummary organization, ApplicationContact person)
    {
        if (person.ContactIdentifier != 0) return person.ContactIdentifier;
        
        var names = SplitPerson(person.Name);
        
        var user = new User
        {
            StatusId = 1,
            FirstName = names.Item1,
            LastName = names.Item2,
            Organizations =
            [
                new UserOrganization()
                {
                    OrganizationIdentifier = organization.Id,
                    OrganizationName = organization.Name,
                    OrganizationVat = organization.Vat,
                    IsAdministrator = false
                }
            ],
            Type = UserType.Client,
            EmailConfirmed = true
        };
        
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }
    
    private class OrganizationSummary
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Vat { get; set; } = string.Empty;
    }

    private class UserSummary
    {
        public int Id { get; set; } = 0;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
    
    private static readonly Func<ApplicationDbContext, IEnumerable<(int, IEnumerable<string>)>> GetSchemaNames = 
        EF.CompileQuery((ApplicationDbContext context) => 
            context.Schemas
                .AsNoTracking()
                .TagWith("GetSchemaNames")
                .Select(x => new ValueTuple<int, IEnumerable<string>>() { Item1 = x.Id, Item2 = x.Names }));
    
    private static readonly Func<ApplicationDbContext, int, IEnumerable<string>> GetSchemaName = 
        EF.CompileQuery((ApplicationDbContext context, int schemaId) => 
            context.Schemas
                .AsNoTracking()
                .TagWith("GetSchemaName")
                .Where(x => x.Id == schemaId)
                .Select(x => x.Names)
                .FirstOrDefault() ?? new List<string>());
    
    private static readonly Func<ApplicationDbContext, IEnumerable<(int, string)>> GetProjectNumbers = 
        EF.CompileQuery((ApplicationDbContext context) => 
            context.Projects
                .AsNoTracking()
                .TagWith("GetProjectNumbers")
                .Select(x => new ValueTuple<int, string>() { Item1 = x.Id, Item2 = x.Number }));
    
    private static readonly Func<ApplicationDbContext, int, string> GetProjectNumber = 
        EF.CompileQuery((ApplicationDbContext context, int projectId) => 
            context.Projects
                .AsNoTracking()
                .TagWith("GetProjectNumber")
                .Where(x => x.Id == projectId)
                .Select(x => x.Number)
                .FirstOrDefault() ?? string.Empty);
    
    private static readonly Func<ApplicationDbContext, IEnumerable<OrganizationSummary>> GetOrganizations = 
        EF.CompileQuery((ApplicationDbContext context) => 
            context.Organizations
                .AsNoTracking()
                .TagWith("GetOrganizations")
                .Select(x => new OrganizationSummary { Id = x.Id, Name = x.Name, Vat = x.Vat }));
    
    private static readonly Func<ApplicationDbContext, int, OrganizationSummary> GetOrganization = 
        EF.CompileQuery((ApplicationDbContext context, int organizationId) => 
            context.Organizations
                .AsNoTracking()
                .TagWith("GetOrganization")
                .Where(x => x.Id == organizationId)
                .Select(x => new OrganizationSummary { Id = x.Id, Name = x.Name, Vat = x.Vat })
                .FirstOrDefault() ?? new OrganizationSummary { Id = 0, Name = "" });
    
    private static readonly Func<ApplicationDbContext, IEnumerable<UserSummary>> GetUsers = 
        EF.CompileQuery((ApplicationDbContext context) => 
            context.Users
                .AsNoTracking()
                .TagWith("GetUsers")
                .Select(x => new UserSummary { Id = x.Id, FullName = x.FirstName + " " + x.LastName, Email = x.Email ?? "", PhoneNumber = x.PhoneNumber ?? "" }));
    
    private static readonly Func<ApplicationDbContext, int, UserSummary> GetUser = 
        EF.CompileQuery((ApplicationDbContext context, int userId) => 
            context.Users
                .AsNoTracking()
                .TagWith("GetUser")
                .Where(x => x.Id == userId)
                .Select(x => new UserSummary { Id = x.Id, FullName = x.FirstName + " " + x.LastName, Email = x.Email ?? "", PhoneNumber = x.PhoneNumber ?? "" })
                .FirstOrDefault() ?? new UserSummary { Id = 0, FullName = "", Email = "", PhoneNumber = ""});
    
    private static readonly Func<ApplicationDbContext, int, int, decimal> GetOurContribution =
        EF.CompileQuery((ApplicationDbContext context, int applicationId, int projectId) =>
            context.Applications
                .AsNoTracking()
                .TagWith("GetOurContribution")
                .Where(x => x.Id != applicationId && x.ProjectId == projectId)
                .Select(x => x.OurContribution)
                .Sum());
    
    private static readonly Func<ApplicationDbContext, int[], IConfiguration, IEnumerable<(int, ProjectApplication)>> GetProjectApplicationsBulk =
        EF.CompileQuery((ApplicationDbContext context, int[] projectIds, IConfiguration configuration) =>
            context.Applications
                .AsNoTracking()
                .TagWith("GetProjectApplicationsBulk")
                .Where(x => projectIds.Contains(x.ProjectId))
                .Select(x => new ValueTuple<int, ProjectApplication>() 
                { 
                    Item1 = x.Id, 
                    Item2 = new ProjectApplication 
                    {
                        ApplicationIdentifier = x.Id,
                        ApplicationStatusId = x.StatusId,
                        ApplicationSchemaId = x.SchemaId,
                        ApplicationSchemaNames = x.SchemaNames,
                        ApplicationTitle = x.Title,
                        ApplicationProducer = new ApplicationContact(){ ContactIdentifier = x.Producer.ContactIdentifier, Name = x.Producer.Name, Email = x.Producer.Email, PhoneNumber = x.Producer.PhoneNumber },
                        ApplicationApplicant = new ApplicationContact(){ ContactIdentifier = x.Applicant.ContactIdentifier, Name = x.Applicant.Name, Email = x.Applicant.Email, PhoneNumber = x.Applicant.PhoneNumber },
                        ApplicationProjectManager= new ApplicationContact(){ ContactIdentifier = x.ProjectManager.ContactIdentifier, Name = x.ProjectManager.Name, Email = x.ProjectManager.Email, PhoneNumber = x.ProjectManager.PhoneNumber },
                        ApplicationProductionManager = new ApplicationContact(){ ContactIdentifier = x.ProductionManager.ContactIdentifier, Name = x.ProductionManager.Name, Email = x.ProductionManager.Email, PhoneNumber = x.ProductionManager.PhoneNumber },
                        ApplicationFinanceManager = new ApplicationContact(){ ContactIdentifier = x.FinanceManager.ContactIdentifier, Name = x.FinanceManager.Name, Email = x.FinanceManager.Email, PhoneNumber = x.FinanceManager.PhoneNumber },
                        ApplicationScriptManager = new ApplicationContact(){ ContactIdentifier = x.ScriptManager.ContactIdentifier, Name = x.ScriptManager.Name, Email = x.ScriptManager.Email, PhoneNumber = x.ScriptManager.PhoneNumber },
                        ApplicationDistributionManager = new ApplicationContact(){ ContactIdentifier = x.DistributionManager.ContactIdentifier, Name = x.DistributionManager.Name, Email = x.DistributionManager.Email, PhoneNumber = x.DistributionManager.PhoneNumber },
                        ApplicationContractManager = new ApplicationContact(){ ContactIdentifier = x.ContractManager.ContactIdentifier, Name = x.ContractManager.Name, Email = x.ContractManager.Email, PhoneNumber = x.ContractManager.PhoneNumber },
                        ApplicationCreatedDate = x.CreatedDate
                    }
                }));
    
    private static readonly Func<ApplicationDbContext, int, IEnumerable<ProjectApplication>> GetProjectApplications =
        EF.CompileQuery((ApplicationDbContext context, int projectId) =>
            context.Applications
                .AsNoTracking()
                .TagWith("GetProjectApplications")
                .Where(x => x.ProjectId == projectId)
                .Select(x => new ProjectApplication 
                {
                    ApplicationIdentifier = x.Id,
                    ApplicationStatusId = x.StatusId,
                    ApplicationSchemaId = x.SchemaId,
                    ApplicationSchemaNames = x.SchemaNames,
                    ApplicationTitle = x.Title,
                    ApplicationProducer = new ApplicationContact(){ ContactIdentifier = x.Producer.ContactIdentifier, Name = x.Producer.Name, Email = x.Producer.Email, PhoneNumber = x.Producer.PhoneNumber },
                    ApplicationApplicant = new ApplicationContact(){ ContactIdentifier = x.Applicant.ContactIdentifier, Name = x.Applicant.Name, Email = x.Applicant.Email, PhoneNumber = x.Applicant.PhoneNumber },
                    ApplicationProjectManager= new ApplicationContact(){ ContactIdentifier = x.ProjectManager.ContactIdentifier, Name = x.ProjectManager.Name, Email = x.ProjectManager.Email, PhoneNumber = x.ProjectManager.PhoneNumber },
                    ApplicationProductionManager = new ApplicationContact(){ ContactIdentifier = x.ProductionManager.ContactIdentifier, Name = x.ProductionManager.Name, Email = x.ProductionManager.Email, PhoneNumber = x.ProductionManager.PhoneNumber },
                    ApplicationFinanceManager = new ApplicationContact(){ ContactIdentifier = x.FinanceManager.ContactIdentifier, Name = x.FinanceManager.Name, Email = x.FinanceManager.Email, PhoneNumber = x.FinanceManager.PhoneNumber },
                    ApplicationScriptManager = new ApplicationContact(){ ContactIdentifier = x.ScriptManager.ContactIdentifier, Name = x.ScriptManager.Name, Email = x.ScriptManager.Email, PhoneNumber = x.ScriptManager.PhoneNumber },
                    ApplicationDistributionManager = new ApplicationContact(){ ContactIdentifier = x.DistributionManager.ContactIdentifier, Name = x.DistributionManager.Name, Email = x.DistributionManager.Email, PhoneNumber = x.DistributionManager.PhoneNumber },
                    ApplicationContractManager = new ApplicationContact(){ ContactIdentifier = x.ContractManager.ContactIdentifier, Name = x.ContractManager.Name, Email = x.ContractManager.Email, PhoneNumber = x.ContractManager.PhoneNumber },
                    ApplicationCreatedDate = x.CreatedDate
                }));
}