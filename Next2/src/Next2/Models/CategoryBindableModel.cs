using Prism.Mvvm;

namespace Next2.Models
{
    public class CategoryBindableModel : BindableBase, ISelectable
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsSelected { get; set; }
    }
}
