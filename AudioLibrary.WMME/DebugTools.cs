#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;
using AudioLibrary.Interfaces;

namespace AudioLibrary.WMME
{
    public static class DebugTools
    {
        private static readonly StringBuilder _log = new StringBuilder();

        public static void Log(string message)
        {
            _log.AppendLine(message);
        }

        public static string GetLog()
        {
            return _log.ToString();
        }

        public static string DumpAudioDevice(IAudioDevice audioDevice)
        {
            AudioDevice device = audioDevice as AudioDevice;
            if (device == null) return "Device is not supported\r\n";

            return String.Format(@"
Device:                {0}
DeviceId:              {1}
Playback:              {2}
Recording:             {3}
DefaultLine:           
---
{4}
---
Handle:                {5}
Lines:
+++
{6}
+++
", device.Name, device.DeviceId, device.PlaybackSupport, device.RecordingSupport, DumpAudioLine(device.DefaultLine),
 device.Handle, DumpCollection<AudioLine>(device.Lines, new DumpDelegate<AudioLine>(DumpAudioLine)));
        }

        public static string DumpAudioLine(AudioLine audioLine)
        {
            AudioLine line = audioLine as AudioLine;
            if (line == null) return "Line is not supported or null\r\n";

            return String.Format(@"
  Line:                {0}
  LineId:              {1}
  Active:              {2}
  Balance:             {3}
  CControls:           {4}
  Channel:             {5}
  Channels:            {6}
  ComponentType:       {7}
  Connected:           {8}
  Connections:         {9}
  Controls:
  +++
{10}
  +++
  Destination:         {11}
  Direction:           {12}
  HasBalance:          {13}
  HasVolume:           {14}
  Mute:                {15}
  Source:              {16}
  Volume:              {17}
", line.Name, line.Id, line.Active, line.Balance, line.CControls, Enum.GetName(typeof(Native.Channel), line.Channel),
 line.Channels, Enum.GetName(typeof(Native.MIXERLINE_COMPONENTTYPE), line.ComponentType), line.Connected, line.Connections,
 DumpCollection(line.Controls, new DumpDelegate<AudioLineControl>(DumpLineControl)), line.Destination, line.Direction, line.HasBalance, line.HasVolume,
 line.Mute, line.Source, line.Volume);
        }

        internal static string DumpLineControl(AudioLineControl lineControl)
        {
            return String.Format(@"
    LineControl:       {0}
    LineControlId:     {1}
    ControlFlag:       {2}
    ControlType:       {3}
    Minimum:           {4}
    Maximum:           {5}
    Value:             {6}
", lineControl.Name, lineControl.Id, Enum.GetName(typeof(Native.MIXERCONTROL_CONTROLFLAG), lineControl.ControlFlag), 
 Enum.GetName(typeof(Native.MIXERCONTROL_CONTROLTYPE), lineControl.ControlType), lineControl.Minimum, lineControl.Maximum, lineControl.Value);
        }

        private delegate string DumpDelegate<T>(T obj);
        private static string DumpCollection<T>(IEnumerable<T> collection, DumpDelegate<T> dumpMethod)
        {
            string result = "";

            foreach (var item in collection)
                result += dumpMethod.Invoke(item) + "\r\n";

            return result;
        }
    }
}

#endif
