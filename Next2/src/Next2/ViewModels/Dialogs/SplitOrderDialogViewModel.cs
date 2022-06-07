using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
     public class SplitOrderDialogViewModel : BindableBase
    {
        private decimal _maxValue = 100;

        public SplitOrderDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            AcceptCommand = new Command(() => RequestClose(null));
            DeclineCommand = new Command(() => RequestClose(null));

            LoadData(param);
        }

        #region -- Public Properties --

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; }

        public FullOrderBindableModel Order { get; set; }

        public decimal SplitValue { get; set; }

        public decimal SplitTotal { get; set; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _selectValueCommand;
        public ICommand SelectValueCommand => _selectValueCommand ??= new AsyncCommand<object>(OnSelectValueCommandAsync, allowsMultipleExecutions: false);

        private ICommand _incrementCommand;
        public ICommand IncrementCommand => _incrementCommand ??= new AsyncCommand(OnIncrementCommandAsync, allowsMultipleExecutions: false);

        private ICommand _decrementCommand;
        public ICommand DecrementCommand => _decrementCommand ??= new AsyncCommand(OnDecrementCommandAsync, allowsMultipleExecutions: false);

        public ICommand DeclineCommand { get; }

        public ICommand AcceptCommand { get; }

        #endregion

        #region -- Private Helpers --

        private void LoadData(IDialogParameters param)
        {
            var isAllParamExist = param.TryGetValue(Constants.DialogParameterKeys.MODEL, out FullOrderBindableModel order)
                & param.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish);

            if (isAllParamExist)
            {
                Order = order;
                SelectedDish = selectedDish;
                var seats = Order.Seats.Where(x => x.SelectedDishes.All(x => x.Id != SelectedDish.Id));

                foreach (var seat in seats)
                {
                    seat.SeatSelectionCommand = new AsyncCommand(OnSeatSelectionCommandAsync, allowsMultipleExecutions: false);
                    seat.SelectedItem = SelectedDish;
                    seat.SelectedItem.TotalPrice = 0;
                }

                Seats = new ObservableCollection<SeatBindableModel>(seats);
            }
        }

        private Task OnSeatSelectionCommandAsync()
        {
            throw new NotImplementedException();
        }

        private Task OnSelectValueCommandAsync(object? arg)
        {
            decimal.TryParse(arg as string, out decimal value);
            SplitValue = value;
            return Task.CompletedTask;
        }

        private Task OnIncrementCommandAsync()
        {
            if (SplitValue < _maxValue)
            {
                SplitValue++;
            }

            return Task.CompletedTask;
        }

        private Task OnDecrementCommandAsync()
        {
            if (SplitValue > 0)
            {
                SplitValue--;
            }

            return Task.CompletedTask;
        }

        #endregion

    }
}
