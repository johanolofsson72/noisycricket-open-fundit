using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Global.Interface;
using Shared.Statistics.Services;

namespace Shared.Statistics.Endpoints;

public class StatisticsEndpoint : IEndpoint
{
    public void RegisterEndpoints(WebApplication app)
    {
        var posts = app.MapGroup("/api/v1/statistics")
            .WithName("Statistics")
            .WithTags("Statistics");

        posts.MapGet("/", GetStatistics)
            .WithName("GetStatistics")
            .WithOpenApi();
    }

    private static async Task<IResult> GetStatistics(
        [FromServices] StatisticService service,
        CancellationToken ct)
    {
        var result = await service.AllStatisticsAsync();
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }
}
