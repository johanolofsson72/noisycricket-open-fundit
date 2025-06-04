
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shared.Data.DbContext;

namespace Server.Test.Helpers;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private string _database = "fundit";//Guid.NewGuid().ToString() + ".db";
    
    public string DataBaseName()
    {
        return _database;
    }
    
    public CustomWebApplicationFactory()
    {
        
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
            // Unregister existing database service.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null) services.Remove(descriptor);

            // Register new database service.
            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=123;Database=fundit;User Id=sa;Password=123;TrustServerCertificate=True;", sub =>
                        sub.MigrationsAssembly("Server").UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).EnableSensitiveDataLogging());
            
            services.AddQuartz();
            services.AddQuartzHostedService(opt =>
            {
                opt.WaitForJobsToComplete = true;
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        
        /*
        // Clean up the database file.
        File.Delete("/Users/jool/repos/noisycricket-fundit-dawkins/Solution/src/Server/Data/Db/" + _database);
        File.Delete("/Users/jool/repos/noisycricket-fundit-dawkins/Solution/src/Server/Data/Db/" + _database + "-shm");
        File.Delete("/Users/jool/repos/noisycricket-fundit-dawkins/Solution/src/Server/Data/Db/" + _database + "-wal");
        */
        
        base.Dispose(disposing);
    }
}