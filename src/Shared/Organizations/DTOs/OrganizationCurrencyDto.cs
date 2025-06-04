namespace Shared.Organizations.DTOs;

public class OrganizationCurrencyDto
{
    public int OrganizationCurrencyIdentifier { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18, 4)")] public decimal Rate { get; set; }
    public DateTime CreatedDate { get; set; }
}