using System;
using System.Collections.Generic;
using Shared.Schemas.DTOs;

namespace Shared.Applications.DTOs;

public class ReplyApplicationDto
{
    public int Id { get; set; } = 0;
    public int ProjectId { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public string ProjectNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ApplicationContact ProjectManager { get; set; } = new ApplicationContact();
    public ApplicationContact Organization { get; set; } = new ApplicationContact();
}