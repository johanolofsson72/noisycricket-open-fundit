namespace Shared.Applications.DTOs;

public class MediumApplicationDto
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
    public ApplicationContactDto ProjectManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ProductionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ContractManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto DistributionManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto FinanceManager { get; set; } = new ApplicationContactDto();
    public ApplicationContactDto ScriptManager { get; set; } = new ApplicationContactDto();
    public List<ApplicationControlDto> Controls { get; set; } = [];
}