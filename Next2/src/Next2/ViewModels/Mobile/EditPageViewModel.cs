using Prism.Navigation;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Next2.Models;

namespace Next2.ViewModels.Mobile
{
    public class EditPageViewModel : BaseViewModel
    {
        private SetBindableModel _selectedDish;

        public EditPageViewModel(INavigationService navigationService)
          : base(navigationService)
        {
        }

        #region -- Public properties --

        public SetBindableModel SelectedDish { get; set; }

        private ICommand _openModifyCommand;
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync);

        private ICommand _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync);

        private ICommand _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            SelectedDish = parameters.TryGetValue("SelectedDish", out _selectedDish) ? _selectedDish : null;
        }

        #endregion

        #region -- Private helpers --

        private Task OnOpenHoldSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnOpenModifyCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnOpenRemoveCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
