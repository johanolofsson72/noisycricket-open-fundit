namespace Shared.Schemas.DTOs;

public class AddSchemaProgressDto
{
    public int SchemaId { get; set; } = 0;
    public decimal PercentageOfAmount { get; set; } = 0;
    public int MonthToExpire { get; set; } = 0;
    public SchemaProgressDto Progress { get; set; } = new SchemaProgressDto();
}