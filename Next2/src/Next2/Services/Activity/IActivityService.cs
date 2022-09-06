using System;

namespace Next2.Services.Activity
{
    public interface IActivityService
    {
        int UserInactivityTimeLimit { get; set; }

        EventHandler UserActivityEnded { get; set; }

        void StartMonitoringActivity();

        void RefreshTimeLastActivity();
    }
}
