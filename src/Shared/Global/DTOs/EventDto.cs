
using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class EventDto
{
    public EventDto(){}
    public EventDto(int identifier, int order, int eventTypeId)
    {
        ApplicationEventIdentifier = identifier;
        Order = order;
        EventTypeId = eventTypeId;
    }

    public int ApplicationEventIdentifier { get; set; } = 0;
    public int Order { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int EventTypeId { get; set; } = 0;
    public List<ActionDto> Actions { get; set; } = new();
    
}