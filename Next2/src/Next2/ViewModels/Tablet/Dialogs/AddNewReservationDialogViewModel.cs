﻿using Next2.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using static System.Net.Mime.MediaTypeNames;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class AddNewReservationDialogViewModel : BindableBase
    {
        public AddNewReservationDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters()));

            GenerateCollection(GuestsAmount, 25);
            GenerateCollection(Tables, 10);

            var date = DateTime.Now;

            Hour = date.ToString("hh");
            Minute = date.ToString("mm");
            TimeType = date.ToString("tt");

            SelectedDate = date;
        }

        #region -- Public properties --

        public string Name { get; set; }

        public string Phone { get; set; }

        public int SelectedAmountGuests { get; set; }

        public ObservableCollection<int> GuestsAmount { get; set; } = new();

        public int SelectedTable { get; set; }

        public ObservableCollection<int> Tables { get; set; } = new();

        public string Notes { get; set; } = string.Empty;

        public DateTime SelectedTime { get; set; } = DateTime.Now;

        public DateTime SelectedDate { get; set; }

        public DateTime MinimumSelectableDate { get; set; } = DateTime.Now;

        public string Hour { get; set; }

        public string Minute { get; set; }

        public string TimeType { get; set; } = string.Empty;

        public bool IsValidName { get; set; }

        public bool IsValidPhone { get; set; }

        public bool CanAddNewReservation { get; set; }

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand DeclineCommand { get; }

        private ICommand _changeTimeFormatCommand;
        public ICommand ChangeTimeFormatCommand => _changeTimeFormatCommand ??= new AsyncCommand<string>(OnChangeTimeFormatCommandAsync, allowsMultipleExecutions: false);

        private ICommand _goInputNotesCommand;
        public ICommand GoInputNotesCommand => _goInputNotesCommand ??= new AsyncCommand(OnGoInputNotesCommandAsync, allowsMultipleExecutions: false);

        private ICommand _acceptCommand;
        public ICommand AcceptCommand => _acceptCommand ??= new AsyncCommand(OnAcceptCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName
                is nameof(IsValidName)
                or nameof(IsValidPhone)
                or nameof(SelectedAmountGuests))
            {
                ChangeCanAddNewReservation();
            }
            else if (args.PropertyName
                is nameof(SelectedDate)
                or nameof(Hour)
                or nameof(Minute)
                or nameof(TimeType))
            {
                ChangeSelectedTime();
                ChangeCanAddNewReservation();
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnAcceptCommandAsync()
        {
            ChangeCanAddNewReservation();

            var param = new DialogParameters();

            if (CanAddNewReservation)
            {
                try
                {
                    Regex regexPhone = new(Constants.Validators.PHONE_MASK_REPLACE);

                    var phone = regexPhone.Replace(Phone, string.Empty);

                    var newReservation = new ReservationModel()
                    {
                        CustomerName = Name,
                        Phone = phone,
                        GuestsAmount = SelectedAmountGuests,
                        TableNumber = SelectedTable,
                        Comment = Notes,
                        DateTime = SelectedTime,
                    };

                    param.Add(Constants.DialogParameterKeys.ACCEPT, newReservation);
                }
                catch (Exception)
                {
                }
            }

            RequestClose(param);

            return Task.CompletedTask;
        }

        private Task OnGoInputNotesCommandAsync()
        {
            var param = new DialogParameters()
            {
                { Constants.Navigations.INPUT_VALUE, Notes },
                { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["CommentForReservation"] },
            };

            var popupPage = new Views.Tablet.Dialogs.InputDialog(param, AddNewReservationDialogCallBack);

            return PopupNavigation.PushAsync(popupPage);
        }

        private void AddNewReservationDialogCallBack(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.Navigations.INPUT_VALUE, out string text))
            {
                Notes = text;
            }

            if (PopupNavigation.PopupStack.Count > 0)
            {
                PopupNavigation.PopAsync();
            }
        }

        private Task OnChangeTimeFormatCommandAsync(string state)
        {
            TimeType = state;

            return Task.CompletedTask;
        }

        private void GenerateCollection(ObservableCollection<int> collection, int count)
        {
            for (int i = 1; i < count; i++)
            {
                collection.Add(i);
            }
        }

        private void ChangeSelectedTime()
        {
            var date = SelectedDate.ToString("MM/dd/yyyy");

            var dateTime = $"{date} {Hour}:{Minute} {TimeType}";

            if (DateTime.TryParse(dateTime, out DateTime result))
            {
                SelectedTime = result;
            }
        }

        private void ChangeCanAddNewReservation()
        {
            CanAddNewReservation = IsValidName && IsValidPhone && SelectedAmountGuests > 0 && SelectedTime > DateTime.Now;
        }

        #endregion
    }
}
