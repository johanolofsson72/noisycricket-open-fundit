namespace Shared.Applications.Entities;

public class ApplicationProgress
{
    public int Id { get; set; } = 0;
    public int ApplicationProgressIdentifier { get; set; } = 0;
    [Column(TypeName = "decimal(18, 4)")] public decimal PercentageOfAmount { get; set; } = 0;
    public int MonthToExpire { get; set; } = 0;
    public List<ApplicationProgressRequirement> Requirements { get; set; } = [];
    public virtual Application Application { get; set; } = new Application();
}

public static class ApplicationProgressExtensions
{
    public static ApplicationProgressDto ToDto(this ApplicationProgress entity)
    {
        return new ApplicationProgressDto()
        {
            Id = entity.Id,
            ApplicationProgressIdentifier = entity.ApplicationProgressIdentifier,
            PercentageOfAmount = entity.PercentageOfAmount,
            MonthToExpire = entity.MonthToExpire,
            Requirements = entity.Requirements.Select(x => x.ToDto()).ToList()
        };
    }
    
    public static ApplicationProgress ToEntity(this ApplicationProgressDto dto)
    {
        return new ApplicationProgress()
        {
            Id = dto.Id,
            ApplicationProgressIdentifier = dto.ApplicationProgressIdentifier,
            PercentageOfAmount = dto.PercentageOfAmount,
            MonthToExpire = dto.MonthToExpire,
            Requirements = dto.Requirements.Select(x => x.ToEntity()).ToList()
        };
    }
}