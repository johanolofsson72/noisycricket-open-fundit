using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Shared.Milestones.DTOs;

namespace Shared.Milestones.Entities;

public class MilestonePayment
{
    public int MilestonePaymentIdentifier { get; set; } = 0;
    [Column(TypeName = "decimal(18, 4)")] public decimal Amount { get; set; } = 0;
    public DateTime CreatedDate { get; set; }
    [MaxLength(250)] public string Note { get; set; } = string.Empty;
    public virtual Milestone Milestone { get; set; } = new Milestone();
}

public static class MilestonePaymentExtensions
{
    public static IEnumerable<MilestonePaymentDto> ToDto(this IEnumerable<MilestonePayment> entities)
    {
        return entities.Select(entity => entity.ToDto());
    }
    
    public static IEnumerable<MilestonePayment> ToEntity(this IEnumerable<MilestonePaymentDto> dtos)
    {
        return dtos.Select(dto => dto.ToEntity());
    }
    
    public static MilestonePaymentDto ToDto(this MilestonePayment entity)
    {
        return new MilestonePaymentDto()
        {
            MilestonePaymentIdentifier = entity.MilestonePaymentIdentifier,
            Amount = entity.Amount,
            CreatedDate = entity.CreatedDate,
            Note = entity.Note
        };
    }
    
    public static MilestonePayment ToEntity(this MilestonePaymentDto dto)
    {   
        return new MilestonePayment()
        {
            MilestonePaymentIdentifier = dto.MilestonePaymentIdentifier,
            Amount = dto.Amount,
            CreatedDate = dto.CreatedDate,
            Note = dto.Note
        };
    }
    
}







