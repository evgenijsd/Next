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
        private int _firstVisibleItemIndex;

        private double _viewItemWidth;

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

        private ICommand _tapLeftButtonCommand;
        public ICommand TapLeftButtonCommand => _tapLeftButtonCommand ??= new Command(OnTapLeftButtonCommand);

        private ICommand _tapRightButtonCommand;
        public ICommand TapRightButtonCommand => _tapRightButtonCommand ??= new Command(OnTapRightButtonCommand);

        #endregion

        #region --Private methods--

        private void OnTapLeftButtonCommand()
        {
            if (_firstVisibleItemIndex > 1)
            {
                collectionView.ScrollTo(_firstVisibleItemIndex - 2, -1, ScrollToPosition.Start, true);
            }
        }

        private void OnTapRightButtonCommand()
        {
            collectionView.ScrollTo(_firstVisibleItemIndex + 2, -1, ScrollToPosition.Start, true);
        }

        private void collectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (_viewItemWidth == 0f && sender is CollectionView collectionView)
            {
                var template = collectionView.ItemTemplate;
                var view = (View)template.CreateContent();
                _viewItemWidth = view.WidthRequest;
            }

            _firstVisibleItemIndex = (int)(e.HorizontalOffset / _viewItemWidth) * 2;
        }

        #endregion
    }
}