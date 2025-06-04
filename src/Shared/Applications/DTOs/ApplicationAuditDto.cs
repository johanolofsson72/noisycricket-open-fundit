namespace Shared.Applications.DTOs;

public class ApplicationAuditDto
{
    public int Id { get; set; } = 0;
    public int ApplicationAuditIdentifier { get; set; } = 0;
    public string Event { get; set; } = string.Empty;
    public List<string> Fields { get; set; } = [];
    public DateTime Executed { get; set; } = DateTime.MinValue;
    public string ExecutedBy { get; set; } = string.Empty;
}