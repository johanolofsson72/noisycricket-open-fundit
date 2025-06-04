using Shared.Schemas.DTOs;

namespace Shared.Schemas.Entities;

public class SchemaProgressRequirement
{
    public int Id { get; set; } = 0;
    public int SchemaProgressRequirementIdentifier { get; set; } = 0;
    public int MilestoneRequirementTypeId { get; set; } = 0;
    public int DocumentDeliveryTypeId { get; set; } = 0;
}

public static class SchemaProgressRequirementExtensions
{
    public static SchemaProgressRequirementDto ToDto(this SchemaProgressRequirement entity)
    {
        return new SchemaProgressRequirementDto()
        {
            SchemaProgressRequirementIdentifier = entity.SchemaProgressRequirementIdentifier,
            MilestoneRequirementTypeId = entity.MilestoneRequirementTypeId,
            DocumentDeliveryTypeId = entity.DocumentDeliveryTypeId
        };
    }
    
    public static SchemaProgressRequirement ToEntity(this SchemaProgressRequirementDto dto)
    {
        return new SchemaProgressRequirement()
        {
            SchemaProgressRequirementIdentifier = dto.SchemaProgressRequirementIdentifier,
            MilestoneRequirementTypeId = dto.MilestoneRequirementTypeId,
            DocumentDeliveryTypeId = dto.DocumentDeliveryTypeId
        };
    }
    
    public static ApplicationProgressRequirement ToApplicationProgressRequirement(this SchemaProgressRequirement entity)
    {
        return new ApplicationProgressRequirement()
        {
            ApplicationProgressRequirementIdentifier = entity.SchemaProgressRequirementIdentifier,
            MilestoneRequirementTypeId = entity.MilestoneRequirementTypeId,
            DocumentDeliveryTypeId = entity.DocumentDeliveryTypeId
        };
    }
    
    public static SchemaProgressRequirement ToSchemaProgressRequirement(this ApplicationProgressRequirement dto)
    {
        return new SchemaProgressRequirement()
        {
            SchemaProgressRequirementIdentifier = dto.ApplicationProgressRequirementIdentifier,
            MilestoneRequirementTypeId = dto.MilestoneRequirementTypeId,
            DocumentDeliveryTypeId = dto.DocumentDeliveryTypeId
        };
    }
    
    public static ApplicationProgressRequirementDto ToApplicationProgressRequirementDto(this SchemaProgressRequirementDto entity)
    {
        return new ApplicationProgressRequirementDto()
        {
            ApplicationProgressRequirementIdentifier = entity.SchemaProgressRequirementIdentifier,
            MilestoneRequirementTypeId = entity.MilestoneRequirementTypeId,
            DocumentDeliveryTypeId = entity.DocumentDeliveryTypeId
        };
    }
    
    public static SchemaProgressRequirementDto ToSchemaProgressRequirementDto(this ApplicationProgressRequirementDto dto)
    {
        return new SchemaProgressRequirementDto()
        {
            SchemaProgressRequirementIdentifier = dto.ApplicationProgressRequirementIdentifier,
            MilestoneRequirementTypeId = dto.MilestoneRequirementTypeId,
            DocumentDeliveryTypeId = dto.DocumentDeliveryTypeId
        };
    }
}