using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bogus.Hollywood;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;
using Shared.Applications.Entities;
using Shared.Controls.Entities;
using Shared.Data.DbContext;
using Shared.Documents.Entities;
using Shared.Global.Entities;
using Shared.Jobs.QuartzJobs;
using Shared.Milestones.Entities;
using Shared.Organizations.Entities;
using Shared.Statistics.Entities;
using Xunit.Abstractions;

namespace Server.Test.Helpers;

public class Movie
{
    public int id { get; set; } = 0;
    public string title { get; set; } = "";
}

public class MovieCatalog
{
    public List<Movie> movies { get; set; } = [];
}

public static class Database
{
    public static async Task InitializeTestDatabase(WebApplicationFactory<Program> factory, string dbName, ITestOutputHelper _testOutputHelper)
    {
        try
        {
            _testOutputHelper.WriteLine($"{DateTime.Now}: Initializing test database at {dbName}");
            CreateTestDatabase(factory);
            CreateSystemInfo(factory);
            CreateStatistics(factory);
            SeedControls(factory);
            var schemas = Schemas.SeedSchemas(factory);
            ReorderControls.Execute(factory);
            var adminRoleId = AdminRole.Create(factory);
            var userRoleId = UserRole.Create(factory);
            var (organizationId, organizationNumber) = Organizations.Create(factory);
            var password = "YOUR_TEST_PASSWORD";
            var p1 = Users.Create(factory,"Test", "Admin", "admin@yourdomain.com", password, ["employee", "NUA", "ADM", "CFO", "PG", "CEO", "AA", "IFA", "KFA", "DFA", "KFA", "TVA", "SFA", "PR", "PRK", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p2 = Users.Create(factory,"Test", "User", "user@yourdomain.com", password, ["employee", "CFO", "NUA", "PG", "CEO", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p3 = Users.Create(factory,"Test", "Manager", "manager@yourdomain.com", password, ["employee", "CFO", "PG", "AA", "ADM", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p4 = Users.Create(factory,"Test", "Staff", "staff@yourdomain.com", password, ["employee", "IFA", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p5 = Users.Create(factory,"Test", "Lead", "lead@yourdomain.com", password, ["employee", "PRK", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p6 = Users.Create(factory,"Bobba", "Fett", "bobba.fett@funditbyus.com", password, ["employee", "DK", "DA", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p7 = Users.Create(factory,"R2", "D2", "r2.d2@funditbyus.com", password, ["employee", "DK", "EA", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p8 = Users.Create(factory,"Darth", "Maul", "darth.maul@funditbyus.com", password, ["employee", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p9 = Users.Create(factory,"Lando", "Calrissian", "lando.calrissian@funditbyus.com", password, ["employee", "IFA", "KFA", "DFA", "PR", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p10 = Users.Create(factory,"Kylo", "Ren", "kylo.ren@funditbyus.com", password, ["employee", "KFA", "TVA", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p11 = Users.Create(factory,"Padmé", "Amidala", "padme.amidala@funditbyus.com", password, ["employee", "PC", "SFA", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p12 = Users.Create(factory,"Qui-Gon", "Jinn", "qui-gon.jinn@funditbyus.com", password, ["employee", "MK", "PRK", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p13 = Users.Create(factory,"Poe", "Dameron", "poe.dameron@funditbyus.com", password, ["employee", "MK", "PRK", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p14 = Users.Create(factory,"Ahsoka", "Tano", "ahsoka.tano@funditbyus.com", password, ["employee", "SFA", "TVA", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p15 = Users.Create(factory,"Mace", "Windu", "mace.windu@funditbyus.com", password, ["employee", "AA", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p16 = Users.Create(factory,"Nien", "Numb", "nien.numb@funditbyus.com", password, ["employee", "DFA", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p17 = Users.Create(factory,"BB", "8", "bb.8@funditbyus.com", password, ["employee", "ADM", "PK", "ALL"], organizationId, organizationNumber, adminRoleId);
            var p18 = Users.Create(factory,"Greve", "Dooku", "greve.dooku@funditbyus.com", password, ["employee", "P"], organizationId, organizationNumber, adminRoleId);
            
            (organizationId, organizationNumber) = Organizations.CreateForCustomer(factory);
            var applicant = Users.CreateForCustomer(factory,"Han", "Solo", "jool@me.com", password, organizationId, organizationNumber, userRoleId);

            /*
            var catalog = JsonConvert.DeserializeObject<MovieCatalog>(File.ReadAllText("/Users/jool/repos/noisycricket-fundit/Solution/tests/Server.Test/Helpers/random_movie_titles.json"));
            var rnd = new Random();
            
            foreach (var schema in schemas)
            {
                //var titel = new Bogus.Faker().Company.CompanyName() + " the movie";
                if (catalog != null)
                {
                    var index = rnd.Next(catalog.movies.Count);
                    var titel = catalog.movies[index].title;
                    var number = Projects.NextProjectNumber(factory);
                    var (projectId, applicationId) = await Projects.CreateApplication(factory, organizationId, applicant, p11, p6, p3, schema, number, titel);
                
                
                    //await Projects.AddDocuments(factory, applicationId, schema.Id);

                
                    Projects.UpdateProject(factory, projectId, applicationId, schema, titel, number, organizationId);
                }

                //await Aggregate(factory, organizationId, projectId, applicationId);
            }*/
            
            await Aggregate(factory, 0, 0, 0);
            //CleanUp(factory);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _testOutputHelper.WriteLine(ex.Message);
        }
        
        Assert.Equal(1, 1);
        _testOutputHelper.WriteLine($"{DateTime.Now}: Done testing database at {dbName}");
    }
    
    private static async Task Aggregate(WebApplicationFactory<Program> factory, int organizationId, int projectId, int applicationId)
    {
        var scope = factory.Services.CreateScope();
        var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler(new CancellationToken());
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        try
        {
 
            // QuartzStatisticsJob
            var quartzStatisticsJob = JobBuilder.Create<QuartzStatisticsJob>()
                .WithIdentity($"QuartzStatisticsJob", "SingleJob")
                .Build();

            var quartzStatisticsTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzStatisticsTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzStatisticsJob, quartzStatisticsTrigger, new CancellationToken());
            
            // QuartzUsersJob
            var quartzUsersJob = JobBuilder.Create<QuartzUsersJob>()
                .WithIdentity($"QuartzUsersJob", "SingleJob")
                .Build();
            
            var quartzUsersTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzUsersTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzUsersJob, quartzUsersTrigger, new CancellationToken());
            
            // QuartzOpenAiUsersJob
            var quartzOpenAiUsersJob = JobBuilder.Create<QuartzOpenAiUsersJob>()
                .WithIdentity($"QuartzOpenAiUsersJob", "SingleJob")
                .Build();
            
            var quartzOpenAiUsersTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzOpenAiUsersTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzOpenAiUsersJob, quartzOpenAiUsersTrigger, new CancellationToken());
            
            // QuartzMessagesJob
            var quartzMessagesJob = JobBuilder.Create<QuartzMessagesJob>()
                .WithIdentity($"QuartzMessagesJob", "SingleJob")
                .Build();
            
            var quartzMessagesTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzMessagesTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzMessagesJob, quartzMessagesTrigger, new CancellationToken());
            
            // QuartzMilestonesJob
            var quartzMilestonesJob = JobBuilder.Create<QuartzMilestonesJob>()
                .WithIdentity($"QuartzMilestonesJob", "SingleJob")
                .Build();
            
            var quartzMilestonesTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzMilestonesTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzMilestonesJob, quartzMilestonesTrigger, new CancellationToken());
            
            // QuartzOrganizationsJob
            var quartzOrganizationsJob = JobBuilder.Create<QuartzOrganizationsJob>()
                .WithIdentity($"QuartzOrganizationsJob", "SingleJob")
                .Build();
            
            var quartzOrganizationsTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzOrganizationsTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzOrganizationsJob, quartzOrganizationsTrigger, new CancellationToken());
            
            // QuartzDocumentsJob
            var quartzDocumentsJob = JobBuilder.Create<QuartzDocumentsJob>()
                .WithIdentity($"QuartzDocumentsJob", "SingleJob")
                .Build();
            
            var quartzDocumentsTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzDocumentsTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzDocumentsJob, quartzDocumentsTrigger, new CancellationToken());
            
            // QuartzPrimaryApplicationsJob
            var quartzPrimaryApplicationsJob = JobBuilder.Create<QuartzPrimaryApplicationsJob>()
                .WithIdentity($"QuartzPrimaryApplicationsJob", "SingleJob")
                .Build();
            
            var quartzPrimaryApplicationsTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzPrimaryApplicationsTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzPrimaryApplicationsJob, quartzPrimaryApplicationsTrigger, new CancellationToken());
            
            // QuartzSecondaryApplicationsJob
            var quartzSecondaryApplicationsJob = JobBuilder.Create<QuartzSecondaryApplicationsJob>()
                .WithIdentity($"QuartzSecondaryApplicationsJob", "SingleJob")
                .Build();
            
            var quartzSecondaryApplicationsTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzSecondaryApplicationsTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzSecondaryApplicationsJob, quartzSecondaryApplicationsTrigger, new CancellationToken());
            
            // QuartzTertiaryApplicationsJob
            var quartzTertiaryApplicationsJob = JobBuilder.Create<QuartzTertiaryApplicationsJob>()
                .WithIdentity($"QuartzTertiaryApplicationsJob", "SingleJob")
                .Build();
            
            var quartzTertiaryApplicationsTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzTertiaryApplicationsTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzTertiaryApplicationsJob, quartzTertiaryApplicationsTrigger, new CancellationToken());
            
            // QuartzProjectsJob
            var quartzProjectsJob = JobBuilder.Create<QuartzProjectsJob>()
                .WithIdentity($"QuartzProjectsJob", "SingleJob")
                .Build();
            
            var quartzProjectsTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzProjectsTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzProjectsJob, quartzProjectsTrigger, new CancellationToken());
            
            // QuartzTranslationsJob
            /*var quartzTranslationsJob = JobBuilder.Create<QuartzTranslationsJob>()
                .WithIdentity($"QuartzTranslationsJob", "SingleJob")
                .Build();
            
            var quartzTranslationsTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzTranslationsTrigger", "SingleTrigger")
                .StartNow()
                               .Build();

            await scheduler.ScheduleJob(quartzTranslationsJob, quartzTranslationsTrigger, new CancellationToken());*/
            
            // QuartzCleanUpJob
            var quartzCleanUpJob = JobBuilder.Create<QuartzCleanUpJob>()
                .WithIdentity($"QuartzCleanUpJob", "SingleJob")
                .Build();
            
            var quartzCleanUpTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzCleanUpTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzCleanUpJob, quartzCleanUpTrigger, new CancellationToken());
            
            // QuartzDatabaseBackupJob
            var quartzDatabaseBackupJob = JobBuilder.Create<QuartzDatabaseBackupJob>()
                .WithIdentity($"QuartzDatabaseBackupJob", "SingleJob")
                .Build();
            
            var quartzDatabaseBackupTrigger = TriggerBuilder.Create()
                .WithIdentity($"QuartzDatabaseBackupTrigger", "SingleTrigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(quartzDatabaseBackupJob, quartzDatabaseBackupTrigger, new CancellationToken());
                
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    private static void CleanUp(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Projects.Where(x => x.Id > 0).ExecuteDelete();
        context.Applications.ExecuteUpdate(x => x
            .SetProperty(p => p.ProjectId, 0)
            .SetProperty(p => p.ProjectNumber, ""));
    }
    
    private static string CreateFakeEmailAddress()
    {
        var faker = new Bogus.Faker();
        var firstName = faker.Name.FirstName();
        var middleName = faker.Random.Int(10, 99).ToString();
        var lastName = faker.Name.LastName();
        
        return $"{firstName.ToLower()}.{lastName.ToLower()}{middleName.ToLower()}@funditbyus.com";
    }

    public static bool CreateTestDatabase(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // ❌ 1️⃣ Ta bort databasen först
        context.Database.EnsureDeleted();

        // 🔥 2️⃣ Skapa en ny `DbContext`-instans som pekar på `master`
        var masterOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=10.211.55.2;Database=master;User Id=sa;Password=D4yCru!s3r2025!;TrustServerCertificate=True;")
            .Options;

        using (var masterContext = new ApplicationDbContext(masterOptions))
        {
            masterContext.Database.ExecuteSqlRaw("CREATE DATABASE fundit COLLATE Finnish_Swedish_CI_AS;");
        }

        // ✅ 3️⃣ Vänta tills SQL Server har skapat databasen
        System.Threading.Thread.Sleep(3000); // Vänta 3 sekunder

        // 🔄 4️⃣ Hämta en ny `DbContext`-instans med den NYA databasen
        context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // 💾 5️⃣ Skapa tabellerna i den nya databasen
        context.Database.EnsureCreated();

        // 🔄 Lägg till en dummy-rad i Projects för att undvika seed-fel
        context.Database.ExecuteSqlRaw(@"
            SET IDENTITY_INSERT Projects ON;
            INSERT INTO Projects (Id, StatusId, Number, Title, ApplicationCount, UpdatedDate, CreateDate, Applications, Organization)
            VALUES (0, 0, 0, '[]', 0, '0001-01-01 00:00:00', '0001-01-01 00:00:00', '[]', '[]');
            SET IDENTITY_INSERT Projects OFF;
        ");

        // Create indexes
        context.Database.ExecuteSqlRaw("CREATE INDEX Application_ProjectId_index ON Applications(ProjectId);");
        context.Database.ExecuteSqlRaw("CREATE INDEX Application_StatusId_index ON Applications(StatusId);");
        context.Database.ExecuteSqlRaw("CREATE INDEX Projects_idx_00535779 ON Projects(Number);");
        context.Database.ExecuteSqlRaw("CREATE INDEX Messages_idx_1bbaa8f1 ON Messages(StatusId);");
        context.Database.ExecuteSqlRaw("CREATE INDEX Messages_idx_a6ca473e ON Messages(StatusId, ApplicationId);");
        context.Database.ExecuteSqlRaw("CREATE INDEX Messages_idx_29d85d37 ON Messages(ProjectId, ApplicationId, EventId, EventTypeId, Title);");
        context.Database.ExecuteSqlRaw("CREATE INDEX Milestones_idx_0c4d1936 ON Milestones(ApplicationId, StatusId);");
        context.Database.ExecuteSqlRaw("CREATE INDEX ApplicationStates_idx_cb69e717 ON ApplicationStates(OrganizationId, UserId, ApplicationId);");
        context.Database.ExecuteSqlRaw("CREATE INDEX Documents_idx_6e3116fa ON Documents(ApplicationId, RequirementTypeId, StatusId);");
        
        
        return true;
    }
    
    public static void CreateSystemInfo(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var currency = new Currency()
        {
            Name = "SEK",
            Rate = 1,
            CreatedDate = DateTime.UtcNow
        };
        context.Currencies.Add(currency);
        
        context.SaveChanges();
    }
    
    // Kör alla jobb
    // Testa sedan sql-frågor mot OpenAiProjects tabellen
    // Skriv in dessa frågor här nedanför
    
    // 1 -> 5 är klara
    
    public static void CreateStatistics(WebApplicationFactory<Program> factory)
    {
        try
        {

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var statistics = new List<Statistic>
        {
            new Statistic()
            {
                Name = "Vårt bidrag detta året", 
                Description = "OurContributionThisYear", 
                Query = "SELECT SUM(p.OurContribution) AS OurContribution\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Uppdaterad'\n  AND JSON_VALUE(a.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Produktionsbolag med flest stödda projekt ", 
                Description = "ProductionCompanySupportedProjects", 
                Query = "SELECT TOP 5 p.CompanyName, COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Uppdaterad'\n  AND JSON_VALUE(a.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n  AND JSON_VALUE(p.StatusNames, '$[0]') NOT IN ('Avslag', 'History', 'Bordlagt', 'Ej komplett')\nGROUP BY p.CompanyName\nORDER BY Count DESC;\n", 
                Columns = 2,
                Rows = 2,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Budgetvärde detta året", 
                Description = "TotalBudgetThisYear", 
                Query = "SELECT ISNULL(SUM(CAST(JSON_VALUE(a.[value], '$.Value') AS DECIMAL)), 0) AS Sum\nFROM OpenAiProjects\nCROSS APPLY OPENJSON(Data) AS a\nCROSS APPLY OPENJSON(Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Filmens totala budget' \n  AND JSON_VALUE(a.[value], '$.Value') > 0\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Könsfördelning producenter", 
                Description = "ProducerGenders", 
                Query = "SELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nCROSS APPLY OPENJSON(p.Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Producenter' \n  AND LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%man%')\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n\nUNION ALL\n\nSELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nCROSS APPLY OPENJSON(p.Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Producenter' \n  AND (LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%kvinna%') \n       OR LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%woman%'))\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n\nUNION ALL\n\nSELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nCROSS APPLY OPENJSON(p.Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Producenter' \n  AND (LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%båda%') \n       OR LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%both%'))\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n\nUNION ALL\n\nSELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nCROSS APPLY OPENJSON(p.Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Producenter' \n  AND (LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%annat%') \n       OR LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%other%'))\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n", 
                Columns = 2,
                Rows = 2,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            { 
                Name = "Vårt bidrag förra året", 
                Description = "OurContributionLastYear", 
                Query = "SELECT SUM(p.OurContribution) AS OurContribution\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Uppdaterad'\n  AND JSON_VALUE(a.[value], '$.Value') = FORMAT(DATEADD(YEAR, -1, GETDATE()), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "SEK",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Budgetvärde förra året", 
                Description = "TotalBudgetLastYear", 
                Query = "SELECT ISNULL(SUM(CAST(JSON_VALUE(a.[value], '$.Value') AS DECIMAL)), 0) AS Sum\nFROM OpenAiProjects\nCROSS APPLY OPENJSON(Data) AS a\nCROSS APPLY OPENJSON(Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Filmens totala budget' \n  AND JSON_VALUE(a.[value], '$.Value') > 0\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(DATEADD(YEAR, -1, GETDATE()), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "SEK",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Projektstöd detta året", 
                Description = "SupportedProjectsThisYear", 
                Query = "SELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nWHERE JSON_VALUE(p.StatusNames, '$[0]') NOT IN ('Avslag', 'History', 'Bordlagt', 'Ej komplett')\n  AND JSON_VALUE(a.[value], '$.Key') = 'Uppdaterad'\n  AND JSON_VALUE(a.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Senaste inkomna dokument", 
                Description = "LatestIncomingDocuments", 
                Query = "SELECT a.Title AS ApplicationTitle, \n       d.FileName AS DocumentTitle, \n       d.CreatedDate, \n       a.ProjectId, \n       a.Id AS ApplicationId, \n       d.Id AS DocumentId\nFROM Documents d\nJOIN Applications a ON a.Id = d.ApplicationId\nLEFT JOIN Users u1 ON u1.Id = JSON_VALUE(a.ProjectManager, '$.ContactIdentifier')\nLEFT JOIN Users u2 ON u2.Id = JSON_VALUE(a.ProductionManager, '$.ContactIdentifier')\nLEFT JOIN Users u3 ON u3.Id = JSON_VALUE(a.FinanceManager, '$.ContactIdentifier')\nLEFT JOIN Users u4 ON u4.Id = JSON_VALUE(a.ContractManager, '$.ContactIdentifier')\nLEFT JOIN Users u5 ON u5.Id = JSON_VALUE(a.DistributionManager, '$.ContactIdentifier')\nLEFT JOIN Users u6 ON u6.Id = JSON_VALUE(a.ScriptManager, '$.ContactIdentifier')\nWHERE (u1.Id = #userId# OR u2.Id = #userId# OR u3.Id = #userId# OR u4.Id = #userId# OR u5.Id = #userId# OR u6.Id = #userId#)\n  AND d.DocumentTypeId NOT IN (59, 60, 70, 74, 80)\nORDER BY d.CreatedDate DESC\nOFFSET 0 ROWS FETCH NEXT 3 ROWS ONLY\n", 
                Columns = 3,
                Rows = 1,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Könsfördelning regissörer", 
                Description = "DirectorGenders", 
                Query = "SELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nCROSS APPLY OPENJSON(p.Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Regissörer' \n  AND LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%man%')\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n\nUNION ALL\n\nSELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nCROSS APPLY OPENJSON(p.Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Regissörer' \n  AND (LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%kvinna%') \n       OR LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%woman%'))\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n\nUNION ALL\n\nSELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nCROSS APPLY OPENJSON(p.Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Regissörer' \n  AND (LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%båda%') \n       OR LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%both%'))\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n\nUNION ALL\n\nSELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nCROSS APPLY OPENJSON(p.Data) AS b\nWHERE JSON_VALUE(a.[value], '$.Key') = 'Regissörer' \n  AND (LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%annat%') \n       OR LOWER(JSON_VALUE(a.[value], '$.Value')) LIKE LOWER('%other%'))\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n", 
                Columns = 2,
                Rows = 2,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Projektstöd förra året", 
                Description = "SupportedProjectsLastYear", 
                Query = "SELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nWHERE JSON_VALUE(p.StatusNames, '$[0]') NOT IN ('Avslag', 'History', 'Bordlagt', 'Ej komplett')\n  AND JSON_VALUE(a.[value], '$.Key') = 'Uppdaterad'\n  AND JSON_VALUE(a.[value], '$.Value') = FORMAT(DATEADD(YEAR, -1, GETDATE()), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Senaste inkomna meddelanden", 
                Description = "LatestIncomingMessages", 
                Query = "SELECT Title, ProjectTitle, ExpireDate\nFROM Messages\nWHERE JSON_VALUE(Receiver, '$.ContactIdentifier') = #userId#\nORDER BY Id DESC\nOFFSET 0 ROWS FETCH NEXT 3 ROWS ONLY\n", 
                Columns = 3,
                Rows = 1,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Senaste interna dokument", 
                Description = "LatestInternalDocuments", 
                Query = "SELECT a.Title AS ApplicationTitle, \n       d.FileName AS DocumentTitle, \n       d.CreatedDate, \n       a.ProjectId, \n       a.Id AS ApplicationId, \n       d.Id AS DocumentId\nFROM Documents d\nJOIN Applications a ON a.Id = d.ApplicationId\nLEFT JOIN Users u1 ON u1.Id = JSON_VALUE(a.ProjectManager, '$.ContactIdentifier')\nLEFT JOIN Users u2 ON u2.Id = JSON_VALUE(a.ProductionManager, '$.ContactIdentifier')\nLEFT JOIN Users u3 ON u3.Id = JSON_VALUE(a.FinanceManager, '$.ContactIdentifier')\nLEFT JOIN Users u4 ON u4.Id = JSON_VALUE(a.ContractManager, '$.ContactIdentifier')\nLEFT JOIN Users u5 ON u5.Id = JSON_VALUE(a.DistributionManager, '$.ContactIdentifier')\nLEFT JOIN Users u6 ON u6.Id = JSON_VALUE(a.ScriptManager, '$.ContactIdentifier')\nWHERE (u1.Id = #userId# OR u2.Id = #userId# OR u3.Id = #userId# OR u4.Id = #userId# OR u5.Id = #userId# OR u6.Id = #userId#)\n  AND d.DocumentTypeId IN (59, 60, 70, 74, 80)\nORDER BY d.CreatedDate DESC\nOFFSET 0 ROWS FETCH NEXT 3 ROWS ONLY\n", 
                Columns = 3,
                Rows = 1,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Senaste utgående meddelanden", 
                Description = "LatestOutgoingMessages", 
                Query = "SELECT m.Title, m.ProjectTitle, m.ExpireDate\nFROM Messages AS m\nJOIN Users AS u ON u.Id = JSON_VALUE(m.Receiver, '$.ContactIdentifier')\nWHERE u.Type = 1\nORDER BY m.Id DESC\nOFFSET 0 ROWS FETCH NEXT 3 ROWS ONLY\n", 
                Columns = 3,
                Rows = 1,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Ansökningar de senaste tre åren", 
                Description = "ActivityTheLastThreeYears", 
                Query = "WITH Months AS (\n    -- Start from January of last year\n    SELECT DATEADD(YEAR, -2, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) AS MonthStart\n    UNION ALL\n    -- Add one month at a time\n    SELECT DATEADD(MONTH, 1, MonthStart)\n    FROM Months\n    -- Continue until the end of this year\n    WHERE DATEADD(MONTH, 1, MonthStart) <= DATEADD(YEAR, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1))\n)\nSELECT\n    FORMAT(MonthStart, 'yyyy-MM') AS Month,\n    COUNT(A.Id) AS NumberOfApplications\nFROM\n    Months\n        LEFT JOIN Applications A\n                  ON FORMAT(A.CreatedDate, 'yyyy-MM') = FORMAT(MonthStart, 'yyyy-MM')\nGROUP BY\n    MonthStart\nOPTION (MAXRECURSION 100);\n", 
                Columns = 6,
                Rows = 3,
                Unit = "",
                IsPublic = false
            },
            new Statistic()
            {
                Name = "Meddelanden", 
                Description = "Messages", 
                Query = "", 
                Columns = 4,
                Rows = 4,
                Unit = "",
                IsPublic = true
            },
            new Statistic()
            {
                Name = "Assistenten", 
                Description = "Assistent", 
                Query = "", 
                Columns = 2,
                Rows = 5,
                Unit = "",
                IsPublic = true
            },
            new Statistic()
            {
                Name = "Projektstöd förra året", 
                Description = "SupportedProjectsLastYear", 
                Query = "SELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nWHERE LOWER(p.CompanyName) = LOWER('#organizationName#') \n  AND JSON_VALUE(p.StatusNames, '$[0]') NOT IN ('Avslag', 'History', 'Bordlagt', 'Ej komplett')\n  AND JSON_VALUE(a.[value], '$.Key') = 'Uppdaterad'\n  AND JSON_VALUE(a.[value], '$.Value') = FORMAT(DATEADD(YEAR, -1, GETDATE()), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "",
                IsPublic = true
            },
            new Statistic()
            {
                Name = "Projektstöd detta året", 
                Description = "SupportedProjectsThisYear", 
                Query = "SELECT COUNT(*) AS Count\nFROM OpenAiProjects AS p\nCROSS APPLY OPENJSON(p.Data) AS a\nWHERE LOWER(p.CompanyName) = LOWER('#organizationName#') \n  AND JSON_VALUE(p.StatusNames, '$[0]') NOT IN ('Avslag', 'History', 'Bordlagt', 'Ej komplett')\n  AND JSON_VALUE(a.[value], '$.Key') = 'Uppdaterad'\n  AND JSON_VALUE(a.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "",
                IsPublic = true
            },
            new Statistic()
            {
                Name = "Budgetvärde detta året", 
                Description = "TotalBudgetThisYear", 
                Query = "SELECT ISNULL(SUM(CAST(JSON_VALUE(a.[value], '$.Value') AS DECIMAL)), 0) AS Sum\nFROM OpenAiProjects\nCROSS APPLY OPENJSON(Data) AS a\nCROSS APPLY OPENJSON(Data) AS b\nWHERE LOWER(OpenAiProjects.CompanyName) = LOWER('#organizationName#') \n  AND JSON_VALUE(a.[value], '$.Key') = 'Filmens totala budget' \n  AND JSON_VALUE(a.[value], '$.Value') > 0\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(GETDATE(), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "",
                IsPublic = true
            },
            new Statistic()
            {
                Name = "Budgetvärde förra året", 
                Description = "TotalBudgetLastYear", 
                Query = "SELECT ISNULL(SUM(CAST(JSON_VALUE(a.[value], '$.Value') AS DECIMAL)), 0) AS Sum\nFROM OpenAiProjects\nCROSS APPLY OPENJSON(Data) AS a\nCROSS APPLY OPENJSON(Data) AS b\nWHERE LOWER(OpenAiProjects.CompanyName) = LOWER('#organizationName#') \n  AND JSON_VALUE(a.[value], '$.Key') = 'Filmens totala budget' \n  AND JSON_VALUE(a.[value], '$.Value') > 0\n  AND JSON_VALUE(b.[value], '$.Key') = 'Uppdaterad' \n  AND JSON_VALUE(b.[value], '$.Value') = FORMAT(DATEADD(YEAR, -1, GETDATE()), 'yyyy')\n", 
                Columns = 1,
                Rows = 1,
                Unit = "SEK",
                IsPublic = true
            },
            new Statistic()
            {
                Name = "Ansökningar de senaste tre åren", 
                Description = "ActivityTheLastThreeYears", 
                Query = "WITH Months AS (\n    -- Start from January of last year\n    SELECT DATEADD(YEAR, -2, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) AS MonthStart\n    UNION ALL\n    -- Add one month at a time\n    SELECT DATEADD(MONTH, 1, MonthStart)\n    FROM Months\n    -- Continue until the end of this year\n    WHERE DATEADD(MONTH, 1, MonthStart) <= DATEADD(YEAR, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1))\n)\nSELECT\n    FORMAT(MonthStart, 'yyyy-MM') AS Month,\n    COUNT(A.Id) AS NumberOfApplications\nFROM\n    Months\n        LEFT JOIN Applications A\n                  ON FORMAT(A.CreatedDate, 'yyyy-MM') = FORMAT(MonthStart, 'yyyy-MM') \n                  AND LOWER(JSON_VALUE(A.Organization, '$.Name')) = LOWER('#organizationName#')\nGROUP BY\n    MonthStart\nOPTION (MAXRECURSION 100);\n", 
                Columns = 6,
                Rows = 3,
                Unit = "",
                IsPublic = true
            }
        };
        
        context.Statistics.AddRange(statistics);
        
        context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static bool SeedControls(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            var metrics = new List<Control>();

            metrics.Add(
                new Control()
                {
                    ControlTypeId = 2,
                    ControlTypeName =  "Textbox",
                    ValueType = "string",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 3,
                    ControlTypeName =  "Multiline-Textbox",
                    ValueType = "string",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 4,
                    ControlTypeName =  "Numeric-Textbox",
                    ValueType =  "int",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 5,
                    ControlTypeName =  "Dropdown",
                    ValueType =  "int",
                    BaseStructure = "[{\"value\": 0,\"text\": \"\"}]"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 6,
                    ControlTypeName =  "Checkbox",
                    ValueType =  "bool",
                    BaseStructure = "false"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 7,
                    ControlTypeName =  "Radio",
                    ValueType =  "string",
                    BaseStructure = "[{\"value\": \"\",\"text\": \"\"}]"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 8,
                    ControlTypeName =  "Multiselect",
                    ValueType =  "string[]",
                    BaseStructure = "[{\"value\": \"\",\"text\": \"\"}]"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 9,
                    ControlTypeName =  "Listbox-Name-Email-Age",
                    ValueType =  "json",
                    BaseStructure =
                        "[{\"name\": {\"type\": \"textbox\",\"base\": \"\"  },\"email\": {\"type\": \"textbox\",\"base\": \"\"  },\"age\": {\"type\": \"numeric-textbox\",\"base\": 0  }}]"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 10,
                    ControlTypeName =  "Listbox-Days-Location",
                    ValueType =  "json",
                    BaseStructure =
                        "[{\"days\": {\"type\": \"numeric-textbox\",\"base\": 0  },\"location\": {\"type\": \"textbox\",\"base\": \"\"  }}]"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 11,
                    ControlTypeName =  "Listbox-Name-PricesReceived-Attended",
                    ValueType =  "json",
                    BaseStructure =
                        "[{\"name\": {\"type\": \"textbox\",\"base\": \"\"  },\"pricesreceived\": {\"type\": \"numeric-textbox\",\"base\": 0  },\"attended\": {\"type\": \"checkbox\",\"base\": false  }}]"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 12,
                    ControlTypeName =  "Listbox-Name-Email-Phonenumber-Gender",
                    ValueType =  "json",
                    BaseStructure =
                        "[{\"name\": {\"type\": \"textbox\",\"base\": \"string\"  },\"email\": {\"type\": \"textbox\",\"base\": \"string\"  },\"phonenumber\": {\"type\": \"textbox\",\"base\": \"string\"  },\"gender\": {\"type\": \"radio\",\"base\": [  {    \"value\": \"Male\",    \"text\": \"Male\"  },  {    \"value\": \"Female\",    \"text\": \"Female\"  },  {    \"value\": \"Other\",    \"text\": \"Other\"  }]  }}]"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 13,
                    ControlTypeName =  "Upload",
                    ValueType =  "string[]",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 14,
                    ControlTypeName =  "Listbox-Name-Gender",
                    ValueType =  "json",
                    BaseStructure =
                        "[{\"name\": {\"type\": \"textbox\",\"base\": \"string\"  },\"gender\": {\"type\": \"radio\",\"base\": [  {    \"value\": \"Male\",    \"text\": \"Male\"  },  {    \"value\": \"Female\",    \"text\": \"Female\"  },  {    \"value\": \"Other\",    \"text\": \"Other\"  }]  }}]"
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 15,
                    ControlTypeName =  "Header",
                    ValueType =  "string",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 16,
                    ControlTypeName =  "Date",
                    ValueType =  "date",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 17,
                    ControlTypeName =  "Hour-Minute-Numeric-Textbox",
                    ValueType =  "hour-minute",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 18,
                    ControlTypeName =  "Date-From-To",
                    ValueType =  "date",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 19,
                    ControlTypeName =  "Button",
                    ValueType =  "date",
                    BaseStructure = ""
                }
            );
            metrics.Add(
                new Control()
                {
                    ControlTypeId = 20,
                    ControlTypeName =  "Date-Approved",
                    ValueType =  "date",
                    BaseStructure = ""
                }
            );
            context.Controls.AddRange(metrics);
            
            var genders = new List<Gender>
            {
                new() 
                { 
                    Names = new List<string> 
                    {
                        "Man", "Man", "Mand", "Mann", "Hombre", "Homme", "Uomo", "Mann" 
                    } 
                },
                new() 
                { 
                    Names = new List<string> 
                    {
                        "Kvinna", "Woman", "Kvinde", "Frau", "Mujer", "Femme", "Donna", "Kvinne" 
                    } 
                },
                new() 
                { 
                    Names = new List<string> 
                    {
                        "Annat", "Other", "Andet", "Anderes", "Otro", "Autre", "Altro", "Annet" 
                    } 
                },
                new() 
                { 
                    Names = new List<string> 
                    {
                        "Båda", "Both", "Begge", "Beide", "Ambos", "Tous les deux", "Entrambi", "Begge" 
                    } 
                }
            };
            context.Genders.AddRange(genders);
            
            var applicationBudgetTypes = new List<ApplicationBudgetType>
            {
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Default", "Default", "Standard", "Standard", "Por defecto", "Par défaut", "Default", "Standard" } },
                new() { Names = new List<string> { "Förproduktion", "Pre-production", "Forproduktion", "Vorproduktion", "Preproducción", "Pré-production", "Pre-produzione", "Forproduksjon" } },
                new() { Names = new List<string> { "Upphovsrätt", "Copyright", "Ophavsret", "Urheberrecht", "Copyright", "Droit d'auteur", "Copyright", "Opphavsrett" } },
                new() { Names = new List<string> { "Inspelning", "Recording", "Optagelse", "Aufnahme", "Grabación", "Enregistrement", "Registrazione", "Opptak" } },
                new() { Names = new List<string> { "Postproduktion", "Post-production", "Postproduktion", "Postproduktion", "Postproducción", "Post-production", "Post-produzione", "Etterproduksjon" } },
                new() { Names = new List<string> { "Marknad, Export och Övrigt", "Marketing, Export, and Miscellaneous", "Marked, eksport og diverse", "Marketing, Export und Sonstiges", "Marketing, exportación y varios", "Marketing, exportation et divers", "Marketing, exportazione e vari", "Markedsføring, eksport og diverse" } },
                new() { Names = new List<string> { "Contingency / Reserv", "Contingency / Reserve", "Kontingens / Reserve", "Kontingenz / Reserve", "Contingencia / Reserva", "Contingence / Réserve", "Contingenza / Riserva", "Kontingent / Reserve" } }
            };

            context.ApplicationBudgetTypes.AddRange(applicationBudgetTypes);
            
            var sections = new List<Section>
            {
                new () { Order = 1, Enabled = false, Names = new List<string> { "Standard", "Default", "Standard", "Standard", "Defecto", "Par défaut", "Standard", "Standard" } },
                new () { Order = 3, Enabled = false, Names = new List<string> { "Sökande", "Applicant", "Ansøger", "Antragsteller", "Solicitante", "Candidat", "Richiedente", "Søker" } },
                new () { Order = 5, Enabled = false, Names = new List<string> { "Budget", "Budget", "Budget", "Budget", "Presupuesto", "Budget", "Bilancio", "Budsjett" } },
                new () { Order = 7, Enabled = false, Names = new List<string> { "Bilagor", "Attachments", "Bilag", "Anhänge", "Anexos", "Pièces jointes", "Allegati", "Vedlegg" } },
                new () { Order = 9, Enabled = false, Names = new List<string> { "Summering / Verifiering", "Summary / Verification", "Opsummering / Bekræftelse", "Zusammenfassung / Verifizierung", "Resumen / Verificación", "Résumé / Vérification", "Riepilogo / Verifica", "Sammendrag / Verifisering" } },
                new () { Order = 13, Enabled = true, Names = new List<string> { "Info", "Info", "Info", "Info", "Información", "Infos", "Informazioni", "Info" } },
                new () { Order = 15, Enabled = true, Names = new List<string> { "Kontaktuppgifter", "Contact Information", "Kontaktoplysninger", "Kontaktinformation", "Información de contacto", "Information de contact", "Informazioni di contatto", "Kontaktinformasjon" } },
                new () { Order = 17, Enabled = true, Names = new List<string> { "LOC & Avtal", "LOC & Agreements", "LOC & Aftaler", "LOC & Vereinbarungen", "LOC & Acuerdos", "LOC et Accords", "LOC e Accordi", "LOC og Avtaler" } },
                new () { Order = 19, Enabled = true, Names = new List<string> { "Finansiering & Spend", "Financing & Spend", "Finansiering & Udgifter", "Finanzierung & Ausgaben", "Financiación & Gastos", "Financement & Dépenses", "Finanziamento & Spese", "Finansiering & Utgifter" } },
                new () { Order = 21, Enabled = true, Names = new List<string> { "Kortsynopsis & POV", "Short Synopsis & POV", "Kort synopsis & POV", "Kurzzusammenfassung & POV", "Sinopsis corta & POV", "Résumé court & POV", "Sinossi breve & POV", "Kort synopses & POV" } },
                new () { Order = 23, Enabled = true, Names = new List<string> { "Inspelning & Postproduktion", "Recording & Post-production", "Optagelse & Postproduktion", "Aufnahme & Postproduktion", "Grabación & Postproducción", "Enregistrement & Post-production", "Registrazione & Post-produzione", "Opptak & Etterproduksjon" } },
                new () { Order = 25, Enabled = true, Names = new List<string> { "PR & Kommunikation", "PR & Communication", "PR & Kommunikation", "PR & Kommunikation", "PR & Comunicación", "PR & Communication", "PR & Comunicazione", "PR & Kommunikasjon" } },
                new () { Order = 27, Enabled = true, Names = new List<string> { "Premiär, Publik & Festival", "Premiere, Audience & Festival", "Premiere, Publikum & Festival", "Premiere, Publikum & Festival", "Estreno, Público & Festival", "Première, Public & Festival", "Première, Pubblico & Festival", "Premiere, Publik & Festival" } },
                new () { Order = 29, Enabled = false, Names = new List<string> { "LOC", "LOC", "LOC", "LOC", "LOC", "LOC", "LOC", "LOC" } },
                new () { Order = 31, Enabled = false, Names = new List<string> { "Avslagsbrev", "Rejection Letter", "Afslagbrev", "Ablehnungsbrief", "Carta de rechazo", "Lettre de rejet", "Lettera di rifiuto", "Avslagsbrev" } },
                new () { Order = 33, Enabled = false, Names = new List<string> { "Underlag", "Basis", "Underlag", "Unterlagen", "Base", "Base", "Base", "Grunnlag" } },
                new () { Order = 35, Enabled = false, Names = new List<string> { "Uppladdade dokument", "Uploaded Documents", "Uploadede dokumenter", "Hochgeladene Dokumente", "Documentos subidos", "Documents téléchargés", "Documenti caricati", "Opplastede dokumenter" } },
                new () { Order = 37, Enabled = false, Names = new List<string> { "Publiksiffror", "Audience Numbers", "Publikumstal", "Zuschauerzahlen", "Números de audiencia", "Chiffres d'audience", "Numeri del pubblico", "Publikumstall" } },
                new () { Order = 39, Enabled = false, Names = new List<string> { "Festivaler", "Festivals", "Festivaler", "Festivals", "Festivales", "Festivals", "Festival", "Festivaler" } },
                new () { Order = 11, Enabled = true, Names = new List<string> { "Ansökan", "Application", "Anvendelse", "Application", "Application", "Application", "Applicazione", "Søknad" } }
            };

            context.Sections.AddRange(sections);
            
            
            var documentDeliveryTypes = new List<DocumentDeliveryType>
            {
                new () { Names = new List<string> { "Standard", "Default", "Standard", "Standard", "Defecto", "Par défaut", "Standard", "Standard" } },
                new () { Names = new List<string> { "Uppladdad av klient", "Uploaded by client", "Uploadet af klient", "Hochgeladen vom Kunden", "Subido por el cliente", "Téléchargé par le client", "Caricato dal cliente", "Lastet opp av klient" } },
                new () { Names = new List<string> { "Uppladdad av admin", "Uploaded by admin", "Uploadet af admin", "Hochgeladen von Admin", "Subido por el administrador", "Téléchargé par l'administrateur", "Caricato dall'amministratore", "Lastet opp av administrator" } }
            };
            context.DocumentDeliveryTypes.AddRange(documentDeliveryTypes);
            
            
            
            
            var actionTypes = new List<ActionType>
            {
                new () { 
                    Names = new List<string> 
                    { 
                        "Standard", 
                        "Default", 
                        "Standard", 
                        "Standard", 
                        "Predefinito", 
                        "Predefinito", 
                        "Forhåndsinnstilt" 
                    } 
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Meddelande", 
                        "Message", 
                        "Besked", 
                        "Nachricht", 
                        "Mensaje", 
                        "Message", 
                        "Messaggio", 
                        "Melding" 
                    } 
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "E-post", 
                        "Email", 
                        "E-mail", 
                        "E-Mail", 
                        "Correo electrónico", 
                        "E-mail", 
                        "E-mail", 
                        "E-post" 
                    } 
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Statusuppdatering", 
                        "Status update", 
                        "Statusopdatering", 
                        "Statusaktualisierung", 
                        "Actualización de estado", 
                        "Mise à jour du statut", 
                        "Aggiornamento di stato", 
                        "Statusoppdatering" 
                    } 
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Radera meddelande", 
                        "Delete message", 
                        "Slet besked", 
                        "Nachricht löschen", 
                        "Eliminar mensaje", 
                        "Supprimer le message", 
                        "Elimina messaggio", 
                        "Slette melding" 
                    } 
                }
            };
            context.ActionTypes.AddRange(actionTypes);
            
            var claimTypes = new List<ClaimType>
            {
                new () { 
                    Names = new List<string> 
                    { 
                        "Standard", 
                        "Default", 
                        "Standard", 
                        "Standard", 
                        "Predefinito", 
                        "Predefinito", 
                        "Forhåndsinnstilt" 
                    },
                    Tag = "DEF"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Verkställande direktör", 
                        "Chief Executive Officer", 
                        "Administrerende direktør", 
                        "Geschäftsführer", 
                        "Director ejecutivo", 
                        "Directeur général", 
                        "Amministratore delegato", 
                        "Administrerende direktør" 
                    },
                    Tag = "CEO"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Finanschef", 
                        "Chief Financial Officer", 
                        "Økonomidirektør", 
                        "Finanzchef", 
                        "Director financiero", 
                        "Directeur financier", 
                        "Direttore finanziario", 
                        "Økonomidirektør" 
                    },
                    Tag = "CFO"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Produktionsgruppen", 
                        "Production Group", 
                        "Produktionsgruppe", 
                        "Produktionsgruppe", 
                        "Grupo de producción", 
                        "Groupe de production", 
                        "Gruppo di produzione", 
                        "Produksjonsgruppent" 
                    },
                    Tag = "PG"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Produktionschef", 
                        "Production Manager", 
                        "Produktionschef", 
                        "Produktionsleiter", 
                        "Gerente de producción", 
                        "Responsable de la production", 
                        "Responsabile della produzione", 
                        "Produksjonssjef" 
                    },
                    Tag = "PC"
                },
                
                new () { 
                    Names = new List<string> 
                    { 
                        "Svensk filmansvarig", 
                        "Swedish Film Manager", 
                        "Svensk film ansvarlig", 
                        "Verantwortlicher für Schwedische Filme", 
                        "Responsable de películas suecas", 
                        "Responsable des films suédois", 
                        "Responsabile dei film svedesi", 
                        "Ansvarlig for svensk film" 
                    },
                    Tag = "SFA"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Internationell filmansvarig", 
                        "International Film Manager", 
                        "International film ansvarlig", 
                        "Verantwortlicher für internationale Filme", 
                        "Responsable de películas internacionales", 
                        "Responsable des films internationaux", 
                        "Responsabile dei film internazionali", 
                        "Ansvarlig for internasjonale filmer" 
                    },
                    Tag = "IFA"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Kortfilmsansvarig", 
                        "Short Film Manager", 
                        "Kortfilm ansvarlig", 
                        "Kurzfilm-Verantwortlicher", 
                        "Responsable de cortometrajes", 
                        "Responsable des courts métrages", 
                        "Responsabile dei cortometraggi", 
                        "Ansvarlig for kortfilmer" 
                    },
                    Tag = "KFA"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Dokumentärfilmsansvarig", 
                        "Documentary Film Manager", 
                        "Dokumentarfilm ansvarlig", 
                        "Verantwortlicher für Dokumentarfilme", 
                        "Responsable de películas documentales", 
                        "Responsable des films documentaires", 
                        "Responsabile dei film documentari", 
                        "Ansvarlig for dokumentarfilmer" 
                    },
                    Tag = "DFA"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Dramaserieansvarig", 
                        "TV Drama Series Manager", 
                        "Drama serie ansvarlig", 
                        "Verantwortlicher für TV-Dramaserien", 
                        "Responsable de series dramáticas de TV", 
                        "Responsable des séries dramatiques TV", 
                        "Responsabile delle serie drammatiche televisive", 
                        "Ansvarlig for TV-drama-serier" 
                    },
                    Tag = "TVA"
                },
                                
                new () { 
                    Names = new List<string> 
                    { 
                        "Ekonomiansvarig", 
                        "Chief Financial Officer", 
                        "Økonomidirektør", 
                        "Finanzchef", 
                        "Director financiero", 
                        "Directeur financier", 
                        "Direttore finanziario", 
                        "Økonomidirektør" 
                    },
                    Tag = "EA"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Diarieansvarig", 
                        "Registry Manager", 
                        "Registreringsansvarlig", 
                        "Registerverantwortlicher", 
                        "Responsable de registros", 
                        "Responsable des registres", 
                        "Responsabile della cassa", 
                        "Registeransvarlig" 
                    },
                    Tag = "DA"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Public relations/kommunikation", 
                        "Public Relations/Communication", 
                        "Public Relations/Kommunikation", 
                        "Öffentlichkeitsarbeit/Kommunikation", 
                        "Relaciones públicas/Comunicación", 
                        "Relations publiques/Communication", 
                        "Relazioni pubbliche/Comunicazione", 
                        "PR/Kommunikasjon" 
                    },
                    Tag = "PRK"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Planeringschef", 
                        "Planning Manager", 
                        "Planlægningschef", 
                        "Planungsleiter", 
                        "Gerente de planificación", 
                        "Responsable de la planification", 
                        "Responsabile della pianificazione", 
                        "Planleggingssjef" 
                    },
                    Tag = "PLC"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Avtalsansvarig", 
                        "Contract Manager", 
                        "Kontraktansvarlig", 
                        "Vertragsverantwortlicher", 
                        "Gerente de contrato", 
                        "Responsable des contrats", 
                        "Responsabile dei contratti", 
                        "Kontraktsansvarlig" 
                    },
                    Tag = "AA"
                },
                                
                                new () { 
                    Names = new List<string> 
                    { 
                        "Manuskonsult", 
                        "Script Consultant", 
                        "Manuskonsulent", 
                        "Drehbuchberater", 
                        "Consultor de guion", 
                        "Consultant en scénario", 
                        "Consulente di sceneggiatura", 
                        "Manuskonsulent" 
                    },
                    Tag = "MK"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Produktionskoordinator", 
                        "Production Coordinator", 
                        "Produktionskoordinator", 
                        "Produktionskoordinator", 
                        "Coordinador de producción", 
                        "Coordinateur de production", 
                        "Coordinatore di produzione", 
                        "Produksjonskoordinator" 
                    },
                    Tag = "PK"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Distributionskonsult", 
                        "Distribution Consultant", 
                        "Distributionskonsulent", 
                        "Distributionsberater", 
                        "Consultor de distribución", 
                        "Consultant en distribution", 
                        "Consulente di distribuzione", 
                        "Distribusjonskonsulent" 
                    },
                    Tag = "DK"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Public relationsansvarig", 
                        "Public Relations Manager", 
                        "Public Relations ansvarlig", 
                        "Öffentlichkeitsarbeitsleiter", 
                        "Gerente de relaciones públicas", 
                        "Responsable des relations publiques", 
                        "Responsabile delle relazioni pubbliche", 
                        "PR ansvarlig" 
                    },
                    Tag = "PR"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Administratör", 
                        "Administrator", 
                        "Administrator", 
                        "Administrator", 
                        "Administrador", 
                        "Administrateur", 
                        "Amministratore", 
                        "Administrator" 
                    },
                    Tag = "ADM"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Producent", 
                        "Producer", 
                        "Producer", 
                        "Produzent", 
                        "Productor", 
                        "Producteur", 
                        "Produttore", 
                        "Produsent" 
                    },
                    Tag = "P"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Alla interna", 
                        "All Internal", 
                        "Alle interne", 
                        "Alle internen", 
                        "Todos los internos", 
                        "Tous les internes", 
                        "Tutti interni", 
                        "Alle interne" 
                    },
                    Tag = "ALL"
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Godkänna användare", 
                        "Approve users", 
                        "Godkende brugere", 
                        "Benutzer genehmigen", 
                        "Aprobar usuarios", 
                        "Approuver les utilisateurs", 
                        "Approvare utenti", 
                        "Godkjenne brukere" 
                    },
                    Tag = "NUA"
                }
            };
            context.ClaimTypes.AddRange(claimTypes);
            
            var controlTypes = new List<ControlType>
            {
                new () { Name = "Default" },
                new () { Name = "Textbox" },
                new () { Name = "Multiline-Textbox" },
                new () { Name = "Numeric-Textbox" },
                new () { Name = "Dropdown" },
                new () { Name = "Checkbox" },
                new () { Name = "Radio" },
                new () { Name = "Multiselect" },
                new () { Name = "Listbox-Name-Email-Age" },
                new () { Name = "Listbox-Days-Location" },
                new () { Name = "Listbox-Name-PricesReceived-Attended" },
                new () { Name = "Listbox-Name-Email-Phonenumber-Gender" },
                new () { Name = "Upload" },
                new () { Name = "Listbox-Name-Gender" },
                new () { Name = "Header" },
                new () { Name = "Date" },
                new () { Name = "Hour-Minute-Numeric-Textbox" },
                new () { Name = "Date-From-To" },
                new () { Name = "Button" },
                new () { Name = "Date-Approved" }
            };
            context.ControlTypes.AddRange(controlTypes);
            
            var eventTypes = new List<EventType>
            {
                new EventType() { 
                    Names = new List<string> 
                    { 
                        "Standard", 
                        "Default", 
                        "Standard", 
                        "Standard", 
                        "Predeterminado", 
                        "Par défaut", 
                        "Predefinito", 
                        "Standard"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Ej valt", 
                        "Not Selected", 
                        "Ikke valgt", 
                        "Nicht ausgewählt", 
                        "No seleccionado", 
                        "Non sélectionné", 
                        "Non selezionato", 
                        "Ikke valgt"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Applikation skapad", 
                        "Application Created", 
                        "Ansøgning oprettet", 
                        "Anwendung erstellt", 
                        "Aplicación creada", 
                        "Application créée", 
                        "Applicazione creata", 
                        "Applikasjon opprettet"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Public360-id begärts", 
                        "Public360 ID Requested", 
                        "Public360 ID anmodet", 
                        "Public360-ID angefordert", 
                        "ID de Public360 solicitado", 
                        "ID Public360 demandé", 
                        "ID Pubblic360 richiesto", 
                        "Public360 ID etterspurt"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Public360-id uppdaterat", 
                        "Public360 ID Updated", 
                        "Public360 ID opdateret", 
                        "Public360-ID aktualisiert", 
                        "ID de Public360 actualizado", 
                        "ID Public360 mis à jour", 
                        "ID Pubblic360 aggiornato", 
                        "Public360 ID oppdatert"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Projekt skapat", 
                        "Project Created", 
                        "Projekt oprettet", 
                        "Projekt erstellt", 
                        "Proyecto creado", 
                        "Projet créé", 
                        "Progetto creato", 
                        "Prosjekt opprettet"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Projekt är komplett", 
                        "Project is Complete", 
                        "Projektet er afsluttet", 
                        "Projekt ist abgeschlossen", 
                        "El proyecto está completo", 
                        "Le projet est complet", 
                        "Il progetto è completo", 
                        "Prosjektet er fullført"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Handläggarens bedömning låst", 
                        "Officer's Assessment Locked", 
                        "Sagsbehandlers vurdering låst", 
                        "Bewertung des Sachbearbeiters gesperrt", 
                        "Evaluación del oficial bloqueada", 
                        "Évaluation de l'agent verrouillée", 
                        "Valutazione dell'ufficiale bloccata", 
                        "Saksbehandlers vurdering låst"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "När handläggarens bedömning låses med nej", 
                        "No on Officer's Assessment", 
                        "Nej på sagsbehandlers vurdering", 
                        "Nein bei der Bewertung des Sachbearbeiters", 
                        "No a la evaluación del oficial", 
                        "Non à l'évaluation de l'agent", 
                        "No alla valutazione dell'ufficiale", 
                        "Nei på saksbehandlers vurdering"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "När handläggarens bedömning låses med ja", 
                        "Yes on Officer's Assessment", 
                        "Ja på sagsbehandlers vurdering", 
                        "Ja bei der Bewertung des Sachbearbeiters", 
                        "Sí a la evaluación del oficial", 
                        "Oui à l'évaluation de l'agent", 
                        "Sì alla valutazione dell'ufficiale", 
                        "Ja på saksbehandlers vurdering"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Nej på produktionsgruppens bedömning", 
                        "No on Production Group’s Assessment", 
                        "Nej på produktionsgruppens vurdering", 
                        "Nein bei der Bewertung der Produktionsgruppe", 
                        "No a la evaluación del grupo de producción", 
                        "Non à l'évaluation du groupe de production", 
                        "No alla valutazione del gruppo di produzione", 
                        "Nei på produksjonsgruppens vurdering"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Avslagsbrev skickat", 
                        "Rejection Letter Sent", 
                        "Afslagsbrev sendt", 
                        "Ablehnungsbrief gesendet", 
                        "Carta de rechazo enviada", 
                        "Lettre de refus envoyée", 
                        "Lettera di rifiuto inviata", 
                        "Avslagsbrev sendt"
                    }
                },
                new () { 
                    Names = new List<string> 
                    { 
                        "Projektet är avslutat", 
                        "Project is Completed", 
                        "Projektet er afsluttet", 
                        "Projekt ist abgeschlossen", 
                        "El proyecto está terminado", 
                        "Le projet est terminé", 
                        "Il progetto è terminato", 
                        "Prosjektet er avsluttet"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Ja på produktionsgruppens bedömning", 
                        "Yes on Production Group’s Assessment", 
                        "Ja på produktionsgruppens vurdering", 
                        "Ja bei der Bewertung der Produktionsgruppe", 
                        "Sí a la evaluación del grupo de producción", 
                        "Oui à l'évaluation du groupe de production", 
                        "Sì alla valutazione del gruppo di produzione", 
                        "Ja på produksjonsgruppens vurdering"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Public360 möte bokat", 
                        "Public360 Meeting Scheduled", 
                        "Public360 møde planlagt", 
                        "Public360-Meeting geplant", 
                        "Reunión de Public360 programada", 
                        "Réunion Public360 programmée", 
                        "Incontro Public360 programmato", 
                        "Public360 møte planlagt"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Loc/loi skickat", 
                        "LOC/LOI Sent", 
                        "LOC/LOI sendt", 
                        "LOC/LOI gesendet", 
                        "LOC/LOI enviado", 
                        "LOC/LOI envoyé", 
                        "LOC/LOI inviato", 
                        "LOC/LOI sendt"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Färdigfinansierat", 
                        "Fully Funded", 
                        "Komplet finansieret", 
                        "Vollständig finanziert", 
                        "Totalmente financiado", 
                        "Entièrement financé", 
                        "Interamente finanziato", 
                        "Fullt finansiert"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avtalsprocess klar?", 
                        "Contract process complete?", 
                        "Aftaleproces færdig?", 
                        "Vertragsprozess abgeschlossen?", 
                        "¿Proceso contractual completo?", 
                        "Entièrement financé, en attente d'approbation du contrat", 
                        "Processo contrattuale completato?", 
                        "Avtaleprosess ferdig?"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Underlaget är låst", 
                        "The Document is Locked", 
                        "Dokumentet er låst", 
                        "Das Dokument ist gesperrt", 
                        "El documento está bloqueado", 
                        "Le document est verrouillé", 
                        "Il documento è bloccato", 
                        "Dokumentet er låst"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Kontraktet är godkänt", 
                        "The Contract is Approved", 
                        "Kontrakten er godkendt", 
                        "Der Vertrag ist genehmigt", 
                        "El contrato está aprobado", 
                        "Le contrat est approuvé", 
                        "Il contratto è approvato", 
                        "Kontrakten er godkjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Kontraktet är journalfört", 
                        "The Contract is Registered", 
                        "Kontrakten er registreret", 
                        "Der Vertrag ist registriert", 
                        "El contrato está registrado", 
                        "Le contrat est enregistré", 
                        "Il contratto è registrato", 
                        "Kontrakten er registrert"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "VD raderar meddelande gällande underlag", 
                        "CEO Deletes Message Regarding Document", 
                        "CEO sletter besked om dokument", 
                        "CEO löscht Nachricht bezüglich Dokument", 
                        "El CEO elimina el mensaje sobre el documento", 
                        "Le PDG supprime le message concernant le document", 
                        "Il CEO elimina il messaggio riguardante il documento", 
                        "CEO sletter melding angående dokument"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten har skickat in startdatum för inspelning", 
                        "Producer has Submitted Filming Start Date", 
                        "Producenten har indsendt startdato for optagelser", 
                        "Der Produzent hat das Filmbeginn-Datum eingereicht", 
                        "El productor ha enviado la fecha de inicio de rodaje", 
                        "Le producteur a soumis la date de début de tournage", 
                        "Il produttore ha inviato la data di inizio delle riprese", 
                        "Produsenten har sendt inn startdato for innspilling"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten har skickat in teamlista", 
                        "Producer has Submitted Team List", 
                        "Producenten har indsendt holdliste", 
                        "Der Produzent hat die Teamliste eingereicht", 
                        "El productor ha enviado la lista del equipo", 
                        "Le producteur a soumis la liste de l'équipe", 
                        "Il produttore ha inviato la lista del team", 
                        "Produsenten har sendt inn teamliste"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Handläggaren har laddat upp teamlista", 
                        "Officer has Uploaded Team List", 
                        "Sagsbehandleren har uploadet holdliste", 
                        "Der Sachbearbeiter hat die Teamliste hochgeladen", 
                        "El oficial ha subido la lista del equipo", 
                        "L'agent a téléchargé la liste de l'équipe", 
                        "L'agente ha caricato la lista del team", 
                        "Saksbehandler har lastet opp teamliste"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Rat 1 är klar", 
                        "Draft 1 is Ready", 
                        "Udkast 1 er færdigt", 
                        "Entwurf 1 ist fertig", 
                        "El borrador 1 está listo", 
                        "Le brouillon 1 est prêt", 
                        "La bozza 1 è pronta", 
                        "Utkast 1 er klart"
                    }
                },
                                
                                new () {
                    Names = new List<string> 
                    { 
                        "Godkänd arbetskopia är klar", 
                        "Approved Work Copy is Ready", 
                        "Godkendt arbejdskopi er klar", 
                        "Genehmigte Arbeitskopie ist fertig", 
                        "La copia de trabajo aprobada está lista", 
                        "La copie de travail approuvée est prête", 
                        "La copia di lavoro approvata è pronta", 
                        "Godkjent arbeidskopi er klar"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten har skickat in uppdaterad teamlista", 
                        "Producer has Submitted Updated Team List", 
                        "Producenten har indsendt opdateret holdliste", 
                        "Der Produzent hat die aktualisierte Teamliste eingereicht", 
                        "El productor ha enviado la lista del equipo actualizada", 
                        "Le producteur a soumis la liste de l'équipe mise à jour", 
                        "Il produttore ha inviato la lista del team aggiornata", 
                        "Produsenten har sendt inn oppdatert teamliste"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten har skickat in slutdatum för inspelning", 
                        "Producer has Submitted Filming End Date", 
                        "Producenten har indsendt slutdato for optagelser", 
                        "Der Produzent hat das Enddatum des Drehs eingereicht", 
                        "El productor ha enviado la fecha de finalización del rodaje", 
                        "Le producteur a soumis la date de fin de tournage", 
                        "Il produttore ha inviato la data di fine riprese", 
                        "Produsenten har sendt inn sluttdato for innspilling"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Rat 2 är klar", 
                        "Draft 2 is Ready", 
                        "Udkast 2 er færdigt", 
                        "Entwurf 2 ist fertig", 
                        "El borrador 2 está listo", 
                        "Le brouillon 2 est prêt", 
                        "La bozza 2 è pronta", 
                        "Utkast 2 er klart"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Rough cut är klart", 
                        "Rough Cut is Ready", 
                        "Rå klip er klar", 
                        "Rough Cut ist fertig", 
                        "El corte preliminar está listo", 
                        "Le montage brut est prêt", 
                        "Il montaggio grezzo è pronto", 
                        "Rough Cut er klart"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Final cut är klart", 
                        "Final Cut is Ready", 
                        "Endelig klip er klar", 
                        "Final Cut ist fertig", 
                        "El corte final está listo", 
                        "Le montage final est prêt", 
                        "Il montaggio finale è pronto", 
                        "Final Cut er klart"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten har skickat in DCP-datum", 
                        "Producer has Submitted DCP Date", 
                        "Producenten har indsendt DCP-dato", 
                        "Der Produzent hat das DCP-Datum eingereicht", 
                        "El productor ha enviado la fecha de DCP", 
                        "Le producteur a soumis la date du DCP", 
                        "Il produttore ha inviato la data del DCP", 
                        "Produsenten har sendt inn DCP-dato"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Spend rapport är godkänd", 
                        "Spend Report is Approved", 
                        "Udgiftsrapport er godkendt", 
                        "Ausgabenbericht ist genehmigt", 
                        "El informe de gastos está aprobado", 
                        "Le rapport de dépenses est approuvé", 
                        "Il rapporto di spesa è approvato", 
                        "Utgiftsrapport er godkjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten har skickat in premiärdatum", 
                        "Producer has Submitted Premiere Date", 
                        "Producenten har indsendt premieredato", 
                        "Der Produzent hat das Premierendatum eingereicht", 
                        "El productor ha enviado la fecha de estreno", 
                        "Le producteur a soumis la date de la première", 
                        "Il produttore ha inviato la data della prima", 
                        "Produsenten har sendt inn premieredato"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Handläggaren har satt premiärdatum", 
                        "Officer has Set Premiere Date", 
                        "Sagsbehandleren har sat premieredato", 
                        "Der Sachbearbeiter hat das Premierendatum festgelegt", 
                        "El oficial ha fijado la fecha de estreno", 
                        "L'agent a fixé la date de la première", 
                        "L'agente ha fissato la data della prima", 
                        "Saksbehandler har satt premieredato"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Datum för premiär har passerat", 
                        "Premiere Date has Passed", 
                        "Premiere datoen er passeret", 
                        "Das Premierendatum ist vorbei", 
                        "La fecha de estreno ha pasado", 
                        "La date de la première est passée", 
                        "La data della prima è passata", 
                        "Premieredatoen har passert"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Slutrapport är godkänd", 
                        "Final Report is Approved", 
                        "Slutrapport er godkendt", 
                        "Der Abschlussbericht ist genehmigt", 
                        "El informe final está aprobado", 
                        "Le rapport final est approuvé", 
                        "Il rapporto finale è approvato", 
                        "Sluttrapport er godkjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Spend-rapport och slutrapport är godkända", 
                        "Spend Report and Final Report are Approved", 
                        "Udgiftsrapport og slutrapport er godkendt", 
                        "Ausgabenbericht und Abschlussbericht sind genehmigt", 
                        "El informe de gastos y el informe final están aprobados", 
                        "Le rapport de dépenses et le rapport final sont approuvés", 
                        "Il rapporto di spesa e il rapporto finale sono approvati", 
                        "Utgiftsrapport og sluttrapport er godkjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Pr-material har mottagits", 
                        "PR Material has Been Received", 
                        "PR-materiale er modtaget", 
                        "PR-Material wurde erhalten", 
                        "Se ha recibido material de prensa", 
                        "Les matériaux de RP ont été reçus", 
                        "Il materiale PR è stato ricevuto", 
                        "PR-materiale mottatt"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Projektet är klart utan pr-material", 
                        "Project is Completed Without PR Material", 
                        "Projektet er afsluttet uden PR-materiale", 
                        "Das Projekt ist ohne PR-Material abgeschlossen", 
                        "El proyecto está terminado sin material de prensa", 
                        "Le projet est terminé sans matériel de RP", 
                        "Il progetto è completato senza materiale PR", 
                        "Prosjektet er ferdig uten PR-materiale"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Projektet är klart med pr-material", 
                        "Project is Completed With PR Material", 
                        "Projektet er afsluttet med PR-materiale", 
                        "Das Projekt ist mit PR-Material abgeschlossen", 
                        "El proyecto está terminado con material de prensa", 
                        "Le projet est terminé avec du matériel de RP", 
                        "Il progetto è completato con materiale PR", 
                        "Prosjektet er ferdig med PR-materiale"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten har skickat in ekonomisk och konstnärlig rapport", 
                        "Producer has Submitted Financial and Artistic Report", 
                        "Producenten har indsendt økonomisk og kunstnerisk rapport", 
                        "Der Produzent hat den Finanz- und Kunstbericht eingereicht", 
                        "El productor ha enviado el informe financiero y artístico", 
                        "Le producteur a soumis le rapport financier et artistique", 
                        "Il produttore ha inviato il rapporto finanziario e artistico", 
                        "Produsenten har sendt inn økonomisk og kunstnerisk rapport"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Ekonomisk och konstnärlig rapport är godkänd", 
                        "Financial and Artistic Report is Approved", 
                        "Økonomisk og kunstnerisk rapport er godkendt", 
                        "Der Finanz- und Kunstbericht ist genehmigt", 
                        "El informe financiero y artístico está aprobado", 
                        "Le rapport financier et artistique est approuvé", 
                        "Il rapporto finanziario e artistico è approvato", 
                        "Økonomisk og kunstnerisk rapport er godkjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Datum för ekonomisk och konstnärlig rapport har passerat", 
                        "Date for Financial and Artistic Report has Passed", 
                        "Dato for økonomisk og kunstnerisk rapport er passeret", 
                        "Datum für den Finanz- und Kunstbericht ist vergangen", 
                        "La fecha para el informe financiero y artístico ha pasado", 
                        "La date du rapport financier et artistique est passée", 
                        "La data del rapporto finanziario e artistico è passata", 
                        "Dato for økonomisk og kunstnerisk rapport har passert"
                    }
                },
                new EventType() { Names = new List<string> { "Producenten registrerar sig", "Producer registers", "Producent registrerer sig", "Der Produzent registriert sich", "El productor se registra", "Le producteur s'enregistre", "Il produttore si registra", "Produsenten registrerer seg" } },
new EventType() { Names = new List<string> { "Producenten ber om nytt lösenord", "Producer requests a new password", "Producent anmoder om nyt adgangskode", "Der Produzent fordert ein neues Passwort an", "El productor solicita una nueva contraseña", "Le producteur demande un nouveau mot de passe", "Il produttore richiede una nuova password", "Produsenten ber om nytt passord" } },
new EventType() { Names = new List<string> { "Producenten godkänd", "Producer approved", "Producenten godkendt", "Produzent genehmigt", "Productor aprobado", "Producteur approuvé", "Produttore approvato", "Produsenten godkjent" } },
new EventType() { Names = new List<string> { "Producenten loggar in", "Producer logs in", "Producent logger ind", "Produzent meldet sich an", "El productor inicia sesión", "Le producteur se connecte", "Il produttore accede", "Produsenten logger inn" } },
new EventType() { Names = new List<string> { "Producenten uppdaterar profil", "Producer updates profile", "Producent opdaterer profil", "Produzent aktualisiert Profil", "El productor actualiza el perfil", "Le producteur met à jour le profil", "Il produttore aggiorna il profilo", "Produsenten oppdaterer profil" } },
new EventType() { Names = new List<string> { "Producenten påbörjar applikation", "Producer starts application", "Producent påbegynder applikation", "Produzent startet Anwendung", "El productor inicia la aplicación", "Le producteur commence l'application", "Il produttore avvia l'applicazione", "Produsenten starter applikasjon" } },
new EventType() { Names = new List<string> { "Producenten skickar in applikation", "Producer submits application", "Producent indsender applikation", "Produzent reicht Anwendung ein", "El productor envía la aplicación", "Le producteur soumet l'application", "Il produttore invia l'applicazione", "Produsenten sender inn applikasjon" } },
new EventType() { Names = new List<string> { "Producenten uppdaterar applikation", "Producer updates application", "Producent opdaterer applikation", "Produzent aktualisiert Anwendung", "El productor actualiza la aplicación", "Le producteur met à jour l'application", "Il produttore aggiorna l'applicazione", "Produsenten oppdaterer applikasjon" } },
new EventType() { Names = new List<string> { "Producenten raderar applikation", "Producer deletes application", "Producent sletter applikation", "Produzent löscht Anwendung", "El productor elimina la aplicación", "Le producteur supprime l'application", "Il produttore elimina l'applicazione", "Produsenten sletter applikasjon" } },
new EventType() { Names = new List<string> { "Producenten skickar in dokument", "Producer submits document", "Producent indsender dokument", "Produzent reicht Dokument ein", "El productor envía el documento", "Le producteur soumet le document", "Il produttore invia il documento", "Produsenten sender inn dokument" } },
new EventType() { Names = new List<string> { "Producenten raderar meddelande", "Producer deletes message", "Producent sletter meddelelse", "Produzent löscht Nachricht", "El productor elimina el mensaje", "Le producteur supprime le message", "Il produttore elimina il messaggio", "Produsenten sletter melding" } },
new EventType() { Names = new List<string> { "Projekt uppdateras", "Project is updated", "Projekt opdateres", "Projekt wird aktualisiert", "Proyecto actualizado", "Projet mis à jour", "Progetto aggiornato", "Prosjekt oppdateres" } },
new EventType() { Names = new List<string> { "Rat skapas", "Rat is created", "Rat oprettes", "Rat wird erstellt", "Rat creado", "Rat créé", "Rat creato", "Rat opprettes" } },
new EventType() { Names = new List<string> { "Rat uppdateras", "Rat is updated", "Rat opdateres", "Rat wird aktualisiert", "Rat actualizado", "Rat mis à jour", "Rat aggiornato", "Rat oppdateres" } },
new EventType() { Names = new List<string> { "Rat raderas", "Rat is deleted", "Rat slettes", "Rat wird gelöscht", "Rat eliminado", "Rat supprimé", "Rat eliminato", "Rat slettes" } },
new EventType() { Names = new List<string> { "Rat låst", "Rat is locked", "Rat er låst", "Rat ist gesperrt", "Rat bloqueado", "Rat verrouillé", "Rat bloccato", "Rat låst" } },
new EventType() { Names = new List<string> { "Rat upplåst", "Rat is unlocked", "Rat er låst op", "Rat ist entsperrt", "Rat desbloqueado", "Rat déverrouillé", "Rat sbloccato", "Rat ulåst" } },
new EventType() { Names = new List<string> { "Rat godkänd", "Rat approved", "Rat godkendt", "Rat genehmigt", "Rat aprobado", "Rat approuvé", "Rat approvato", "Rat godkjent" } },
new EventType() { Names = new List<string> { "Villkor skapas", "Condition is created", "Betingelse oprettes", "Bedingung wird erstellt", "Condición creada", "Condition créée", "Condizione creata", "Betingelse opprettes" } },
new EventType() { Names = new List<string> { "Villkor uppdateras", "Condition is updated", "Betingelse opdateres", "Bedingung wird aktualisiert", "Condición actualizada", "Condition mise à jour", "Condizione aggiornata", "Betingelse oppdateres" } },
new EventType() { Names = new List<string> { "Villkor raderas", "Condition is deleted", "Betingelse slettes", "Bedingung wird gelöscht", "Condición eliminada", "Condition supprimée", "Condizione eliminata", "Betingelse slettes" } },
new EventType() { Names = new List<string> { "Villkor godkänt", "Condition approved", "Betingelse godkendt", "Bedingung genehmigt", "Condición aprobada", "Condition approuvée", "Condizione approvata", "Betingelse godkjent" } },
new EventType() { Names = new List<string> { "Villkor ej utförd i tid", "Condition not executed on time", "Betingelse ikke udført til tiden", "Bedingung nicht rechtzeitig ausgeführt", "Condición no ejecutada a tiempo", "Condition non exécutée à temps", "Condizione non eseguita in tempo", "Betingelse ikke utført i tide" } },
new EventType() { Names = new List<string> { "Dokument skapas", "Document is created", "Dokument oprettes", "Dokument wird erstellt", "Documento creado", "Document créé", "Documento creato", "Dokument opprettes" } },
new EventType() { Names = new List<string> { "Dokument uppdateras", "Document is updated", "Dokument opdateres", "Dokument wird aktualisiert", "Documento actualizado", "Document mis à jour", "Documento aggiornato", "Dokument oppdateres" } },
new EventType() { Names = new List<string> { "Dokument raderas", "Document is deleted", "Dokument slettes", "Dokument wird gelöscht", "Documento eliminado", "Document supprimé", "Documento eliminato", "Dokument slettes" } },
new EventType() { Names = new List<string> { "Dokument låst", "Document is locked", "Dokument er låst", "Dokument ist gesperrt", "Documento bloqueado", "Document verrouillé", "Documento bloccato", "Dokument låst" } },
new EventType() { Names = new List<string> { "Dokument upplåst", "Document is unlocked", "Dokument er låst op", "Dokument ist entsperrt", "Documento desbloqueado", "Document déverrouillé", "Documento sbloccato", "Dokument ulåst" } },
new EventType() { Names = new List<string> { "Dokument signerat", "Document is signed", "Dokument er underskrevet", "Dokument ist unterschrieben", "Documento firmado", "Document signé", "Documento firmato", "Dokument signert" } },
new EventType() { Names = new List<string> { "Dokument Skickat", "Document sent", "Dokument sendt", "Dokument gesendet", "Documento enviado", "Document envoyé", "Documento inviato", "Dokument sendt" } },
new EventType() { Names = new List<string> { "Personal skapad", "Staff is created", "Personale oprettet", "Personal wird erstellt", "Personal creado", "Personnel créé", "Personale creato", "Personell opprettet" } },
new EventType() { Names = new List<string> { "Personal loggar in", "Staff logs in", "Personale logger ind", "Personal meldet sich an", "El personal inicia sesión", "Le personnel se connecte", "Il personale accede", "Personell logger inn" } },
new EventType() { Names = new List<string> { "Personal uppdaterad", "Staff is updated", "Personale opdateret", "Personal wird aktualisiert", "Personal actualizado", "Personnel mis à jour", "Personale aggiornato", "Personell oppdatert" } },
new EventType() { Names = new List<string> { "Personal raderad", "Staff is deleted", "Personale slettet", "Personal wird gelöscht", "Personal eliminado", "Personnel supprimé", "Personale eliminato", "Personell slettet" } },
new EventType() { Names = new List<string> { "Personal kopplas till organisation", "Staff connected to organization", "Personale tilsluttet organisation", "Personal mit Organisation verbunden", "Personal conectado a la organización", "Personnel connecté à l'organisation", "Personale collegato all'organizzazione", "Personell tilknyttet organisasjon" } },
new EventType() { Names = new List<string> { "Personal tas bort från organisation", "Staff removed from organization", "Personale fjernet fra organisation", "Personal wird aus der Organisation entfernt", "Personal eliminado de la organización", "Personnel retiré de l'organisation", "Personale rimosso dall'organizzazione", "Personell fjernet fra organisasjon" } },
new EventType() { Names = new List<string> { "Personal får nytt ansvarsområde", "Staff gets new responsibility", "Personale får nyt ansvarsområde", "Personal erhält neue Verantwortung", "El personal obtiene nueva responsabilidad", "Le personnel obtient une nouvelle responsabilité", "Il personale ottiene una nuova responsabilità", "Personell får nytt ansvar" } },
new EventType() { Names = new List<string> { "Personal tas bort från ansvarsområde", "Staff removed from responsibility", "Personale fjernet fra ansvarsområde", "Personal wird von der Verantwortung entfernt", "Personal eliminado de la responsabilidad", "Personnel retiré de la responsabilité", "Il personale rimosso dalla responsabilità", "Personell fjernet fra ansvar" } },
new EventType() { Names = new List<string> { "Produktionsbolag uppdateras", "Production company updated", "Produktionsselskab opdateret", "Produktionsfirma aktualisiert", "Empresa de producción actualizada", "Société de production mise à jour", "Azienda di produzione aggiornata", "Produksjonsselskap oppdatert" } },
new EventType() { Names = new List<string> { "Producent uppdateras", "Producer updated", "Producent opdateret", "Produzent aktualisiert", "Productor actualizado", "Producteur mis à jour", "Produttore aggiornato", "Produsent oppdatert" } },
new EventType() { Names = new List<string> { "Roll skapas", "Role is created", "Rolle oprettes", "Rolle wird erstellt", "Rol creado", "Rôle créé", "Ruolo creato", "Rolle opprettes" } },
new EventType() { Names = new List<string> { "Roll uppdateras", "Role is updated", "Rolle opdateres", "Rolle wird aktualisiert", "Rol actualizado", "Rôle mis à jour", "Ruolo aggiornato", "Rolle oppdateres" } },
new EventType() { Names = new List<string> { "Roll raderas", "Role is deleted", "Rolle slettes", "Rolle wird gelöscht", "Rol eliminado", "Rôle supprimé", "Ruolo eliminato", "Rolle slettes" } },
new EventType() { Names = new List<string> { "Sektion skapas", "Section is created", "Sektion oprettes", "Sektion wird erstellt", "Sección creada", "Section créée", "Sezione creata", "Seksjon opprettes" } },
new EventType() { Names = new List<string> { "Sektion uppdateras", "Section is updated", "Sektion opdateres", "Sektion wird aktualisiert", "Sección actualizada", "Section mise à jour", "Sezione aggiornata", "Seksjon oppdateres" } },
new EventType() { Names = new List<string> { "Sektion raderas", "Section is deleted", "Sektion slettes", "Sektion wird gelöscht", "Sección eliminada", "Section supprimée", "Sezione eliminata", "Seksjon slettes" } },
new EventType() { Names = new List<string> { "Status skapas", "Status is created", "Status oprettes", "Status wird erstellt", "Estado creado", "Statut créé", "Stato creato", "Status opprettes" } },
new EventType() { Names = new List<string> { "Status uppdateras", "Status is updated", "Status opdateres", "Status wird aktualisiert", "Estado actualizado", "Statut mis à jour", "Stato aggiornato", "Status oppdateres" } },
new EventType() { Names = new List<string> { "Status raderas", "Status is deleted", "Status slettes", "Status wird gelöscht", "Estado eliminado", "Statut supprimé", "Stato eliminato", "Status slettes" } },
new EventType() { Names = new List<string> { "Aktionstyp skapas", "Action type is created", "Handlingstype oprettes", "Aktionstyp wird erstellt", "Tipo de acción creado", "Type d'action créé", "Tipo di azione creato", "Handlingstype opprettes" } },
new EventType() { Names = new List<string> { "Aktionstyp uppdateras", "Action type is updated", "Handlingstype opdateres", "Aktionstyp wird aktualisiert", "Tipo de acción actualizado", "Type d'action mis à jour", "Tipo di azione aggiornato", "Handlingstype oppdateres" } },
new EventType() { Names = new List<string> { "Aktionstyp raderas", "Action type is deleted", "Handlingstype slettes", "Aktionstyp wird gelöscht", "Tipo de acción eliminado", "Type d'action supprimé", "Tipo di azione eliminato", "Handlingstype slettes" } },
new EventType() { Names = new List<string> { "Budgettyp skapas", "Budget type is created", "Budgettype oprettes", "Budgettyp wird erstellt", "Tipo de presupuesto creado", "Type de budget créé", "Tipo di budget creato", "Budsjetttype opprettes" } },
new EventType() { Names = new List<string> { "Budgettyp uppdateras", "Budget type is updated", "Budgettype opdateres", "Budgettyp wird aktualisiert", "Tipo de presupuesto actualizado", "Type de budget mis à jour", "Tipo di budget aggiornato", "Budsjetttype oppdateres" } },
new EventType() { Names = new List<string> { "Budgettyp raderas", "Budget type is deleted", "Budgettype slettes", "Budgettyp wird gelöscht", "Tipo de presupuesto eliminado", "Type de budget supprimé", "Tipo di budget eliminato", "Budsjetttype slettes" } },
new EventType() { Names = new List<string> { "Kontrolltyp skapas", "Control type is created", "Kontroltype oprettes", "Kontrolltyp wird erstellt", "Tipo de control creado", "Type de contrôle créé", "Tipo di controllo creato", "Kontrolltype opprettes" } },
new EventType() { Names = new List<string> { "Kontrolltyp uppdateras", "Control type is updated", "Kontroltype opdateres", "Kontrolltyp wird aktualisiert", "Tipo de control actualizado", "Type de contrôle mis à jour", "Tipo di controllo aggiornato", "Kontrolltype oppdateres" } },
new EventType() { Names = new List<string> { "Kontrolltyp raderas", "Control type is deleted", "Kontroltype slettes", "Kontrolltyp wird gelöscht", "Tipo de control eliminado", "Type de contrôle supprimé", "Tipo di controllo eliminato", "Kontrolltype slettes" } },
new EventType() { Names = new List<string> { "Kontrolltyp skapas", "Control type is created", "Kontroltype oprettes", "Kontrolltyp wird erstellt", "Tipo de control creado", "Type de contrôle créé", "Tipo di controllo creato", "Kontrolltype opprettes" } },
new EventType() { Names = new List<string> { "Kontrolltyp uppdateras", "Control type is updated", "Kontroltype opdateres", "Kontrolltyp wird aktualisiert", "Tipo de control actualizado", "Type de contrôle mis à jour", "Tipo di controllo aggiornato", "Kontrolltype oppdateres" } },
new EventType() { Names = new List<string> { "Kontrolltyp raderas", "Control type is deleted", "Kontroltype slettes", "Kontrolltyp wird gelöscht", "Tipo de control eliminado", "Type de contrôle supprimé", "Tipo di controllo eliminato", "Kontrolltype slettes" } },
new EventType() { Names = new List<string> { "Dokumenttyp skapas", "Document type is created", "Dokumenttype oprettes", "Dokumenttyp wird erstellt", "Tipo de documento creado", "Type de document créé", "Tipo di documento creato", "Dokumenttype opprettes" } },
new EventType() { Names = new List<string> { "Dokumenttyp uppdateras", "Document type is updated", "Dokumenttype opdateres", "Dokumenttyp wird aktualisiert", "Tipo de documento actualizado", "Type de document mis à jour", "Tipo di documento aggiornato", "Dokumenttype oppdateres" } },
new EventType() { Names = new List<string> { "Dokumenttyp raderas", "Document type is deleted", "Dokumenttype slettes", "Dokumenttyp wird gelöscht", "Tipo de documento eliminado", "Type de document supprimé", "Tipo di documento eliminato", "Dokumenttype slettes" } },
new EventType() { Names = new List<string> { "Dokumentleveranstyp skapas", "Document delivery type is created", "Delleveringstyp oprettes", "Dokument Lieferungstyp wird erstellt", "Tipo de entrega de documento creado", "Type de livraison document créé", "Tipo di consegna documento creato", "Dokumentleveringstype opprettes" } },
new EventType() { Names = new List<string> { "Dokumentleveranstyp uppdateras", "Document delivery type is updated", "Delleveringstyp opdateres", "Dokument Lieferungstyp wird aktualisiert", "Tipo de entrega de documento actualizado", "Type de livraison document mis à jour", "Tipo di consegna documento aggiornato", "Dokumentleveringstype oppdateres" } },
new EventType() { Names = new List<string> { "Dokumentleveranstyp raderas", "Document delivery type is deleted", "Delleveringstyp slettes", "Dokument Lieferungstyp wird gelöscht", "Tipo de entrega de documento eliminado", "Type de livraison document supprimé", "Tipo di consegna documento eliminato", "Dokumentleveringstype slettes" } },
new EventType() { Names = new List<string> { "Kön skapas", "Queue is created", "Kø oprettes", "Warteschlange wird erstellt", "Cola creada", "File créée", "Coda creata", "Kø opprettes" } },
new EventType() { Names = new List<string> { "Kön uppdateras", "Queue is updated", "Kø opdateres", "Warteschlange wird aktualisiert", "Cola actualizada", "File mise à jour", "Coda aggiornata", "Kø oppdateres" } },
new EventType() { Names = new List<string> { "Kön raderas", "Queue is deleted", "Kø slettes", "Warteschlange wird gelöscht", "Cola eliminada", "File supprimée", "Coda eliminata", "Kø slettes" } },
new EventType() { Names = new List<string> { "Meddelandetyp skapas", "Message type is created", "Meddelelsestyp oprettes", "Nachrichtentyp wird erstellt", "Tipo de mensaje creado", "Type de message créé", "Tipo di messaggio creato", "Meldingstype opprettes" } },
new EventType() { Names = new List<string> { "Meddelandetyp uppdateras", "Message type is updated", "Meddelelsestyp opdateres", "Nachrichtentyp wird aktualisiert", "Tipo de mensaje actualizado", "Type de message mis à jour", "Tipo di messaggio aggiornato", "Meldingstype oppdateres" } },
new EventType() { Names = new List<string> { "Meddelandetyp raderas", "Message type is deleted", "Meddelelsestyp slettes", "Nachrichtentyp wird gelöscht", "Tipo de mensaje eliminado", "Type de message supprimé", "Tipo di messaggio eliminato", "Meldingstype slettes" } },
new EventType() { Names = new List<string> { "Villkorstyp skapas", "Condition type is created", "Betingelsestyp oprettes", "Bedingungstyp wird erstellt", "Tipo de condición creado", "Type de condition créé", "Tipo di condizione creato", "Betingelsestype opprettes" } },
new EventType() { Names = new List<string> { "Villkorstyp uppdateras", "Condition type is updated", "Betingelsestyp opdateres", "Bedingungstyp wird aktualisiert", "Tipo de condición actualizado", "Type de condition mis à jour", "Tipo di condizione aggiornato", "Betingelsestype oppdateres" } },
new EventType() { Names = new List<string> { "Villkorstyp raderas", "Condition type is deleted", "Betingelsestyp slettes", "Bedingungstyp wird gelöscht", "Tipo de condición eliminado", "Type de condition supprimé", "Tipo di condizione eliminato", "Betingelsestype slettes" } },
new EventType() { Names = new List<string> { "Telefonnummertyp skapas", "Phone number type is created", "Telefonnummerstype oprettes", "Rufnummerart wird erstellt", "Tipo de número de teléfono creado", "Type de numéro de téléphone créé", "Tipo di numero di telefono creato", "Telefonnummertype opprettes" } },
new EventType() { Names = new List<string> { "Telefonnummertyp uppdateras", "Phone number type is updated", "Telefonnummerstype opdateres", "Rufnummerart wird aktualisiert", "Tipo de número de teléfono actualizado", "Type de numéro de téléphone mis à jour", "Tipo di numero di telefono aggiornato", "Telefonnummertype oppdateres" } },
new EventType() { Names = new List<string> { "Telefonnummertyp raderas", "Phone number type is deleted", "Telefonnummerstype slettes", "Rufnummerart wird gelöscht", "Tipo de número de teléfono eliminado", "Type de numéro de téléphone supprimé", "Tipo di numero di telefono eliminato", "Telefonnummertype slettes" } },
new EventType() { Names = new List<string> { "Reaktionstyp skapas", "Reaction type is created", "Reaktionstype oprettes", "Reaktionstyp wird erstellt", "Tipo de reacción creado", "Type de réaction créé", "Tipo di reazione creato", "Reaksjonstype opprettes" } },
new EventType() { Names = new List<string> { "Reaktionstyp uppdateras", "Reaction type is updated", "Reaktionstype opdateres", "Reaktionstyp wird aktualisiert", "Tipo de reacción actualizado", "Type de réaction mis à jour", "Tipo di reazione aggiornato", "Reaksjonstype oppdateres" } },
new EventType() { Names = new List<string> { "Reaktionstyp raderas", "Reaction type is deleted", "Reaktionstype slettes", "Reaktionstyp wird gelöscht", "Tipo de reacción eliminado", "Type de réaction supprimé", "Tipo di reazione eliminato", "Reaksjonstype slettes" } },
new EventType() { Names = new List<string> { "Meddelandedestination skapas", "Message destination is created", "Meldingsdestination oprettes", "Nachrichtenziel wird erstellt", "Destino de mensaje creado", "Destination de message créée", "Destinazione del messaggio creata", "Meldingsdestinasjon opprettes" } },
new EventType() { Names = new List<string> { "Meddelandedestination uppdateras", "Message destination is updated", "Meldingsdestination opdateres", "Nachrichtenziel wird aktualisiert", "Destino de mensaje actualizado", "Destination de message mise à jour", "Destinazione del messaggio aggiornata", "Meldingsdestinasjon oppdateres" } },
new EventType() { Names = new List<string> { "Meddelandedestination raderas", "Message destination is deleted", "Meldingsdestination slettes", "Nachrichtenziel wird gelöscht", "Destino de mensaje eliminado", "Destination de message supprimée", "Destinazione del messaggio eliminata", "Meldingsdestinasjon slettes" } },
new EventType() { Names = new List<string> { "Rat-utbetalning skapas", "Installment payment is created", "Rat-betaling oprettes", "Ratenzahlung wird erstellt", "Pago por cuotas creado", "Paiement par versements créé", "Pagamento a rate creato", "Råtbetaling opprettes" } },
new EventType() { Names = new List<string> { "Rat-utbetalning uppdateras", "Installment payment is updated", "Rat-betaling opdateres", "Ratenzahlung wird aktualisiert", "Pago por cuotas actualizado", "Paiement par versements mis à jour", "Pagamento a rate aggiornato", "Råtbetaling oppdateres" } },
new EventType() { Names = new List<string> { "Rat-utbetalning raderas", "Installment payment is deleted", "Rat-betaling slettes", "Ratenzahlung wird gelöscht", "Pago por cuotas eliminado", "Paiement par versements supprimé", "Pagamento a rate eliminato", "Råtbetaling slettes" } },

new () { 
    Names = new List<string> 
    { 
        "Nej på handläggarens bedömning", 
        "No on Production Group’s Assessment", 
        "Nej på produktionsgruppens vurdering", 
        "Nein bei der Bewertung der Produktionsgruppe", 
        "No a la evaluación del grupo de producción", 
        "Non à l'évaluation du groupe de production", 
        "No alla valutazione del gruppo di produzione", 
        "Nei på produksjonsgruppens vurdering"
    }
},
new () { 
Names = new List<string> 
{ 
"Ja på handläggarens bedömning", 
"Yes on Officer's Assessment", 
"Ja på sagsbehandlers vurdering", 
"Ja bei der Bewertung des Sachbearbeiters", 
"Sí a la evaluación del oficial", 
"Oui à l'évaluation de l'agent", 
"Sì alla valutazione dell'ufficiale", 
"Ja på saksbehandlers vurdering"
            }
            },
            };
            context.EventTypes.AddRange(eventTypes);
            
            var phoneNumberTypes = new List<PhoneNumberType>
            {
                new () {
                    Names = new List<string> 
                    { 
                        "Default", 
                        "Standard", 
                        "Standard", 
                        "Standard", 
                        "Predeterminado", 
                        "Par défaut", 
                        "Predefinito", 
                        "Standard"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Fixed line", 
                        "Fast linje", 
                        "Fastnet", 
                        "Festnetz", 
                        "Línea fija", 
                        "Ligne fixe", 
                        "Linea fissa", 
                        "Fastlinje"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Mobile", 
                        "Mobil", 
                        "Mobil", 
                        "Mobilfunk", 
                        "Móvil", 
                        "Mobile", 
                        "Cellulare", 
                        "Mobil"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Fixed line or mobile", 
                        "Fast linje eller mobil", 
                        "Fastnet eller mobil", 
                        "Festnetz oder Mobilfunk", 
                        "Línea fija o móvil", 
                        "Ligne fixe ou mobile", 
                        "Linea fissa o mobile", 
                        "Fastlinje eller mobil"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Toll free", 
                        "Avgiftsfri", 
                        "Gratisnummer", 
                        "Kostenlos", 
                        "Llamada gratuita", 
                        "Gratuit", 
                        "Numero verde", 
                        "Toll free"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Premium rate", 
                        "Premiumtaxa", 
                        "Højtakstnummer", 
                        "Mehrwertnummer", 
                        "Tarifa premium", 
                        "Tarif premium", 
                        "Tariffa premium", 
                        "Premiumnummer"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Shared cost", 
                        "Delad kostnad", 
                        "Delt takst", 
                        "Kosten teilen", 
                        "Coste compartido", 
                        "Coût partagé", 
                        "Costo condiviso", 
                        "Delt kostnad"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "VoIP", 
                        "VoIP", 
                        "VoIP", 
                        "VoIP", 
                        "VoIP", 
                        "VoIP", 
                        "VoIP", 
                        "VoIP"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Personal number", 
                        "Personligt nummer", 
                        "Personligt nummer", 
                        "Persönliche Nummer", 
                        "Número personal", 
                        "Numéro personnel", 
                        "Numero personale", 
                        "Personlig nummer"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Pager", 
                        "Sökare", 
                        "Søger", 
                        "Pager", 
                        "Radiolocalizador", 
                        "Téléavertisseur", 
                        "Cercapersone", 
                        "Personsøk"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "UAN", 
                        "UAN", 
                        "UAN", 
                        "UAN", 
                        "UAN", 
                        "UAN", 
                        "UAN", 
                        "UAN"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Voicemail", 
                        "Röstmeddelande", 
                        "Telefonsvarer", 
                        "Voicemail", 
                        "Correo de voz", 
                        "Messagerie vocale", 
                        "Segreteria telefonica", 
                        "Telefonsvarer"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Unknown", 
                        "Okänt", 
                        "Ukendt", 
                        "Unbekannt", 
                        "Desconocido", 
                        "Inconnu", 
                        "Sconosciuto", 
                        "Ukjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Work number", 
                        "Arbetsnummer", 
                        "Arbejde nummer", 
                        "Arbeitsnummer", 
                        "Número de trabajo", 
                        "Numéro de travail", 
                        "Numero di lavoro", 
                        "Arbeidsnummer"
                    }
                }
            };
            context.PhoneNumberTypes.AddRange(phoneNumberTypes);
            
            var reactionTypes = new List<ReactionType>
            {
                new () {
                    Names = new List<string> 
                    { 
                        "Default", 
                        "Standard", 
                        "Default", 
                        "Standard", 
                        "Predeterminado", 
                        "Par défaut", 
                        "Predefinito", 
                        "Standard"
                    },
                    Messages = new List<string> 
                    { 
                        "Default", 
                        "Standard", 
                        "Default", 
                        "Standard", 
                        "Predeterminado", 
                        "Par défaut", 
                        "Predefinito", 
                        "Standard"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Ej valt", 
                        "Not Selected", 
                        "Ikke valgt", 
                        "Nicht gewählt", 
                        "No seleccionado", 
                        "Non sélectionné", 
                        "Non selezionato", 
                        "Ikke valgt"
                    },
                    Messages = new List<string> 
                    { 
                        "Ej valt", 
                        "Not Selected", 
                        "Ikke valgt", 
                        "Nicht gewählt", 
                        "No seleccionado", 
                        "Non sélectionné", 
                        "Non selezionato", 
                        "Ikke valgt"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten ska skicka in datum för inspelningsstart", 
                        "Producer Must Submit Start Date for Filming", 
                        "Producenten skal indsende startdato for optagelserne", 
                        "Produzent muss Startdatum für Dreharbeiten einreichen", 
                        "El productor debe enviar la fecha de inicio de filmación", 
                        "Le producteur doit soumettre la date de début de tournage", 
                        "Il produttore deve inviare la data di inizio delle riprese", 
                        "Produsenten må sende inn startdato for innspilling"
                    },
                    Messages = new List<string> 
                    { 
                        "Skicka in datum för inspelningsstart", 
                        "Submit Start Date for Filming", 
                        "Indsend startdato for optagelserne", 
                        "Startdatum für Dreharbeiten einreichen", 
                        "Enviar la fecha de inicio de filmación", 
                        "Soumettre la date de début de tournage", 
                        "Inviar la data di inizio delle riprese", 
                        "Send inn startdato for innspilling"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten ska skicka in datum för inspelningsslut", 
                        "Producer Must Submit End Date for Filming", 
                        "Producenten skal indsende slutdato for optagelserne", 
                        "Produzent muss Enddatum für Dreharbeiten einreichen", 
                        "El productor debe enviar la fecha de finalización de filmación", 
                        "Le producteur doit soumettre la date de fin de tournage", 
                        "Il produttore deve inviare la data di fine delle riprese", 
                        "Produsenten må sende inn sluttdato for innspilling"
                    },
                    Messages = new List<string> 
                    { 
                        "Skicka in datum för inspelningsslut", 
                        "Submit End Date for Filming", 
                        "Indsend slutdato for optagelserne", 
                        "Enddatum für Dreharbeiten einreichen", 
                        "Enviar la fecha de finalización de filmación", 
                        "Soumettre la date de fin de tournage", 
                        "Inviar la data di fine delle riprese", 
                        "Send inn sluttdato for innspilling"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten ska skicka in datum för Svensk, international och festival premiär", 
                        "Producer Must Submit Dates for Swedish, International and Festival Premieres", 
                        "Producenten skal indsende datoer for svensk, international og festivalpremiere", 
                        "Produzent muss Daten für schwedische, internationale und Festivalpremieren einreichen", 
                        "El productor debe enviar las fechas para las premieres sueca, internacional y de festivales", 
                        "Le producteur doit soumettre les dates pour les premières suédoises, internationales et de festival", 
                        "Il produttore deve inviare le date per le prime svedesi, internazionali e festival", 
                        "Produsenten må sende inn datoer for svensk, internasjonal og festivalpremierer"
                    },
                    Messages = new List<string> 
                    { 
                        "Skicka in datum för Svensk, international och festival premiär", 
                        "Submit Dates for Swedish, International and Festival Premieres", 
                        "Indsend datoer for svensk, international og festivalpremiere", 
                        "Daten für schwedische, internationale und Festivalpremieren einreichen", 
                        "Enviar las fechas para las premieres sueca, internacional y de festivales", 
                        "Soumettre les dates pour les premières suédoises, internationales et de festival", 
                        "Inviar le date per le prime svedesi, internazionali e festival", 
                        "Send inn datoer for svensk, internasjonal og festivalpremierer"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten ska skicka in manus, ekonomi plan, projektbeskrivning och andra dokument", 
                        "Producer Must Submit Manuscript, Financial Plan, Project Description and Other Documents", 
                        "Producenten skal indsende manuskript, økonomiplan, projektbeskrivelse og andre dokumenter", 
                        "Produzent muss Manuskript, Finanzplan, Projektbeschreibung und andere Dokumente einreichen", 
                        "El productor debe enviar el manuscrito, el plan financiero, la descripción del proyecto y otros documentos", 
                        "Le producteur doit soumettre le manuscrit, le plan financier, la description du projet et d'autres documents", 
                        "Il produttore deve inviare il manoscritto, il piano finanziario, la descrizione del progetto e altri documenti", 
                        "Produsenten må sende inn manus, økonomisk plan, prosjektbeskrivelse og andre dokumenter"
                    },
                    Messages = new List<string> 
                    { 
                        "Skicka in manus, ekonomi plan, projektbeskrivning och andra dokument", 
                        "Submit Manuscript, Financial Plan, Project Description and Other Documents", 
                        "Indsende manuskript, økonomiplan, projektbeskrivelse og andre dokumenter", 
                        "Manuskript, Finanzplan, Projektbeschreibung und andere Dokumente einreichen", 
                        "Enviar el manuscrito, el plan financiero, la descripción del proyecto y otros documentos", 
                        "Soumettre le manuscrit, le plan financier, la description du projet et d'autres documents", 
                        "Inviare il manoscritto, il piano finanziario, la descrizione del progetto e altri documenti", 
                        "Send inn manus, økonomisk plan, prosjektbeskrivelse og andre dokumenter"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten ska skicka in teamlista", 
                        "Producer Must Submit Team List", 
                        "Producenten skal indsende holdliste", 
                        "Produzent muss Teamliste einreichen", 
                        "El productor debe enviar la lista del equipo", 
                        "Le producteur doit soumettre la liste de l'équipe", 
                        "Il produttore deve inviare la lista del team", 
                        "Produsenten må sende inn teamliste"
                    },
                    Messages = new List<string> 
                    { 
                        "Skicka in teamlista", 
                        "Submit Team List", 
                        "Indsende holdliste", 
                        "Teamliste einreichen", 
                        "Enviar la lista del equipo", 
                        "Soumettre la liste de l'équipe", 
                        "Inviare la lista del team", 
                        "Send inn teamliste"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten ska skicka in faktura för rat 1", 
                        "Producer Must Submit Invoice for Draft 1", 
                        "Producenten skal indsende faktura for udkast 1", 
                        "Produzent muss Rechnung für Entwurf 1 einreichen", 
                        "El productor debe enviar la factura del borrador 1", 
                        "Le producteur doit soumettre la facture du brouillon 1", 
                        "Il produttore deve inviare la fattura per la bozza 1", 
                        "Produsenten må sende inn faktura for utkast 1"
                    },
                    Messages = new List<string> 
                    { 
                        "Skicka in faktura för rat 1", 
                        "Submit Invoice for Draft 1", 
                        "Indsende faktura for udkast 1", 
                        "Rechnung für Entwurf 1 einreichen", 
                        "Enviar la factura del borrador 1", 
                        "Soumettre la facture du brouillon 1", 
                        "Inviare la fattura per la bozza 1", 
                        "Send inn faktura for utkast 1"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten ska skicka in datum för Dcp", 
                        "Producer Must Submit Date for DCP", 
                        "Producenten skal indsende dato for DCP", 
                        "Produzent muss Datum für DCP einreichen", 
                        "El productor debe enviar la fecha para el DCP", 
                        "Le producteur doit soumettre la date pour le DCP", 
                        "Il produttore deve inviare la data per il DCP", 
                        "Produsenten må sende inn dato for DCP"
                    },
                    Messages = new List<string> 
                    { 
                        "Skicka in datum för Dcp", 
                        "Submit Date for DCP", 
                        "Indsende dato for DCP", 
                        "Datum für DCP einreichen", 
                        "Enviar la fecha para el DCP", 
                        "Soumettre la date pour le DCP", 
                        "Inviare la data per il DCP", 
                        "Send inn dato for DCP"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Producenten ska skicka in artistisk och ekonomisk redovisning", 
                        "Producer Must Submit Artistic and Financial Accounting", 
                        "Producenten skal indsende kunstnerisk og økonomisk opgørelse", 
                        "Produzent muss künstlerische und finanzielle Abrechnung einreichen", 
                        "El productor debe enviar el informe artístico y financiero", 
                        "Le producteur doit soumettre le compte-rendu artistique et financier", 
                        "Il produttore deve inviare la rendicontazione artistica e finanziaria", 
                        "Produsenten må sende inn kunstnerisk og økonomisk rapport"
                    },
                    Messages = new List<string> 
                    { 
                        "Skicka in artistisk och ekonomisk redovisning", 
                        "Submit Artistic and Financial Accounting", 
                        "Indsende kunstnerisk og økonomisk opgørelse", 
                        "Künstlerische und finanzielle Abrechnung einreichen", 
                        "Enviar el informe artístico y financiero", 
                        "Soumettre le compte-rendu artistique et financier", 
                        "Inviare la rendicontazione artistica e finanziaria", 
                        "Send inn kunstnerisk og økonomisk rapport"
                    }
                }
            };
            context.ReactionTypes.AddRange(reactionTypes);
            
            var statuses = new List<Status>
            {
                new () {
                    Names = new List<string> 
                    { 
                        "Default", 
                        "Standard", 
                        "Default", 
                        "Default", 
                        "Predeterminado", 
                        "Par défaut", 
                        "Predefinito", 
                        "Standard"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Ok", 
                        "Ok", 
                        "Ok", 
                        "Ok", 
                        "Ok", 
                        "Ok", 
                        "Ok", 
                        "Ok"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Obehandlad", 
                        "Unprocessed", 
                        "Ubehandlet", 
                        "Unbehandelt", 
                        "No procesado", 
                        "Non traité", 
                        "Non elaborato", 
                        "Ubehandlet"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Ej komplett", 
                        "Incomplete", 
                        "Ufuldstændig", 
                        "Unvollständig", 
                        "Incompleto", 
                        "Incomplet", 
                        "Incompleto", 
                        "Ufullstendig"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Inför beslut", 
                        "Pending Decision", 
                        "Afventer beslutning", 
                        "Vor Entscheidung", 
                        "Pendiente de decisión", 
                        "En attente de décision", 
                        "In attesa di decisione", 
                        "Venter på avgjørelse"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avslag", 
                        "Rejected", 
                        "Afvist", 
                        "Abgelehnt", 
                        "Rechazado", 
                        "Rejeté", 
                        "Rifiutato", 
                        "Avvist"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "FIV Produktionsbeslut", 
                        "FIV Production Decision", 
                        "FIV Produktionsbeslutning", 
                        "FIV Produktionsentscheidung", 
                        "Decisión de producción del FIV", 
                        "Décision de production du FIV", 
                        "Decisione di produzione FIV", 
                        "FIV produksjonsbeslutning"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avtalsprocess", 
                        "Contract Process", 
                        "Aftaleproces", 
                        "Vertragsprozess", 
                        "Proceso de contrato", 
                        "Processus de contrat", 
                        "Processo contrattuale", 
                        "Kontraktprosess"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avtalsprocess klar", 
                        "Contract Process Completed", 
                        "Aftaleproces afsluttet", 
                        "Vertragsprozess abgeschlossen", 
                        "Proceso de contrato completado", 
                        "Processus de contrat terminé", 
                        "Processo contrattuale completato", 
                        "Kontraktprosess fullført"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Rough Cut klar/Godkänd arbetskopia klar", 
                        "Rough Cut Ready/Approved Work Copy Ready", 
                        "Klar klipning / Godkendt arbejdskopi klar", 
                        "Rohschnitt fertig / Genehmigte Arbeitskopie fertig", 
                        "Corte preliminar listo / Copia de trabajo aprobada lista", 
                        "Montage brut prêt / Copie de travail approuvée prête", 
                        "Montaggio grezzo pronto / Copia di lavoro approvata pronta", 
                        "Rough Cut klar / Godkjent arbeidskopi klar"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Final cut / DCP klar", 
                        "Final Cut/DCP Ready", 
                        "Endelig klip / DCP klar", 
                        "Endschnitt / DCP fertig", 
                        "Corte final / DCP listo", 
                        "Montage final / DCP prêt", 
                        "Montaggio finale / DCP pronto", 
                        "Final cut / DCP klar"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Spendredovisning klar", 
                        "Spend Report Ready", 
                        "Udgiftsrapport klar", 
                        "Ausgabenbericht fertig", 
                        "Informe de gastos listo", 
                        "Rapport de dépenses prêt", 
                        "Rapporto di spesa pronto", 
                        "Utgiftsrapport klar"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Spendredovisning godkänd", 
                        "Spend Report Approved", 
                        "Udgiftsrapport godkendt", 
                        "Ausgabenbericht genehmigt", 
                        "Informe de gastos aprobado", 
                        "Rapport de dépenses approuvé", 
                        "Rapporto di spesa approvato", 
                        "Utgiftsrapport godkjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Slutredovisning godkänd", 
                        "Final Report Approved", 
                        "Slutrapport godkendt", 
                        "Abschlussbericht genehmigt", 
                        "Informe final aprobado", 
                        "Rapport final approuvé", 
                        "Rapporto finale approvato", 
                        "Sluttrapport godkjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "PR material mottaget", 
                        "PR Material Received", 
                        "PR-material modtaget", 
                        "PR-Material erhalten", 
                        "Material de prensa recibido", 
                        "Matériaux de RP reçus", 
                        "Materiale PR ricevuto", 
                        "PR-materiale mottatt"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avklarat", 
                        "Completed", 
                        "Afsluttet", 
                        "Abgeschlossen", 
                        "Completado", 
                        "Terminé", 
                        "Completato", 
                        "Avklart"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avklarat utan PR material", 
                        "Completed Without PR Material", 
                        "Afsluttet uden PR-materiale", 
                        "Abgeschlossen ohne PR-Material", 
                        "Completado sin material de prensa", 
                        "Terminé sans matériel de RP", 
                        "Completato senza materiale PR", 
                        "Fullført uten PR-materiale"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avklarad projektutveckling", 
                        "Completed Project Development", 
                        "Afsluttet projektudvikling", 
                        "Abgeschlossene Projektentwicklung", 
                        "Desarrollo de proyecto completado", 
                        "Développement de projet terminé", 
                        "Sviluppo del progetto completato", 
                        "Avsluttet prosjektutvikling"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Deleted", 
                        "Raderad", 
                        "Slettet", 
                        "Gelöscht", 
                        "Eliminado", 
                        "Supprimé", 
                        "Eliminato", 
                        "Slettet"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Historia", 
                        "History", 
                        "Historie", 
                        "Geschichte", 
                        "Historia", 
                        "Histoire", 
                        "Storia", 
                        "Historie"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avslutad utan samproduktion", 
                        "Completed Without Co-production", 
                        "Afsluttet uden medproduktion", 
                        "Abgeschlossen ohne Koproduktion", 
                        "Terminado sin coproducción", 
                        "Terminé sans coproduction", 
                        "Completato senza coproduzione", 
                        "Avsluttet uten samproduksjon"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Spendredovisning och slutredovisning godkänd", 
                        "Spend Report and Final Report Approved", 
                        "Udgiftsrapport og slutrapport godkendt", 
                        "Ausgabenbericht und Abschlussbericht genehmigt", 
                        "Informe de gastos e informe final aprobados", 
                        "Rapport de dépenses et rapport final approuvés", 
                        "Rapporto di spesa e rapporto finale approvati", 
                        "Utgiftsrapport og sluttrapport godkjent"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Åtgärdat", 
                        "Resolved", 
                        "Løst", 
                        "Behoben", 
                        "Resuelto", 
                        "Résolu", 
                        "Risolto", 
                        "Løst"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Ej läst", 
                        "Unread", 
                        "Ulæst", 
                        "Ungelesen", 
                        "No leído", 
                        "Non lu", 
                        "Non letto", 
                        "Ulest"
                    }
                }
            };
            context.Statuses.AddRange(statuses);
            
            var systemMessageDestinations = new List<SystemMessageDestination>
            {
                new () {
                    Names = new List<string> 
                    { 
                        "Default", 
                        "Standard", 
                        "Default", 
                        "Default", 
                        "Predeterminado", 
                        "Par défaut", 
                        "Predefinito", 
                        "Standard"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Obehandlad ansökan", 
                        "Unprocessed Application", 
                        "Ubehandlet ansøgning", 
                        "Unbearbeiteter Antrag", 
                        "Solicitud no procesada", 
                        "Demande non traitée", 
                        "Domanda non elaborata", 
                        "Ubehandlet søknad"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Projekt översikt", 
                        "Project Overview", 
                        "Projektoversigt", 
                        "Projektübersicht", 
                        "Resumen del proyecto", 
                        "Vue d'ensemble du projet", 
                        "Panoramica del progetto", 
                        "Prosjektoversikt"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Underlag", 
                        "Supporting Documents", 
                        "Underlag", 
                        "Unterlagen", 
                        "Documentos de apoyo", 
                        "Documents de support", 
                        "Documenti di supporto", 
                        "Underlag"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Avslagsbrev", 
                        "Rejection Letter", 
                        "Afslagsbrev", 
                        "Ablehnungsschreiben", 
                        "Carta de rechazo", 
                        "Lettre de refus", 
                        "Lettera di rifiuto", 
                        "Avslagsbrev"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Hantera rater", 
                        "Manage Installments", 
                        "Håndter rater", 
                        "Raten verwalten", 
                        "Gestionar cuotas", 
                        "Gérer les versements", 
                        "Gestire le rate", 
                        "Administrer avdrag"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "LOC/LOI", 
                        "LOC/LOI", 
                        "LOC/LOI", 
                        "LOC/LOI", 
                        "LOC/LOI", 
                        "LOC/LOI", 
                        "LOC/LOI", 
                        "LOC/LOI"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Dokument översikt", 
                        "Document Overview", 
                        "Dokumentoversigt", 
                        "Dokumentenübersicht", 
                        "Resumen del documento", 
                        "Vue d'ensemble du document", 
                        "Panoramica del documento", 
                        "Dokumentoversikt"
                    }
                },
                new () {
                    Names = new List<string> 
                    { 
                        "Underlag, event 22", 
                        "Supporting Documents, event 22", 
                        "Underlag, event 22", 
                        "Unterlagen, event 22", 
                        "Documentos de apoyo, event 22", 
                        "Documents de support, event 22", 
                        "Documenti di supporto, event 22", 
                        "Underlag, event 22"
                    }
                }
            };
            context.SystemMessageDestinations.AddRange(systemMessageDestinations);
            
            
            context.SaveChanges();

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}