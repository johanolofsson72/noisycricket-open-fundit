using System;
using Shared.Applications.DTOs;

namespace Shared.Applications.Entities;

public class ApplicationControl
{
    public int Id { get; set; } = 0;
    public int ApplicationControlIdentifier { get; set; } = 0;
    public int ControlId { get; set; }
    public int ControlTypeId { get; set; } = 0;
    [MaxLength(500)] public string ControlValueType { get; set; } = string.Empty;
    [MaxLength(500)] public string ControlTypeName { get; set; } = string.Empty;
    [MaxLength(5000)] public string BaseStructure { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public bool VisibleOnApplicationForm { get; set; }
    public bool VisibleOnOverview { get; set; }
    public int ApplicationFormPage { get; set; } = 0;
    public int ApplicationFormOrder { get; set; } = 0;
    public bool ApplicationFormRequired { get; set; }
    [MaxLength(500)] public string Css { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = ["", "", "", "", "", "", "", ""];
    public int ApplicationFormSectionId { get; set; } = 0;
    public int ApplicationSectionId { get; set; } = 0;
    [MaxLength(5000)] public string Value { get; set; } = string.Empty;
    public List<string> Placeholders { get; set; } = ["", "", "", "", "", "", "", ""];
    [MaxLength(5000)] public string DataSource { get; set; } = string.Empty;
    public List<string> SubLabels { get; set; } = ["", "", "", "", "", "", "", ""];
    public Guid UniqueId { get; set; } = Guid.Empty;
    
    public int MaxValueLength { get; set; } = 0;
    //public virtual Application Application { get; set; } = new Application();
}

public static class ApplicationControlExtensions
{
    public static ApplicationControlDto ToDto(this ApplicationControl entity)
    {
        return new ApplicationControlDto()
        {
            Id = entity.Id,
            ControlId = entity.ControlId,
            ControlTypeName = entity.ControlTypeName,
            ControlTypeId = entity.ControlTypeId,
            ControlValueType = entity.ControlValueType,
            BaseStructure = entity.BaseStructure,
            Order = entity.Order,
            VisibleOnApplicationForm = entity.VisibleOnApplicationForm,
            VisibleOnOverview = entity.VisibleOnOverview,
            ApplicationFormPage = entity.ApplicationFormPage,
            ApplicationFormOrder = entity.ApplicationFormOrder,
            ApplicationFormRequired = entity.ApplicationFormRequired,
            Css = entity.Css,
            Labels = entity.Labels,
            ApplicationFormSectionId = entity.ApplicationFormSectionId,
            ApplicationSectionId = entity.ApplicationSectionId,
            Value = entity.Value,
            Placeholders = entity.Placeholders,
            DataSource = entity.DataSource,
            SubLabels = entity.SubLabels,
            UniqueId = entity.UniqueId,
            MaxValueLength = entity.MaxValueLength
        };
    }

    public static ApplicationControl ToEntity(this ApplicationControlDto dto)
    {
        return new ApplicationControl
        {
            Id = dto.Id,
            ApplicationControlIdentifier = dto.Id,
            ControlId = dto.ControlId,
            ControlTypeName = dto.ControlTypeName,
            ControlTypeId = dto.ControlTypeId,
            ControlValueType = dto.ControlValueType,
            BaseStructure = dto.BaseStructure,
            Order = dto.Order,
            VisibleOnApplicationForm = dto.VisibleOnApplicationForm,
            VisibleOnOverview = dto.VisibleOnOverview,
            ApplicationFormPage = dto.ApplicationFormPage,
            ApplicationFormOrder = dto.ApplicationFormOrder,
            ApplicationFormRequired = dto.ApplicationFormRequired,
            Css = dto.Css,
            Labels = dto.Labels,
            ApplicationFormSectionId = dto.ApplicationFormSectionId,
            ApplicationSectionId = dto.ApplicationSectionId,
            Value = dto.Value,
            Placeholders = dto.Placeholders,
            DataSource = dto.DataSource,
            SubLabels = dto.SubLabels,
            UniqueId = dto.UniqueId,
            MaxValueLength = dto.MaxValueLength
        };
    }
}