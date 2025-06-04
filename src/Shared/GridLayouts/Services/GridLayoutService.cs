using System.Text.Json;
using Shared.Data.DbContext;
using Shared.GridLayouts.DTOs;
using Shared.GridLayouts.Enities;
using Telerik.Blazor.Components;

namespace Shared.GridLayouts.Services;

public class GridLayoutService(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<Result<List<GridLayoutDto>, Exception>> GetAllAsync(string userName, string gridName, CancellationToken ct)
    {
        try
        {
            if (userName.Length == 0) throw new Exception("userName is required");
            if (gridName.Length == 0) throw new Exception("gridName is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var existingLayouts = await context.GridLayouts
                .AsNoTracking()
                .ToListAsync(ct);
            
            return existingLayouts.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<GridLayoutDto, Exception>> SaveGridStateAsync(string userName, string gridName, string title, GridState<ProjectSearchResultDto> gridState, CancellationToken ct)
    {
        try
        {
            if (userName.Length == 0) throw new Exception("userName is required");
            if (gridName.Length == 0) throw new Exception("gridName is required");
            if (gridState is null) throw new Exception("gridState is required");
            if (title.Length == 0) throw new Exception("title is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var layout = new GridLayoutDto();
            var jsonState = JsonSerializer.Serialize(gridState, new JsonSerializerOptions { WriteIndented = false });

            var newLayout = new GridLayout
            {                    
                Title = title,
                UserName = userName,
                GridName = gridName,
                GridStateJson = jsonState
            };
            context.Add(newLayout);
            layout = new GridLayoutDto()
            {
                Id = newLayout.Id,                    
                Title = newLayout.Title,
                UserName = newLayout.UserName, 
                GridName = newLayout.GridName, 
                GridStateJson = newLayout.GridStateJson
            };

            await context.SaveChangesAsync(ct);

            return layout;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<GridState<ProjectSearchResultDto>, Exception>> LoadGridStateFromViewIdAsync(int vyId, CancellationToken ct)
    {
        try
        {
            if (vyId == 0) throw new Exception("vyId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var layout = await context.GridLayouts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vyId, ct);

            if (layout is null) throw new Exception("View do not exist");
            
            return JsonSerializer.Deserialize<GridState<ProjectSearchResultDto>>(layout.GridStateJson) ?? throw new InvalidOperationException("Failed to deserialize grid state");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> DeleteGridStateAsync(int vyId, CancellationToken ct)
    {
        try
        {
            if (vyId == 0) throw new Exception("vyId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var existingLayout = await context.GridLayouts
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == vyId, ct);
            
            if (existingLayout is null) throw new Exception("View do not exist");
            
            context.GridLayouts.Remove(existingLayout);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}