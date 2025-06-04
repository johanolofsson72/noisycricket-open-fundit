using System.Collections.Generic;
using System.Linq;
using Shared.Organizations.Entities;
using Shared.Statistics.DTOs;

namespace Shared.Statistics.Entities;

public class Statistic
{
    public int Id { get; set; } = 0;
    public int StatisticIdentifier { get; set; } = 0;
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string Description { get; set; } = string.Empty;
    [MaxLength(5000)] public string Query { get; set; } = string.Empty;
    [MaxLength(500)] public string Unit { get; set; } = string.Empty;
    public int Columns { get; set; } = 0;
    public int Rows { get; set; } = 0;
    public bool IsPublic { get; set; } = false;
}

public static class StatisticExtensions
{

    public static IEnumerable<StatisticDto> ToDto(this IEnumerable<Statistic> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<Statistic> ToEntity(this IEnumerable<StatisticDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
    public static StatisticDto ToDto(this Statistic entity)
    {
        return new StatisticDto()
        {
            Id = entity.Id,
            StatisticIdentifier = entity.StatisticIdentifier,
            Name = entity.Name,
            Description = entity.Description,
            Query = entity.Query,
            Columns = entity.Columns,
            Unit = entity.Unit,
            Rows = entity.Rows,
            IsPublic = entity.IsPublic
        };
    }

    public static Statistic ToEntity(this StatisticDto dto)
    {
        return new Statistic
        {
            Id = dto.Id,
            StatisticIdentifier = dto.StatisticIdentifier,
            Name = dto.Name,
            Description = dto.Description,
            Query = dto.Query,
            Unit = dto.Unit,
            Columns = dto.Columns,
            Rows = dto.Rows,
            IsPublic = dto.IsPublic
        };
    }
    
    public static UserStatistic ToOrganizationStatistic(this Statistic entity)
    {
        return new UserStatistic
        {
            StatisticIdentifier = entity.StatisticIdentifier,
            Name = entity.Name,
            Description = entity.Description,
            Query = entity.Query,
            Unit = entity.Unit,
            Columns = entity.Columns,
            Rows = entity.Rows
        };
    }
    
    public static Statistic ToStatistic(this UserStatistic entity)
    {
        return new Statistic
        {
            StatisticIdentifier = entity.StatisticIdentifier,
            Name = entity.Name,
            Description = entity.Description,
            Query = entity.Query,
            Unit = entity.Unit,
            Columns = entity.Columns,
            Rows = entity.Rows
        };
    }
}