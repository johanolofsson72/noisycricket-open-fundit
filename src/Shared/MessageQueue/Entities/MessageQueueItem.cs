using Shared.MessageQueue.DTOs;
using Shared.Notifications;

namespace Shared.MessageQueue.Entities;

public class MessageQueueItem
{
    [Key] public int Id { get; set; } = 0;
    public NotificationType NotificationType { get; set; } = NotificationType.Default;
    public int StatusId { get; set; } = 0;
}

public static class MessageQueueItemExtensions
{
    public static IEnumerable<MessageQueueItemDto> ToDto(this IEnumerable<MessageQueueItem> entities)
    {
        return entities.Select(entity => entity.ToDto());
    }
    
    public static IEnumerable<MessageQueueItem> ToEntity(this IEnumerable<MessageQueueItemDto> dtos)
    {
        return dtos.Select(dto => dto.ToEntity());
    }
    
    public static MessageQueueItemDto ToDto(this MessageQueueItem entity)
    {
        return new MessageQueueItemDto()
        {
            Id = entity.Id,
            NotificationType = entity.NotificationType,
            StatusId = entity.StatusId
        };
    }
    
    public static MessageQueueItem ToEntity(this MessageQueueItemDto dto)
    {   
        return new MessageQueueItem()
        {
            Id = dto.Id,
            NotificationType = dto.NotificationType,
            StatusId = dto.StatusId
        };
    }
    
}



