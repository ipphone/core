using System;
using ComponentFactory.Krypton.Toolkit;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Commands
{
    internal class PluginUIElementCommand : KryptonCommand
    {
        private readonly IPluginUIElement _uiElement;

        public PluginUIElementCommand(IPluginUIElement uiElement)
        {
            _uiElement = uiElement;

            Text = _uiElement.Text;
            ImageLarge = _uiElement.Image;
            ImageSmall = _uiElement.Image;

            _uiElement.CommandExecuted += CommandExecuted;
            _uiElement.UIChanged += UIChanged;
        }

        protected override void OnExecute(EventArgs e)
        {
            ExecuteInner();
            base.OnExecute(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _uiElement.CommandExecuted -= CommandExecuted;
                _uiElement.UIChanged -= UIChanged;
            }

            base.Dispose(disposing);
        }

        protected virtual void ExecuteInner()
        {
            _uiElement.Execute(this);
        }

        protected virtual void UpdateImage()
        {
            ImageLarge = _uiElement.Image;
            ImageSmall = _uiElement.Image;
        }

        protected virtual void CommandExecuted(object sender, IPluginUIElement pluginUIElement)
        { }

        protected virtual void UIChanged(IPluginUIElement obj)
        {
            Enabled = _uiElement.Enabled;
            UpdateImage();
        }
    }
}
