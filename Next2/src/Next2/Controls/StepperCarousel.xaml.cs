using Next2.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class StepperCarousel : StackLayout
    {
        private int firstVisibleItemIndex;

        public StepperCarousel()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<IEntityModel>),
            typeof(StepperCarousel),
            default(IEnumerable<IEntityModel>));

        public IEnumerable<IEntityModel> ItemsSource
        {
            get => (IEnumerable<IEntityModel>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(IEntityModel),
            declaringType: typeof(StepperCarousel),
            defaultBindingMode: BindingMode.TwoWay);

        public IEntityModel SelectedItem
        {
            get => (IEntityModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public ICommand TapLeftButtonCommand => new AsyncCommand(OnTapLeftButtonCommandAsync);

        public ICommand TapRightButtonCommand => new AsyncCommand(OnTapRightButtonCommandAsync);

        public ICommand ScrolledCommand => new AsyncCommand<int>(OnScrolledCommandAsync);

        #endregion

        #region --Private methods--

        private Task OnTapLeftButtonCommandAsync()
        {
            if (firstVisibleItemIndex > 1)
            {
                collectionView.ScrollTo(firstVisibleItemIndex - 2, -1, ScrollToPosition.Start, true);
            }

            return Task.CompletedTask;
        }

        private Task OnTapRightButtonCommandAsync()
        {
            collectionView.ScrollTo(firstVisibleItemIndex + 2, -1, ScrollToPosition.Start, true);
            return Task.CompletedTask;
        }

        private Task OnScrolledCommandAsync(int firstVisibleItemIdx)
        {
            firstVisibleItemIndex = firstVisibleItemIdx;
            return Task.CompletedTask;
        }

        #endregion
    }
}