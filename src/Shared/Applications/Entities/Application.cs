using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Applications.DTOs;
using Shared.Projects.Entities;

namespace Shared.Applications.Entities;


public class Application
{
    public int Id { get; set; } = 0;
    public int ParentId { get; set; } = 0;
    public int ProjectId { get; set; } = 0;
    [MaxLength(500)] public string ProjectNumber { get; set; } = string.Empty;
    public int StatusId { get; set; } = 0;
    public int SchemaId { get; set; }
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string SchemaClaimTag { get; set; } = "";
    public ApplicationContact Producer { get; set; } = new ApplicationContact();
    public ApplicationContact Organization { get; set; } = new ApplicationContact();
    public ApplicationContact Applicant { get; set; } = new ApplicationContact();
    public ApplicationContact ProjectManager { get; set; } = new ApplicationContact();
    public ApplicationContact ProductionManager { get; set; } = new ApplicationContact();
    public ApplicationContact FinanceManager { get; set; } = new ApplicationContact();
    public ApplicationContact ScriptManager { get; set; } = new ApplicationContact();
    public ApplicationContact DistributionManager { get; set; } = new ApplicationContact();
    public ApplicationContact ContractManager { get; set; } = new ApplicationContact();
    [MaxLength(500)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string Number { get; set; } = string.Empty;
    public List<ApplicationBudget> InternalBudgets { get; set; } = new();
    public int NewEventCounter { get; set; } = 0;
    public int NewAuditCounter { get; set; } = 0;
    public decimal InternalBudgetsTotalAmount { get; set; } = 0;
    public decimal MilestonePayoutTotalAmount { get; set; } = 0;
    public decimal EarlierSupportTotalAmount { get; set; } = 0;
    public bool InternalBudgetsApproved { get; set; }
    public List<ApplicationControl> Controls { get; set; } = [];
    public List<ApplicationEvent> Events { get; set; } = [];
    public List<ApplicationProgress> Progress { get; set; } = [];
    public List<ApplicationRequiredDocument> RequiredDocuments { get; set; } = [];
    public List<ApplicationAudit> Audits { get; set; } = [];
    [Column(TypeName = "decimal(18, 4)")] public decimal BudgetAmount { get; set; } = 0;
    [Column(TypeName = "decimal(18, 4)")] public decimal AppliedAmount { get; set; } = 0;
    [Column(TypeName = "decimal(18, 4)")] public decimal OurContribution { get; set; } = 0;
    public DateTime SignedContractDate { get; set; } = DateTime.MinValue;
    public DateTime DecisionDate { get; set; } = DateTime.MinValue;
    public DateTime UpdatedDate { get; set; } = DateTime.MinValue;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public DateTime DeliveryDate { get; set; } = DateTime.MinValue;
}

public static class ApplicationExtensions
{
    public static ApplicationDto ToDto(this Application entity)
    {
        return new ApplicationDto()
        {
            Id = entity.Id,
            ParentId = entity.ParentId,
            ProjectId = entity.ProjectId,
            ProjectNumber = entity.ProjectNumber,
            StatusId = entity.StatusId,
            SchemaId = entity.SchemaId,
            SchemaNames = entity.SchemaNames,
            SchemaClaimTag = entity.SchemaClaimTag,
            Organization = entity.Organization.ToDto(),
            Producer = entity.Producer.ToDto(),
            Applicant = entity.Applicant.ToDto(),
            ProjectManager = entity.ProjectManager.ToDto(),
            ProductionManager = entity.ProductionManager.ToDto(),
            FinanceManager = entity.FinanceManager.ToDto(),
            ScriptManager = entity.ScriptManager.ToDto(),
            DistributionManager = entity.DistributionManager.ToDto(),
            ContractManager = entity.ContractManager.ToDto(),
            Title = entity.Title,
            Number = entity.Number,
            NewEventCounter = entity.NewEventCounter,
            NewAuditCounter = entity.NewAuditCounter,
            InternalBudgets = entity.InternalBudgets.Select(b => b.ToDto()).ToList(),
            InternalBudgetsTotalAmount = entity.InternalBudgetsTotalAmount,
            MilestonePayoutTotalAmount = entity.MilestonePayoutTotalAmount,
            EarlierSupportTotalAmount = entity.EarlierSupportTotalAmount,
            InternalBudgetsApproved = entity.InternalBudgetsApproved,
            Controls = entity.Controls.Select(m => m.ToDto()).ToList(),
            Events = entity.Events.Select(x => x.ToDto()).ToList(),
            BudgetAmount = entity.BudgetAmount,
            AppliedAmount = entity.AppliedAmount,
            DecisionDate = entity.DecisionDate,
            UpdatedDate = entity.UpdatedDate,
            CreatedDate = entity.CreatedDate,
            DeliveryDate = entity.DeliveryDate,
            OurContribution = entity.OurContribution,
            Progress = entity.Progress.Select(p => p.ToDto()).ToList(),
            RequiredDocuments = entity.RequiredDocuments.ToDto(),
            Audits = entity.Audits.Select(a => a.ToDto()).ToList()
        };
    }
    
