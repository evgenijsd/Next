using AutoMapper;
using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class AddSetToOrderDialogViewModel : BindableBase
    {
        public AddSetToOrderDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(null));
            TapAddCommand = new Command(() =>
            {
                Set.Portion = SelectedPortion;

                RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.SET, Set } });
            });

            if (param.ContainsKey(Constants.DialogParameterKeys.SET) && param.ContainsKey(Constants.DialogParameterKeys.PORTIONS))
            {
                SetModel set;
                IEnumerable<PortionModel> portions;

                if (param.TryGetValue(Constants.DialogParameterKeys.SET, out set) && param.TryGetValue(Constants.DialogParameterKeys.PORTIONS, out portions))
                {
                    MapperConfiguration config = new MapperConfiguration(cfg => cfg.CreateMap<SetModel, SetBindableModel>());
                    var mapper = new Mapper(config);

                    Set = mapper.Map<SetModel, SetBindableModel>(set);
                    Portions = portions;
                    SelectedPortion = Portions.FirstOrDefault();
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