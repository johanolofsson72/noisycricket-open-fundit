namespace Shared.Applications.Entities;

public class ApplicationDaysLocation
{
    public int DaysLocationIdentifier { get; set; } = 0;
    public int Days { get; set; } = 0;
    public string Location { get; set; } = string.Empty;
    public virtual Application Application { get; set; } = new Application();
}