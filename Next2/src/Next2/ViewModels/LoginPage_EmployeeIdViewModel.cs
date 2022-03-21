﻿using Next2.Models;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using static Next2.Constants;

namespace Next2.ViewModels
{
    public class LoginPage_EmployeeIdViewModel : BaseViewModel
    {
        public LoginPage_EmployeeIdViewModel(INavigationService navigationService)
          : base(navigationService)
        {
        }

        #region -- Public properties--

        public string EmployeeId { get; set; }

        public string Comment { get; set; }

        public SetBindableModel? Set { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.TryGetValue("SelectedSet", out SetBindableModel set))
            {
                Set = set;
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            Set.Comment = Comment;
        }

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync()
        {
            if (!App.IsTablet)
            {
                EmployeeId = new string(EmployeeId?.Where(char.IsDigit).ToArray());

                var navigationParameters = new NavigationParameters
                {
                    { nameof(EmployeeId), EmployeeId },
                    { nameof(Navigations.REFRESH_ORDER), Constants.Navigations.REFRESH_ORDER },
                };
                await _navigationService.GoBackAsync(navigationParameters);
            }
            else
            {
                var navigationParameters = new NavigationParameters
                {
                    { nameof(Navigations.REFRESH_ORDER), Constants.Navigations.REFRESH_ORDER },
                };
                await _navigationService.GoBackAsync(navigationParameters);
            }

            Set.Comment = Comment;
        }

        #endregion
    }
}