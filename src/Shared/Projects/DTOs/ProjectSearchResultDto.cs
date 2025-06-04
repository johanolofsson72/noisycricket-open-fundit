namespace Shared.Projects.DTOs;

public class ProjectSearchResultDto
{
    public int Id { get; set; } = 0;
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<ProjectApplicationSearchResultDto> Applications { get; set; } = [];
    public string OrganizationName { get; set; } = string.Empty;
    public string ProjectManager { get; set; } = string.Empty;
    public DateTime LatestCreatedDate { get; set; } = DateTime.MinValue;
}