using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Services.Services
{
    public interface ISettingsManager
    {
        int UserId { get; set; }
        string UserName { get; set; }
    }
}
