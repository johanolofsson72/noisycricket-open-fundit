using System;

namespace Shared.AuditEntries.Entities;

public class AuditEntry
{
    public int Id { get; set; } = 0;
    public string Metadata { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } = DateTime.Now;
    public DateTime EndTime { get; set; } = DateTime.Now;
    public bool Succeeded { get; set; } = false;
    public string ErrorMessage { get; set; } = string.Empty;
}