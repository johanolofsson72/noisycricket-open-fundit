using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Documents.Entities;

public class DocumentType
{
    public int Id { get; set; }
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class DocumentTypeExtensions
{
    public static List<DocumentTypeDto> ToDto(this List<DocumentType> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }

    public static List<DocumentType> ToEntity(this List<DocumentTypeDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }
    
    public static DocumentTypeDto ToDto(this DocumentType entity)
    {
        return new DocumentTypeDto()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    }

    public static DocumentType ToEntity(this DocumentTypeDto dto)
    {
        return new DocumentType()
        {
            Id = dto.Id,
            Names = dto.Names
        };
    }
}