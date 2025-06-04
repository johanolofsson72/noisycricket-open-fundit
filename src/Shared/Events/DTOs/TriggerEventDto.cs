
namespace Shared.Events.DTOs;

public class TriggerEventDto
{
    public int ApplicationStatusId { get; set; } = 0;
    public bool ApplicationStatusIsUpdated { get; set; } = false;
    public List<int> InternalMessagesCreated { get; set; } = [];
    public List<int> InternalMessagesDeleted { get; set; } = [];
    public int AmountOfUnprocessedMessages { get; set; } = 0;
}