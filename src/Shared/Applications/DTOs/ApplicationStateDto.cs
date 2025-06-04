using System;

namespace Shared.Applications.DTOs;

public class ApplicationStateDto
{
    public int Id { get; set; } = 0;
    public int OrganizationId { get; set; } = 0;
    public int UserId { get; set; } = 0;
    public int ApplicationId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = [];
    public string Title { get; set; } = string.Empty;
    public string TempPath { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;

}