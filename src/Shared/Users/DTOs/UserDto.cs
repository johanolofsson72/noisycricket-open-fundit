using System;
using System.Collections.Generic;
using Shared.Organizations.DTOs;
using Shared.Users.Enums;

namespace Shared.Users.DTOs;

public class UserDto
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => FirstName + " " + LastName;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; } = null;
    public string? ConfirmPassword { get; set; } = null;
    public List<UserAddressDto> Addresses { get; set; } = [];
    public List<UserPhoneNumberDto> PhoneNumbers { get; set; } = [];
    public List<UserOrganizationDto> Organizations { get; set; } = [];
    public List<DateTime> LastLoginDate { get; set; } = [];
    public List<string> LastProject { get; set; } = [];
    public int MessageCount { get; set; }
    public List<int> VisibleApplicationTypes { get; set; } = [];
    public List<int> Favorites { get; set; } = [];
    public List<UserStatisticDto> Statistics { get; set; } = [];
    public UserType Type { get; set; } = UserType.Default;
}