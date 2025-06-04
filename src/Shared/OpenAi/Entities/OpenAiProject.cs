using System;
using System.Collections.Generic;

namespace Shared.OpenAi.Entities;

public class OpenAiProject
{
    public int Id { get; set; } = 0;
    public int ProjectId { get; set; } = 0;
    public string ProjectTitle { get; set; } = string.Empty;
    public string ProjectNumber { get; set; } = string.Empty;
    public string ProjectManagerName { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string ProducerName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int ApplicationId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public List<string> StatusNames { get; set; } = [];
    [Column(TypeName = "decimal(18, 4)")] public decimal BudgetAmount { get; set; } = 0;
    [Column(TypeName = "decimal(18, 4)")] public decimal AppliedAmount { get; set; } = 0;
    [Column(TypeName = "decimal(18, 4)")] public decimal OurContribution { get; set; } = 0;
    public List<OpenAiProjectData> Data { get; set; } = [];
    public byte[]? Embedding { get; set; }
    public DateTime ExpireDate { get; set; } = DateTime.MinValue;
}