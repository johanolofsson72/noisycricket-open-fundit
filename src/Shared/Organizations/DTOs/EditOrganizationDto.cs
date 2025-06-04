using Shared.Global.Entities;

namespace Shared.Organizations.DTOs;

public class EditOrganizationDto
{
    public int Id { get; set; }
    public int StatusId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Vat { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string Bic { get; set; } = string.Empty;
}