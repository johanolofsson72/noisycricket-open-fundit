using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.DbContext;

namespace Server.Test.Helpers;

public static class ReorderControls
{
    public static void Execute(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var schemas = context.Schemas.AsTracking().ToList();

        foreach (var schema in schemas)
        {
            var index = 1;
            foreach (var control in schema.Controls.Where(x => x.ApplicationFormSectionId == 2).OrderBy(x => x.Order))
            {
                control.ApplicationFormOrder = index;
                index++;
            }
                
            index = 1;
            foreach (var control in schema.Controls.Where(x => x.ApplicationFormSectionId == 3).OrderBy(x => x.Order))
            {
                control.ApplicationFormOrder = index;
                index++;
            }
                
            index = 1;
            foreach (var control in schema.Controls.Where(x => x.ApplicationFormSectionId == 4).OrderBy(x => x.Order))
            {
                control.ApplicationFormOrder = index;
                index++;
            }
                
            index = 1;
            foreach (var control in schema.Controls.Where(x => x.ApplicationFormSectionId == 1).OrderBy(x => x.Order))
            {
                control.ApplicationFormOrder = index;
                index++;
            }
        }
            
        context.SaveChanges();
    }
}