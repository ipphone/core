using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Plugins.HotKeys.Ui;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.HotKeys
{
    [Plugin("{F8A2C5ED-E343-4290-A201-E117EEF89163}", "Hot keys", HaveSettingsForm = true)]
    public class HotKeysPlugin : Plugin
    {
        private bool _isStarted = false;
        private readonly List<IPluginUIElement> _uiElements = new List<IPluginUIElement>();
        private readonly HotKeysListener _hotKeysListener;

        public override bool IsStarted
        {
            get { return _isStarted; }
        }

        public override IEnumerable<IPluginUIElement> UIElements
        {
            get { return this._uiElements; }
        }

        internal HotKeysListener HotKeysListener
        {
            get { return this._hotKeysListener; }
        }

        public HotKeysPlugin(IPluginManager pluginManager)
            : base(pluginManager)
        {
            Logger.LogNotice("HotKeys service initializing...");

            _hotKeysListener = new HotKeysListener(this);
        }

        ~HotKeysPlugin()
        {
            Stop();
        }

        public override void ShowSettingsDialog()
        {
            new SettingsForm(this).ShowDialog();
        }

        public override void Start()
        {
            Logger.LogNotice("Starting hotkeys");

            TryStart();
        }

        public override void Stop()
        {
            if (_isStarted)
            {
                this._isStarted = false;
            }

            RaiseStoppedEvent("Normal stop");
        }

        private void TryStart()
        {
            if (!this._isStarted)
            {
                _hotKeysListener.Start();

                if (_hotKeysListener.IsStarted)
                {
                    this._isStarted = true;

                    RaiseStartedEvent();
                }
            }
        }
    }
}
