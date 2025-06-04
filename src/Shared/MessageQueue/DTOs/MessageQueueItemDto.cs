using Shared.Notifications;

namespace Shared.MessageQueue.DTOs;

public class MessageQueueItemDto
{
    public int Id { get; set; } = 0;
    public NotificationType NotificationType { get; set; } = NotificationType.Default;
    public int StatusId { get; set; } = 0;
}