
namespace Shared.Schemas.DTOs;

public class AddSchemaEventDto
{
    public int EventTypeId { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = ["", "", "", "", "", "", "", ""];
    public int DependOnEventId { get; set; } = 0;
    public bool IsFirstInChain { get; set; } = false;
    public bool IsLastInChain { get; set; } = false;
    public bool IsStandAlone { get; set; } = false;
}