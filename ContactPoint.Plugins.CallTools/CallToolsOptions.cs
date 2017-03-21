using System;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using ContactPoint.Plugins.CallTools.Pause;

namespace ContactPoint.Plugins.CallTools
{
    internal class CallToolsOptions
    {
        internal const string NotHideCallWindowName = "CallToolsPluginNotHideCallWindow";
        internal const string AutoAnswerName = "CallToolsPluginAutoAnswer";
        internal const string OneLineServiceName = "CallToolsPluginOnelineService";
        internal const string ShowIncomingCallWindowName = "CallToolsPluginShowIncomingCallWindow";
        internal const string PauseName = "CallToolsPluginPause";
        internal const string PauseType = "CallToolsPluginPauseType";
        internal const string ActionExecutorEnabledName = "ActionExecutorEnabled";

        private readonly ISettingsManagerSection _settingsManager;

        public bool Pause => _settingsManager.GetValueOrSetDefault(PauseName, false);

        public Func<IPlugin, PauseUiElement> PauseUiElementFactory
        {
            get
            {
                var pauseType = _settingsManager.GetValueOrSetDefault<string>("CallToolsPluginPauseType", null);
                return "AccountState".Equals(pauseType, StringComparison.InvariantCultureIgnoreCase)
                    ? (p => new AccountStateUiElement(p))
                    : "PauseButton".Equals(pauseType, StringComparison.InvariantCultureIgnoreCase)
                    ? (Func<IPlugin, PauseUiElement>)(p => new PauseButtonUiElement(p))
                    : (p => new PauseUiElement(p));
            }
        }

        public bool AutoAnswer
        {
            get { return _settingsManager.GetValueOrSetDefault(AutoAnswerName, false); }
            set { _settingsManager.Set(AutoAnswerName, value); }
        }

        public bool OneLineService
        {
            get { return _settingsManager.GetValueOrSetDefault(OneLineServiceName, false); }
            set { _settingsManager.Set(OneLineServiceName, value); }
        }

        public bool ShowIncomingCallWindow
        {
            get { return _settingsManager.GetValueOrSetDefault(ShowIncomingCallWindowName, false); }
            set { _settingsManager.Set(ShowIncomingCallWindowName, value); }
        }

        public bool NotHideCallWindow
        {
            get { return _settingsManager.GetValueOrSetDefault(NotHideCallWindowName, false); }
            set { _settingsManager.Set(NotHideCallWindowName, value); }
        }

        public bool ActionExecutorEnabled
        {
            get { return _settingsManager.GetValueOrSetDefault(ActionExecutorEnabledName, false); }
            set { _settingsManager.GetValueOrSetDefault(ActionExecutorEnabledName, value); }
        }

        internal CallToolsOptions(ISettingsManagerSection settingsManager)
        {
            _settingsManager = settingsManager;
        }

        internal CallToolsOptions Clone()
        {
            return (CallToolsOptions)MemberwiseClone();
        }

        internal void Assign(CallToolsOptions actualOptions)
        {
            AutoAnswer = actualOptions.AutoAnswer;
            OneLineService = actualOptions.OneLineService;
            ShowIncomingCallWindow = actualOptions.ShowIncomingCallWindow;
            NotHideCallWindow = actualOptions.NotHideCallWindow;
        }
    }
}
