using Microsoft.AspNetCore.Components;
using Shared.Notifications;
using Telerik.Blazor.Components;

namespace AppClient.Components;

public partial class NotificationContainer
{
    #region Services

    [Inject] public NotificationService NotificationService { get; set; } = null!;

    #endregion Services

    protected override void OnInitialized()
    {
        NotificationService.OnShowNotification += (notificationModel) =>
        {
            InvokeAsync(() =>
            {
                if (_notification is not null) _notification.Show(notificationModel);
            });
        };
    }

    private MarkupString RawText(string text)
    {
        return new MarkupString(text);
    }

    #region Properties

    private TelerikNotification _notification { get; set; } = null!;

    #endregion Properties
}