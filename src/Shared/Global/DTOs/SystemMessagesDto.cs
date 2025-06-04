using Shared.Messages.DTOs;

namespace Shared.Global.DTOs;

public class SystemMessagesDto()
{
    public int MessageId { get; set; } = 0;
    public MessageContactDto Receiver { get; set; } = new MessageContactDto();
}