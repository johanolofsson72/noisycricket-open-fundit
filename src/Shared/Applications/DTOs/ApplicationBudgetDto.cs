using System;

namespace Shared.Applications.DTOs;

public class ApplicationBudgetDto
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int ApplicationBudgetTypeId { get; set; } = 0;
    public decimal Amount { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public DateTime ApprovedDate { get; set; } = DateTime.MinValue;
    public string Name { get; set; } = string.Empty;

}