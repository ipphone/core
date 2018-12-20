using ContactPoint.Common;
using System.Timers;
using System.Linq;

namespace ContactPoint.Services
{
    // TODO: Need to move this class into CallTools plugin when it will be available in Main branch
    internal class AutoAnswerService
    {
        private static AutoAnswerService _instance;
        private readonly Timer _timer = new Timer();
        private bool _active;
        private ICall _callToPickup;

        public ICore Core { get; }

        public int Interval
        {
            get { return (int)_timer.Interval; }
            set
            {
                _timer.Interval = value;

                Core.SettingsManager.Set("AutoAnswerInterval", value);

                if (value == 0) Stop();
                else if (_timer.Enabled)
                {
                    _timer.Stop();
                    _timer.Start();
                }
            }
        }

        public bool IsStarted
        {
            get { return _active; }
            set
            {
                if (value && !_active) Start();
                if (!value && _active) Stop();

                Core.SettingsManager.Set("AutoAnswerActive", value);
            }
        }

        public static AutoAnswerService Instance => _instance;

        public AutoAnswerService(ICore core)
        {
            _instance = this;
            Core = core;

            _timer.Interval = core.SettingsManager.GetValueOrSetDefault<int>("AutoAnswerInterval", 2000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _active = core.SettingsManager.GetValueOrSetDefault("AutoAnswerActive", false) && Interval > 0;

            core.CallManager.OnIncomingCall += new CallDelegate(CallManager_OnIncomingCall);
            core.CallManager.OnCallStateChanged += new CallDelegate(CallManager_OnCallStateChanged);
            core.CallManager.OnCallRemoved += new CallRemovedDelegate(CallManager_OnCallRemoved);

            if (_active)
            {
                Start();
            }
        }

        public static AutoAnswerService Create(ICore core) => new AutoAnswerService(core);

        public void Start()
        {
            if (Interval == 0) return;

            _active = true;
        }

        public void Stop()
        {
            _active = false;
            _timer.Stop();
        }

        void CallManager_OnIncomingCall(ICall call)
        {
            if (!_active || _callToPickup != null) return;

            if (CanPickupCall(call))
            {
                _callToPickup = call;
                _timer.Start();
            }
        }

        void CallManager_OnCallStateChanged(ICall call)
        {
            if (call != _callToPickup) return;
        }

        void CallManager_OnCallRemoved(ICall call, CallRemoveReason reason)
        {
            if (call != _callToPickup) return;

            _callToPickup = null;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            if (!_active || _callToPickup == null) return;

            if (_callToPickup.State == CallState.INCOMING)
                _callToPickup.Answer();
            else
                _callToPickup = null;
        }

        bool CanPickupCall(ICall callToPickup)
        {
            return !Core.CallManager.ToArray().Any(call => 
                call != callToPickup && (
                    call.State == CallState.ACTIVE || call.State == CallState.HOLDING || // Check if we have an active call
                    call.State == CallState.CONNECTING || call.State == CallState.ALERTING)); // Check if we have an outgoing call
        }
    }
}
