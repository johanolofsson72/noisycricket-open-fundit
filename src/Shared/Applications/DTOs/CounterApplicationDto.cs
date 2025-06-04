namespace Shared.Applications.DTOs;

public class CounterApplicationDto
{
    public int Id { get; set; } = 0;
    public decimal OurContribution { get; set; } = 0;
    public int NewEventCounter { get; set; } = 0;
    public int NewAuditCounter { get; set; } = 0;
}