﻿using Prism.Navigation;

namespace Next2.ViewModels
{
    public class ReservationsViewModel : BaseViewModel
    {
        public ReservationsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "Reservations";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion
    }
}