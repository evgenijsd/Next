using System.Threading.Tasks;

namespace Next2.Services.Notifications
{
    public interface INotificationsService
    {
        Task ShowInfoDialogAsync(string titleText, string descriptionText, string okText);

        Task ResponseToBadRequestAsync(string statusCode);

        Task CloseAllPopupAsync();

        Task ClosePopupAsync();
    }
}
