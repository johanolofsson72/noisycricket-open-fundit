namespace Shared.Applications.Entities;

public class CounterApplication
{
    public int Id { get; set; } = 0;
    public decimal OurContribution { get; set; } = 0;
    public int NewEventCounter { get; set; } = 0;
    public int NewAuditCounter { get; set; } = 0;
}



public static class CounterApplicationExtensions
{
    public static CounterApplicationDto ToDto(this CounterApplication entity)
    {
        return new CounterApplicationDto()
        {
            Id = entity.Id,
            OurContribution = entity.OurContribution,
            NewEventCounter = entity.NewEventCounter,
            NewAuditCounter = entity.NewAuditCounter,
        };
    }

    public static CounterApplication ToEntity(this CounterApplicationDto dto)
    {
        return new CounterApplication
        {
            Id = dto.Id,
            OurContribution = dto.OurContribution,
            NewEventCounter = dto.NewEventCounter,
            NewAuditCounter = dto.NewAuditCounter,
        };
    }

    public static IEnumerable<CounterApplicationDto> ToDto(this IEnumerable<CounterApplication> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<CounterApplication> ToEntity(this IEnumerable<CounterApplicationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
}
