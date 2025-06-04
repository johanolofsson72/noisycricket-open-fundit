namespace Shared.Messages.DTOs;

public class MessageContactDto
{
    public int ContactIdentifier { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}