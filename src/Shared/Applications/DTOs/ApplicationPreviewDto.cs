using System;
using System.Collections.Generic;
using Shared.Documents.DTOs;

namespace Shared.Applications.DTOs;

public class ApplicationPreviewDto()
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string ProjectNumber { get; set; } = string.Empty;
    public int SchemaId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public string Comments { get; set; } = string.Empty;
    public string Pitch { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public string ProjectManagerName { get; set; } = string.Empty;
    public string ProjectManagerEmail { get; set; } = string.Empty;
    public string ProductionManagerName { get; set; } = string.Empty;
    public string ProductionManagerEmail { get; set; } = string.Empty;
    public string ContractManagerName { get; set; } = string.Empty;
    public string ContractManagerEmail { get; set; } = string.Empty;
    public int OrganizationId { get; set; } = 0;
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationVat { get; set; } = string.Empty;
    public string OrganizationPhoneNumber { get; set; } = string.Empty;
    public string OrganizationEmail { get; set; } = string.Empty;
    public string OrganizationAddress { get; set; } = string.Empty;
    public string OrganizationPostalCode { get; set; } = string.Empty;
    public string OrganizationCity { get; set; } = string.Empty;
    public string OrganizationCountry { get; set; } = string.Empty;
    public string OrganizationUrl { get; set; } = string.Empty;
    public IEnumerable<DocumentOverviewDto> Documents { get; set; } = new List<DocumentOverviewDto>();
}