using Next2.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class CategoryViewModel : BaseViewModel
    {
        public CategoryViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Categories = new ObservableCollection<CategoryBindableModel>()
            {
            };
        }

        #region -- Public properties --

        public ObservableCollection<CategoryBindableModel> Categories { get; set; }

        #endregion
    }
}
