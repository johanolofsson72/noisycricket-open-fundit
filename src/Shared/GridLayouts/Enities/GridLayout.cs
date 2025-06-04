using Shared.GridLayouts.DTOs;

namespace Shared.GridLayouts.Enities;

public class GridLayout
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string GridName { get; set; } = string.Empty;
    public string GridStateJson { get; set; } = string.Empty;
}

public static class GridLayoutExtensions
{
    public static GridLayoutDto ToDto(this GridLayout entity)
    {
        return new GridLayoutDto()
        {
            Id = entity.Id,
            Title = entity.Title,
            UserName = entity.UserName,
            GridName = entity.GridName,
            GridStateJson = entity.GridStateJson,
        };
    }
    
    public static GridLayout ToEntity(this GridLayoutDto dto)
    {
        return new GridLayout()
        {
            Id = dto.Id,
            Title = dto.Title,
            UserName = dto.UserName,
            GridName = dto.GridName,
            GridStateJson = dto.GridStateJson,
        };
    }
}