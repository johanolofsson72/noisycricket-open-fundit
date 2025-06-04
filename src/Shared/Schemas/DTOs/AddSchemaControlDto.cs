
namespace Shared.Schemas.DTOs;

public class AddSchemaControlDto
{
    public int SchemaId { get; set; } = 0;
    public SchemaControlDto Control { get; set; } = new SchemaControlDto();
}