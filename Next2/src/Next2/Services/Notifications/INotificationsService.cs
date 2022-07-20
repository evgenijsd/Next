using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Notifications
{
    public interface INotificationsService
    {
        Task ShowInfoDialogAsync(string titleText, string descriptionText, string okText);

        Task ResponseToBadRequestAsync(string statusCode);

        Task CloseAllPopupAsync();
    }
}
