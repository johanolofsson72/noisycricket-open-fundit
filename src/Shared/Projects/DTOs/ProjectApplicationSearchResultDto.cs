namespace Shared.Projects.DTOs;

public class ProjectApplicationSearchResultDto
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string OrganizationName { get; set; } = string.Empty;
    public int Index { get; set; } = 0;
    public int ProductionYear { get; set; } = 0;
    public decimal TotalBudget { get; set; } = 0;
    public decimal ReportedSpend { get; set; } = 0;
    public decimal SpendRequirement { get; set; } = 0;
    public decimal OurContribution { get; set; } = 0;
    public DateTime SignedContractDate { get; set; } = DateTime.MinValue;
    public string RecordingLocation { get; set; } = string.Empty;
    public int RecordingDays { get; set; } = 0;
    public DateTime RecordingPeriodStart { get; set; } = DateTime.MinValue;
    public DateTime RecordingPeriodEnd { get; set; } = DateTime.MinValue;
    public int ApplicationYear { get; set; } = 0;
    public int PremiereYear { get; set; } = 0;
    public string RecordingComment { get; set; } = string.Empty;
    public string Distributor { get; set; } = string.Empty;
    public List<ListboxNameGenderDto> Producers { get; set; } = [];
    public List<ListboxNameGenderDto> Writers { get; set; } = [];
    public List<ListboxNameGenderDto> Directors { get; set; } = [];
    public string ProducerSummary { get; set; } = string.Empty;
    public string WriterSummary { get; set; } = string.Empty;
    public string DirectorSummary { get; set; } = string.Empty;
    public string FinancialInformation { get; set; } = string.Empty;
}