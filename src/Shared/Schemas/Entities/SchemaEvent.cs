using System.Collections.Generic;
using System.Linq;
using Shared.Applications.Entities;
using Shared.Schemas.DTOs;

namespace Shared.Schemas.Entities;

public class SchemaEvent
{
    public SchemaEvent(){}
    public SchemaEvent(int identifier, int order, int eventTypeId, int dependsOnEventId = 0, bool isFirstInChain = false, bool isLastInChain = false, bool isStandAlone = false, List<string>? labels = null, string description = "")
    {
        Id = identifier;
        SchemaEventIdentifier = identifier;
        Order = order;
        EventTypeId = eventTypeId;
        DependOnEventId = dependsOnEventId;
        IsFirstInChain = isFirstInChain;
        IsLastInChain = isLastInChain;
        IsStandAlone = isStandAlone;
        Description = description;
        Labels = labels ?? new List<string>();
    }
    
    public int Id { get; set; } = 0;
    [MaxLength(500)] public string Description { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = ["", "", "", "", "", "", "", ""];
    public int SchemaEventIdentifier { get; set; } = 0;
    public int Order { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int EventTypeId { get; set; } = 0;
    public int DependOnEventId { get; set; } = 0;
    public bool IsFirstInChain { get; set; } = false;
    public bool IsLastInChain { get; set; } = false;
    public bool IsStandAlone { get; set; } = false;
    public List<SchemaEventAction> Actions { get; set; } = new();
    public virtual Schema Schema { get; set; } = new Schema();
}

public static class SchemaEventExtensions
{
    public static List<ApplicationEvent> ToApplicationEvent(this List<SchemaEvent> entities)
    {
        return entities.Select(x => x.ToApplicationEventAction()).ToList();
    }
    
    public static List<SchemaEvent> ToSchemaEvent(this List<ApplicationEvent> dtos)
    {
        return dtos.Select(x => x.ToSchemaEventAction()).ToList();
    }
    
    public static ApplicationEvent ToApplicationEventAction(this SchemaEvent entity)
    {
        return new ApplicationEvent()
        {
            Id = entity.Id,
            ApplicationEventIdentifier = entity.SchemaEventIdentifier,
            Labels = entity.Labels,
            Description = entity.Description,
            Order = entity.Order,
            EventTypeId = entity.EventTypeId,
            StatusId = entity.StatusId,
            DependOnEventId = entity.DependOnEventId,
            IsFirstInChain = entity.IsFirstInChain,
            IsLastInChain = entity.IsLastInChain,
            IsStandAlone = entity.IsStandAlone,
            Actions = entity.Actions.ToApplicationEventAction()
        };
    }
    
    public static SchemaEvent ToSchemaEventAction(this ApplicationEvent dto)
    {
        return new SchemaEvent()
        {
            Id = dto.Id,
            SchemaEventIdentifier = dto.ApplicationEventIdentifier,
            Labels = dto.Labels,
            Description = dto.Description,
            Order = dto.Order,
            EventTypeId = dto.EventTypeId,
            StatusId = dto.StatusId,
            DependOnEventId = dto.DependOnEventId,
            IsFirstInChain = dto.IsFirstInChain,
            IsLastInChain = dto.IsLastInChain,
            IsStandAlone = dto.IsStandAlone,
            Actions = dto.Actions.ToSchemaEventAction()
        };
    }
    
    public static List<SchemaEventDto> ToSchemaEventDto(this List<SchemaEvent> entities)
    {
        return entities.Select(x => x.ToSchemaEventDto()).ToList();
    }
    
    public static List<SchemaEvent> ToSchemaEvent(this List<SchemaEventDto> dtos)
    {
        return dtos.Select(x => x.ToSchemaEvent()).ToList();
    }
    
    public static SchemaEventDto ToSchemaEventDto(this SchemaEvent entity)
    {
        return new SchemaEventDto()
        {
            Id = entity.Id,
            SchemaEventIdentifier = entity.SchemaEventIdentifier,
            Labels = entity.Labels,
            Description = entity.Description,
            Order = entity.Order,
            EventTypeId = entity.EventTypeId,
            StatusId = entity.StatusId,
            DependOnEventId = entity.DependOnEventId,
            IsFirstInChain = entity.IsFirstInChain,
            IsLastInChain = entity.IsLastInChain,
            IsStandAlone = entity.IsStandAlone,
            Actions = entity.Actions.ToSchemaEventDto()
        };
    }
    
    public static SchemaEvent ToSchemaEvent(this SchemaEventDto dto)
    {
        return new SchemaEvent()
        {
            Id = dto.SchemaEventIdentifier,
            SchemaEventIdentifier = dto.SchemaEventIdentifier,
            Labels = dto.Labels,
            Description = dto.Description,
            Order = dto.Order,
            EventTypeId = dto.EventTypeId,
            StatusId = dto.StatusId,
            DependOnEventId = dto.DependOnEventId,
            IsFirstInChain = dto.IsFirstInChain,
            IsLastInChain = dto.IsLastInChain,
            IsStandAlone = dto.IsStandAlone,
            Actions = dto.Actions.ToSchemaEventAction()
        };
    }
    
    public static ApplicationEvent ToApplicationEvent(this SchemaEventDto entity)
    {
        return new ApplicationEvent()
        {
            Id = entity.Id,
            ApplicationEventIdentifier = entity.SchemaEventIdentifier,
            Labels = entity.Labels,
            Description = entity.Description,
            Order = entity.Order,
            EventTypeId = entity.EventTypeId,
            StatusId = entity.StatusId,
            DependOnEventId = entity.DependOnEventId,
            IsFirstInChain = entity.IsFirstInChain,
            IsLastInChain = entity.IsLastInChain,
            IsStandAlone = entity.IsStandAlone,
            Actions = entity.Actions.ToApplicationEventAction()
        };
    }
    
    public static SchemaEventDto ToSchemaEventDto(this ApplicationEvent entity)
    {
        return new SchemaEventDto()
        {
            Id = entity.Id,
            SchemaEventIdentifier = entity.ApplicationEventIdentifier,
            Labels = entity.Labels,
            Description = entity.Description,
            Order = entity.Order,
            EventTypeId = entity.EventTypeId,
            StatusId = entity.StatusId,
            DependOnEventId = entity.DependOnEventId,
            IsFirstInChain = entity.IsFirstInChain,
            IsLastInChain = entity.IsLastInChain,
            IsStandAlone = entity.IsStandAlone,
            Actions = entity.Actions.ToSchemaEventActionDto()
        };
    }
    
    public static List<ApplicationEvent> ToApplicationEventList(this List<SchemaEventDto> entities)
    {
        return entities.Select(x => x.ToApplicationEvent()).ToList();
    }
    
    public static List<SchemaEventDto> ToSchemaEventDtoList(this List<ApplicationEvent> dtos)
    {
        return dtos.Select(x => x.ToSchemaEventDto()).ToList();
    }
    
}
