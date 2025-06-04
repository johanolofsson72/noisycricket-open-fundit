namespace Shared.Applications.Entities;

public class MiniApplication
{
    public int Id { get; set; } = 0;
    public decimal OurContribution { get; set; } = 0;
}

public static class MiniApplicationExtensions
{
    public static MiniApplicationDto ToDto(this MiniApplication entity)
    {
        return new MiniApplicationDto()
        {
            Id = entity.Id,
            OurContribution = entity.OurContribution
        };
    }

    public static MiniApplication ToEntity(this MiniApplicationDto dto)
    {
        return new MiniApplication
        {
            Id = dto.Id,
            OurContribution = dto.OurContribution
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
