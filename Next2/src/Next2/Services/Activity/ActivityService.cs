using System;
using System.Timers;

namespace Next2.Services.Activity
{
    public class ActivityService : IActivityService
    {
        private Timer? _timer;
        private bool _isMonitoringStarted;

        #region -- IActivityService Implementation --

        private int _userInactivityTimeLimit;
        public int UserInactivityTimeLimit
        {
            get => _userInactivityTimeLimit;
            set
            {
                _userInactivityTimeLimit = value;

                if (_isMonitoringStarted)
                {
                    ResetAndStartTimer();
                }
            }
        }

        public EventHandler UserActivityEnded { get; set; }

        public void StartMonitoringActivity()
        {
            ResetAndStartTimer();

            _isMonitoringStarted = true;
        }

        public void RefreshTimeLastActivity()
        {
            if (_isMonitoringStarted && _timer is not null)
            {
                _timer.Stop();
                _timer.Start();
            }
        }

        #endregion

        #region -- Private helpers --

        private void ReinitializeTimer()
        {
            if (_timer is not null)
            {
                _timer.Stop();
                _timer.Dispose();
            }

            if (_userInactivityTimeLimit > 0)
            {
                _timer = new Timer(_userInactivityTimeLimit * 1000);

                _timer.Elapsed += OnUserActivityEndedCallback;
            }
        }

        private void ResetAndStartTimer()
        {
            ReinitializeTimer();

            _timer?.Start();
        }

        private void OnUserActivityEndedCallback(object sender, ElapsedEventArgs e)
        {
            _timer?.Stop();

            UserActivityEnded?.Invoke(this, new());
        }

        #endregion
    }
}
