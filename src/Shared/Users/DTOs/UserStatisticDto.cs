
namespace Shared.Organizations.DTOs;

public class UserStatisticDto
{
    public int StatisticIdentifier { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Columns { get; set; } = 0;
    public int Rows { get; set; } = 0;
    public string Unit { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}