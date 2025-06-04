namespace Shared.Projects.DTOs;

public class ProjectGridItemDto
{
    public int Id { get; set; } = 0;
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<ProjectApplicationGridItemDto> Applications { get; set; } = [];
    public string OrganizationName { get; set; } = string.Empty;
    public string ProjectManager { get; set; } = string.Empty;
    public DateTime LatestCreatedDate { get; set; } = DateTime.MinValue;
}