using AppAdmin.State;
using Quartz;
using Shared.Global.Services;
using Shared.MessageQueue.DTOs;
using Shared.MessageQueue.Services;
using Shared.Notifications;

namespace AppAdmin;

public class MessageQueueHostedService(IServiceProvider serviceProvider, ILogger<MessageQueueHostedService> logger) : IHostedService, IDisposable
{
    private int _executionCount = 0;
    private Timer? _timer = null;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);

        if (UnitTestDetector.IsInUnitTest) return;
        
        using var scope = serviceProvider.CreateScope();
        var messageQueueService = scope.ServiceProvider.GetRequiredService<MessageQueueService>();

        _ = await messageQueueService.ClearQueueAsync(cancellationToken);

        logger.LogInformation("MessageQueueHostedService running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }
    
    private void DoWork(object? state)
    {
        _ = DoWorkAsync(state);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);

        if (UnitTestDetector.IsInUnitTest) return;
        
        using var scope = serviceProvider.CreateScope();
        var messageQueueService = scope.ServiceProvider.GetRequiredService<MessageQueueService>();

        _ = await messageQueueService.ClearQueueAsync(cancellationToken);
        
        logger.LogInformation("MessageQueueHostedService is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
           
    }
    
    private async Task DoWorkAsync(object? state)
    {
        if (!await _semaphore.WaitAsync(0))
        {
            return; 
        }

        try
        {
            using var scope = serviceProvider.CreateScope();
            var appState = scope.ServiceProvider.GetRequiredService<AppState>();
            var messageQueueService = scope.ServiceProvider.GetRequiredService<MessageQueueService>();

            var messageQueueResult = await messageQueueService.MessageQueueAsync(new CancellationToken());
            if (!messageQueueResult.IsOk)
            {
                logger.LogError("Error getting message queue: {0}", messageQueueResult.Error);
                return;
            }
            
            var messageQueue = messageQueueResult.Value;

            if (messageQueue is null) return;

            foreach (var messageQueueItem in messageQueue)
            {
                appState.Notify = messageQueueItem.NotificationType;
            }

            var messageQueueExecutedResult = await messageQueueService.AcknowledgedMessageQueueAsync(messageQueue.Select(x => x.Id).ToArray(), new CancellationToken());
            if (!messageQueueExecutedResult.IsOk)
            {
                logger.LogError("Error acknowledging message queue: {0}", messageQueueExecutedResult.Error);
                return;
            }
            
            var messageQueueExecuted = messageQueueExecutedResult.Value;
            
            if (messageQueueExecuted != messageQueue.Count)
            {
                logger.LogError("Error acknowledging message queue: {0}", messageQueueExecutedResult.Error);
                return;
            }
            
            var count = Interlocked.Increment(ref _executionCount);
            //logger.LogInformation($"{nameof(MessageQueueHostedService)} is working. Count: {count}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while performing DoWork.");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _semaphore.Dispose();
    }
}