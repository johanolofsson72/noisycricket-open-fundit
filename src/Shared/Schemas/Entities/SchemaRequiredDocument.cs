using Shared.Schemas.DTOs;

namespace Shared.Schemas.Entities;

public class SchemaRequiredDocument
{
    public int Id { get; set; } = 0;
    public int RequiredDocumentIdentifier { get; set; } = 0;
    public int RequiredDocumentId { get; set; } = 0;
}

public static class SchemaRequiredDocumentExtensions
{
    public static List<SchemaRequiredDocumentDto> ToDto(this List<SchemaRequiredDocument> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }

    public static List<SchemaRequiredDocument> ToEntity(this List<SchemaRequiredDocumentDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }
    
    public static List<ApplicationRequiredDocument> ToApplicationRequiredDocument(this List<SchemaRequiredDocument> entities)
    {
        return entities.Select(x => x.ToApplicationRequiredDocument()).ToList();
    }
    
    public static SchemaRequiredDocumentDto ToDto(this SchemaRequiredDocument entity)
    {
        return new SchemaRequiredDocumentDto()
        {
            Id = entity.Id,
            RequiredDocumentIdentifier = entity.RequiredDocumentIdentifier,
            RequiredDocumentId = entity.RequiredDocumentId
        };
    }

    public static SchemaRequiredDocument ToEntity(this SchemaRequiredDocumentDto dto)
    {
        return new SchemaRequiredDocument()
        {
            Id = dto.Id,
            RequiredDocumentIdentifier = dto.RequiredDocumentIdentifier,
            RequiredDocumentId = dto.RequiredDocumentId
        };
    }
    
    public static ApplicationRequiredDocument ToApplicationRequiredDocument(this SchemaRequiredDocument entity)
    {
        return new ApplicationRequiredDocument()
        {
            Id = entity.Id,
            RequiredDocumentIdentifier = entity.RequiredDocumentIdentifier,
            RequiredDocumentId = entity.RequiredDocumentId
        };
    }

}