using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class TableBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int TableNumber { get; set; }
    }
}
