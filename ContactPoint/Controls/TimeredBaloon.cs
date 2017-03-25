using System;
using System.Linq;
using System.Windows.Forms;
using ContactPoint.Common;

namespace ContactPoint.Controls
{
    class TimeredBaloon
    {
        public TimeredBaloon(Control control, string textTitle, string text)
              : this(control, textTitle, text, ToolTipIcon.Info)
        { }

        public TimeredBaloon(Control control, string textTitle, string text, ToolTipIcon icon)
            : this(control, textTitle, text, icon, 1200)
        { }

        public TimeredBaloon(Control control, string textTitle, string text, ToolTipIcon icon, int duration)
        {
            var baloon = new ToolTip
            {
                IsBalloon = true,
                ToolTipTitle = textTitle,
                ToolTipIcon = icon,
                InitialDelay = 100,
                AutomaticDelay = 100,
                AutoPopDelay = 100,
                ReshowDelay = 100,
                ShowAlways = true,
                UseAnimation = true,
                UseFading = true,
                Active = true
            };

            try
            {
                var linesCount = text.Count(x => '\n'.Equals(x)) + 1;
                baloon.Show(text, control, 10, -(35 + linesCount * 15), duration);
            }
            catch (Exception e)
            {
                Logger.LogWarn("Unable to show baloon tip: " + e.Message);
            }
        }

        public static TimeredBaloon Show(Control control, string textTitle, string text, ToolTipIcon icon = ToolTipIcon.Info, int duration = 1200)
        {
            return new TimeredBaloon(control, textTitle, text, icon, duration);
        }
    }
}
