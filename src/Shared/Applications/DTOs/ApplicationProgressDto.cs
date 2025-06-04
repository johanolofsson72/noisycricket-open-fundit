namespace Shared.Applications.DTOs;

public class ApplicationProgressDto
{
    public int Id { get; set; } = 0;
    public int ApplicationProgressIdentifier { get; set; } = 0;
    public decimal PercentageOfAmount { get; set; } = 0;
    public int MonthToExpire { get; set; } = 0;
    public List<ApplicationProgressRequirementDto> Requirements { get; set; } = [];
}