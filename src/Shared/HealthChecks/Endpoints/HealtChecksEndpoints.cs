using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Shared.Email.Services;
using Shared.Global.Interface;

namespace Shared.HealthChecks.Endpoints;
public class HealtChecksEndpoints : IEndpoint
{
    public void RegisterEndpoints(WebApplication app)
    {
        var posts = app.MapGroup("/api/v1/healthchecks").WithName("HealthChecks").WithTags("HealthChecks");

        posts.MapPost("/notify", Notify)
            .WithName("Notify")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Notify by email regarding health status",
                Description = "Send an email to the administrators when health is changed, Health checks can be found at /health and /healthcheck-ui",
                Tags = new List<OpenApiTag> { new() { Name = "Notify by email regarding health status" } }
            });
    }

    private static Task<IResult> Notify([FromServices] EmailService emailService, [FromServices] IConfiguration configuration, NotificationMessage notificationMessage)
    {
        var result = emailService.SendEmailAsync(
            email: configuration.GetValue<string>("EmailConfiguration:From")!,
            subject: "Health status changed at " + configuration.GetValue<string>("API_BASE_ADDRESS")!, 
            htmlMessage: notificationMessage.Message);

        return Task.FromResult(Results.Ok(result));
    }
}