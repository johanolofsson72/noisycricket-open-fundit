namespace Shared.Schemas.DTOs;

public class SchemaDto
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public string ClaimTag { get; set; } = "";
    public DateTime UpdatedDate { get; set; } = DateTime.MinValue;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public List<SchemaControlDto> Controls { get; set; } = new();
    public List<SchemaEventDto> Events { get; set; } = new();
    public List<SchemaRequiredDocumentDto> RequiredDocuments { get; set; } = new();
    public List<SchemaProgressDto> Progress { get; set; } = new();
    public bool Enabled { get; set; } = false;
}