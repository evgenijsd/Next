using System;
using System.Timers;

namespace Next2.Services.Activity
{
    public class ActivityService : IActivityService
    {
        private Timer _timer;

        public ActivityService()
        {
            _timer = new Timer(Constants.Limits.USER_ACTIVITY_TIME);

            _timer.Elapsed += OnUserActivityEndedCallback;

            _timer.Start();
        }

        #region -- IActivityService Implementation --

        public EventHandler UserActivityEnded { get; set; }

        public void RefreshTimeLastActivity()
        {
            _timer.Stop();
            _timer.Start();
        }

        #endregion

        #region -- Private helpers --

        private void OnUserActivityEndedCallback(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            UserActivityEnded?.Invoke(this, new());
        }

        #endregion
    }
}
