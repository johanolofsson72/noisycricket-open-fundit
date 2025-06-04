namespace AppClient.Components;

public static class UploadEndpoints
{
    public static void MapUploadEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/upload").WithTags("Upload");

        _ = group.MapPost("/save", async (HttpRequest request) =>
            {
                try
                {
                    var keys = request.Form.Keys;
                    var files = request.Form.Files;
                    if (files.Count <= 0 || keys.Count != 1) return Results.Ok();

                    foreach (var file in files)
                    {
                        var saveLocation = Path.Combine(keys.First(), file.FileName);

                        await using var fileStream = new FileStream(saveLocation, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                    }
                }
                catch
                {
                    return Results.BadRequest();
                }

                return Results.Ok();
            })
            .WithName("Save");

        _ = group.MapPost("/remove", (HttpRequest request) =>
            {
                try
                {
                    var keys = request.Form.Keys;
                    var files = request.Form.Files;
                    if (files.Count <= 0 || keys.Count <= 0) return Results.Ok();

                    foreach (var file in files)
                    {
                        var deleteLocation = Path.Combine(keys.First(), file.FileName);
                        System.IO.File.Delete(deleteLocation);
                    }
                }
                catch
                {
                    return Results.BadRequest();
                }

                return Results.Ok();
            })
            .WithName("Delete");
    }

}