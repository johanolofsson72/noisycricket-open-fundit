using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Applications.DTOs;
using Shared.Projects.Entities;

namespace Shared.Applications.Entities;


public class ReplyApplication
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


public static class ReplyApplicationExtensions
{
    public static ReplyApplicationDto ToDto(this ReplyApplication entity)
    {
        return new ReplyApplicationDto()
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            StatusId = entity.StatusId,
            SchemaId = entity.SchemaId,
            ProjectNumber = entity.ProjectNumber,
            Title = entity.Title,
            ProjectManager = entity.ProjectManager,
            Organization = entity.Organization
        };
    }

    public static ReplyApplication ToEntity(this ReplyApplicationDto dto)
    {
        return new ReplyApplication
        {
            Id = dto.Id,
            ProjectId = dto.ProjectId,
            StatusId = dto.StatusId,
            SchemaId = dto.SchemaId,
            ProjectNumber = dto.ProjectNumber,
            Title = dto.Title,
            ProjectManager = dto.ProjectManager,
            Organization = dto.Organization
        };
    }

    public static IEnumerable<ReplyApplicationDto> ToDto(this IEnumerable<ReplyApplication> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<ReplyApplication> ToEntity(this IEnumerable<ReplyApplicationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
}
