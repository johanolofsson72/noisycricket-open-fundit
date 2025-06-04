using Shared.Global.Entities;

namespace Shared.Organizations.DTOs;

public record PhoneNumberDto(
    int Id,
    string Number,
    int Type
);