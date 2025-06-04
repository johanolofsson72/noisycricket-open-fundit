using System;

namespace Shared.Projects.DTOs;

public class ProjectApplicationDto
{
    public int ApplicationId { get; set; }
    public int ApplicationStatusId { get; set; }
    public int ApplicationSchemaId { get; set; } = 0;
    public List<string> ApplicationSchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public int ApplicationType { get; set; }
    public string ApplicationTitle { get; set; } = string.Empty;
    public ApplicationContactDto ApplicationProducer { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ApplicationApplicant { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ApplicationProjectManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ApplicationProductionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ApplicationFinanceManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ApplicationScriptManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ApplicationDistributionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ApplicationContractManager { get; set; } = new ApplicationContactDto();
    public DateTime ApplicationCreatedDate { get; set; }
}