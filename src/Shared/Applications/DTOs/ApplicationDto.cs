using System;
using System.Collections.Generic;
using Shared.Schemas.DTOs;

namespace Shared.Applications.DTOs;

public class ApplicationDto
{
    public int Id { get; set; } = 0;
    public int ParentId { get; set; } = 0;
    public int ProjectId { get; set; } = 0;
    public string ProjectNumber { get; set; } = string.Empty;
    public int StatusId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string SchemaClaimTag { get; set; } = "";
    public ApplicationContactDto Producer { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto Organization { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto Applicant { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ProjectManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ProductionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto FinanceManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ScriptManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto DistributionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ContractManager { get; set; } = new ApplicationContactDto();
    public string Title { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public List<ApplicationBudgetDto> InternalBudgets { get; set; } = new();
    public int NewEventCounter { get; set; } = 0;
    public int NewAuditCounter { get; set; } = 0;
    public decimal InternalBudgetsTotalAmount { get; set; } = 0;
    public decimal MilestonePayoutTotalAmount { get; set; } = 0;
    public decimal EarlierSupportTotalAmount { get; set; } = 0;
    public bool InternalBudgetsApproved { get; set; }
    public List<ApplicationControlDto> Controls { get; set; } = [];
    public List<ApplicationEventDto> Events { get; set; } = [];
    public List<ApplicationProgressDto> Progress { get; set; } = [];
    public List<ApplicationRequiredDocumentDto> RequiredDocuments { get; set; } = [];
    public List<ApplicationAuditDto> Audits { get; set; } = [];
    public decimal BudgetAmount { get; set; } = 0;
    public decimal AppliedAmount { get; set; } = 0;
    public decimal OurContribution { get; set; } = 0;
    public DateTime DecisionDate { get; set; } = DateTime.MinValue;
    public DateTime UpdatedDate { get; set; } = DateTime.MinValue;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public DateTime DeliveryDate { get; set; } = DateTime.MinValue;
}