namespace Shared.Organizations.DTOs;

public class OrganizationBankInformationDto
{
    public int BankInformationIdentifier { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string Bic { get; set; } = string.Empty;
}



