using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class PaidOrderBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public bool IsUnsavedChangesExist { get; set; }

        public CustomerModel Customer { get; set; } = new ();

        public ObservableCollection<RewardBindabledModel> Rewards { get; set; } = new ();

        public ObservableCollection<SeatWithFreeSetsBindableModel> Seats { get; set; } = new ();

        public float Subtotal { get; set; }

        public float Coupon { get; set; }

        public float Tax { get; set; }

        public float Tip { get; set; }

        public float GiftCards { get; set; }

        public float Cash { get; set; }

        public float Change { get; set; }

        public float Total { get; set;  }
    }
}
