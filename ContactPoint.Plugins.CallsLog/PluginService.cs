using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Core.PluginManager;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common;

namespace ContactPoint.Plugins.CallsLog
{
    [Plugin("{978D9644-8D02-4E4D-B076-EE42D86DEBD7}", "Calls log")]
    public class PluginService : Plugin
    {
        private readonly List<CallEntry> _calls = new List<CallEntry>();
        private readonly List<IPluginUIElement> _uiElements = new List<IPluginUIElement>();

        internal event Action<CallEntry> CallEntryAdded;

        internal List<CallEntry> Calls => _calls;

        private void CallManager_OnCallRemoved(ICall call, CallRemoveReason reason)
        {
            if (call.LastState == CallState.IDLE)
                return;

            if (reason != CallRemoveReason.NULL || (reason == CallRemoveReason.NULL && call.LastUserAction == CallAction.NULL))
            {
                var callEntry = new CallEntry(call, reason);
                _calls.Add(callEntry);

                CallEntryAdded?.Invoke(callEntry);
            }
        }

        #region Internal plugin routines and definitions

        private bool _isStarted;

        public override IEnumerable<IPluginUIElement> UIElements => _uiElements;
        public override bool IsStarted => _isStarted;

        public PluginService(IPluginManager pluginManager)
            : base(pluginManager)
        {
            _uiElements.Add(new FormUiElement(this));
        }

        public override void Start()
        {
            PluginManager.Core.CallManager.OnCallRemoved += CallManager_OnCallRemoved;

            _isStarted = true;
            RaiseStartedEvent();
        }

        public override void Stop()
        {
            PluginManager.Core.CallManager.OnCallRemoved -= CallManager_OnCallRemoved;

            _isStarted = false;
            RaiseStoppedEvent("Normal stop");
        }

        #endregion
    }
}
