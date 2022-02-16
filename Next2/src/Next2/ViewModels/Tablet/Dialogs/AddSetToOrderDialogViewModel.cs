using Next2.Models;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class AddSetToOrderDialogViewModel
    {
        public AddSetToOrderDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(null));

            if (param.ContainsKey(Constants.DialogParameterKeys.SET) && param.ContainsKey(Constants.DialogParameterKeys.PORTIONS))
            {
                SetModel set;
                IEnumerable<PortionModel> portions;

                if (param.TryGetValue(Constants.DialogParameterKeys.SET, out set) && param.TryGetValue(Constants.DialogParameterKeys.PORTIONS, out portions))
                {
                    Set = set;
                    Portions = portions;
                    SelectedPortion = Portions.FirstOrDefault();
                }
            }
        }

        #region --Public Properties--

        public SetModel Set { get; }

        public IEnumerable<PortionModel> Portions { get; }

        public PortionModel SelectedPortion { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        private ICommand _tapAddCommand;
        public ICommand TapAddCommand => _tapAddCommand ??= new AsyncCommand(OnTapCommandAsync);

        #endregion

        #region --Private methods--

        private async Task OnTapCommandAsync()
        {
        }

        #endregion
    }
}