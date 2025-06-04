using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Data.DbContext;
using Shared.Global.Interface;
using Shared.OpenAi.Services;

namespace Shared.OpenAi.Endpoints;

public class OpenAiEndpoints : IEndpoint
{
    public void RegisterEndpoints(WebApplication app)
    {
        var posts = app.MapGroup("/api/v1/semanticsearches").WithName("SemanticSearches").WithTags("SemanticSearches");

        posts.MapGet("/{question}", SemanticSearch)
            .WithName("SemanticSearch")
            .WithOpenApi();;
    }

    private static async Task<IResult> SemanticSearch([FromServices] OpenAiService service, [FromServices] ApplicationDbContext context, string question, CancellationToken ct)
    {
        var result = await service.SearchAsync(context, question);
        return Results.Ok(result);
    }
}