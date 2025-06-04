namespace Shared.Projects.DTOs;

public class ProjectHeaderDto
{
    public int Id { get; set; } = 0;
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationAddress { get; set; } = string.Empty;
    public string OrganizationPostalCode { get; set; } = string.Empty;
    public string OrganizationCity { get; set; } = string.Empty;
    public string OrganizationCountry { get; set; } = string.Empty;
    public string OrganizationUrl{ get; set; } = string.Empty;
    public string ProducerName { get; set; } = string.Empty;
    public string ProducerPhoneNumber { get; set; } = string.Empty;
    public string ProducerEmail { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantPhoneNumber { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public string ProjectManagerName { get; set; } = string.Empty;
    public string ProjectManagerPhoneNumber { get; set; } = string.Empty;
    public string ProjectManagerEmail { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
}