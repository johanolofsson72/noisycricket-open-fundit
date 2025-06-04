using System.ComponentModel.DataAnnotations;
using Shared.Documents.DTOs;

namespace Shared.Documents.Entities;

public class DocumentMetaData
{
    public int DocumentMetaDataIdentifier { get; set; } = 0;
    [MaxLength(500)] public string Key { get; set; } = string.Empty;
    [MaxLength(5000)] public string Value { get; set; } = string.Empty;
    
    public virtual Document Document { get; set; } = new Document();
}

public static class DocumentMetaDataExtensions
{
    public static DocumentMetaDataDto ToDto(this DocumentMetaData entity) =>
        new ()
        {
            DocumentMetaDataIdentifier = entity.DocumentMetaDataIdentifier,
            Key = entity.Key,
            Value = entity.Value
        };
    
    public static DocumentMetaData ToEntity(this DocumentMetaDataDto dto) => new()
    {
        DocumentMetaDataIdentifier = dto.DocumentMetaDataIdentifier,
        Key = dto.Key,
        Value = dto.Value
    };
}



