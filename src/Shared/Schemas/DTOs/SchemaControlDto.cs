
using System;

namespace Shared.Schemas.DTOs;

public class SchemaControlDto
{
    public int Id { get; set; } = 0;
    public int SchemaControlIdentifier { get; set; } = 0;
    public int ControlId { get; set; } = 0;
    public int ControlTypeId { get; set; } = 0;
    
    public string ControlTypeName { get; set; } = string.Empty;
    public string ControlValueType { get; set; } = string.Empty;
    public string BaseStructure { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public bool VisibleOnApplicationForm { get; set; }
    public bool VisibleOnOverview { get; set; }
    public int ApplicationFormPage { get; set; } = 0;
    public int ApplicationFormOrder { get; set; } = 0;
    public bool ApplicationFormRequired { get; set; }
    public string Css { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = ["", "", "", "", "", "", "", ""];
    public int ApplicationSectionId { get; set; } = 0;
    public int ApplicationFormSectionId { get; set; } = 0;
    public List<string> Placeholders { get; set; } = ["", "", "", "", "", "", "", ""];
    public string DataSource { get; set; } = string.Empty;
    public List<string> SubLabels { get; set; } = ["", "", "", "", "", "", "", ""];
    public Guid UniqueId { get; set; } = Guid.Empty;
    public int MaxValueLength { get; set; }
}