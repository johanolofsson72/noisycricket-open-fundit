using System;
using System.Collections.Generic;
using Shared.Projects.Entities;

namespace Shared.Projects.DTOs;

public class ProjectDto
{
    public int Id { get; set; } = 0;
    public ProjectOrganization Organization { get; set; } = new ProjectOrganization();
    public int StatusId { get; set; } = 0;
    public string Number { get; set; } = string.Empty;
    public List<string> Title { get; set; } = new();
    public List<ProjectApplicationDto> Applications { get; set; } = [];
    public int ApplicationCount { get; set; } = 0;
    public DateTime UpdatedDate { get; set; } = DateTime.MinValue;
    public DateTime CreateDate { get; set; } = DateTime.MinValue;
    public int Index { get; set; } = 0;
}