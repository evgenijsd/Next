using AutoMapper;
using Next2.Models.API;
using Next2.Models.API.DTO;
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
                    Dish.DishProportion = SelectedPortion;
                    Dish.Products = new();

                    RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.DISH, Dish } });
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

            if (param.ContainsKey(Constants.DialogParameterKeys.DISH) && param.ContainsKey(Constants.DialogParameterKeys.PORTIONS))
            {
                if (param.TryGetValue(Constants.DialogParameterKeys.DISH, out DishModelDTO dish) && param.TryGetValue(Constants.DialogParameterKeys.PORTIONS, out IEnumerable<SimpleDishProportionModelDTO> portions))
                {
                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DishModelDTO, Models.DishBindableModel>()).CreateMapper();

                    Dish = mapper.Map<DishModelDTO, Models.DishBindableModel>(dish);

                    Portions = portions.Select(row => new PortionModel()
                    {
                        Id = row.Id,
                        Price = row.PriceRatio == 1 ? Dish.OriginalPrice : Dish.OriginalPrice * (1.0 + row.PriceRatio),
                        Title = row.ProportionName,
                    });

                    SelectedPortion = Portions.FirstOrDefault();
                }
            }
        }

        #region --Public Properties--

        public Models.DishBindableModel Dish { get; }

        public IEnumerable<PortionModel> Portions { get; }

        public PortionModel SelectedPortion { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand TapAddCommand { get; }

        #endregion
    }
}