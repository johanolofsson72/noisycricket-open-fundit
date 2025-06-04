
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Applications.Entities;

namespace Shared.Projects.Entities;

public class ProjectApplication
{
    public int ApplicationIdentifier { get; set; } = 0;
    public int ApplicationStatusId { get; set; } = 0;
    public int ApplicationSchemaId { get; set; } = 0;
    public List<string> ApplicationSchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    [MaxLength(500)] public string ApplicationTitle { get; set; } = string.Empty;
    public ApplicationContact ApplicationProducer { get; set; } = new ApplicationContact();
    public ApplicationContact ApplicationApplicant { get; set; } = new ApplicationContact();
    public ApplicationContact ApplicationProjectManager { get; set; } = new ApplicationContact();
    public ApplicationContact ApplicationProductionManager { get; set; } = new ApplicationContact();
    public ApplicationContact ApplicationFinanceManager { get; set; } = new ApplicationContact();
    public ApplicationContact ApplicationScriptManager { get; set; } = new ApplicationContact();
    public ApplicationContact ApplicationDistributionManager { get; set; } = new ApplicationContact();
    public ApplicationContact ApplicationContractManager { get; set; } = new ApplicationContact();
    public DateTime ApplicationCreatedDate { get; set; } = DateTime.MinValue;
}


public static class ProjectApplicationExtensions
{
    public static ProjectApplicationDto ToDto(this ProjectApplication entity)
    {
        return new ProjectApplicationDto()
        {
            ApplicationId = entity.ApplicationIdentifier,
            ApplicationStatusId = entity.ApplicationStatusId,
            ApplicationSchemaId = entity.ApplicationSchemaId,
            ApplicationSchemaNames = entity.ApplicationSchemaNames,
            ApplicationTitle = entity.ApplicationTitle,
            ApplicationProducer = entity.ApplicationProducer.ToDto(),
            ApplicationApplicant = entity.ApplicationApplicant.ToDto(),
            ApplicationProjectManager = entity.ApplicationProjectManager.ToDto(),
            ApplicationProductionManager = entity.ApplicationProductionManager.ToDto(),
            ApplicationFinanceManager = entity.ApplicationFinanceManager.ToDto(),
            ApplicationScriptManager = entity.ApplicationScriptManager.ToDto(),
            ApplicationDistributionManager = entity.ApplicationDistributionManager.ToDto(),
            ApplicationContractManager = entity.ApplicationContractManager.ToDto(),
            ApplicationCreatedDate = entity.ApplicationCreatedDate
        };
    }
    
    public static ProjectApplication ToEntity(this ProjectApplicationDto dto)
    {
        return new ProjectApplication
        {
            ApplicationIdentifier = dto.ApplicationId,
            ApplicationStatusId = dto.ApplicationStatusId,
            ApplicationSchemaId = dto.ApplicationSchemaId,
            ApplicationSchemaNames = dto.ApplicationSchemaNames,
            ApplicationTitle = dto.ApplicationTitle,
            ApplicationProducer = dto.ApplicationProducer.ToEntity(),
            ApplicationApplicant = dto.ApplicationApplicant.ToEntity(),
            ApplicationProjectManager = dto.ApplicationProjectManager.ToEntity(),
            ApplicationProductionManager = dto.ApplicationProductionManager.ToEntity(),
            ApplicationScriptManager = dto.ApplicationScriptManager.ToEntity(),
            ApplicationDistributionManager = dto.ApplicationDistributionManager.ToEntity(),
            ApplicationContractManager = dto.ApplicationContractManager.ToEntity(),
            ApplicationCreatedDate = dto.ApplicationCreatedDate
        };
    }

    public static IEnumerable<ProjectApplicationDto> ToDto(this IEnumerable<ProjectApplication> entity)
    {
        return entity.Select(e => e.ToDto());
    } 
    
    public static IEnumerable<ProjectApplication> ToEntity(this IEnumerable<ProjectApplicationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
}





