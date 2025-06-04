using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Applications.DTOs;

namespace Shared.Applications.Entities;

public class ApplicationState
{
    public int Id { get; set; }= 0;
    public int OrganizationId { get; set; } = 0;
    public int UserId { get; set; }= 0;
    public int ApplicationId { get; set; }= 0;
    public int SchemaId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = [];
    [MaxLength(500)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string TempPath { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public static class ApplicationStateExtensions
{
    public static ApplicationStateDto ToDto(this ApplicationState entity)
    {
        return new ApplicationStateDto()
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            UserId = entity.UserId,
            ApplicationId = entity.ApplicationId,
            SchemaId = entity.SchemaId,
            SchemaNames = entity.SchemaNames,
            Title = entity.Title,
            TempPath = entity.TempPath,
            CreatedDate = entity.CreatedDate
        };
    }
    
    public static ApplicationState ToEntity(this ApplicationStateDto dto)
    {
        return new ApplicationState
        {
            Id = dto.Id,
            OrganizationId = dto.OrganizationId,
            UserId = dto.UserId,
            ApplicationId = dto.ApplicationId,
            SchemaId = dto.SchemaId,
            SchemaNames = dto.SchemaNames,
            Title = dto.Title,
            TempPath = dto.TempPath,
            CreatedDate = dto.CreatedDate
        };
    }
    
    public static List<ApplicationStateDto> ToDto(this List<ApplicationState> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
    
    public static List<ApplicationState> ToEntity(this List<ApplicationStateDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }
}
