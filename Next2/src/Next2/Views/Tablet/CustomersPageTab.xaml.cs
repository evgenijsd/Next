using Next2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Views.Tablet
{
    public partial class CustomersPageTab : BaseContentPage
    {
        public CustomersPageTab()
        {
            InitializeComponent();
        }

        private void ItemTappedHandler(object sender, EventArgs args)
        {
            selectButton.Opacity = 1;
            infoButton.Opacity = 1;
        }

        private int _oldId = -1;
        private void TappedHandler(object sender, EventArgs args)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var image = grid.Children[0] as Image;
                var label = grid.Children[6] as Label;
                var id = int.Parse(label?.Text);
                if (_oldId == -1)
                {
                    _oldId = id;
                    image.Source = "ic_check_box_checked_primary_24x24";
                }
                else if (_oldId == id)
                {
                    image.Source = "ic_check_box_unhecked_24x24";
                    _oldId = -1;
                }
                else if (_oldId != id)
                {
                    var v = collectionView.ItemsSource as IEnumerable<CustomersViewModel>;
                    var b = v.Where(x => x.Id == _oldId).FirstOrDefault();
                    if (b != null)
                    {
                        b.CheckboxImage = "ic_check_box_unhecked_24x24";
                    }

                    image.Source = "ic_check_box_checked_primary_24x24";
                    _oldId = id;
                }
            }
        }
    }
}