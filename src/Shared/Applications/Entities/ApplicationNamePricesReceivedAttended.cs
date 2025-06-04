namespace Shared.Applications.Entities;

public class ApplicationNamePricesReceivedAttended
{
    public int NamePricesReceivedAttendedIdentifier { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public int PricesReceived { get; set; } = 0;
    public bool Attended { get; set; }
}