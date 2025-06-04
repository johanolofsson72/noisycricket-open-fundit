using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Data.DbContext;
using Shared.Extensions;
using Shared.Global.DTOs;
using Shared.Global.Services;
using Shared.Global.Structs;
using Shared.Organizations.DTOs;
using Shared.Organizations.Entities;
using Shared.Statistics.Entities;

namespace Shared.Organizations.Services;

public class OrganizationService(IDbContextFactory<ApplicationDbContext> factory)
{
    
    public async Task<Result<bool, Exception>> DeleteOrganizationAsync(int organizationId, CancellationToken ct)
    {
        try
        {
            if (organizationId == 0) throw new Exception("organizationId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            return await context.Organizations
                .Where(x => x.Id == organizationId)
                .ExecuteDeleteAsync(cancellationToken: ct) > 0;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<OrganizationDto, Exception>> UpdateOrganizationAsync(int organizationId, UpdateOrganizationDto dto, CancellationToken ct)
    {
        try
        {
            if (organizationId == 0) throw new Exception("organizationId is required");
            if (dto.Name == string.Empty) throw new Exception("Name is required");
            if (dto.Vat == string.Empty) throw new Exception("Vat is required");
            if (dto.Mail == string.Empty) throw new Exception("Mail is required");
            if (!dto.Mail.IsValidEmailAddress()) throw new Exception("Mail is not valid");
                
            await using var context = await factory.CreateDbContextAsync(ct);
            var organization = context.Organizations
                .AsTracking()
                .FirstOrDefault(x => x.Id == organizationId) ?? throw new Exception("Organization not found");
            
            organization.Name = dto.Name;
            organization.Vat = dto.Vat;
            organization.Mail = dto.Mail;
            organization.Url = dto.Url;
            organization.Country = dto.Country;
            organization.Logo = dto.Logo;
            organization.StatusId = dto.StatusId;
            organization.Addresses = dto.Addresses.Select(x => x.ToEntity()).ToList();
            organization.PhoneNumbers = dto.PhoneNumbers.Select(x => x.ToEntity()).ToList();
            organization.BankInformation = dto.BankInformation.Select(x => x.ToEntity()).ToList();

            context.Organizations.Update(organization);
            await context.SaveChangesAsync(ct);

            return new OrganizationDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<OrganizationDto, Exception>> CreateOrganizationAsync(CreateOrganizationDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Name == string.Empty) throw new Exception("Title is required");
            if (dto.Vat == string.Empty) throw new Exception("Vat is required");
            if (dto.Mail == string.Empty) throw new Exception("Mail is required");
            if (!dto.Mail.IsValidEmailAddress()) throw new Exception("Mail is not valid");
                
            var organization = new Organization
            {
                StatusId = 1,
                Name = dto.Name,
                Vat = dto.Vat,
                Mail = dto.Mail,
                Url = dto.Url,
                Country = dto.Country,
                Addresses = dto.Addresses.Select(x => x.ToEntity()).ToList(),
                PhoneNumbers = dto.PhoneNumbers.Select(x => x.ToEntity()).ToList(),
                BankInformation = dto.BankInformation.Select(x => x.ToEntity()).ToList()
            };

            await using var context = await factory.CreateDbContextAsync(ct);
            await context.Organizations.AddAsync(organization, ct);
            await context.SaveChangesAsync(ct);
            
            return new OrganizationDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<OrganizationDto, Exception>> OrganizationByIdAsync(int organizationId)
    {
        try
        {
            if (organizationId == 0) throw new Exception("organizationId is required");

            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var organization = (await GetOrganizationById(context, organizationId) ?? throw new Exception("Organization not found")).ToDto();
            
            return organization;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<IEnumerable<OrganizationDto>, Exception>> AllOrganizationAsync()
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var organizations = GetAllOrganization(context) ?? throw new Exception("Organizations not found");
            return await organizations.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<IEnumerable<OrganizationGridItemDto>, Exception>> AllOrganizationGridItemsAsync()
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var organizations = GetAllOrganizationGridItems(context) ?? throw new Exception("Organizations not found");
            return await organizations.ToListAsync();
            
            /*
            var organizations = await context.Organizations
                .AsNoTracking()
                .Select(x => new OrganizationGridItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Vat = x.Vat,
                    Mail = x.Mail,
                    Url = x.Url,
                    Country = x.Country,
                    Addresses = x.Addresses.ToDto().ToList(),
                    PhoneNumbers = x.PhoneNumbers.ToDto().ToList(),
                    BankInformation = x.BankInformation.ToDto().ToList()
                })
                .ToListAsync() ?? throw new Exception("Organizations not found");

            return organizations;*/
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"ex: {ex}");
            return ex;
        }
    }

    public async Task<Result<IEnumerable<OrganizationSummaryDto>, Exception>> AllOrganizationSummaryAsync()
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var organizations = GetAllOrganizationSummary(context) ?? throw new Exception("Organizations not found");
            return await organizations.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"ex: {ex}");
            return ex;
        }
    }

    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<OrganizationSummaryDto>> GetAllOrganizationSummary = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Organizations
                .AsNoTracking()
                .TagWith("GetAllOrganizationSummary")
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationSummaryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Vat = x.Vat
                }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<OrganizationGridItemDto>> GetAllOrganizationGridItems = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Organizations
                .AsNoTracking()
                .TagWith("GetAllOrganizationGridItems")
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationGridItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Vat = x.Vat,
                    Mail = x.Mail,
                    Url = x.Url,
                    Country = x.Country,
                    Addresses = x.Addresses.ToDto().ToList(),
                    PhoneNumbers = x.PhoneNumbers.ToDto().ToList(),
                    BankInformation = x.BankInformation.ToDto().ToList()
                }));

    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<OrganizationDto>> GetAllOrganization = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Organizations
                .AsNoTracking()
                .TagWith("GetAllOrganization")
                .OrderBy(x => x.Id)
                .Select(x => x.ToDto()));
    
    private static readonly Func<ApplicationDbContext, int, Task<Organization?>> GetOrganizationById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int organizationId) => 
            context.Organizations
                .AsNoTracking()
                .TagWith("GetOrganizationById")
                .FirstOrDefault(x => x.Id == organizationId));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<CurrencyDto>> GetCurrencies = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Currencies
                .AsNoTracking()
                .TagWith("GetCurrencies")
                .Select(x => new CurrencyDto{ Name = x.Name, Rate = x.Rate, CreatedDate = x.CreatedDate}));
}