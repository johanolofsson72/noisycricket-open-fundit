using Shared.Data.DbContext;
using Shared.Users.DTOs;
using Shared.Users.Enums;
using Message = Shared.Messages.Entities.Message;

namespace Shared.Users.Services;

public class UserService(IDbContextFactory<ApplicationDbContext> factory)
{

    public async Task<Result<bool, Exception>> AggregateAsync(CancellationToken ct, int userId = 0)
    {
        try
        {
            await Task.Delay(0, ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> ExternalUserRegisteredAsync(
        ExternalUserRegisteredDto externalUserRegisteredDto, CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            return await context.Users
                .Where(x => x.Email == externalUserRegisteredDto.Email)
                .FirstOrDefaultAsync(ct) != null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> AddExternalProviderAsync(AddExternalProviderDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Id == 0) throw new Exception("userId is required");
            if (dto.Provider == string.Empty) throw new Exception("Provider is required");
            if (dto.ProviderKey == string.Empty) throw new Exception("ProviderKey is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.Id == dto.Id)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");

            // External login
            var providerKeyExists = await context.UserLogins
                .Where(x => x.UserId == dto.Id && x.ProviderKey == dto.ProviderKey)
                .FirstOrDefaultAsync(ct) != null;

            if (dto.Provider.Length > 0 && dto.ProviderKey.Length > 0 && providerKeyExists is false)
            {
                context.UserLogins.Add(new IdentityUserLogin<int>
                {
                    UserId = dto.Id, LoginProvider = dto.Provider, ProviderKey = dto.ProviderKey,
                    ProviderDisplayName = dto.Provider
                });
            }

            await context.SaveChangesAsync(ct);

            await AggregateAsync(ct, user.Id);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> RegisterClientUserAsync(RegisterClientUserDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Id == 0) throw new Exception("userId is required");
            if (dto.FirstName == string.Empty) throw new Exception("FirstName is required");
            if (dto.LastName == string.Empty) throw new Exception("LastName is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var organization = await context.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Vat.ToLower() == dto.OrganizationNumber.ToLower(), ct);

            if (organization is null)
            {
                organization = new Organization()
                {
                    StatusId = 2,
                    Name = dto.OrganizationName,
                    Vat = dto.OrganizationNumber,
                    Mail = dto.Email,
                    Url = dto.Url,
                    Logo = "",
                    Country = dto.Country,
                    Addresses =
                    [
                        new OrganizationAddress()
                        {
                            AddressIdentifier = 1,
                            Line1 = dto.Address,
                            Line2 = "",
                            City = dto.City,
                            PostalCode = dto.Postalcode,
                            Country = dto.Country
                        }
                    ],
                    BankInformation =
                    [
                        new OrganizationBankInformation()
                        {
                            BankInformationIdentifier = 1,
                            Name = dto.BankName,
                            Account = dto.AccountNumber,
                            Iban = dto.Iban,
                            Bic = dto.Bic
                        }
                    ],
                    PhoneNumbers =
                    [
                        new OrganizationPhoneNumber()
                        {
                            PhoneNumberIdentifier = 1,
                            Number = dto.PhoneNumber,
                            Type = 1
                        }
                    ]
                };
                context.Organizations.Add(organization);
                await context.SaveChangesAsync(ct);
            }

            var user = await context.Users
                .AsTracking()
                .Where(x => x.Id == dto.Id)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");
            
            user.StatusId = 5;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Organizations.Add(new UserOrganization()
            {
                OrganizationIdentifier = organization.Id,
                OrganizationName = organization.Name,
                OrganizationVat = organization.Vat,
                IsAdministrator = false
            });
            user.Addresses.Add(new UserAddress()
            {
                AddressIdentifier = 1,
                Line1 = dto.Address,
                Line2 = "",
                City = dto.City,
                PostalCode = dto.Postalcode,
                Country = dto.Country
            });
            user.PhoneNumbers.Add(new UserPhoneNumber()
            {
                PhoneNumberIdentifier = 1,
                Number = dto.PhoneNumber,
                Type = 1
            });
            user.VisibleApplicationTypes = [1, 2, 3, 4, 5, 6, 7, 8];
            user.Statistics = [];
            user.Type = UserType.Client;
            user.LastLoginDate.Add(DateTime.UtcNow);
            user.EmailConfirmed = true;

            var userOrganization = new UserOrganization
            {
                OrganizationIdentifier = organization.Id,
                OrganizationName = organization.Name,
                OrganizationVat = organization.Vat,
                IsAdministrator = true
            };
            context.Users.Update(user);

            //set user role
            context.UserRoles.Add(new IdentityUserRole<int> { RoleId = 2, UserId = dto.Id });

            //set user claims
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "userId", ClaimValue = dto.Id.ToString() });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "applicationRole", ClaimValue = "customer" });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "organizationId", ClaimValue = organization.Id.ToString() });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "organizationNumber", ClaimValue = organization.Vat });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "firstName", ClaimValue = dto.FirstName });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "lastName", ClaimValue = dto.LastName });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "fullName", ClaimValue = dto.FirstName + " " + dto.LastName });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "startPage", ClaimValue = "/" });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "userType", ClaimValue = "User" });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "isAdministrator", ClaimValue = "false" });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "role", ClaimValue = "P" });

            // Message to responsible
            var nua = await context.UserClaims
                .Where(x => x.ClaimType== "role" && x.ClaimValue == "NUA")
                .Select(x => x.UserId)
                .ToListAsync(cancellationToken: ct);

            var title = $"Ny användare har registrerat sej <br>{dto.FirstName} {dto.LastName}<br>från produktionsbolaget {organization.Name}";
            
            foreach (var id in nua)
            {
                var receiver = await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);
                
                if (receiver is null) continue;
                
                context.Messages.Add(new Message()
                {
                    Receiver = new MessageContactDto()
                    {
                        ContactIdentifier = receiver.Id,
                        Name = receiver.FullName,
                        Email = receiver.Email ?? ""
                    },
                    MessageTypeId = 1001,
                    StatusId = 3,
                    OrganizationId = organization.Id,
                    OrganizationName = organization.Name,
                    Title = title,
                    CreatedDate = DateTime.UtcNow,
                    ExecutionDate = DateTime.UtcNow,
                    ExpireDate = DateTime.UtcNow.AddDays(7)
                });
            }

            // External login
            var providerKeyExists = await context.UserLogins
                .Where(x => x.UserId == dto.Id && x.ProviderKey == dto.ProviderKey)
                .FirstOrDefaultAsync(ct) != null;

            if (dto.Provider.Length > 0 && dto.ProviderKey.Length > 0 && providerKeyExists is false)
            {
                context.UserLogins.Add(new IdentityUserLogin<int>
                {
                    UserId = dto.Id, LoginProvider = dto.Provider, ProviderKey = dto.ProviderKey,
                    ProviderDisplayName = dto.Provider
                });
            }

            await context.SaveChangesAsync(ct);

            await AggregateAsync(ct, user.Id);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> RegisterAdminUserAsync(RegisterAdminUserDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Id == 0) throw new Exception("userId is required");
            if (dto.FirstName == string.Empty) throw new Exception("FirstName is required");
            if (dto.LastName == string.Empty) throw new Exception("LastName is required");
            if (dto.Provider == string.Empty) throw new Exception("Provider is required");
            if (dto.ProviderKey == string.Empty) throw new Exception("ProviderKey is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var organization = await context.Organizations
                .AsNoTracking()
                .Where(x => x.Id == 1)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Organization not found");

            var user = await context.Users
                .Where(x => x.Id == dto.Id)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");

            var userOrganization = new UserOrganization
            {
                OrganizationIdentifier = organization.Id,
                OrganizationName = organization.Name,
                OrganizationVat = organization.Vat,
                IsAdministrator = true
            };

            var current = user.Organizations is not null ? user.Organizations.ToList() : new List<UserOrganization>();

            current.Add(userOrganization);

            user.Organizations = current;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            context.Users.Update(user);

            //set user role
            context.UserRoles.Add(new IdentityUserRole<int> { RoleId = 1, UserId = dto.Id });

            //set user claims
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "userId", ClaimValue = dto.Id.ToString() });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "role", ClaimValue = "employee" });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "role", ClaimValue = "ALL" });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "organizationId", ClaimValue = organization.Id.ToString() });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "organizationNumber", ClaimValue = organization.Vat });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "firstName", ClaimValue = dto.FirstName });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "lastName", ClaimValue = dto.LastName });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "fullName", ClaimValue = dto.FirstName + " " + dto.LastName });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "startPage", ClaimValue = "/" });
            context.UserClaims.Add(new IdentityUserClaim<int>
                { UserId = dto.Id, ClaimType = "userType", ClaimValue = "Admin" });

            // External login
            var providerKeyExists = await context.UserLogins
                .Where(x => x.UserId == dto.Id && x.ProviderKey == dto.ProviderKey)
                .FirstOrDefaultAsync(ct) != null;

            if (dto.Provider.Length > 0 && dto.ProviderKey.Length > 0 && providerKeyExists is false)
            {
                context.UserLogins.Add(new IdentityUserLogin<int>
                {
                    UserId = dto.Id, LoginProvider = dto.Provider, ProviderKey = dto.ProviderKey,
                    ProviderDisplayName = dto.Provider
                });
            }

            await context.SaveChangesAsync(ct);

            await AggregateAsync(ct, user.Id);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> AddUserToOrganizationAsync(int organizationId, int userId,
        bool isAdministrator, CancellationToken ct)
    {
        try
        {
            if (organizationId == 0) throw new Exception("organizationId is required");
            if (userId == 0) throw new Exception("userId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var organization = await context.Organizations
                .Where(x => x.Id == organizationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Organization not found");

            var user = await context.Users
                .AsTracking()
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");

            var userOrganization = new UserOrganization
            {
                OrganizationIdentifier = organization.Id,
                OrganizationName = organization.Name,
                OrganizationVat = organization.Vat,
                IsAdministrator = isAdministrator
            };

            var current = user.Organizations is not null ? user.Organizations.ToList() : new List<UserOrganization>();

            current.Add(userOrganization);

            user.Organizations = current;

            context.Users.Update(user);
            await context.SaveChangesAsync(ct);

            await AggregateAsync(ct, user.Id);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> DeleteUserFromOrganizationAsync(int organizationId, int userId,
        CancellationToken ct)
    {
        try
        {
            if (organizationId == 0) throw new Exception("organizationId is required");
            if (userId == 0) throw new Exception("userId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var organization = await context.Organizations
                .Where(x => x.Id == organizationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Organization not found");

            var user = await context.Users
                .AsTracking()
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");

            user.Organizations = user.Organizations.Where(x => x.OrganizationIdentifier != organization.Id).ToList();

            context.Users.Update(user);
            await context.SaveChangesAsync(ct);

            await AggregateAsync(ct, user.Id);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> DeleteUserAsync(int userId, CancellationToken ct)
    {
        try
        {
            if (userId == 0) throw new Exception("userId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            return await context.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(v => v
                    .SetProperty(s => s.StatusId, 19), ct) > 0;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<UserDto, Exception>> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        try
        {
            if (userId == 0) throw new Exception("userId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");

            return user.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<UserDto, Exception>> UpdateUserAsync(int userId, UpdateUserDto dto, CancellationToken ct)
    {
        try
        {
            if (userId == 0) throw new Exception("userId is required");
            if (dto.StatusId == 0) throw new Exception("StatusId is required");
            if (dto.FirstName == string.Empty) throw new Exception("FirstName is required");
            if (dto.LastName == string.Empty) throw new Exception("LastName is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var user = await context.Users
                .AsTracking()
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");

            user.StatusId = dto.StatusId;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Addresses = dto.Addresses.Select(x => x.ToEntity()).ToList();
            user.PhoneNumbers = dto.PhoneNumbers.Select(x => x.ToEntity()).ToList();
            user.Organizations = dto.Organizations.Select(x => x.ToEntity()).ToList();
            user.Statistics = dto.Statistics.Select(x => x.ToEntity()).ToList();
            user.Favorites = dto.Favorites;

            context.Users.Update(user);
            await context.SaveChangesAsync(ct);

            return user.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<int>, Exception>> FavoritesAsync(int userId, CancellationToken ct)
    {
        try
        {
            if (userId == 0) throw new Exception("userId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var user = await context.Users
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");

            return user.Favorites;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<UserDto, Exception>> UpdateFavoritesAsync(int userId, List<int> favorites, CancellationToken ct)
    {
        try
        {
            if (userId == 0) throw new Exception("userId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var user = await context.Users
                .AsTracking()
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("User not found");

            user.Favorites = favorites;

            context.Users.Update(user);
            await context.SaveChangesAsync(ct);

            return user.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<UserDto>, Exception>> UsersByOrganizationIdAsync(int organizationId,
        CancellationToken ct)
    {
        try
        {
            if (organizationId == 0) throw new Exception("organizationId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var organizations = GetUsersByOrganizationId(context, organizationId) ??
                                throw new Exception("Users not found");
            return await organizations.ToListAsync(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<UserDto>, Exception>> UsersByClaimTagAsync(string claimTag,
        CancellationToken ct)
    {
        try
        {
            if (claimTag.Length == 0) throw new Exception("claimTag is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var claimUsers = await context.UserClaims
                .AsNoTracking()
                .Where(x => x.ClaimType == "role" && x.ClaimValue == claimTag)
                .Select(x => x.UserId)
                .ToListAsync(ct) ?? throw new Exception("ProjectManagers not found");

            var users = await context.Users
                .AsNoTracking()
                .Where(x => claimUsers.Contains(x.Id))
                .ToListAsync(ct) ?? throw new Exception("Users not found");

            return users.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<UserDto>, Exception>> UsersByClaimTagsAsync(string[] claimTags,
        CancellationToken ct)
    {
        try
        {
            if (claimTags.Length == 0) throw new Exception("claimTags is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var claimUsers = await context.UserClaims
                .AsNoTracking()
                .Where(x => x.ClaimType == "role" && claimTags.Contains(x.ClaimValue))
                .Select(x => x.UserId)
                .ToListAsync(ct) ?? throw new Exception("ProjectManagers not found");

            var users = await context.Users
                .AsNoTracking()
                .Where(x => claimUsers.Contains(x.Id))
                .ToListAsync(ct) ?? throw new Exception("Users not found");

            return users.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<UserDto>, Exception>> ProjectManagersAsync(int schemaId, CancellationToken ct)
    {
        try
        {
            var claimValue = schemaId switch
            {
                1 => "SFA",
                2 => "KFA",
                3 => "TVA",
                4 => "IFA",
                5 => "DFA",
                6 => "SFA",
                7 => "TVA",
                8 => "DFA",
                _ => "ADM"
            };

            await using var context = await factory.CreateDbContextAsync(ct);
            var claimUsers = await context.UserClaims
                .AsNoTracking()
                .Where(x => x.ClaimType == "role" && x.ClaimValue == claimValue)
                .Select(x => x.UserId)
                .ToListAsync(ct) ?? throw new Exception("ProjectManagers not found");

            var users = await context.Users
                .AsNoTracking()
                .Where(x => claimUsers.Contains(x.Id))
                .ToListAsync(ct) ?? throw new Exception("Users not found");

            return users.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<User, Exception>> GetUserAsync(int userId, string loginName, string organizationVat,
        CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            var query = context.Users
                .AsNoTracking();

            if (userId > 0)
            {
                query = query.Where(x => x.Id == userId);
            }

            if (loginName.Length > 0)
            {
                query = query.Where(x => x.Email!.ToLower() == loginName.ToLower());
            }

            if (organizationVat.Length > 0)
            {
                query = query.Where(x =>
                    x.Organizations.Any(x => x.OrganizationVat.ToLower() == organizationVat.ToLower()));
            }

            var user = await query
                .FirstOrDefaultAsync(ct) ?? throw new Exception("cant find user");

            return user;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<UserSummaryDto>, Exception>> GetUsersSummaryByOrganizationIdAsync(
        int organizationId, CancellationToken ct)
    {
        try
        {
            if (organizationId == 0) throw new Exception("organizationId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var summary = await context.Users
                .AsNoTracking()
                .Where(x => x.Organizations.Any(x => x.OrganizationIdentifier == organizationId) && x.StatusId != 19)
                .Select(x => new UserSummaryDto()
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email ?? string.Empty
                })
                .ToListAsync(ct) ?? throw new Exception("Projects not found");

            return summary;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> ActivateUserAsync(int userId, CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            return await context.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(s => s.StatusId, 2), ct) > 0;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }


    private static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<UserDto>> GetUsersByOrganizationId =
        EF.CompileAsyncQuery((ApplicationDbContext context, int organizationId) =>
            context.Users
                .AsNoTracking()
                .TagWith("GetUsersByOrganizationId")
                .Where(x => x.Organizations.Any(u => u.OrganizationIdentifier == organizationId) && x.StatusId != 19)
                .OrderBy(x => x.Id)
                .Select(x => x.ToDto()));

}