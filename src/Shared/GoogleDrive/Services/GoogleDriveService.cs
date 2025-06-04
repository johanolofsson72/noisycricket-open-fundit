using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Shared.GoogleDrive.Services;

public class GoogleDriveService(IConfiguration configuration)
{
    
    private readonly string _credentialsPath = configuration.GetValue<string>("GoogleDrive:CredentialsPath")!;
    private readonly string _folderId = configuration.GetValue<string>("GoogleDrive:FolderId")!;
    
    public string DeleteFolder(string folderId)
    {
        return Delete(folderId);
    }
    
    public string DeleteFile(string fileId)
    {
        return Delete(fileId);
    }
    
    private string Delete(string id)
    {
        var service = GetService();
        var command = service.Files.Delete(id);
        return command.Execute();
    }
    
    public IEnumerable<Google.Apis.Drive.v3.Data.File> GetFiles(string folder)
    {
        var service = GetService();
    
        var fileList = service.Files.List();
        fileList.Q =$"mimeType!='application/vnd.google-apps.folder' and '{folder}' in parents";
        fileList.Fields = "nextPageToken, files(id, name, size, mimeType)";
    
        var result = new List<Google.Apis.Drive.v3.Data.File>();
        string? pageToken = null;
        do
        {
            fileList.PageToken = pageToken;
            var filesResult = fileList.Execute();
            var files = filesResult.Files;
            pageToken = filesResult.NextPageToken;
            result.AddRange(files);
        } while (pageToken != null);
    
        return result;
    }
    
    public string CreateFolder(string folderName)
    {
        var service = GetService();
        var driveFolder = new Google.Apis.Drive.v3.Data.File
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = [ _folderId ]
        };
        var command = service.Files.Create(driveFolder);
        var file = command.Execute();
        return file.Id;
    }
    
    public string UploadFile(string folderId, string fileToUpload)
    {
        var service = GetService();

        var fileMetaData = new Google.Apis.Drive.v3.Data.File()
        {
            Name = Path.GetFileName(fileToUpload),
            Parents = new List<string> { folderId }
        };

        FilesResource.CreateMediaUpload request;
        using (var streamFile = new FileStream(fileToUpload, FileMode.Open))
        {
            request = service.Files.Create(fileMetaData, streamFile, "");
            request.Fields = "id";
            request.Upload();
        }
        var uploadedFile = request.ResponseBody;
        Console.WriteLine($"File '{fileMetaData.Name}' uploaded with ID: {uploadedFile.Id}");

        return uploadedFile.Id;
    }
    
    private DriveService GetService()
    {
        GoogleCredential credential;
        using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);
        credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
        {
           DriveService.ScopeConstants.DriveFile,
           DriveService.ScopeConstants.Drive
        });
        var service = new DriveService(new BaseClientService.Initializer()
        {
           HttpClientInitializer = credential,
           ApplicationName = "Fundit"
        });
        
        return service;
    }
}