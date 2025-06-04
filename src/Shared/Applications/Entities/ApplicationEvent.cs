using System.Collections.Generic;
using System.Linq;
using Shared.Applications.DTOs;

namespace Shared.Applications.Entities;

public class ApplicationEvent
{
    public ApplicationEvent(){}
    public ApplicationEvent(int identifier, int order, int eventTypeId, int dependsOnEventId = 0, bool isFirstInChain = false, bool isLastInChain = false, bool isStandAlone = false, List<string>? labels = null, string description = "")
    {
        Id = identifier;
        ApplicationEventIdentifier = identifier;
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
    public int ApplicationEventIdentifier { get; set; } = 0;
    public int Order { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int EventTypeId { get; set; } = 0;
    public int DependOnEventId { get; set; } = 0;
    public bool IsFirstInChain { get; set; } = false;
    public bool IsLastInChain { get; set; } = false;
    public bool IsStandAlone { get; set; } = false;
    public List<ApplicationEventAction> Actions { get; set; } = [];
    public virtual Application Application { get; set; } = new Application();
}

public static class ApplicationEventExtensions
{
    public static ApplicationEventDto ToDto(this ApplicationEvent entity)
    {
        return new ApplicationEventDto()
        {
            ApplicationEventIdentifier = entity.ApplicationEventIdentifier,
            Description = entity.Description,
            Labels = entity.Labels,
            Order = entity.Order,
            StatusId = entity.StatusId,
            EventTypeId = entity.EventTypeId,
            DependOnEventId = entity.DependOnEventId,
            IsFirstInChain = entity.IsFirstInChain,
            IsLastInChain = entity.IsLastInChain,
            IsStandAlone = entity.IsStandAlone,
            Actions = entity.Actions.Select(x => x.ToDto()).ToList()
        };
    }
    
    public static ApplicationEvent ToEntity(this ApplicationEventDto dto)
    {
        return new ApplicationEvent(dto.ApplicationEventIdentifier, dto.Order, dto.EventTypeId)
        {
            Id = dto.ApplicationEventIdentifier,
            Description = dto.Description,
            Labels = dto.Labels,
            Order = dto.Order,
            StatusId = dto.StatusId,
            EventTypeId = dto.EventTypeId,
            DependOnEventId = dto.DependOnEventId,
            IsFirstInChain = dto.IsFirstInChain,
            IsLastInChain = dto.IsLastInChain,
            IsStandAlone = dto.IsStandAlone,
            Actions = dto.Actions.Select(x => x.ToEntity()).ToList()
        };
    }
}