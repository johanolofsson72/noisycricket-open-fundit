using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Organizations.DTOs;
using Shared.Users.Entities;

namespace Shared.Organizations.Entities;

public class UserStatistic
{
    public int StatisticIdentifier { get; set; } = 0;
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string Description { get; set; } = string.Empty;
    [MaxLength(500)] public string Unit { get; set; } = string.Empty;
    public int Columns { get; set; } = 0;
    public int Rows { get; set; } = 0;
    [MaxLength(1000)] public string Query { get; set; } = string.Empty;
    [MaxLength(5000)] public string Value { get; set; } = string.Empty;
    public virtual User User { get; set; } = new User();
}

public static class UserStatisticExtensions
{

    public static IEnumerable<UserStatisticDto> ToDto(this IEnumerable<UserStatistic> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<UserStatistic> ToEntity(this IEnumerable<UserStatisticDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
    public static UserStatisticDto ToDto(this UserStatistic entity)
    {
        return new UserStatisticDto()
        {
            StatisticIdentifier = entity.StatisticIdentifier,
            Name = entity.Name,
            Description = entity.Description,
            Columns = entity.Columns,
            Rows = entity.Rows,
            Query = entity.Query,
            Value = entity.Value,
            Unit = entity.Unit
        };
    }

    public static UserStatistic ToEntity(this UserStatisticDto dto)
    {
        return new UserStatistic
        {
            StatisticIdentifier = dto.StatisticIdentifier,
            Name = dto.Name,
            Description = dto.Description,
            Columns = dto.Columns,
            Rows = dto.Rows,
            Query = dto.Query,
            Value = dto.Value,
            Unit = dto.Unit
        };
    }
}