using AutoMapper;
using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class AddSetToOrderDialogViewModel : BindableBase
    {
        private bool _canExecute = true;

        public AddSetToOrderDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(null));
            TapAddCommand = new Command(
                execute: () =>
                {
                    Set.Portion = SelectedPortion;
                    Set.Portions = new(Portions);

                    RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.SET, Set } });
                },
                canExecute: () =>
                {
                    bool result = false;

                    if (_canExecute)
                    {
                        _canExecute = false;
                        result = true;
                    }

                    return result;
                });

            if (param.ContainsKey(Constants.DialogParameterKeys.SET) && param.ContainsKey(Constants.DialogParameterKeys.PORTIONS))
            {
                if (param.TryGetValue(Constants.DialogParameterKeys.SET, out SetModel set) && param.TryGetValue(Constants.DialogParameterKeys.PORTIONS, out IEnumerable<PortionModel> portions))
                {
                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SetModel, SetBindableModel>()).CreateMapper();

                    Set = mapper.Map<SetModel, SetBindableModel>(set);
                    Portions = portions;
                    SelectedPortion = Portions.FirstOrDefault(row => row.Id == set.DefaultPortionId);
                }
            }
        }

        #region --Public Properties--

        public SetBindableModel Set { get; }

        public IEnumerable<PortionModel> Portions { get; }

        public PortionModel SelectedPortion { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand TapAddCommand { get; }

        #endregion
    }
}