using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Organizations.Entities;

public class Currency
{
    public int Id { get; set; } = 0;
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18, 4)")] public decimal Rate { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
}


public static class CurrencyExtensions
{

    public static IEnumerable<CurrencyDto> ToDto(this IEnumerable<Currency> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<Currency> ToEntity(this IEnumerable<CurrencyDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
    public static CurrencyDto ToDto(this Currency entity)
    {
        return new CurrencyDto()
        {
            Name = entity.Name,
            Rate = entity.Rate,
            CreatedDate = entity.CreatedDate
        };
    }

    public static Currency ToEntity(this CurrencyDto dto)
    {
        return new Currency
        {
            Name = dto.Name,
            Rate = dto.Rate,
            CreatedDate = dto.CreatedDate
        };
    }
}


