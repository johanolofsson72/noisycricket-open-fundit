using System;

namespace Shared.Applications.DTOs;

public class ApplicationControlDto
{
    public int Id { get; set; } = 0;
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
    
    public int ApplicationFormSectionId { get; set; } = 0;
    public int ApplicationSectionId { get; set; } = 0;
    public string Value { get; set; } = string.Empty;
    public List<string> Placeholders { get; set; } = ["", "", "", "", "", "", "", ""];
    public string DataSource { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool Confirm { get; set; } = false;
    public List<string> SubLabels { get; set; } = ["", "", "", "", "", "", "", ""];
    public Guid UniqueId { get; set; } = Guid.Empty;
    public int MaxValueLength { get; set; } = 0;

}