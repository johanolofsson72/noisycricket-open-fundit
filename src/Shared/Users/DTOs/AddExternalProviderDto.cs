namespace Shared.Users.DTOs;

public class AddExternalProviderDto
{
    public int Id { get; set; } = 0;
    public string Provider { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
}