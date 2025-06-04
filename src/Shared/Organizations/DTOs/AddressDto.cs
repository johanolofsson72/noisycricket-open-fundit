namespace Shared.Organizations.DTOs;

public record AddressDto(
    int Id,
    string Line1,
    string Line2,
    string City,
    string Country,
    string PostalCode
);