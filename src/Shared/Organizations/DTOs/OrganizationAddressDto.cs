namespace Shared.Organizations.DTOs;

public class OrganizationAddressDto
{
    public int AddressIdentifier { get; set; } = 0;
    public string Line1 { get; set; } = string.Empty;
    public string Line2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}



