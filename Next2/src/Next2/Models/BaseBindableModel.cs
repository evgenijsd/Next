using System.ComponentModel;

namespace Next2.Models
{
    public interface IBaseBindableModel :
        IEntityModelBase,
        INotifyPropertyChanged
    {
    }
}
