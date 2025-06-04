using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Applications.DTOs;
using Shared.Projects.Entities;

namespace Shared.Applications.Entities;


public class SlimApplication
{
    public int Id { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public int OrganizationId { get; set; } = 0;
    public string Title { get; set; } = "";
    public string ProjectNumber { get; set; } = "";
    public List<ApplicationControl> Controls { get; set; } = [];
}


public static class ClientApplicationExtensions
{
    public static SlimApplicationDto ToDto(this SlimApplication entity)
    {
        return new SlimApplicationDto()
        {
            Id = entity.Id,
            SchemaId = entity.SchemaId,
            OrganizationId = entity.OrganizationId,
            Title = entity.Title,
            Controls = entity.Controls.Select(m => m.ToDto()).ToList()
        };
    }

    public static SlimApplication ToEntity(this SlimApplicationDto dto)
    {
        return new SlimApplication
        {
            Id = dto.Id,
            SchemaId = dto.SchemaId,
            OrganizationId = dto.OrganizationId,
            Title = dto.Title,
            Controls = dto.Controls.Select(m => m.ToEntity()).ToList()
        };
    }

    public static IEnumerable<SlimApplicationDto> ToDto(this IEnumerable<SlimApplication> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<SlimApplication> ToEntity(this IEnumerable<SlimApplicationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
}
