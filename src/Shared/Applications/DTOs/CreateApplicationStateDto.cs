namespace Shared.Applications.DTOs;

public class CreateApplicationStateDto
{
    public int OrganizationId { get; set; } = 0;
    public int ApplicantId { get; set; } = 0;
    public int ApplicationId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public string TempPath { get; set; } = string.Empty;

}