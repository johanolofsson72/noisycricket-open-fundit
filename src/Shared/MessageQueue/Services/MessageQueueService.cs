using Shared.Data.DbContext;
using Shared.Global.Entities;
using Shared.MessageQueue.DTOs;
using Shared.MessageQueue.Entities;
using Shared.Notifications;

namespace Shared.MessageQueue.Services;

public class MessageQueueService(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<Result<bool, Exception>> PublishAsync(NotificationType notificationType, CancellationToken ct)
    {
        try
        {
            if (notificationType == NotificationType.Default) throw new Exception("NotificationType is required");

            var messageQueueItem = new MessageQueueItem()
            {
                NotificationType = notificationType,
                StatusId = 3
            };

            await using var context = await factory.CreateDbContextAsync(ct);
            await context.MessageQueue.AddAsync(messageQueueItem, ct);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<List<MessageQueueItemDto>, Exception>> MessageQueueAsync(CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            var messageQueue = await GetMessageQueue(context).ToListAsync(cancellationToken: ct);
            foreach (var messageQueueItem in messageQueue)
            {
                messageQueueItem.StatusId = 5;
            }

            await context.SaveChangesAsync(ct);

            return messageQueue.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<int, Exception>> AcknowledgedMessageQueueAsync(int[] messageQueueItemIds, CancellationToken ct)
    {
        try
        {
            var count = 0;
            await using var context = await factory.CreateDbContextAsync(ct);
            foreach (var id in messageQueueItemIds)
            {
                await context.MessageQueue.Where(x => x.Id == id)
                    .ExecuteUpdateAsync(u => u
                        .SetProperty(p => p.StatusId, 16),ct);
                count++;
            }

            return count;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> ClearQueueAsync(CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            return await context.MessageQueue.ExecuteDeleteAsync(ct) > 0;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<MessageQueueItem>> GetMessageQueue = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.MessageQueue
                .AsTracking()
                .TagWith("GetMessageQueue")
                .Where(x => x.StatusId == 3));
    
    private static readonly Func<ApplicationDbContext, int, Task<MessageQueueItem?>> GetMessageQueueItem = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int messageQueueId) => 
            context.MessageQueue
                .AsTracking()
                .TagWith("GetMessageQueueItem")
                .FirstOrDefault(x => x.Id == messageQueueId));
    
}