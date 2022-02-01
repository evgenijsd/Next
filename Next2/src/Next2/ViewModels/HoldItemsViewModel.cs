﻿using Prism.Navigation;

namespace Next2.ViewModels
{
    public class HoldItemsViewModel : BaseViewModel
    {
        public HoldItemsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "HoldItems";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion
    }
}
