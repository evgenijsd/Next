﻿using Next2.Services.Authentication;
using Next2.Views.Tablet;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;

namespace Next2.Services.Messages
{
    public class MessagesService : IMessagesService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPopupNavigation _popupNavigation;
        private readonly INavigationService _navigationService;

        public MessagesService(
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            IPopupNavigation popupNavigation)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _popupNavigation = popupNavigation;
        }

        #region -- IMessagesService implementation --

        public async Task ResponseToBadRequestAsync(string statusCode)
        {
            if (statusCode == Constants.StatusCode.UNAUTHORIZED)
            {
                _authenticationService.ClearSession();

                var navigationParameters = new NavigationParameters
                {
                    { Constants.Navigations.LOGOUT, true },
                };

                await _navigationService.NavigateAsync($"{nameof(LoginPage)}", navigationParameters);
            }
            else if (statusCode == Constants.StatusCode.SOCKET_CLOSED)
            {
                await ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["RequestTimedOut"],
                    LocalizationResourceManager.Current["Ok"]);
            }
            else
            {
                await ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["SomethingWentWrong"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        public Task ShowInfoDialogAsync(string titleText, string descriptionText, string okText)
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, titleText },
                { Constants.DialogParameterKeys.DESCRIPTION,  descriptionText },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, okText },
            };

            PopupPage infoDialog = App.IsTablet
                ? new Views.Tablet.Dialogs.InfoDialog(parameters, () => _popupNavigation.PopAsync())
                : new Views.Mobile.Dialogs.InfoDialog(parameters, () => _popupNavigation.PopAsync());

            return _popupNavigation.PushAsync(infoDialog);
        }

        #endregion
    }
}
