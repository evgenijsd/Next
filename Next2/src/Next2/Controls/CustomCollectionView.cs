using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Next2.Controls
{
    public class CustomCollectionView : CollectionView
    {
        #region -- Public Properties --

        public static readonly BindableProperty IndexLastVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IndexLastVisible),
            returnType: typeof(double),
            declaringType: typeof(CustomCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public double IndexLastVisible
        {
            get => (double)GetValue(IndexLastVisibleProperty);
            set => SetValue(IndexLastVisibleProperty, value);
        }

        #endregion
    }
}
