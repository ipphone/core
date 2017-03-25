using System.Drawing;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Commands
{
    internal class CheckedPluginUIElementCommand : PluginUIElementCommand
    {
        private readonly IPluginCheckedUIElement _uiElement;

        public CheckedPluginUIElementCommand(IPluginCheckedUIElement uiElement) : base(uiElement)
        {
            _uiElement = uiElement;

            Checked = _uiElement.Checked;
            SetImage(_uiElement.Checked ? _uiElement.ImageChecked : _uiElement.Image);

            _uiElement.CheckedChanged += OnCheckedChanged;
        }

        protected override void UpdateImage()
        {
            if (_uiElement != null)
            {
                SetImage(_uiElement.Checked ? _uiElement.ImageChecked : _uiElement.Image);
            }
        }

        protected override void ExecuteInner()
        {
            _uiElement.ExecuteChecked(this, !Checked);
        }

        private void SetImage(Image image)
        {
            ImageLarge = image;
            ImageSmall = image;
        }

        void OnCheckedChanged(IPluginUIElement obj)
        {
            Checked = _uiElement.Checked;
        }
    }
}
