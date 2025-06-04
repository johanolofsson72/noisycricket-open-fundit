namespace Shared.Applications.Entities;

public class ApplicationProgressRequirement
{
    public int Id { get; set; } = 0;
    public int ApplicationProgressRequirementIdentifier { get; set; } = 0;
    public int MilestoneRequirementTypeId { get; set; } = 0;
    public int DocumentDeliveryTypeId { get; set; } = 0;
}

public static class ApplicationProgressRequirementExtensions
{
    public static ApplicationProgressRequirementDto ToDto(this ApplicationProgressRequirement entity)
    {
        return new ApplicationProgressRequirementDto()
        {
            ApplicationProgressRequirementIdentifier = entity.ApplicationProgressRequirementIdentifier,
            MilestoneRequirementTypeId = entity.MilestoneRequirementTypeId,
            DocumentDeliveryTypeId = entity.DocumentDeliveryTypeId
        };
    }
    
    public static ApplicationProgressRequirement ToEntity(this ApplicationProgressRequirementDto dto)
    {
        return new ApplicationProgressRequirement()
        {
            ApplicationProgressRequirementIdentifier = dto.ApplicationProgressRequirementIdentifier,
            MilestoneRequirementTypeId = dto.MilestoneRequirementTypeId,
            DocumentDeliveryTypeId = dto.DocumentDeliveryTypeId
        };
    }
}