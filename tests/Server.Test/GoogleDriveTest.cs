using System.IO.Compression;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Server.Test.Helpers;
using Shared.Events.Services;
using Shared.GoogleDrive.Services;
using Telerik.Zip;
using Xunit;
using Xunit.Abstractions;

namespace Server.Test;

public class GoogleDriveTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public GoogleDriveTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        //_ = Database.InitializeTestDatabase(factory, factory.DataBaseName(), testOutputHelper);
    }
    
    [Fact]
    public void TestUploadAndDeleteFile()
    {
        using var scope = _factory.Services.CreateScope();
        var googleDriveService = scope.ServiceProvider.GetRequiredService<GoogleDriveService>();

        var fileId = googleDriveService.UploadFile("YOUR_GOOGLE_DRIVE_FOLDER_ID", "/path/to/your/test/files/test.zip");
        
        googleDriveService.DeleteFile(fileId);
    }
    
    [Fact]
    public void TestCreateFolder()
    {
        using var scope = _factory.Services.CreateScope();
        var googleDriveService = scope.ServiceProvider.GetRequiredService<GoogleDriveService>();

        googleDriveService.CreateFolder("Test mappen");
    }
    
    [Fact]
    public void TestCreateFolderAndUploadAndGetAndDeleteFiles()
    {
        using var scope = _factory.Services.CreateScope();
        var googleDriveService = scope.ServiceProvider.GetRequiredService<GoogleDriveService>();
        
        var folderId = googleDriveService.CreateFolder("Test mappen");

        var fileId1 = googleDriveService.UploadFile(folderId, "/path/to/your/test/files/test.zip");
        
        var files = googleDriveService.GetFiles(folderId);
        
        Assert.NotEmpty(files);

        foreach (var file in files)
        {
            _testOutputHelper.WriteLine(googleDriveService.DeleteFile(file.Id));
        }
        
        _testOutputHelper.WriteLine(googleDriveService.DeleteFile(folderId));
        
    }
}