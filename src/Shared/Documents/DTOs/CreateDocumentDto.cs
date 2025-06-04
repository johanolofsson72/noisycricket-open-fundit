using System.Collections.Generic;

namespace Shared.Documents.DTOs;

public class CreateDocumentDto
{
    public int ApplicationId { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int RequirementTypeId { get; set; } = 0;
    public int DeliveryTypeId { get; set; } = 0;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Phrases { get; set; } = string.Empty;
    public string Summarize { get; set; } = string.Empty;
    public byte[] Binary { get; set; } = [];
    public List<DocumentMetaDataDto> Metadata { get; set; } = new();
    public bool IsDelivered { get; set; }
    public bool IsSigned { get; set; }
    public bool IsCertified { get; set; }
    public bool IsLocked { get; set; }
}