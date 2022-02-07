using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class OrderBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public EOrderType Ordertype { get; set; }

        public float Total { get; set; }
    }
}
