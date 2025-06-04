using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Data.DbContext;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzStatisticsJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzStatistics", "SingleJob");
    
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var statisticService = scope.ServiceProvider.GetRequiredService<StatisticService>();

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzStatistics started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Get all users
            var users = await context.Users
                .AsTracking()
                .Where(x => x.StatusId != 19)
                .ToListAsync();

            // Loop through all users
            foreach (var user in users)
            {
                foreach (var statistic in user.Statistics)
                {
                    try
                    {
                        if (statistic.Query.Contains("#userId#"))
                        {
                            statistic.Query = statistic.Query.Replace("#userId#", user.Id.ToString());
                        }
                        
                        if (statistic.Query.Contains("#organizationName#"))
                        {
                            statistic.Query = statistic.Query.Replace("#organizationName#", user.Organizations.First().OrganizationName.ToLower());
                        }
                        
                        await using var command = context.Database.GetDbConnection().CreateCommand();
                        command.CommandText = statistic.Query;
                        command.CommandType = CommandType.Text;

                        await context.Database.OpenConnectionAsync();

                        await using var result = await command.ExecuteReaderAsync();
                        var dataTable = new DataTable();
                        dataTable.Load(result);
                        var json = JsonConvert.SerializeObject(dataTable);
                        var jarray = JsonConvert.DeserializeObject<JArray>(json);
                        if (jarray != null)
                        {
                            var statisticRender = await statisticService.ConvertJArrayToStatisticRenderAsync(jarray, new CancellationToken());
                            json = JsonConvert.SerializeObject(statisticRender.Value);
                        }

                        statistic.Value = json;
                        
                        await context.Database.CloseConnectionAsync();
                    }
                    catch
                    {
                        // ignored
                        statistic.Value = "{\"SingleRowOneItemValue\":\"\",\"SingleRowTwoItemsValue\":[],\"SingleRowThreeItemsValue\":[],\"SingleRowFourItemsValue\":[],\"ManyRowsOneItem\":[],\"ManyRowsTwoItems\":[],\"ManyRowsThreeItems\":[],\"ManyRowsFourItems\":[],\"ManyRowsFiveItems\":[],\"ManyRowsSixItems\":[],\"ManyRowsSevenItems\":[],\"ReturnType\":0}";
                        await context.Database.CloseConnectionAsync();
                    }
                }
            }
            
            // Save changes
            await context.SaveChangesAsync();
            
            Console.WriteLine($"<= QuartzStatistics succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzStatistics failed " + ex);
        }
    }
    
}