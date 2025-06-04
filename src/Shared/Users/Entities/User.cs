using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Organizations.Entities;
using Shared.Users.DTOs;
using Shared.Users.Enums;

namespace Shared.Users.Entities;


public class User : IdentityUser<int>
{
    public int StatusId { get; set; } = 0;
    [MaxLength(500)] public string FirstName { get; set; } = string.Empty;
    [MaxLength(500)] public string LastName { get; set; } = string.Empty;
    public string FullName => FirstName + " " + LastName;
    public List<UserAddress> Addresses { get; set; } = [];
    public List<UserPhoneNumber> PhoneNumbers { get; set; } = [];
    public List<UserOrganization> Organizations { get; set; } = [];
    public List<DateTime> LastLoginDate { get; set; } = [];
    public List<string> LastProject { get; set; } = [];
    public int MessageCount { get; set; }
    public List<int> VisibleApplicationTypes { get; set; } = [];
    public List<int> Favorites { get; set; } = [];
    public List<UserStatistic> Statistics { get; set; } = [];
    public UserType Type { get; set; } = UserType.Default;
}

public static class UserExtensions
{
    public static UserDto ToDto(this User entity) =>
        new()
        {
            Id = entity.Id,
            StatusId = entity.StatusId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email ?? string.Empty,
            Addresses = entity.Addresses.Select(x => x.ToDto()).ToList(),
            PhoneNumbers = entity.PhoneNumbers.Select(x => x.ToDto()).ToList(),
            Organizations = entity.Organizations.Select(x => x.ToDto()).ToList(),
            LastLoginDate = entity.LastLoginDate,
            LastProject = entity.LastProject,
            MessageCount = entity.MessageCount,
            VisibleApplicationTypes = entity.VisibleApplicationTypes,
            Statistics = entity.Statistics.Select(x => x.ToDto()).ToList(),
            Type = entity.Type,
            Favorites = entity.Favorites
        };
    
    public static User ToEntity(this UserDto dto) => 
        new()
        {
            Id = dto.Id,
            StatusId = dto.StatusId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Addresses = dto.Addresses.Select(x => x.ToEntity()).ToList(),
            PhoneNumbers = dto.PhoneNumbers.Select(x => x.ToEntity()).ToList(),
            Organizations = dto.Organizations.Select(x => x.ToEntity()).ToList(),
            LastLoginDate = dto.LastLoginDate,
            LastProject = dto.LastProject,
            MessageCount = dto.MessageCount,
            VisibleApplicationTypes = dto.VisibleApplicationTypes,
            Statistics = dto.Statistics.Select(x => x.ToEntity()).ToList(),
            Type = dto.Type,
            Favorites = dto.Favorites
        };
}