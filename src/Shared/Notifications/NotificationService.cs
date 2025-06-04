using System;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace Shared.Notifications;

public class NotificationService
{
    public event Action<NotificationModel> OnShowNotification = null!;

    public void Info(string msg, int closeAfter = 5000)
    {
        OnShowNotification?.Invoke(new NotificationModel
        {
            Text = msg,
            ThemeColor = ThemeConstants.Notification.ThemeColor.Info,
            Icon = "info-circle",
            CloseAfter = closeAfter
        });
    }

    public void Success(string msg, int closeAfter = 5000)
    {
        OnShowNotification?.Invoke(new NotificationModel
        {
            Text = msg,
            ThemeColor = ThemeConstants.Notification.ThemeColor.Success,
            Icon = "check-outline",
            CloseAfter = closeAfter
        });
    }

    public void Warning(string msg, int closeAfter = 5000)
    {
        OnShowNotification?.Invoke(new NotificationModel
        {
            Text = msg,
            ThemeColor = ThemeConstants.Notification.ThemeColor.Warning,
            Icon = "exclamation-circle",
            CloseAfter = closeAfter
        });
    }

    public void Error(string msg, int closeAfter = 5000)
    {
        OnShowNotification?.Invoke(new NotificationModel
        {
            Text = msg,
            ThemeColor = ThemeConstants.Notification.ThemeColor.Error,
            Icon = "x-outline",
            CloseAfter = closeAfter
        });
    }
}
