namespace Shared.Applications.DTOs;

public class DeliverNewApplicationDto
{
    public int UserId { get; set; } = 0;
    public int OrganizationId { get; set; } = 0;
    public string TempPath { get; set; } = string.Empty;
}