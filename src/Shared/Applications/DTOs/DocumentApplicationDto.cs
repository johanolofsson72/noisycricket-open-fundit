namespace Shared.Applications.DTOs;

public class DocumentApplicationDto
{
    public int Id { get; set; } = 0;
    public int ProjectId { get; set; } = 0;
    public int SchemaId { get; set; }
    public List<ApplicationControlDto> Controls { get; set; } = [];
    public DateTime DecisionDate { get; set; } = DateTime.MinValue;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string ProjectNumber { get; set; } = string.Empty;
    public ApplicationContactDto Producer { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto Organization { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ProjectManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ProductionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ScriptManager { get; set; } = new ApplicationContactDto();
}