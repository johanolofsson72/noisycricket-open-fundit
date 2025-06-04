using System.Collections.Generic;
using Shared.Organizations.DTOs;

namespace Shared.Users.DTOs;

public class UpdateUserDto
{
    public int StatusId { get; set; } = 0;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public List<int> Favorites { get; set; } = [];
    public List<UserAddressDto> Addresses { get; set; } = [];
    public List<UserPhoneNumberDto> PhoneNumbers { get; set; } = [];
    public List<UserOrganizationDto> Organizations { get; set; } = [];
    public List<UserStatisticDto> Statistics { get; set; } = [];
}