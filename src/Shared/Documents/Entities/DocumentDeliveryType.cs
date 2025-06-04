using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Documents.Entities;

public class DocumentDeliveryType
{
    public int Id { get; set; }
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class DocumentDeliveryTypeExtensions
{
    public static List<DocumentDeliveryTypeDto> ToDto(this List<DocumentDeliveryType> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }

    public static List<DocumentDeliveryType> ToEntity(this List<DocumentDeliveryTypeDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }
    
    public static DocumentDeliveryTypeDto ToDto(this DocumentDeliveryType entity)
    {
        return new DocumentDeliveryTypeDto()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    }

    public static DocumentDeliveryType ToEntity(this DocumentDeliveryTypeDto dto)
    {
        return new DocumentDeliveryType()
        {
            Id = dto.Id,
            Names = dto.Names
        };
    }
}