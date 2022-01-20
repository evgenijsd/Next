using FFImageLoading.Svg.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Next2.Controls
{
    public class SvgImage : SvgCachedImage
    {
        #region -- Public properties --
​
        public static readonly BindableProperty SvgSourceProperty = BindableProperty.Create(
            propertyName: nameof(SvgSource),
            returnType: typeof(string),
            declaringType: typeof(SvgImage),
            propertyChanged: OnPropertyChanged);
​
        public string SvgSource
        {
            get => (string)GetValue(SvgSourceProperty);
            set => SetValue(SvgSourceProperty, value);
        }​

        #endregion
​
        #region -- Private helpers --
​
        private static void OnPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is not SvgImage control)
            {
                return;
            }​
            if (newvalue is string val)
            {
                control.Source = val.Contains(".svg") ? val : $"resource://TeaCRM.Resources.SvgIcons.{val}.svg";
            }
        }​

        #endregion
    }
}
