using System.Collections.Generic;

namespace Shared.Organizations.DTOs;

public class CreateOrganizationDto
{
    public int StatusId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Vat { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<OrganizationAddressDto> Addresses { get; set; } = new();
    public List<OrganizationPhoneNumberDto> PhoneNumbers { get; set; } = new();
    public List<OrganizationBankInformationDto> BankInformation { get; set; } = new();
}