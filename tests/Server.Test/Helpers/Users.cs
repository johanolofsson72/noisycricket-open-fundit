using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.DbContext;
using Shared.Messages.DTOs;
using Shared.Messages.Entities;
using Shared.Organizations.Entities;
using Shared.Users.Entities;

namespace Server.Test.Helpers;

public static class Users
{
    public static int Create(WebApplicationFactory<Program> factory, string firstName, string lastName, string email, string password, List<string> claimTypes, int organizationId, string organizationNumber, int roleId)
    {
        using var scope = factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var statistics = context.Statistics.ToList();
        
        var stat = statistics
            .Where(x => new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }.Contains(x.Id))
            .OrderBy(x => x.Id)
            .Select((x, index) => new UserStatistic()
            {
                StatisticIdentifier = index++,
                Name = x.Name,
                Description = x.Description,
                Query = x.Query,
                Columns = x.Columns,
                Rows = x.Rows,
                Unit = x.Unit
            })
            .ToList();
        
        var appUser = new User { 
            StatusId = 2,
            Email = email.ToLower(),
            EmailConfirmed = true, 
            FirstName = firstName,
            LastName = lastName,
            UserName = email.ToLower(),
            NormalizedUserName = email.ToUpper(),
            NormalizedEmail = email.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            Organizations = [
                new UserOrganization
                {
                    OrganizationIdentifier = organizationId,
                    OrganizationName = "Sci-Fi AB",
                    OrganizationVat = "5565735569",
                    IsAdministrator = true
                }
            ],
            Statistics = stat.Select(s => new UserStatistic
            {
                StatisticIdentifier = s.StatisticIdentifier,
                Name = s.Name,
                Description = s.Description,
                Columns = s.Columns,
                Rows = s.Rows,
                Query = s.Query,
                Unit = s.Unit,
                Value = ""
            }).ToList()
        };

        //set user password
        var ph = new PasswordHasher<User>();
        appUser.PasswordHash = ph.HashPassword(appUser, password);

        //seed user
        context.Users.Add(appUser);
        context.SaveChanges();
        
        //set user role
        context.UserRoles.Add(new IdentityUserRole<int> { 
            RoleId = roleId, 
            UserId = appUser.Id 
        });
        
        //set user claims
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "userId",
            ClaimValue = appUser.Id.ToString()
        });

        foreach (var claimType in claimTypes)
        {
            context.UserClaims.Add(new IdentityUserClaim<int> { 
                UserId = appUser.Id,
                ClaimType = "role",
                ClaimValue = claimType
            });
        }
        
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "organizationId",
            ClaimValue = organizationId.ToString()
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "organizationNumber",
            ClaimValue = organizationNumber
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "firstName",
            ClaimValue = appUser.FirstName
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "lastName",
            ClaimValue = appUser.LastName
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "fullName",
            ClaimValue = appUser.FirstName + " " + appUser.LastName
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "startPage",
            ClaimValue = "/"
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "userType",
            ClaimValue = "Admin"
        });
        context.SaveChanges();
        
        return appUser.Id;
    }
    
    public static int CreateForCustomer(WebApplicationFactory<Program> factory, string firstName, string lastName, string email, string password, int organizationId, string organizationNumber, int roleId)
    {
        using var scope = factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var statistics = context.Statistics.Where(x => x.IsPublic).ToList();
        
        var stat = statistics
            .OrderBy(x => x.Id)
            .Select((x, index) => new UserStatistic()
            {
                StatisticIdentifier = index++,
                Name = x.Name,
                Description = x.Description,
                Query = x.Query,
                Columns = x.Columns,
                Rows = x.Rows,
                Unit = x.Unit
            })
            .ToList();
        
        var appUser = new User { 
            StatusId = 2,
            Email = email,
            EmailConfirmed = true, 
            FirstName = firstName,
            LastName = lastName,
            UserName = email,
            NormalizedUserName = email.ToUpper(),
            NormalizedEmail = email.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            Organizations = [
                new UserOrganization
                {
                    OrganizationIdentifier = organizationId,
                    OrganizationName = "Star Wars AB",
                    OrganizationVat = "5567144950",
                    IsAdministrator = true
                }
            ],
            Statistics = statistics.Select(s => new UserStatistic
            {
                StatisticIdentifier = s.Id,
                Name = s.Name,
                Description = s.Description,
                Columns = s.Columns,
                Rows = s.Rows,
                Query = s.Query,
                Unit = s.Unit,
                Value = ""
            }).ToList()
        };

        //set user password
        var ph = new PasswordHasher<User>();
        appUser.PasswordHash = ph.HashPassword(appUser, password);

        //seed user
        context.Users.Add(appUser);
        context.SaveChanges();
        
        //set user role
        context.UserRoles.Add(new IdentityUserRole<int> { 
            RoleId = roleId, 
            UserId = appUser.Id 
        });

        //set user claims
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "userId",
            ClaimValue = appUser.Id.ToString()
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "applicationRole",
            ClaimValue = "customer"
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "organizationId",
            ClaimValue = organizationId.ToString()
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "organizationNumber",
            ClaimValue = organizationNumber
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "firstName",
            ClaimValue = appUser.FirstName
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "lastName",
            ClaimValue = appUser.LastName
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "fullName",
            ClaimValue = appUser.FirstName + " " + appUser.LastName
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "startPage",
            ClaimValue = "/"
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "userType",
            ClaimValue = "User"
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "isAdministrator",
            ClaimValue = "false"
        });
        context.UserClaims.Add(new IdentityUserClaim<int> { 
            UserId = appUser.Id,
            ClaimType = "role",
            ClaimValue = "P"
        });

        /*var messages = new List<Message>();
        var titles = new List<string>() {"Hej och välkommen som användare i FundIT.", "Projektet har nu blivit godkänt.", "Vi ber er att att skicka in underlag för utbetalning.", "Er ansökan är inte helt färdig.", "Ansökan ser bra ut!", "Er ansökan är mottagen och kontrolleras nu av vår personal."};
        foreach (var title in titles)
        {
            messages.Add(new Message()
            {
                Receiver = new MessageContactDto(){ ContactIdentifier = appUser.Id, Email = appUser.Email, Name = appUser.FullName },
                MessageTypeId = 1003,
                Outgoing = true,
                StatusId = 3,
                OrganizationId = organizationId,
                OrganizationName = "Star Wars AB",
                ApplicationId = 1,
                ApplicationTitle = "The Dream of Shadow",
                ApplicationStatusId = 2,
                SchemaId = 1,
                SchemaNames = ["Svensk långfilm","Swedish Feature Film","Svensk spillefilm","Schwedischer Spielfilm","Largometraje sueco","Long måtrage suådois","Lungometraggio svedese","Svensk spillefilm"],
                Title = title,
                CreatedDate = DateTime.UtcNow,
                ExecutionDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.AddDays(7)
            });
        }
        
        context.Messages.AddRange(messages);*/
        
        context.SaveChanges();
        return appUser.Id;
    }
}