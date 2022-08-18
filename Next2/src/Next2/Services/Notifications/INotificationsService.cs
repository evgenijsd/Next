using System.Threading.Tasks;

namespace Next2.Services.Notifications
{
    public interface INotificationsService
    {
        Task ResponseToBadRequestAsync(string? statusCode);

        Task ShowInfoDialogAsync(string titleText, string descriptionText, string okText);

        Task ShowNoInternetConnectionDialogAsync();

        Task ShowSomethingWentWrongDialogAsync();

        Task CloseAllPopupAsync();

        Task ClosePopupAsync();
    }
}
