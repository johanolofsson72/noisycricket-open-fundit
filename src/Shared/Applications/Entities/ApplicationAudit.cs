namespace Shared.Applications.Entities;

public class ApplicationAudit
{
    public int Id { get; set; } = 0;
    public int ApplicationAuditIdentifier { get; set; } = 0;
    public string Event { get; set; } = string.Empty;
    public List<string> Fields { get; set; } = [];
    public DateTime Executed { get; set; } = DateTime.MinValue;
    public string ExecutedBy { get; set; } = string.Empty;
}

public static class ApplicationAuditExtensions
{
    public static ApplicationAuditDto ToDto(this ApplicationAudit entity) =>
        new()
        {
            Id = entity.Id,
            ApplicationAuditIdentifier = entity.ApplicationAuditIdentifier,
            Event = entity.Event,
            Fields = entity.Fields,
            Executed = entity.Executed,
            ExecutedBy = entity.ExecutedBy
        };
    
    public static ApplicationAudit ToEntity(this ApplicationAuditDto dto) => 
        new()
        {
            Id = dto.Id,
            ApplicationAuditIdentifier = dto.ApplicationAuditIdentifier,
            Event = dto.Event,
            Fields = dto.Fields,
            Executed = dto.Executed,
            ExecutedBy = dto.ExecutedBy
        };
}