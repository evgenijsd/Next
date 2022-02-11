using Next2.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class DropDownList : Frame
    {
        public DropDownList()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
            propertyName: nameof(IconSource),
            returnType: typeof(string),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public string IconSource
        {
            get => (string)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            returnType: typeof(IList),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(object),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public IEnumerable<DropDownPickerItem> ItemList { get; set; } = new ObservableCollection<DropDownPickerItem>();

        public bool IsExpanded { get; set; }

        private ICommand _expandListCommand;
        public ICommand ExpandListCommand => _expandListCommand ??= new AsyncCommand(OnExpandListCommandAsync);

        private ICommand _selectItemCommand;
        public ICommand SelectItemCommand => _selectItemCommand ??= new AsyncCommand(OnSelectItemCommandAsync);

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(ItemsSource) && ItemsSource?.Count > 0)
            {
                SelectedItem = ItemsSource[0];
            }
        }

        #region -- Private helpers --

        private Task OnExpandListCommandAsync()
        {
            IsExpanded = !IsExpanded;

            return Task.CompletedTask;
        }

        private Task OnSelectItemCommandAsync()
        {
            IsExpanded = false;

            return Task.CompletedTask;
        }

        #endregion
    }
}