using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.DbContext;

namespace Server.Test.Helpers;

public static class UserRole
{
    public static int Create(WebApplicationFactory<Program> factory)
    {
        try
        {
            using var scope = factory.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var role = new IdentityRole<int>
            {
                Name = "User"
            };
        
            context.Roles.Add(role);
            context.SaveChanges();
        
            return role.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 0;
        }
    }
}