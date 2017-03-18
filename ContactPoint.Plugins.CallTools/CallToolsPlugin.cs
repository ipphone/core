using System.Collections.Generic;
using System.Windows.Forms;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.CallTools.AutoAnswer;
using ContactPoint.Plugins.CallTools.CallNotifyWindow;
using ContactPoint.Plugins.CallTools.OneLine;
using ContactPoint.Plugins.CallTools.Ui;

namespace ContactPoint.Plugins.CallTools
{
    [Plugin("{73237940-7A20-46EB-8528-0072CBD1F34A}", "Call tools plugin", HaveSettingsForm = true)]
    public class CallToolsPlugin : Plugin
    {
        private static object _syncObject = new object();
        private static bool _autoAnswerInitialized = false;
        private static bool _incomingCallWindowInitialized = false;
        private static bool _oneLineInitialized = false;

        private readonly List<IPluginUIElement> _uiElements = new List<IPluginUIElement>();
        private bool _isStarted;
        private CallNotifyWindowService _callNotifyWindowService;
        private OneLineService _oneLine;
        private AutoAnswerService _autoAnswerService;
        private SettingsForm _settingsForm;

        public override IEnumerable<IPluginUIElement> UIElements => _uiElements;
        public override bool IsStarted => _isStarted;

        internal CallToolsOptions CallToolsOptions { get; }

        public CallToolsPlugin(IPluginManager pluginManager)
            : base(pluginManager)
        {
            CallToolsOptions = new CallToolsOptions(pluginManager.Core.SettingsManager);
        }

        public override void ShowSettingsDialog()
        {
            _settingsForm = _settingsForm ?? new SettingsForm(this);
            if (_settingsForm.ShowDialog() == DialogResult.OK)
            {
                Stop();
                Start();
            }
        }

        public override void Start()
        {
            lock (_syncObject)
            {
                if (CallToolsOptions.OneLineService && !_oneLineInitialized)
                {
                    _oneLineInitialized = true;
                    _oneLine = _oneLine ?? new OneLineService(this);
                }
                if (CallToolsOptions.OneLineService && this._oneLine != null)
                {
                    _oneLine.Start();
                    _uiElements.Add(new OneLinePluginUIElement(this, _oneLine));
                }

                if (CallToolsOptions.AutoAnswer && !_autoAnswerInitialized)
                {
                    _autoAnswerInitialized = true;
                    _autoAnswerService = _autoAnswerService ?? new AutoAnswerService(this);
                }
                if (CallToolsOptions.AutoAnswer && this._autoAnswerService != null)
                {
                    this._autoAnswerService.Start();
                }

                if (CallToolsOptions.ShowIncomingCallWindow && !_incomingCallWindowInitialized)
                {
                    _incomingCallWindowInitialized = true;
                    _callNotifyWindowService = _callNotifyWindowService ?? new CallNotifyWindowService(this);
                }
                if (CallToolsOptions.ShowIncomingCallWindow && this._callNotifyWindowService != null)
                {
                    _callNotifyWindowService.Start();
                }
            }

            if (CallToolsOptions.Pause)
            {
                _uiElements.Add(CallToolsOptions.PauseUiElementFactory(this));
            }

            _isStarted = true;
        }

        public override void Stop()
        {
            _isStarted = false;

            _callNotifyWindowService?.Stop();
            _oneLine?.Stop();
            _autoAnswerService?.Stop();

            foreach (var pluginUiElement in _uiElements)
            {
                pluginUiElement.Dispose();
            }

            _uiElements.Clear();
        }
    }
}
