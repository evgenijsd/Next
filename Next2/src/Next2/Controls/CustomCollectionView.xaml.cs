using Next2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CustomCollectionView : StackLayout
    {
        private readonly double _heightBonus = App.IsTablet ? Constants.LayoutBonuses.ROW_TABLET_BONUS : Constants.LayoutBonuses.ROW_MOBILE_BONUS;

        public CustomCollectionView()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty BonusesProperty = BindableProperty.Create(
            propertyName: nameof(Bonuses),
            returnType: typeof(ObservableCollection<BonusBindableModel>),
            declaringType: typeof(CustomCollectionView));

        public ObservableCollection<BonusBindableModel> Bonuses
        {
            get => (ObservableCollection<BonusBindableModel>)GetValue(BonusesProperty);
            set => SetValue(BonusesProperty, value);
        }

        public static readonly BindableProperty SelectedBonusProperty = BindableProperty.Create(
            propertyName: nameof(SelectedBonus),
            returnType: typeof(BonusBindableModel),
            declaringType: typeof(CustomCollectionView));

        public BonusBindableModel SelectedBonus
        {
            get => (BonusBindableModel)GetValue(SelectedBonusProperty);
            set => SetValue(SelectedBonusProperty, value);
        }

        public static readonly BindableProperty HeightBonusProperty = BindableProperty.Create(
            propertyName: nameof(HeightBonus),
            returnType: typeof(double),
            defaultValue: 50.0d,
            declaringType: typeof(CustomCollectionView));

        public double HeightBonus
        {
            get => (double)GetValue(HeightBonusProperty);
            set => SetValue(HeightBonusProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CustomCollectionView));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TapSelectCommandProperty = BindableProperty.Create(
            propertyName: nameof(TapSelectCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand TapSelectCommand
        {
            get => (ICommand)GetValue(TapSelectCommandProperty);
            set => SetValue(TapSelectCommandProperty, value);
        }

        public static readonly BindableProperty TapSelectCommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(TapSelectCommandParameter),
            returnType: typeof(BonusBindableModel),
            declaringType: typeof(CustomCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public object TapSelectCommandParameter
        {
            get => (BonusBindableModel)GetValue(TapSelectCommandParameterProperty);
            set => SetValue(TapSelectCommandParameterProperty, value);
        }

        public static readonly BindableProperty ImagePathProperty = BindableProperty.Create(
            propertyName: nameof(ImagePath),
            returnType: typeof(string),
            defaultValue: "ic_check_box_unchecked_white",
            declaringType: typeof(CustomCollectionView));

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public static readonly BindableProperty BorderBonusColorProperty = BindableProperty.Create(
            propertyName: nameof(BorderBonusColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public Color BorderBonusColor
        {
            get => (Color)GetValue(BorderBonusColorProperty);
            set => SetValue(BorderBonusColorProperty, value);
        }

        public static readonly BindableProperty BackColorProperty = BindableProperty.Create(
            propertyName: nameof(BackColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public Color BackColor
        {
            get => (Color)GetValue(BackColorProperty);
            set => SetValue(BackColorProperty, value);
        }

        public double HeightBonusLine = App.IsTablet ? Constants.LayoutBonuses.ROW_TABLET_BONUS : Constants.LayoutBonuses.ROW_MOBILE_BONUS;

        #endregion
    }
}