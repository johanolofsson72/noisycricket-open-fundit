namespace Shared.Applications.DTOs;

public class EconomyApplicationDto
{
    public int Id { get; set; } = 0;
    public decimal MilestonePayoutTotalAmount { get; set; } = 0;
    public decimal EarlierSupportTotalAmount { get; set; } = 0;
    public decimal OurContribution { get; set; } = 0;
    public bool InternalBudgetsApproved { get; set; }
    public List<ApplicationControlDto> Controls { get; set; } = [];
    public List<ApplicationBudgetDto> InternalBudgets { get; set; } = [];
}