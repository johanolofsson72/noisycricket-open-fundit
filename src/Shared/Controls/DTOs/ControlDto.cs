
namespace Shared.Controls.DTOs;

public class ControlDto
{
    public int Id { get; set; } = 0;
    public int ControlTypeId { get; set; } = 0;
    public string ControlTypeName{ get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public string BaseStructure { get; set; } = string.Empty;
}