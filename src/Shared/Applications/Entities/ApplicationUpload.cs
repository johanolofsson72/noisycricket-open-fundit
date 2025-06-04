namespace Shared.Applications.Entities;

public class ApplicationUpload
{
    public int UploadIdentifier { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Content { get; set; } = new byte[0];
}