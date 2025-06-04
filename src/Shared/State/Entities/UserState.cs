namespace Shared.State.Entities;

public class UserState
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Vat { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}