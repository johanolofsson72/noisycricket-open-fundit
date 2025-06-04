using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.DbContext;
using Shared.Organizations.Entities;

namespace Server.Test.Helpers;

public static class Organizations
{
    public static (int, string) Create(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var organization = new Organization()
        {
            Logo = "",
            StatusId = 2,
            Name = "Sci-Fi AB",
            Vat = "5565735569",
            Mail = "info@sci-fi.com",
            Url = "https://www.scifi.se",
            Country = "Sweden",
            Addresses = new List<OrganizationAddress>
            {
                new OrganizationAddress
                {
                    AddressIdentifier = 1,
                    City = "Ronneby",
                    Country = "Sweden",
                    Line1 = "Silverforsvägen 13",
                    Line2 = "",
                    PostalCode = "37231"
                }
            },
            BankInformation = new List<OrganizationBankInformation>
            {
                new OrganizationBankInformation
                {
                    Account = "000098765ASDFG",
                    BankInformationIdentifier = 1,
                    Bic = "BIC",
                    Iban = "IBAN",
                    Name = "SEB"
                }
            },
            PhoneNumbers = new List<OrganizationPhoneNumber>
            {
                new OrganizationPhoneNumber
                {
                    Number = "0046709161669",
                    PhoneNumberIdentifier = 1,
                    Type = 3
                },
                new OrganizationPhoneNumber
                {
                    Number = "004645715859",
                    PhoneNumberIdentifier = 2,
                    Type = 2
                }
            }
        };
        context.Organizations.Add(organization);
        context.SaveChanges();
        
        return (organization.Id, organization.Vat);
    }
    public static (int, string) CreateForCustomer(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var organization = new Organization()
        {
            Logo = "",
            StatusId = 2,
            Name = "Star Wars AB",
            Vat = "5567144950",
            Mail = "jool@me.com",
            Url = "https://www.starwars.se",
            Country = "Sweden",
            Addresses =
            [
                new OrganizationAddress
                {
                    AddressIdentifier = 1,
                    City = "Ronneby",
                    Country = "Sweden",
                    Line1 = "Droppemålavägen 14",
                    Line2 = "",
                    PostalCode = "37273"
                }
            ],
            BankInformation =
            [
                new OrganizationBankInformation
                {
                    Account = "000098765ASDFG",
                    BankInformationIdentifier = 1,
                    Bic = "BIC",
                    Iban = "IBAN",
                    Name = "SEB"
                }
            ],
            PhoneNumbers = new List<OrganizationPhoneNumber>
            {
                new OrganizationPhoneNumber
                {
                    Number = "0046709161669",
                    PhoneNumberIdentifier = 1,
                    Type = 3
                },
                new OrganizationPhoneNumber
                {
                    Number = "004645715859",
                    PhoneNumberIdentifier = 2,
                    Type = 2
                }
            }
        };
        context.Organizations.Add(organization);
        context.SaveChanges();
        
        return (organization.Id, organization.Vat);
    }
}