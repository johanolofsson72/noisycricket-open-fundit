using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Applications.Entities;
using Shared.Controls.DTOs;

namespace Shared.Controls.Entities;

public class Control
{
    public int Id { get; set; }
    public int ControlTypeId { get; set; } = 0;
    [MaxLength(500)] public string ControlTypeName{ get; set; } = string.Empty;
    [MaxLength(500)] public string ValueType { get; set; } = string.Empty;
    [MaxLength(500)] public string BaseStructure { get; set; } = string.Empty;
}

public static class ControlExtensions
{
    public static IEnumerable<ControlDto> ToDto(this IEnumerable<Control> entities)
    {
        return entities.Select(entity => entity.ToDto());
    }
    
    public static IEnumerable<Control> ToEntity(this IEnumerable<ControlDto> dtos)
    {
        return dtos.Select(dto => dto.ToEntity());
    }
    
    public static ControlDto ToDto(this Control entity)
    {
        return new ControlDto()
        {
            Id = entity.Id,
            ControlTypeId = entity.ControlTypeId,
            ControlTypeName = entity.ControlTypeName,
            ValueType = entity.ValueType,
            BaseStructure = entity.BaseStructure
        };
    }
    
    public static Control ToEntity(this ControlDto dto)
    {
        return new Control()
        {
            Id = dto.Id,
            ControlTypeId = dto.ControlTypeId,
            ControlTypeName = dto.ControlTypeName,
            ValueType = dto.ValueType,
            BaseStructure = dto.BaseStructure
        };
    }
    
    public static ApplicationControl ToApplicationControl(this Control entity)
    {
        return new ApplicationControl()
        {
            ControlId = entity.Id,
            ControlTypeId = entity.ControlTypeId,
            ControlTypeName = entity.ControlTypeName,
            ControlValueType = entity.ValueType,
            BaseStructure = entity.BaseStructure
        };
    }
    
    public static Control ToControl(this ApplicationControl entity)
    {
        return new Control()
        {
            Id = entity.ControlId,
            ControlTypeId = entity.ControlTypeId,
            ControlTypeName = entity.ControlTypeName,
            ValueType = entity.ControlValueType,
            BaseStructure = entity.BaseStructure
        };
    }
    
}

