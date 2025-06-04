namespace Shared.Applications.Entities;

public class ApplicationRequiredDocument
{
    public int Id { get; set; } = 0;
    public int RequiredDocumentIdentifier { get; set; }
    public int RequiredDocumentId { get; set; } = 0;
}


public static class SchemaRequiredDocumentApplicationRequiredDocumentExtensions
{
    public static List<ApplicationRequiredDocumentDto> ToDto(this List<ApplicationRequiredDocument> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }

    public static List<ApplicationRequiredDocument> ToEntity(this List<ApplicationRequiredDocumentDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }

    public static ApplicationRequiredDocumentDto ToDto(this ApplicationRequiredDocument entity)
    {
        return new ApplicationRequiredDocumentDto()
        {
            Id = entity.Id,
            RequiredDocumentIdentifier = entity.RequiredDocumentIdentifier,
            RequiredDocumentId = entity.RequiredDocumentId
        };
    }

    public static ApplicationRequiredDocument ToEntity(this ApplicationRequiredDocumentDto dto)
    {
        return new ApplicationRequiredDocument()
        {
            Id = dto.Id,
            RequiredDocumentIdentifier = dto.RequiredDocumentIdentifier,
            RequiredDocumentId = dto.RequiredDocumentId
        };
    }
}