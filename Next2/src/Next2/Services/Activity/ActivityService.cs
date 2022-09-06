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
                    _isMonitoringStarted = ResetAndStartTimer();
                }
            }
        }

        public EventHandler UserActivityEnded { get; set; }

        public void StartMonitoringActivity()
        {
            _isMonitoringStarted = ResetAndStartTimer();
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

        private bool ReinitializeTimer()
        {
            var isSuccessReinitializeTimer = false;

            if (_timer is not null)
            {
                _timer.Stop();

                _timer = null;
            }

            if (_userInactivityTimeLimit > 0)
            {
                _timer = new Timer(_userInactivityTimeLimit * 1000);

                _timer.Elapsed += OnUserActivityEndedCallback;

                isSuccessReinitializeTimer = true;
            }

            return isSuccessReinitializeTimer;
        }

        private bool ResetAndStartTimer()
        {
            var isSuccessReinitializeTimer = ReinitializeTimer();

            if (isSuccessReinitializeTimer)
            {
                _timer?.Start();
            }

            return isSuccessReinitializeTimer;
        }

        private void OnUserActivityEndedCallback(object sender, ElapsedEventArgs e)
        {
            _timer?.Stop();

            UserActivityEnded?.Invoke(this, new());
        }

        #endregion
    }
}
