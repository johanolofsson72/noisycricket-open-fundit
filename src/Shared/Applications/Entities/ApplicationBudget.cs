using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Applications.DTOs;

namespace Shared.Applications.Entities;

public class ApplicationBudget
{
    public int BudgetIdentifier { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int ApplicationBudgetTypeId { get; set; } = 0;
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18, 4)")] public decimal Amount { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public DateTime ApprovedDate { get; set; } = DateTime.MinValue;
    public virtual Application Application { get; set; } = new Application();
}
    
public static class ApplicationBudgetExtensions
{
    public static ApplicationBudgetDto ToDto(this ApplicationBudget entity) =>
        new ApplicationBudgetDto()
        {
            Id = entity.BudgetIdentifier,
            StatusId = entity.StatusId,
            ApplicationBudgetTypeId = entity.ApplicationBudgetTypeId,
            Amount = entity.Amount,
            CreatedDate = entity.CreatedDate,
            ApprovedDate = entity.ApprovedDate,
            Name = entity.Name
        };
    
    public static ApplicationBudget ToEntity(this ApplicationBudgetDto dto) => new()
    {
        BudgetIdentifier = dto.Id,
        StatusId = dto.StatusId,
        ApplicationBudgetTypeId = dto.ApplicationBudgetTypeId,
        Amount = dto.Amount,
        CreatedDate = dto.CreatedDate,
        ApprovedDate = dto.ApprovedDate,
        Name = dto.Name
    };
}