using System;
using System.Collections.Generic;
using Shared.Schemas.DTOs;

namespace Shared.Applications.DTOs;

public class SlimApplicationDto
{
    public int Id { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public int OrganizationId { get; set; } = 0;
    public string Title { get; set; } = "";
    public List<ApplicationControlDto> Controls { get; set; } = [];
}