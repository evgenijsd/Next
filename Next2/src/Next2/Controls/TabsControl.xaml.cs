using Next2.Models;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    [ContentProperty("Template")]
    public partial class TabsControl : ContentView
    {
        private FlexLayout _flexLayout;

        private ICommand _selectedCommand;
        private ICommand SelectedCommand => _selectedCommand = new Command<IItemMenu>(OnSelected);

        public TabsControl()
        {
            InitializeComponent();

            _flexLayout = new FlexLayout();

            Content = _flexLayout;
        }

        #region -- Public properties --

        public static readonly BindableProperty TemplateProperty = BindableProperty.Create(
            propertyName: nameof(Template),
            returnType: typeof(DataTemplate),
            declaringType: typeof(TabsControl));

        public DataTemplate Template
        {
            get => (DataTemplate)GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<IItemMenu>),
            typeof(TabsControl),
            default(IEnumerable<IItemMenu>),
            propertyChanged: (b, o, n) =>
            ((TabsControl)b).OnItemsSourcePropertyChanged((IEnumerable)o, (IEnumerable)n));

        public IEnumerable<IItemMenu> ItemsSource
        {
            get => (IEnumerable<IItemMenu>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedProperty = BindableProperty.Create(
            propertyName: nameof(Selected),
            returnType: typeof(IItemMenu),
            declaringType: typeof(TabsControl),
            default(IItemMenu),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, o, n) =>
            ((TabsControl)b).OnSelectedPropertyChanged((IItemMenu)o, (IItemMenu)n));

        public IItemMenu Selected
        {
            get => (IItemMenu)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }

        public static readonly BindableProperty DirectionProperty = BindableProperty.Create(
            propertyName: nameof(Direction),
            returnType: typeof(FlexDirection),
            declaringType: typeof(TabsControl),
            default(FlexDirection),
            defaultBindingMode: BindingMode.TwoWay);

        public FlexDirection Direction
        {
            get => (FlexDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public static readonly BindableProperty WrapProperty = BindableProperty.Create(
            propertyName: nameof(Wrap),
            returnType: typeof(FlexWrap),
            declaringType: typeof(TabsControl),
            default(FlexWrap),
            defaultBindingMode: BindingMode.TwoWay);

        public FlexWrap Wrap
        {
            get => (FlexWrap)GetValue(WrapProperty);
            set => SetValue(WrapProperty, value);
        }

        public static readonly BindableProperty JustifyContentProperty = BindableProperty.Create(
            propertyName: nameof(JustifyContent),
            returnType: typeof(FlexJustify),
            declaringType: typeof(TabsControl),
            default(FlexJustify),
            defaultBindingMode: BindingMode.TwoWay);

        public FlexJustify JustifyContent
        {
            get => (FlexJustify)GetValue(JustifyContentProperty);
            set => SetValue(JustifyContentProperty, value);
        }

        public static readonly BindableProperty AlignContentProperty = BindableProperty.Create(
            propertyName: nameof(AlignContent),
            returnType: typeof(FlexAlignContent),
            declaringType: typeof(TabsControl),
            default(FlexAlignContent),
            defaultBindingMode: BindingMode.TwoWay);

        public FlexAlignContent AlignContent
        {
            get => (FlexAlignContent)GetValue(AlignContentProperty);
            set => SetValue(AlignContentProperty, value);
        }

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Direction):
                    _flexLayout.Direction = Direction;
                    break;
                case nameof(Wrap):
                    _flexLayout.Wrap = Wrap;
                    break;
                case nameof(JustifyContent):
                    _flexLayout.JustifyContent = FlexJustify.Start;
                    break;
            }
        }

        #region -- Private methods --

        private void OnItemsSourcePropertyChanged(IEnumerable oldItemsSource, IEnumerable newItemsSource)
        {
            Selected = null;

            _flexLayout.Children.Clear();

            CreateItems(ItemsSource);
        }

        private void OnSelectedPropertyChanged(IItemMenu oldSelected, IItemMenu newSelected)
        {
            MakeUnselected();

            if (newSelected != null)
            {
                MakeSelected(newSelected);
            }
        }

        private void CreateItems(IEnumerable items)
        {
            if (items != null)
            {
                foreach (IItemMenu item in items)
                {
                    if (item.IsSelected)
                    {
                        Selected = item;
                    }

                    var clickableLayout = new StackLayout();

                    clickableLayout.BindingContext = item;
                    clickableLayout.Children.Add((View)Template.CreateContent());

                    clickableLayout.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = SelectedCommand,
                        CommandParameter = item,
                    });

                    _flexLayout.Children.Add(clickableLayout);
                }
            }
        }

        private void OnSelected(IItemMenu obj)
        {
            if (Selected != null)
            {
                MakeUnselected();
            }

            MakeSelected(obj);
        }

        private void MakeSelected(IItemMenu obj)
        {
            foreach (View item in _flexLayout.Children)
            {
                if (item.BindingContext is IItemMenu itemMenu && itemMenu == obj)
                {
                    itemMenu.IsSelected = true;
                    Selected = itemMenu;

                    break;
                }
            }
        }

        private void MakeUnselected()
        {
            foreach (View item in _flexLayout.Children)
            {
                if (item.BindingContext is IItemMenu itemMenu && itemMenu.IsSelected)
                {
                    itemMenu.IsSelected = false;
                    break;
                }
            }
        }

        #endregion
    }
}