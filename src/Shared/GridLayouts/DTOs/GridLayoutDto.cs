namespace Shared.GridLayouts.DTOs;

public class GridLayoutDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string GridName { get; set; } = string.Empty;
    public string GridStateJson { get; set; } = string.Empty;
}