namespace Shared.Milestones.DTOs;

public class CreateMilestoneRequirementDto
{
    public int RequirementTypeId { get; set; } = 0;
    public int DeliveryTypeId { get; set; } = 0;
    public int DocumentId { get; set; }  = 0;
    public bool IsApproved { get; set; }
    public DateTime ApprovedDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public string Name { get; set; } = string.Empty;
}