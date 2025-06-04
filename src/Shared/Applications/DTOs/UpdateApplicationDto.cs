using System.Collections.Generic;

namespace Shared.Applications.DTOs;

public class UpdateApplicationDto
{
    public int ParentId { get; set; } = 0;
    public int ProjectId { get; set; } = 0;
    public string ProjectNumber { get; set; } = string.Empty;
    public int StatusId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public ApplicationContactDto Organization { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto Producer { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto Applicant { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ProjectManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ProductionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto FinanceManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ScriptManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto DistributionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ContractManager { get; set; } = new ApplicationContactDto();
    public string Title { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public IEnumerable<ApplicationBudgetDto> InternalBudgets { get; set; } = new List<ApplicationBudgetDto>();
    public decimal InternalBudgetsTotalAmount { get; set; } = 0;
    public decimal MilestonePayoutTotalAmount { get; set; } = 0;
    public decimal EarlierSupportTotalAmount { get; set; } = 0;
    public bool InternalBudgetsApproved { get; set; }
    public IEnumerable<ApplicationControlDto> Controls { get; set; } = new List<ApplicationControlDto>();
    public decimal BudgetAmount { get; set; } = 0;
    public decimal AppliedAmount { get; set; } = 0;
    public decimal OurContribution { get; set; } = 0;
    public DateTime DeliveryDate { get; set; } = DateTime.MinValue;
}