namespace Shared.Applications.Entities;

public class EconomyApplication
{
    public int Id { get; set; } = 0;
    public decimal MilestonePayoutTotalAmount { get; set; } = 0;
    public decimal EarlierSupportTotalAmount { get; set; } = 0;
    public decimal OurContribution { get; set; } = 0;
    public bool InternalBudgetsApproved { get; set; }
    public List<ApplicationControl> Controls { get; set; } = [];
    public List<ApplicationBudget> InternalBudgets { get; set; } = [];
}

public static class EconomyApplicationExtensions
{
    public static EconomyApplicationDto ToDto(this EconomyApplication entity)
    {
        return new EconomyApplicationDto()
        {
            Id = entity.Id,
            MilestonePayoutTotalAmount = entity.MilestonePayoutTotalAmount,
            EarlierSupportTotalAmount = entity.EarlierSupportTotalAmount,
            OurContribution = entity.OurContribution,
            InternalBudgetsApproved = entity.InternalBudgetsApproved,
            Controls = entity.Controls.Select(x => x.ToDto()).ToList(),
            InternalBudgets = entity.InternalBudgets.Select(x => x.ToDto()).ToList()
        };
    }

    public static EconomyApplication ToEntity(this EconomyApplicationDto dto)
    {
        return new EconomyApplication
        {
            Id = dto.Id,
            MilestonePayoutTotalAmount = dto.MilestonePayoutTotalAmount,
            EarlierSupportTotalAmount = dto.EarlierSupportTotalAmount,
            OurContribution = dto.OurContribution,
            InternalBudgetsApproved = dto.InternalBudgetsApproved,
            Controls = dto.Controls.Select(x => x.ToEntity()).ToList(),
            InternalBudgets = dto.InternalBudgets.Select(x => x.ToEntity()).ToList()
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
