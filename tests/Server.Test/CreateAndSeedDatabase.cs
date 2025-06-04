using System.IO.Compression;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Server.Test.Helpers;
using Telerik.Zip;
using Xunit;
using Xunit.Abstractions;

namespace Server.Test;

public class CreateAndSeedDatabase : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public CreateAndSeedDatabase(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _ = Database.InitializeTestDatabase(factory, factory.DataBaseName(), testOutputHelper);
    }
    
    [Fact]
    public void CreateDatabase()
    {
        _testOutputHelper.WriteLine("Done.");
        Assert.Equal(1, 1);
    }

    [Fact]
    public async Task BackupDatabase()
    {
        var filename = $"fundit.db";
        var databasepath = "/Users/jool/repos/noisycricket-fundit/Solution/src/Server/Data/Db";
        var databasebackuppath = "/Users/jool/repos/noisycricket-fundit/Solution/src/Server/Data/Backup";
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
        
        File.Delete(Path.Combine(databasebackuppath, tempfolder, filename));
        Directory.Delete(Path.Combine(databasebackuppath, tempfolder));
    }
}