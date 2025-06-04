using Shared.Schemas.DTOs;

namespace Shared.Schemas.Entities;

public class SchemaProgress
{
    public int Id { get; set; } = 0;
    public int SchemaProgressIdentifier { get; set; } = 0;
    [Column(TypeName = "decimal(18, 4)")] public decimal PercentageOfAmount { get; set; } = 0;
    public int MonthToExpire { get; set; } = 0;
    public List<SchemaProgressRequirement> Requirements { get; set; } = [];
}


public static class SchemaProgressExtensions
{
    public static List<SchemaProgressDto> ToDto(this List<SchemaProgress> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }

    public static List<SchemaProgress> ToEntity(this List<SchemaProgressDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }

    public static SchemaProgressDto ToDto(this SchemaProgress entity)
    {
        return new SchemaProgressDto()
        {
            Id = entity.Id,
            SchemaProgressIdentifier = entity.SchemaProgressIdentifier,
            PercentageOfAmount = entity.PercentageOfAmount,
            MonthToExpire = entity.MonthToExpire,
            Requirements = entity.Requirements.Select(x => x.ToDto()).ToList()
        };
    }

    public static SchemaProgress ToEntity(this SchemaProgressDto dto)
    {
        return new SchemaProgress()
        {
            Id = dto.Id,
            SchemaProgressIdentifier = dto.SchemaProgressIdentifier,
            PercentageOfAmount = dto.PercentageOfAmount,
            MonthToExpire = dto.MonthToExpire,
            Requirements = dto.Requirements.Select(x => x.ToEntity()).ToList()
        };
    }

    public static ApplicationProgress ToApplicationProgress(this SchemaProgress entity)
    {
        return new ApplicationProgress()
        {
            Id = entity.Id,
            ApplicationProgressIdentifier = entity.SchemaProgressIdentifier,
            PercentageOfAmount = entity.PercentageOfAmount,
            MonthToExpire = entity.MonthToExpire,
            Requirements = entity.Requirements.Select(x => x.ToApplicationProgressRequirement()).ToList()
        };
    }

    public static SchemaProgress ToSchemaProgress(this ApplicationProgress dto)
    {
        return new SchemaProgress()
        {
            Id = dto.Id,
            SchemaProgressIdentifier = dto.ApplicationProgressIdentifier,
            PercentageOfAmount = dto.PercentageOfAmount,
            MonthToExpire = dto.MonthToExpire,
            Requirements = dto.Requirements.Select(x => x.ToSchemaProgressRequirement()).ToList()
        };
    }

    public static List<ApplicationProgress> ToApplicationProgress(this List<SchemaProgress> entities)
    {
        return entities.Select(x => x.ToApplicationProgress()).ToList();
    }

    public static List<SchemaProgress> ToSchemaProgress(this List<ApplicationProgress> dtos)
    {
        return dtos.Select(x => x.ToSchemaProgress()).ToList();
    }
}