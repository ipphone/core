using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.Pause
{
    [Plugin("{D114913D-5656-44b8-B57B-DA0C4F116988}", "Pause")]
    public class PauseService : Plugin
    {
        private List<IPluginUIElement> _uiElements = new List<IPluginUIElement>();
        public override List<IPluginUIElement> UIElements
        {
            get { return _uiElements; }
        }

        public PauseService(IPluginManager pluginManager)
            : base(pluginManager)
        {
            _uiElements.Add(new PauseUiElement(this));
        }

        public override void Start()
        {
            _isStarted = true;
            RaiseStartedEvent();
        }

        public override void Stop()
        {
            _isStarted = false;
            RaiseStoppedEvent("Normal stop");
        }

        private bool _isStarted;
        public override bool IsStarted
        {
            get { return _isStarted; }
        }
    }
}
