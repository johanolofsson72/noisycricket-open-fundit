using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Global.Interface;
using Shared.Schemas.DTOs;
using Shared.Schemas.Services;

namespace Shared.Schemas.Endpoints;

public class SchemaEndpoints : IEndpoint
{
    public void RegisterEndpoints(WebApplication app)
    {
        var posts = app.MapGroup("/api/v1/schemas")
            .WithName("Schemas")
            .WithTags("Schemas");

        posts.MapPost("/{schemaId:int}/events/{eventId:int}/actions", AddSchemaEventAction)
            .WithName("AddSchemaEventAction")
            .WithOpenApi();

        posts.MapPut("/{schemaId:int}/events/{eventId:int}/actions", UpdateSchemaEventAction)
            .WithName("UpdateSchemaEventAction")
            .WithOpenApi();

        posts.MapDelete("/{schemaId:int}/events/{eventId:int}/actions/{actionId:int}", DeleteSchemaEventAction)
            .WithName("DeleteSchemaEventAction")
            .WithOpenApi();

        posts.MapPost("/{schemaId:int}/events", AddSchemaEvent)
            .WithName("AddSchemaEvent")
            .WithOpenApi();

        posts.MapPut("/{schemaId:int}/events", UpdateSchemaEvent)
            .WithName("UpdateSchemaEvent")
            .WithOpenApi();

        posts.MapDelete("/{schemaId:int}/events/{eventId:int}", DeleteSchemaEvent)
            .WithName("DeleteSchemaEvent")
            .WithOpenApi();

        posts.MapPost("/{schemaId:int}/controls", AddSchemaControl)
            .WithName("AddSchemaControl")
            .WithOpenApi();

        posts.MapPut("/{schemaId:int}/controls", UpdateSchemaControl)
            .WithName("UpdateSchemaControl")
            .WithOpenApi();

        posts.MapDelete("/{schemaId:int}/controls/{controlId:int}", DeleteSchemaControl)
            .WithName("DeleteSchemaControl")
            .WithOpenApi();

        posts.MapPost("/", CreateSchema)
            .WithName("CreateSchema")
            .WithOpenApi();

        posts.MapPost("/copy/{schemaId:int}", CopySchema)
            .WithName("CopySchema")
            .WithOpenApi();

        posts.MapPut("/{schemaId:int}", UpdateSchema)
            .WithName("UpdateSchema")
            .WithOpenApi();

        posts.MapDelete("/{schemaId:int}", DeleteSchema)
            .WithName("DeleteSchema")
            .WithOpenApi();

        posts.MapGet("/{schemaId:int}", GetSchemaById)
            .WithName("GetSchemaById")
            .WithOpenApi();

        posts.MapGet("/", GetSchemas)
            .WithName("GetSchemas")
            .WithOpenApi();
    }

    private static async Task<IResult> AddSchemaEventAction(
        [FromServices] SchemaService service, 
        int schemaId,
        int eventId,
        AddSchemaEventActionDto dto,
        CancellationToken ct)
    {
        var result = await service.AddSchemaEventActionAsync(schemaId, eventId, dto, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateSchemaEventAction(
        [FromServices] SchemaService service, 
        int schemaId,
        int eventId,
        UpdateSchemaEventActionDto dto,
        CancellationToken ct)
    {
        var result = await service.UpdateSchemaEventActionAsync(schemaId, eventId, dto, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteSchemaEventAction(
        [FromServices] SchemaService service, 
        int schemaId,
        int eventId,
        int actionId,
        CancellationToken ct)
    {
        var result = await service.DeleteSchemaEventActionAsync(schemaId, eventId, actionId, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> AddSchemaEvent(
        [FromServices] SchemaService service, 
        int schemaId,
        AddSchemaEventDto dto,
        CancellationToken ct)
    {
        var result = await service.AddSchemaEventAsync(schemaId, dto, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateSchemaEvent(
        [FromServices] SchemaService service, 
        int schemaId,
        UpdateSchemaEventDto dto,
        CancellationToken ct)
    {
        var result = await service.UpdateSchemaEventAsync(schemaId, dto, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteSchemaEvent(
        [FromServices] SchemaService service, 
        int schemaId,
        int eventId,
        CancellationToken ct)
    {
        var result = await service.DeleteSchemaEventAsync(schemaId, eventId, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> AddSchemaControl(
        [FromServices] SchemaService service, 
        int schemaId,
        AddSchemaControlDto dto,
        CancellationToken ct)
    {
        var result = await service.AddSchemaControlAsync(schemaId, dto, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateSchemaControl(
        [FromServices] SchemaService service, 
        int schemaId,
        UpdateSchemaControlDto dto,
        CancellationToken ct)
    {
        var result = await service.UpdateSchemaControlAsync(schemaId, dto, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteSchemaControl(
        [FromServices] SchemaService service, 
        int schemaId,
        int controlId,
        CancellationToken ct)
    {
        var result = await service.DeleteSchemaControlAsync(schemaId, controlId, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> CopySchema(
        [FromServices] SchemaService service, 
        int schemaId,
        CancellationToken ct)
    {
        var result = await service.CopySchemaAsync(schemaId, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> CreateSchema(
        [FromServices] SchemaService service, 
        CreateSchemaDto dto,
        CancellationToken ct)
    {
        var result = await service.CreateSchemaAsync(dto, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateSchema(
        [FromServices] SchemaService service, 
        int schemaId,
        UpdateSchemaDto dto,
        CancellationToken ct)
    {
        var result = await service.UpdateSchemaAsync(schemaId, dto, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteSchema(
        [FromServices] SchemaService service, 
        int schemaId,
        CancellationToken ct)
    {
        var result = await service.DeleteSchemaAsync(schemaId, ct);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetSchemaById(
        [FromServices] SchemaService service, 
        int schemaId)
    {
        var result = await service.SchemaByIdAsync(schemaId);
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetSchemas(
        [FromServices] SchemaService service)
    {
        var result = await service.SchemasAsync();
        return !result.IsOk ? result.ToProblemDetails() : Results.Ok(result.Value);
    }
}