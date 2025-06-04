namespace Shared.Users.DTOs;

public class UserDashboardItemDto
{
    public int UserDashboardItemIdentifier { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public bool Visible { get; set; } = false;
    public string Unit { get; set; } = string.Empty;
    public int Columns { get; set; } = 0;
    public int Rows { get; set; } = 0;
}