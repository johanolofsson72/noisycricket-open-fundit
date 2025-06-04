
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Shared.Projects.Entities;

public class SearchResult
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string OrganizationName { get; set; } = string.Empty;
    public int Index { get; set; } = 0;
    public List<ApplicationControl> Controls { get; set; } = new();
}

public static class SearchResultExtensions
{

    public static IEnumerable<ProjectApplicationSearchResultDto> ToProjectApplicationSearchResultDto(this IEnumerable<SearchResult> entity)
    {
        return entity.Select(e => e.ToProjectApplicationSearchResultDto());
    } 
    
    public static ProjectApplicationSearchResultDto ToProjectApplicationSearchResultDto(this SearchResult entity)
    {
        return new ProjectApplicationSearchResultDto()
        {
            Id = entity.Id,
            StatusId = entity.StatusId,
            Title = entity.Title,
            CreatedDate = entity.CreatedDate,
            SchemaNames = entity.SchemaNames,
            OrganizationName = entity.OrganizationName,
            Index = entity.Index,
            ProductionYear = entity.Controls.IntValue("978AC998"), // 978AC998
            TotalBudget = entity.Controls.DecimalValue("00010001"),
            ReportedSpend = entity.Controls.DecimalValue("70C68F4A"),
            SpendRequirement = entity.Controls.DecimalValue("025127D7"),
            OurContribution = entity.Controls.DecimalValue("01000001"),
            SignedContractDate = entity.Controls.DateTimeValue("839E5FEB"),
            RecordingLocation = entity.Controls.StringValue("F22E8978"),
            RecordingDays = entity.Controls.IntValue("0DFBAE46"),
            RecordingPeriodStart = entity.Controls.StartDateTimeFromDateTime("09000002"),
            RecordingPeriodEnd = entity.Controls.EndDateTimeFromDateTime("09000002"),
            ApplicationYear = entity.Controls.IntValue("978AC998"),
            PremiereYear = entity.Controls.IntValue("D75EC5DE"),
            RecordingComment = entity.Controls.StringValue("5D5F639E"),
            Distributor = entity.Controls.StringValue("10000008"),
            Producers = entity.Controls.ContactValue("EBA1414F"),
            Writers = entity.Controls.ContactValue("BF962372"),
            Directors = entity.Controls.ContactValue("4D5B21F8"),
            ProducerSummary = entity.Controls.SummaryValue("EBA1414F"),
            WriterSummary = entity.Controls.SummaryValue("BF962372"),
            DirectorSummary = entity.Controls.SummaryValue("4D5B21F8"),
            FinancialInformation = entity.Controls.FinancialInformationValue()
        };
    }
    
    private static string FinancialInformationValue(this IEnumerable<ApplicationControl> controls)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var totalBudget = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith("00010001"));
        var reportedSpend = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith("70C68F4A"));
        var spendRequirement = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith("025127D7"));
        var ourContribution = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith("01000001"));

        decimal.TryParse(totalBudget?.Value, out var totalBudgetValue);
        decimal.TryParse(reportedSpend?.Value, out var reportedSpendValue);
        decimal.TryParse(spendRequirement?.Value, out var spendRequirementValue);
        decimal.TryParse(ourContribution?.Value, out var ourContributionValue);
        
        return $"Total budget: {totalBudgetValue:C}" +
               $", Reported spend: {reportedSpendValue:C}" +
               $", Spend requirement: {spendRequirementValue:C}" +
               $", Our contribution: {ourContributionValue:C}";
    }
    
    private static string SummaryValue(this IEnumerable<ApplicationControl> controls, string uniqueId)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var ctl = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith(uniqueId));

        if (ctl is null) return "";

        ctl.Value = Regex.Unescape(ctl.Value);
        var people = JsonSerializer.Deserialize<List<ListboxNameGenderDto>>(ctl.Value) ?? [];

        return string.Join(", ", people.Where(p => p.Name.Length > 0).Select(p =>
        {
            return p.Name.Length switch
            {
                > 0 when p.Name.Length > 0 => p.Name + " (" + p.Gender + ")",
                > 0 => p.Name,
                _ => ""
            };
        }).Distinct());
    }
    
    private static List<ListboxNameGenderDto> ContactValue(this IEnumerable<ApplicationControl> controls, string uniqueId)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var ctl = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith(uniqueId));

        if (ctl is null) return [];

        ctl.Value = Regex.Unescape(ctl.Value);
        return JsonSerializer.Deserialize<List<ListboxNameGenderDto>>(ctl.Value) ?? [];
    }
    
    private static string StringValue (this IEnumerable<ApplicationControl> controls, string uniqueId)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var ctl = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith(uniqueId));

        if (ctl != null)
        {
            ctl.Value = Regex.Unescape(ctl.Value);
        }
        return ctl?.Value ?? string.Empty;
    } 
    
    private static int IntValue(this IEnumerable<ApplicationControl> controls, string uniqueId)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var ctl = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith(uniqueId));

        if (ctl is null) return 0;

        ctl.Value = Regex.Unescape(ctl.Value);
        int.TryParse(ctl.Value, out var result);

        return result;
    }
    
    private static DateTime DateTimeValue(this IEnumerable<ApplicationControl> controls, string uniqueId)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var ctl = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith(uniqueId));

        if (ctl is null) return DateTime.MinValue;

        ctl.Value = Regex.Unescape(ctl.Value);
        DateTime.TryParse(ctl.Value, out var result);

        return result;
    }
    
    private static DateTime StartDateTimeFromDateTime (this IEnumerable<ApplicationControl> controls, string uniqueId)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var ctl = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith(uniqueId));

        if (ctl is null || ctl.Value == "") return DateTime.MinValue;

        ctl.Value = Regex.Unescape(ctl.Value);
        DateTime.TryParse(ctl.Value.Split(';')[0], out var result);

        return result;
    }
    
    private static DateTime EndDateTimeFromDateTime (this IEnumerable<ApplicationControl> controls, string uniqueId)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var ctl = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith(uniqueId));

        if (ctl is null || ctl.Value == "") return DateTime.MinValue;

        ctl.Value = Regex.Unescape(ctl.Value);
        DateTime.TryParse(ctl.Value.Split(';')[1], out var result);

        return result;
    }
    
    private static decimal DecimalValue(this IEnumerable<ApplicationControl> controls, string uniqueId)
    {
        var applicationControls = controls as ApplicationControl[] ?? controls.ToArray();
        var ctl = applicationControls.FirstOrDefault(c => c.UniqueId.ToString().ToUpper().StartsWith(uniqueId));

        if (ctl is null) return 0;

        ctl.Value = Regex.Unescape(ctl.Value);
        decimal.TryParse(ctl.Value, out var result);

        return result;
    }
}