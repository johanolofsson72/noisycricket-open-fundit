

namespace Shared.Users.DTOs;

public class UserPhoneNumberDto
{
    public int PhoneNumberIdentifier { get; set; }
    public string Number { get; set; } = string.Empty;
    public int Type { get; set; } = 0;
}



