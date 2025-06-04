using System.Collections.Generic;
using Shared.Global.Entities;

namespace Shared.Organizations.DTOs;

public record UpdateOrganizationDto
{
    public int StatusId { get; init; } = 0;
    public string Name { get; init; } = string.Empty;
    public string Vat { get; init; } = string.Empty;
    public string Mail { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Logo { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public IEnumerable<OrganizationAddressDto> Addresses { get; init; } = [];
    public IEnumerable<OrganizationPhoneNumberDto> PhoneNumbers { get; init; } = [];
    public IEnumerable<OrganizationBankInformationDto> BankInformation { get; init; } = [];
}