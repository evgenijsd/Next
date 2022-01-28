using Next2.Models;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    [ContentProperty("Template")]
    public class TabsControl : ContentView
    {
        private FlexLayout _flexLayout = new FlexLayout();

        private ICommand _menuItemCommand;
        private ICommand MenuItemCommand => _menuItemCommand = new Command<ISelectable>(OnTapMenuItem);

        public TabsControl()
        {
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
            typeof(IEnumerable<ISelectable>),
            typeof(TabsControl),
            default(IEnumerable<ISelectable>),
            propertyChanged: (b, o, n) =>
            ((TabsControl)b).OnItemsSourcePropertyChanged((IEnumerable)o, (IEnumerable)n));

        public IEnumerable<ISelectable> ItemsSource
        {
            get => (IEnumerable<ISelectable>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(ISelectable),
            declaringType: typeof(TabsControl),
            default(ISelectable),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, o, n) =>
            ((TabsControl)b).OnSelectedItemPropertyChanged((ISelectable)o, (ISelectable)n));

        public ISelectable SelectedItem
        {
            get => (ISelectable)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
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

        #region -- Overrides --

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

        #endregion

        #region -- Private methods --

        private void OnItemsSourcePropertyChanged(IEnumerable oldItemsSource, IEnumerable newItemsSource)
        {
            SelectedItem = null;

            _flexLayout.Children.Clear();

            CreateMenuItems();
        }

        private void OnSelectedItemPropertyChanged(ISelectable oldSelected, ISelectable newSelected)
        {
            MakeUnselectedItem();

            if (newSelected != null)
            {
                MakeSelectedItem(newSelected);
            }
        }

        private void CreateMenuItems()
        {
            if (ItemsSource != null)
            {
                foreach (ISelectable item in ItemsSource)
                {
                    if (item.IsSelected)
                    {
                        SelectedItem = item;
                    }

                    var clickableLayout = new StackLayout();

                    clickableLayout.BindingContext = item;
                    clickableLayout.Children.Add((View)Template.CreateContent());

                    clickableLayout.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = MenuItemCommand,
                        CommandParameter = item,
                    });

                    _flexLayout.Children.Add(clickableLayout);
                }
            }
        }

        private void OnTapMenuItem(ISelectable obj)
        {
            if (SelectedItem != null)
            {
                MakeUnselectedItem();
            }

            MakeSelectedItem(obj);
        }

        private void MakeSelectedItem(ISelectable obj)
        {
            foreach (View item in _flexLayout.Children)
            {
                if (item.BindingContext is ISelectable itemMenu && itemMenu == obj)
                {
                    itemMenu.IsSelected = true;
                    SelectedItem = itemMenu;

                    break;
                }
            }
        }

        private void MakeUnselectedItem()
        {
            foreach (View item in _flexLayout.Children)
            {
                if (item.BindingContext is ISelectable itemMenu && itemMenu.IsSelected)
                {
                    itemMenu.IsSelected = false;
                    break;
                }
            }
        }

        #endregion
    }
}