    public static Application ToEntity(this ApplicationDto dto)
    {
        return new Application
        {
            Id = dto.Id,
            ParentId = dto.ParentId,
            ProjectId = dto.ProjectId,
            ProjectNumber = dto.ProjectNumber,
            StatusId = dto.StatusId,
            SchemaId = dto.SchemaId,
            SchemaNames = dto.SchemaNames,
            SchemaClaimTag = dto.SchemaClaimTag,
            Organization = dto.Organization.ToEntity(),
            Producer = dto.Producer.ToEntity(),
            Applicant = dto.Applicant.ToEntity(),
            ProjectManager = dto.ProjectManager.ToEntity(),
            ProductionManager = dto.ProductionManager.ToEntity(),
            FinanceManager = dto.FinanceManager.ToEntity(),
            ScriptManager = dto.ScriptManager.ToEntity(),
            DistributionManager = dto.DistributionManager.ToEntity(),
            ContractManager = dto.ContractManager.ToEntity(),
            Title = dto.Title,
            Number = dto.Number,
            NewEventCounter = dto.NewAuditCounter,
            NewAuditCounter = dto.NewAuditCounter,
            InternalBudgets = dto.InternalBudgets.Select(b => b.ToEntity()).ToList(),
            InternalBudgetsTotalAmount = dto.InternalBudgetsTotalAmount,
            MilestonePayoutTotalAmount = dto.MilestonePayoutTotalAmount,
            EarlierSupportTotalAmount = dto.EarlierSupportTotalAmount,
            InternalBudgetsApproved = dto.InternalBudgetsApproved,
            Controls = dto.Controls.Select(m => m.ToEntity()).ToList(),
            Events = dto.Events.Select(x => x.ToEntity()).ToList(),
            BudgetAmount = dto.BudgetAmount,
            AppliedAmount = dto.AppliedAmount,
            DecisionDate = dto.DecisionDate,
            UpdatedDate = dto.UpdatedDate,
            CreatedDate = dto.CreatedDate,
            DeliveryDate = dto.DeliveryDate,
            OurContribution = dto.OurContribution,
            Progress = dto.Progress.Select(p => p.ToEntity()).ToList(),
            RequiredDocuments = dto.RequiredDocuments.ToEntity(),
            Audits = dto.Audits.Select(a => a.ToEntity()).ToList()
        };
    }

    public static IEnumerable<ApplicationDto> ToDto(this IEnumerable<Application> entity)
    {
        return entity.Select(e => e.ToDto());
    } 
    
    public static IEnumerable<Application> ToEntity(this IEnumerable<ApplicationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
    
    public static Application ToEntity(this UpdateApplicationDto dto)
    {
        return new Application
        {
            ParentId = dto.ParentId,
            ProjectId = dto.ProjectId,
            ProjectNumber = dto.ProjectNumber,
            StatusId = dto.StatusId,
            SchemaId = dto.SchemaId,
            Organization = dto.Organization.ToEntity(),
            Producer = dto.Producer.ToEntity(),
            Applicant = dto.Applicant.ToEntity(),
            ProjectManager = dto.ProjectManager.ToEntity(),
            ProductionManager = dto.ProductionManager.ToEntity(),
            FinanceManager = dto.FinanceManager.ToEntity(),
            ScriptManager = dto.ScriptManager.ToEntity(),
            DistributionManager = dto.DistributionManager.ToEntity(),
            ContractManager = dto.ContractManager.ToEntity(),
            Title = dto.Title,
            Number = dto.Number,
            InternalBudgets = dto.InternalBudgets.Select(b => b.ToEntity()).ToList(),
            InternalBudgetsTotalAmount = dto.InternalBudgetsTotalAmount,
            MilestonePayoutTotalAmount = dto.MilestonePayoutTotalAmount,
            EarlierSupportTotalAmount = dto.EarlierSupportTotalAmount,
            InternalBudgetsApproved = dto.InternalBudgetsApproved,
            Controls = dto.Controls.Select(m => m.ToEntity()).ToList(),
            BudgetAmount = dto.BudgetAmount,
            AppliedAmount = dto.AppliedAmount
        };
    }

    public static UpdateApplicationDto ToUpdate(this ApplicationDto dto)
    {
        return new UpdateApplicationDto()
        {
            ParentId = dto.ParentId,
            ProjectId = dto.ProjectId,
            ProjectNumber = dto.ProjectNumber,
            StatusId = dto.StatusId,
            SchemaId = dto.SchemaId,
            Organization = dto.Organization,
            Producer = dto.Producer,
            Applicant = dto.Applicant,
            ProjectManager = dto.ProjectManager,
            ProductionManager = dto.ProductionManager,
            FinanceManager = dto.FinanceManager,
            ScriptManager = dto.ScriptManager,
            DistributionManager = dto.DistributionManager,
            ContractManager = dto.ContractManager,
            Title = dto.Title,
            Number = dto.Number,
            InternalBudgets = dto.InternalBudgets,
            InternalBudgetsTotalAmount = dto.InternalBudgetsTotalAmount,
            MilestonePayoutTotalAmount = dto.MilestonePayoutTotalAmount,
            EarlierSupportTotalAmount = dto.EarlierSupportTotalAmount,
            InternalBudgetsApproved = dto.InternalBudgetsApproved,
            Controls = dto.Controls,
            BudgetAmount = dto.BudgetAmount,
            AppliedAmount = dto.AppliedAmount
        };
    }
}