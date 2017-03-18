using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ContactPoint.Common;

namespace ContactPoint.BaseDesign.Wpf
{
    public static class WindowHelper
    {
        public static void SavePosition(this Window window, ISettingsManagerSection settingsManager)
        {
            settingsManager.Set(String.Format("{0}_Width", window.GetType()), window.Width);
            settingsManager.Set(String.Format("{0}_Height", window.GetType()), window.Height);
            settingsManager.Set(String.Format("{0}_X", window.GetType()), window.Left);
            settingsManager.Set(String.Format("{0}_Y", window.GetType()), window.Top);
        }

        public static void SetPosition(this Window window, ISettingsManagerSection settingsManager)
        {
            window.Width = settingsManager.GetValueOrSetDefault(String.Format("{0}_Width", window.GetType()), window.Width);
            window.Height = settingsManager.GetValueOrSetDefault(String.Format("{0}_Height", window.GetType()), window.Height);
            window.Left = settingsManager.GetValueOrSetDefault(String.Format("{0}_X", window.GetType()), window.Left);
            window.Top = settingsManager.GetValueOrSetDefault(String.Format("{0}_Y", window.GetType()), window.Top);
        }
    }
}
