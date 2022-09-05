using System;

namespace Next2.Services.Activity
{
    public interface IActivityService
    {
        EventHandler UserActivityEnded { get; set; }

        void RefreshTimeLastActivity();
    }
}
