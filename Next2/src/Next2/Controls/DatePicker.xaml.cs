using Next2.Views.Mobile.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class DatePicker : Frame
    {
        private readonly IPopupNavigation _popupNavigation;
        public DatePicker()
        {
            InitializeComponent();
            _popupNavigation = App.Resolve<IPopupNavigation>();
        }

        #region -- Public properties --

        public static readonly BindableProperty DateLabelProperty = BindableProperty.Create(
            propertyName: nameof(DateLabel),
            returnType: typeof(DateTime),
            defaultValue: DateTime.Now,
            declaringType: typeof(DatePicker));

        public DateTime DateLabel
        {
            get => (DateTime)GetValue(DateLabelProperty);
            set => SetValue(DateLabelProperty, value);
        }

        private ICommand _tapCommand;
        public ICommand TapCommand => _tapCommand ??= new AsyncCommand(OnTapCommandAsync);

        #endregion

        #region -- Private --

        private Task OnTapCommandAsync()
        {
            var param = new DialogParameters()
            {
                { " ", " " },
            };

            var popupPage = new SelectDateDialog(param, CloseDialogCallBack);
            return _popupNavigation.PushAsync(popupPage);
        }

        private async void CloseDialogCallBack(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.SELECTED_DATE, out DateTime selectedDate))
            {
                DateLabel = selectedDate;
            }

            await _popupNavigation.PopAllAsync();
        }

        #endregion

    }
}