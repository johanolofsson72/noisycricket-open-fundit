namespace Shared.Organizations.DTOs;

public class OrganizationGridItemDto
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Vat { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<OrganizationAddressDto> Addresses { get; set; } = [];
    public List<OrganizationPhoneNumberDto> PhoneNumbers { get; set; } = [];
    public List<OrganizationBankInformationDto> BankInformation { get; set; } = [];
}