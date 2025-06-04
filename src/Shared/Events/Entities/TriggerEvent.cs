using System.Collections.Generic;
using Shared.Events.DTOs;

namespace Shared.Events.Entities;

public class TriggerEvent
{
    public int ApplicationStatusId { get; set; } = 0;
    public bool ApplicationStatusIsUpdated { get; set; } = false;
    public List<int> InternalMessagesCreated { get; set; } = [];
    public List<int> InternalMessagesDeleted { get; set; } = [];
    public int AmountOfUnprocessedMessages { get; set; } = 0;
}

public static class TriggerEventExtensions
{
    public static TriggerEventDto ToDto(this TriggerEvent entity)
    {
        return new TriggerEventDto()
        {
            ApplicationStatusId = entity.ApplicationStatusId,
            ApplicationStatusIsUpdated = entity.ApplicationStatusIsUpdated,
            InternalMessagesCreated = entity.InternalMessagesCreated,
            InternalMessagesDeleted = entity.InternalMessagesDeleted,
            AmountOfUnprocessedMessages = entity.AmountOfUnprocessedMessages,
        };
    }
    
    public static TriggerEvent ToEntity(this TriggerEventDto dto)
    {
        return new TriggerEvent()
        {
            ApplicationStatusId = dto.ApplicationStatusId,
            ApplicationStatusIsUpdated = dto.ApplicationStatusIsUpdated,
            InternalMessagesCreated = dto.InternalMessagesCreated,
            InternalMessagesDeleted = dto.InternalMessagesDeleted,
            AmountOfUnprocessedMessages = dto.AmountOfUnprocessedMessages,
        };
    }
}