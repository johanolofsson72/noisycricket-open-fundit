
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Projects.DTOs;
using ProjectDto = Shared.Projects.DTOs.ProjectDto;

namespace Shared.Projects.Entities;


public class Project
{
    public int Id { get; set; } = 0;
    public ProjectOrganization Organization { get; set; } = new ProjectOrganization();
    public int StatusId { get; set; } = 0;
    [MaxLength(500)] public string Number { get; set; } = string.Empty;
    public List<string> Title { get; set; } = new();
    public List<ProjectApplication> Applications { get; set; } = [];
    public int ApplicationCount { get; set; } = 0;
    public DateTime UpdatedDate { get; set; } = DateTime.MinValue;
    public DateTime CreateDate { get; set; } = DateTime.MinValue;
    [NotMapped] public int Index { get; set; } = 0;
}

public static class ProjectExtensions
{
    public static ProjectDto ToDto(this Project entity)
    {
        return new ProjectDto()
        {
            Id = entity.Id,
            Organization = entity.Organization,
            StatusId = entity.StatusId,
            Number = entity.Number,
            Title = entity.Title,
            Applications = entity.Applications.ToDto().ToList(),
            ApplicationCount = entity.ApplicationCount,
            UpdatedDate = entity.UpdatedDate,
            CreateDate = entity.CreateDate,
            Index = entity.Index
        };
    }
    
    public static Project ToEntity(this ProjectDto dto)
    {
        return new Project
        {
            Id = dto.Id,
            Organization = dto.Organization,
            StatusId = dto.StatusId,
            Number = dto.Number,
            Title = dto.Title,
            Applications = dto.Applications.ToEntity().ToList(),
            ApplicationCount = dto.ApplicationCount,
            UpdatedDate = dto.UpdatedDate,
            CreateDate = dto.CreateDate,
            Index = dto.Index
        };
    }

    public static IEnumerable<ProjectDto> ToDto(this IEnumerable<Project> entity)
    {
        return entity.Select(e => e.ToDto());
    } 
    
    public static IEnumerable<Project> ToEntity(this IEnumerable<ProjectDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
}



