using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Milestones.DTOs;

public class MilestonePaymentDto
{
    public int MilestonePaymentIdentifier { get; set; } = 0;
    [Column(TypeName = "decimal(18, 4)")] public decimal Amount { get; set; } = 0;
    public DateTime CreatedDate { get; set; }
    public string Note { get; set; } = string.Empty;
}