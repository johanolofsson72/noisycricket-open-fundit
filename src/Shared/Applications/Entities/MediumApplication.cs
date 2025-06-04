using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Applications.DTOs;
using Shared.Projects.Entities;

namespace Shared.Applications.Entities;


public class MediumApplication
{
    public int Id { get; set; } = 0;
    public int ProjectId { get; set; } = 0;
    public string ProjectNumber { get; set; } = string.Empty;
    public int StatusId { get; set; } = 0;
    public int SchemaId { get; set; }
    public string SchemaClaimTag { get; set; } = "";
    public string Title { get; set; } = "";
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public DateTime DeliveryDate { get; set; } = DateTime.MinValue;
    public ApplicationContact ProjectManager { get; set; } = new ApplicationContact();
    public ApplicationContact ProductionManager { get; set; } = new ApplicationContact();
    public ApplicationContact ContractManager { get; set; } = new ApplicationContact();
    public ApplicationContact DistributionManager { get; set; } = new ApplicationContact();
    public ApplicationContact FinanceManager { get; set; } = new ApplicationContact();
    public ApplicationContact ScriptManager { get; set; } = new ApplicationContact();
    public List<ApplicationControl> Controls { get; set; } = [];
}

public static class MediumApplicationExtensions
{
    public static MediumApplicationDto ToDto(this MediumApplication entity)
    {
        return new MediumApplicationDto()
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            ProjectNumber = entity.ProjectNumber,
            StatusId = entity.StatusId,
            SchemaId = entity.SchemaId,
            SchemaClaimTag = entity.SchemaClaimTag,
            Title = entity.Title,
            CreatedDate = entity.CreatedDate,
            DeliveryDate = entity.DeliveryDate,
            ProjectManager = entity.ProjectManager.ToDto(),
            ProductionManager = entity.ProductionManager.ToDto(),
            ContractManager = entity.ContractManager.ToDto(),
            DistributionManager = entity.DistributionManager.ToDto(),
            FinanceManager = entity.FinanceManager.ToDto(),
            ScriptManager = entity.ScriptManager.ToDto(),
            Controls = entity.Controls.Select(x => x.ToDto()).ToList()
        };
    }

    public static MediumApplication ToEntity(this MediumApplicationDto dto)
    {
        return new MediumApplication
        {
            Id = dto.Id,
            ProjectId = dto.ProjectId,
            ProjectNumber = dto.ProjectNumber,
            StatusId = dto.StatusId,
            SchemaId = dto.SchemaId,
            SchemaClaimTag = dto.SchemaClaimTag,
            Title = dto.Title,
            CreatedDate = dto.CreatedDate,
            DeliveryDate = dto.DeliveryDate,
            ProjectManager = dto.ProjectManager.ToEntity(),
            ProductionManager = dto.ProductionManager.ToEntity(),
            ContractManager = dto.ContractManager.ToEntity(),
            DistributionManager = dto.DistributionManager.ToEntity(),
            FinanceManager = dto.FinanceManager.ToEntity(),
            ScriptManager = dto.ScriptManager.ToEntity(),
            Controls = dto.Controls.Select(x => x.ToEntity()).ToList()
        };
    }

    public static IEnumerable<MediumApplicationDto> ToDto(this IEnumerable<MediumApplication> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<MediumApplication> ToEntity(this IEnumerable<MediumApplicationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
}