using System.Diagnostics;
using System.Net;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Data.DbContext;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzCleanUpJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzCleanUp", "SingleJob");
    
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzCleanUp started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Clean up temporary files
            var tempPath = configuration.GetValue<string>("DocumentTempFolder");

            if (tempPath is not null)
            {
                var di = new DirectoryInfo(tempPath);
                
                var referenceDate = DateTime.Now.AddDays(-7);

                foreach (var dir in di.GetDirectories()) 
                {
                    if (dir.CreationTime < referenceDate || dir.LastWriteTime < referenceDate)
                    {
                        foreach (var file in dir.GetFiles()) 
                        {
                            if (file.CreationTime < referenceDate || file.LastWriteTime < referenceDate)
                            {
                                file.Delete();
                            }
                        }
                        // Kontrollera om alla filer i katalogen är raderade innan du tar bort katalogen
                        if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
                        {
                            dir.Delete(true);
                        }
                    }
                }
            }
            
            // Clean up old AI Answers in cache
            var oldCachedAnswersDeleted = await context.OpenAiCacheItems
                .Where(x => x.ExpireDate < DateTime.UtcNow)
                .ExecuteDeleteAsync() > 0;
            
            // Clean up bad AI Answers in cache
            var cachedAnswersToBeDeleted = new List<int>();
            var cachedAnswers = await context.OpenAiCacheItems
                .AsNoTracking()
                .Select(x => new
                {
                    Id = x.Id,
                    Answer = x.Answer
                })
                .ToListAsync();
            
            foreach (var cr in cachedAnswers)
            {
                try
                {
                    await using var command = context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = cr.Answer;
                    command.CommandType = CommandType.Text;

                    await context.Database.OpenConnectionAsync();

                    await using var result = await command.ExecuteReaderAsync();
                    var dataTable = new DataTable();
                    dataTable.Load(result);
                        
                    await context.Database.CloseConnectionAsync();
                }
                catch
                {
                    cachedAnswersToBeDeleted.Add(cr.Id);
                }
            }
            
            var badCachedAnswersDeleted = await context.OpenAiCacheItems
                .Where(x => cachedAnswersToBeDeleted.Contains(x.Id))
                .ExecuteDeleteAsync() > 0;
            
            Console.WriteLine($"<= QuartzCleanUp succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzCleanUp failed " + ex);
        }
    }
    
}