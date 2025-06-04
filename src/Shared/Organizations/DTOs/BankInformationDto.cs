namespace Shared.Organizations.DTOs;

public record BankInformationDto(
    int Id,
    string Name,
    string Account,
    string Iban,
    string Bic
);