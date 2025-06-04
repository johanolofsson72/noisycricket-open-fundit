using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.DbContext;
using Shared.Schemas.Entities;

namespace Server.Test.Helpers;

public static class Schemas
{
    public static List<Schema> SeedSchemas(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var schemas = new List<Schema>();
        
        schemas = SeedSchemasSwedishFeatureFilm.Seed(factory, schemas);
        schemas = SeedSchemasShortFeatureFilm.Seed(factory, schemas);
        schemas = SeedSchemasDramaSeries.Seed(factory, schemas);
        schemas = SeedSchemasInternationalFeatureFilm.Seed(factory, schemas);
        schemas = SeedSchemasDocumentaryFeatureFilm.Seed(factory, schemas);
        schemas = SeedSchemasDevelopmentSwedishFeatureFilm.Seed(factory, schemas);
        schemas = SeedSchemasDevelopmentDramaSeries.Seed(factory, schemas);
        schemas = SeedSchemasDevelopmentDocumentaryFeatureFilm.Seed(factory, schemas);

        foreach (var schema in schemas)
        {
            schema.Enabled = true;
            foreach (var ev in schema.Events)
            {
                var schemaEventIdentifier = ev.SchemaEventIdentifier;
                foreach (var action in ev.Actions)
                {
                    var eventActionCombo = string.Empty;
                    var deleteEventId = action.DeleteEventId;
                    var deleteActionId = action.DeleteActionId;
                    if (deleteEventId > 0 && deleteActionId > 0)
                    {
                        eventActionCombo = $"{deleteEventId}-{deleteActionId}";
                    }
                    action.EventActionCombo = eventActionCombo;
                    action.SchemaEventIdentifier = schemaEventIdentifier;
                }
            }
        }
            
        context.Schemas.AddRange(schemas);
        context.SaveChanges();

        return schemas;
    }

}