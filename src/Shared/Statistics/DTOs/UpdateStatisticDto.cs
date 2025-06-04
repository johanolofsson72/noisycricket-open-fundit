
namespace Shared.Statistics.DTOs;

public class UpdateStatisticDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public int Columns { get; set; } = 0;
    public int Rows { get; set; } = 0;
    public bool IsPublic { get; set; } = false;
}