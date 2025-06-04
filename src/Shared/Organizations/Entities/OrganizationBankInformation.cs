using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Organizations.DTOs;

namespace Shared.Organizations.Entities;

public class OrganizationBankInformation
{
    public int BankInformationIdentifier { get; set; } = 0;
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string Account { get; set; } = string.Empty;
    [MaxLength(500)] public string Iban { get; set; } = string.Empty;
    [MaxLength(500)] public string Bic { get; set; } = string.Empty;
    public virtual Organization Organization { get; set; } = new Organization();
}


public static class OrganizationBankInformationExtensions
{

    public static IEnumerable<OrganizationBankInformationDto> ToDto(this IEnumerable<OrganizationBankInformation> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<OrganizationBankInformation> ToEntity(this IEnumerable<OrganizationBankInformationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
    public static OrganizationBankInformationDto ToDto(this OrganizationBankInformation entity)
    {
        return new OrganizationBankInformationDto()
        {
            BankInformationIdentifier = entity.BankInformationIdentifier,
            Name = entity.Name,
            Account = entity.Account,
            Iban = entity.Iban,
            Bic = entity.Bic
        };
    }

    public static OrganizationBankInformation ToEntity(this OrganizationBankInformationDto dto)
    {
        return new OrganizationBankInformation
        {
            BankInformationIdentifier = dto.BankInformationIdentifier,
            Name = dto.Name,
            Account = dto.Account,
            Iban = dto.Iban,
            Bic = dto.Bic
        };
    }
}



