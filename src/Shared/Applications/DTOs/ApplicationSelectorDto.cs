using System.Collections.Generic;

namespace Shared.Applications.DTOs;

public class ApplicationSelectorDto
{
    public int ApplicationId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public bool Selected { get; set; } = false;
    public bool DeletedOrDenied { get; set; } = false;
}