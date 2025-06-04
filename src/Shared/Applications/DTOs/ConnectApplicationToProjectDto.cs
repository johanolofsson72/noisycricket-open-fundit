namespace Shared.Applications.DTOs;

public class ConnectApplicationToProjectDto
{
    public int OrganizationId { get; set; } = 0;
    public string ProjectNumber { get; set; } = string.Empty;
    public List<string> Titles { get; set; } = [];
    public ApplicationContactDto ProjectManager { get; set; } = new();
    public string SelectedCurrency { get; set; } = string.Empty;
}