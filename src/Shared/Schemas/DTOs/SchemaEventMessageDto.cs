
namespace Shared.Schemas.DTOs;

public class SchemaEventMessageDto
{
    public string Id  => EventId + "-" + ActionId;
    public int EventId { get; set; } = 0;
    public int ActionId { get; set; } = 0;

    public int EventTypeId { get; set; } = 0;
    
    public int ReceiverId { get; set; } = 0;
    public string Message { get; set; } = string.Empty;
}