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

        private double viewItemWidth;

        public StepperCarousel()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<IBaseModel>),
            typeof(StepperCarousel),
            default(IEnumerable<IBaseModel>));

        public IEnumerable<IBaseModel> ItemsSource
        {
            get => (IEnumerable<IBaseModel>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(IBaseModel),
            declaringType: typeof(StepperCarousel),
            defaultBindingMode: BindingMode.TwoWay);

        public IBaseModel SelectedItem
        {
            get => (IBaseModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public ICommand TapLeftButtonCommand => new AsyncCommand(OnTapLeftButtonCommandAsync);

        public ICommand TapRightButtonCommand => new AsyncCommand(OnTapRightButtonCommandAsync);

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

        private void collectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (viewItemWidth == 0f)
            {
                if (sender is CollectionView collectionView)
                {
                    var template = collectionView.ItemTemplate;
                    var view = (View)template.CreateContent();
                    viewItemWidth = view.WidthRequest;
                }
            }

            firstVisibleItemIndex = (int)(e.HorizontalOffset / viewItemWidth) * 2;
        }

        #endregion
    }
}