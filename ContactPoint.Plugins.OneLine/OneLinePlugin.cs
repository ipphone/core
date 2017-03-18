using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.BaseDesign;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.OneLine
{
    [Plugin("{C304B638-F96C-4e6b-8D8B-BB132D55C4AE}", "One line")]
    public class OneLinePlugin : Plugin
    {
        private bool _isOneLine = false;
        private bool _isStarted = false;
        private List<IPluginUIElement> _uiElements;

        public override List<IPluginUIElement> UIElements
        {
            get { return this._uiElements; }
        }

        public bool IsOneLine
        {
            get { return this._isOneLine; }
            set { this._isOneLine = value; }
        }

        public override void Start()
        {
            this.PluginManager.Core.CallManager.OnCallStateChanged += new CallDelegate(CallManager_OnCallStateChanged);

            this._isStarted = true;
            RaiseStartedEvent();
        }

        public override void Stop()
        {
            this.PluginManager.Core.CallManager.OnCallStateChanged -= CallManager_OnCallStateChanged;

            this._isStarted = false;
            RaiseStoppedEvent("Normal stop");
        }

        public override bool IsStarted
        {
            get { return this._isStarted; }
        }

        public OneLinePlugin(IPluginManager pluginManager)
            : base(pluginManager)
        {
            this._uiElements = new List<IPluginUIElement>();
            this._uiElements.Add(new OneLinePluginUIElement(this));
        }

        void CallManager_OnCallStateChanged(ICall call)
        {
            lock (call)
            {
                if (call != null &&
                    !call.IsDisposed &&
                    this.IsOneLine &&
                    call.IsIncoming &&
                    this.PluginManager.Core.CallManager.Count > 1)
                    call.Drop();
            }
        }
    }
}
