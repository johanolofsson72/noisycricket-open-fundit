namespace Shared.Applications.DTOs;

public class CreateApplicationDto
{
    public int SchemaId { get; set; } = 0;
    public int OrganizationId { get; set; } = 0;
    public int ParentId { get; set; } = 0;
    public int ApplicantId { get; set; } = 0;
    public int ProjectManagerId { get; set; } = 0;

}