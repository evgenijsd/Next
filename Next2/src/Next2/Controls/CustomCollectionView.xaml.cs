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
    public partial class CustomCollectionView : Grid
    {
        private readonly double _heightBonus = App.IsTablet ? Constants.LayoutBonuses.ROW_TABLET_BONUS : Constants.LayoutBonuses.ROW_MOBILE_BONUS;

        public CustomCollectionView()
        {
            InitializeComponent();

            HeightBonusAll = _heightBonus * 2;

            HeightAll = HeightBonusAll + 30;
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

        public double HeightBonusAll { get; set; }

        public double HeightAll { get; set; }

        #endregion
    }
}