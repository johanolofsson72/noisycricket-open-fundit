namespace Shared.Applications.DTOs;

public class GridApplicationDto
{
    public int Id { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; } = DateTime.MinValue;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string OrganizationName { get; set; } = string.Empty;
}