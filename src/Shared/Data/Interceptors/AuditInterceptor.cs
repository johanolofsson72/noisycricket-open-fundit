using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.AuditEntries.Entities;

namespace Shared.Data.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly List<AuditEntry> _auditEntries;

    public AuditInterceptor(List<AuditEntry> auditEntries)
    {
        _auditEntries = auditEntries;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);
        
        var startTime = DateTime.UtcNow;
        var entries = eventData.Context.ChangeTracker.Entries()
            .Where(x => x.Entity is not AuditEntry &&
                        x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(x => new AuditEntry()
            {
                StartTime = startTime,
                Metadata = x.DebugView.LongView,

            })
            .ToList();

        if (entries.Count == 0) return base.SavingChangesAsync(eventData, result, cancellationToken);

        _auditEntries.AddRange(entries);
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null) return base.SavedChangesAsync(eventData, result, cancellationToken);
        if (_auditEntries.Count == 0) return base.SavedChangesAsync(eventData, result, cancellationToken);
        
        var endTime = DateTime.UtcNow;

        foreach (var auditEntry in _auditEntries)
        {
            auditEntry.EndTime = endTime;
            auditEntry.Succeeded = true;
        }
            
        eventData.Context.Set<AuditEntry>().AddRange(_auditEntries);
        _auditEntries.Clear();
        eventData.Context.SaveChangesAsync(cancellationToken);

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null) return;
        
        var endTime = DateTime.UtcNow;

        foreach (var auditEntry in _auditEntries)
        {
            auditEntry.ErrorMessage = eventData.Exception.Message;
            auditEntry.EndTime = endTime;
            auditEntry.Succeeded = false;
        }
            
        eventData.Context?.Set<AuditEntry>().AddRange(_auditEntries);
        _auditEntries.Clear();
        if (eventData.Context != null) await eventData.Context.SaveChangesAsync(cancellationToken);
    }
}