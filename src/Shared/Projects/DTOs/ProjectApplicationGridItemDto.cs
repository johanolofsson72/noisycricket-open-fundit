namespace Shared.Projects.DTOs;

public class ProjectApplicationGridItemDto
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string OrganizationName { get; set; } = string.Empty;
    public int Index { get; set; } = 0;
}