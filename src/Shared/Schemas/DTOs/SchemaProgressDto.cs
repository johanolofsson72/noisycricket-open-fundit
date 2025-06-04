namespace Shared.Schemas.DTOs;

public class SchemaProgressDto
{
    public int Id { get; set; } = 0;
    public int SchemaProgressIdentifier { get; set; } = 0;
    public decimal PercentageOfAmount { get; set; } = 0;
    public int MonthToExpire { get; set; } = 0;
    public List<SchemaProgressRequirementDto> Requirements { get; set; } = [];
}