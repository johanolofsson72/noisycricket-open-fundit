using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Data.DbContext;
using Shared.GoogleDrive.Services;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzDatabaseBackupJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzDatabaseBackup", "SingleJob");
    
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var googleDriveService = scope.ServiceProvider.GetRequiredService<GoogleDriveService>();

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzDatabaseBackup started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Clean up old files
            var databaseBackupPath = configuration.GetValue<string>("DatabaseBackupPath")!;
            foreach (var file in Directory.GetFiles(databaseBackupPath))            
            {
                var fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddDays(-7)) fi.Delete();
            }

            // Backup database
            var filename = $"fundit.db";
            var folder = configuration.GetValue<string>("GoogleDrive:FolderId")!;
            var databasepath = configuration.GetValue<string>("DatabasePath")!;
            var databasebackuppath = configuration.GetValue<string>("DatabaseBackupPath")!;
            var tempfolder = Guid.NewGuid().ToString();
            Directory.CreateDirectory(Path.Combine(databasebackuppath, tempfolder));
            await using var location = new SqliteConnection("Data Source=" + Path.Combine(databasepath, filename) + ";");
            await using var destination = new SqliteConnection($"Data Source=" + Path.Combine(databasebackuppath, tempfolder, filename) + ";");
            location.Open();
            destination.Open();
            location.BackupDatabase(destination, "main", "main");
    
            await Task.Delay(500);
        
            ZipFile.CreateFromDirectory(
                Path.Combine(databasebackuppath, tempfolder), 
                $"{Path.Combine(databasebackuppath, tempfolder)}.zip");
        
            await Task.Delay(500);
            
            googleDriveService.UploadFile(folder, $"{Path.Combine(databasebackuppath, tempfolder)}.zip");
        
            await Task.Delay(500);
        
            File.Delete($"{Path.Combine(databasebackuppath, tempfolder)}.zip");
            File.Delete(Path.Combine(databasebackuppath, tempfolder, filename));
            Directory.Delete(Path.Combine(databasebackuppath, tempfolder));
            
            Console.WriteLine($"<= QuartzDatabaseBackup succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzDatabaseBackup failed " + ex);
        }
    }
    
}