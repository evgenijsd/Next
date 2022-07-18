using Next2.Models;
using Next2.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.HoldItem
{
    public interface IHoldItemService
    {
        Task<ObservableCollection<HoldItemBindableModel>> GetHoldItems();
    }
}
