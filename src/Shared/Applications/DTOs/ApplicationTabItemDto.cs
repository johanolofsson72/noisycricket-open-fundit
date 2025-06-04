namespace Shared.Applications.DTOs;

public class ApplicationTabItemDto
{
    public int Id { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public bool Selected { get; set; } = false;
    public bool DeletedOrDenied { get; set; } = false;
}