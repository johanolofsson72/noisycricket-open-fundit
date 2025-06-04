using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Applications.Entities;
using Shared.Schemas.DTOs;

namespace Shared.Schemas.Entities;

public class SchemaControl
{
    public int Id { get; set; } = 0;
    public int SchemaControlIdentifier { get; set; } = 0;
    public int ControlId { get; set; } = 0;
    public int ControlTypeId { get; set; } = 0;
    
    [MaxLength(500)] public string ControlTypeName { get; set; } = string.Empty;
    [MaxLength(500)] public string ControlValueType { get; set; } = string.Empty;
    [MaxLength(5000)] public string BaseStructure { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public bool VisibleOnApplicationForm { get; set; }
    public bool VisibleOnOverview { get; set; }
    public int ApplicationFormPage { get; set; }
    public int ApplicationFormOrder { get; set; }
    public bool ApplicationFormRequired { get; set; }
    [MaxLength(5000)] public string Css { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = ["", "", "", "", "", "", "", ""];
    public int ApplicationSectionId { get; set; } = 0;
    public int ApplicationFormSectionId { get; set; } = 0;
    public List<string> Placeholders { get; set; } = ["", "", "", "", "", "", "", ""];
    [MaxLength(5000)] public string DataSource { get; set; } = string.Empty;
    public List<string> SubLabels { get; set; } = ["", "", "", "", "", "", "", ""];
    public Guid UniqueId { get; set; } = Guid.NewGuid();
    public int MaxValueLength { get; set; }
    public virtual Schema Schema { get; set; } = new Schema();
}


public static class SchemaControlExtensions
{
    public static List<SchemaControlDto> ToDto(this List<SchemaControl> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
    
    public static List<SchemaControl> ToEntity(this List<SchemaControlDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }
    
    public static SchemaControlDto ToDto(this SchemaControl entity)
    {
        return new SchemaControlDto()
        {
            Id = entity.Id,
            SchemaControlIdentifier = entity.SchemaControlIdentifier,
            ControlId = entity.ControlId,
            ControlTypeId = entity.ControlTypeId,
            ControlTypeName = entity.ControlTypeName,
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
            ApplicationSectionId = entity.ApplicationSectionId,
            ApplicationFormSectionId = entity.ApplicationFormSectionId,
            Placeholders = entity.Placeholders,
            DataSource = entity.DataSource,
            SubLabels = entity.SubLabels,
            MaxValueLength = entity.MaxValueLength,
            UniqueId = entity.UniqueId
        };
    }
    
    public static SchemaControl ToEntity(this SchemaControlDto dto)
    {
        return new SchemaControl()
        {
            Id = dto.Id,
            SchemaControlIdentifier = dto.SchemaControlIdentifier,
            ControlId = dto.ControlId,
            ControlTypeId = dto.ControlTypeId,
            ControlTypeName = dto.ControlTypeName,
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
            ApplicationSectionId = dto.ApplicationSectionId,
            ApplicationFormSectionId = dto.ApplicationFormSectionId,
            Placeholders = dto.Placeholders,
            DataSource = dto.DataSource,
            SubLabels = dto.SubLabels,
            MaxValueLength = dto.MaxValueLength,
            UniqueId = dto.UniqueId
        };
    }
    
    public static ApplicationControl ToApplicationControl(this SchemaControl entity)
    {
        return new ApplicationControl()
        {
            Id = entity.Id,
            ApplicationControlIdentifier = entity.SchemaControlIdentifier,
            ControlId = entity.ControlId,
            ControlTypeId = entity.ControlTypeId,
            ControlTypeName = entity.ControlTypeName,
            ControlValueType = entity.ControlValueType,
            BaseStructure = entity.BaseStructure,
            Order = entity.Order,
            VisibleOnApplicationForm = entity.VisibleOnApplicationForm,
            ApplicationFormPage = entity.ApplicationFormPage,
            ApplicationFormOrder = entity.ApplicationFormOrder,
            ApplicationFormRequired = entity.ApplicationFormRequired,
            Css = entity.Css,
            Labels = entity.Labels,
            ApplicationSectionId = entity.ApplicationSectionId,
            ApplicationFormSectionId = entity.ApplicationFormSectionId,
            Value = string.Empty,
            Placeholders = entity.Placeholders,
            DataSource = entity.DataSource,
            SubLabels = entity.SubLabels,
            MaxValueLength = entity.MaxValueLength,
            UniqueId = entity.UniqueId,
        };
    }

    public static SchemaControl ToSchemaControl(this ApplicationControl dto)
    {
        return new SchemaControl()
        {
            Id = dto.Id,
            SchemaControlIdentifier = dto.ApplicationControlIdentifier,
            ControlId = dto.ControlId,
            ControlTypeId = dto.ControlTypeId,
            ControlTypeName = dto.ControlTypeName,
            ControlValueType = dto.ControlValueType,
            BaseStructure = dto.BaseStructure,
            Order = dto.Order,
            VisibleOnApplicationForm = dto.VisibleOnApplicationForm,
            ApplicationFormPage = dto.ApplicationFormPage,
            ApplicationFormOrder = dto.ApplicationFormOrder,
            ApplicationFormRequired = dto.ApplicationFormRequired,
            Css = dto.Css,
            Labels = dto.Labels,
            ApplicationSectionId = dto.ApplicationSectionId,
            ApplicationFormSectionId = dto.ApplicationFormSectionId,
            Placeholders = dto.Placeholders,
            DataSource = dto.DataSource,
            SubLabels = dto.SubLabels,
            MaxValueLength = dto.MaxValueLength,
            UniqueId = dto.UniqueId
        };
    }
    
    public static List<ApplicationControl> ToApplicationControl(this List<SchemaControl> entities)
    {
        return entities.Select(x => x.ToApplicationControl()).ToList();
    }
    
    public static List<SchemaControl> ToSchemaControl(this List<ApplicationControl> dtos)
    {
        return dtos.Select(x => x.ToSchemaControl()).ToList();
    }
    
    public static ApplicationControl ToApplicationControl(this SchemaControlDto dto)
    {
        return new ApplicationControl()
        {
            Id = dto.Id,
            ApplicationControlIdentifier = dto.SchemaControlIdentifier,
            ControlId = dto.ControlId,
            ControlTypeId = dto.ControlTypeId,
            ControlTypeName = dto.ControlTypeName,
            ControlValueType = dto.ControlValueType,
            BaseStructure = dto.BaseStructure,
            Order = dto.Order,
            VisibleOnApplicationForm = dto.VisibleOnApplicationForm,
            ApplicationFormPage = dto.ApplicationFormPage,
            ApplicationFormOrder = dto.ApplicationFormOrder,
            ApplicationFormRequired = dto.ApplicationFormRequired,
            Css = dto.Css,
            Labels = dto.Labels,
            ApplicationSectionId = dto.ApplicationSectionId,
            ApplicationFormSectionId = dto.ApplicationFormSectionId,
            Value = string.Empty,
            Placeholders = dto.Placeholders,
            DataSource = dto.DataSource,
            SubLabels = dto.SubLabels,
            MaxValueLength = dto.MaxValueLength,
            UniqueId = dto.UniqueId
        };
    }
    
    public static SchemaControlDto ToSchemaControlDto(this ApplicationControl dto)
    {
        return new SchemaControlDto()
        {
            Id = dto.Id,
            SchemaControlIdentifier = dto.ApplicationControlIdentifier,
            ControlId = dto.ControlId,
            ControlTypeId = dto.ControlTypeId,
            ControlTypeName = dto.ControlTypeName,
            ControlValueType = dto.ControlValueType,
            BaseStructure = dto.BaseStructure,
            Order = dto.Order,
            VisibleOnApplicationForm = dto.VisibleOnApplicationForm,
            ApplicationFormPage = dto.ApplicationFormPage,
            ApplicationFormOrder = dto.ApplicationFormOrder,
            ApplicationFormRequired = dto.ApplicationFormRequired,
            Css = dto.Css,
            Labels = dto.Labels,
            ApplicationSectionId = dto.ApplicationSectionId,
            ApplicationFormSectionId = dto.ApplicationFormSectionId,
            Placeholders = dto.Placeholders,
            DataSource = dto.DataSource,
            SubLabels = dto.SubLabels,
            MaxValueLength = dto.MaxValueLength,
            UniqueId = dto.UniqueId 
        };
    }
    
    public static List<ApplicationControl> ToApplicationControl(this List<SchemaControlDto> dtos)
    {
        return dtos.Select(x => x.ToApplicationControl()).ToList();
    }
    
    public static List<SchemaControlDto> ToSchemaControlDto(this List<ApplicationControl> dtos)
    {
        return dtos.Select(x => x.ToSchemaControlDto()).ToList();
    }
}
