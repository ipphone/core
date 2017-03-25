using System;
using System.Drawing;
using ContactPoint.Common;
using ContactPoint.Common.SIP.Account;

namespace ContactPoint.Commands
{
    class SipAccountStatusViewModel
    {
        private static DateTime _onlineSinceUtc = DateTime.UtcNow;

        public string Text { get; private set; }
        public Image Image { get; private set; }
        public Color Color { get; private set; }
        public string TimeElapsed { get; private set; }

        private SipAccountStatusViewModel()
        { }

        public static SipAccountStatusViewModel Create(ICore core)
        {
            var result = new SipAccountStatusViewModel();
            if (core.Sip.Account.RegisterState == SipAccountState.Connecting)
            {
                result.Color = Color.DarkMagenta;
                result.Text = CaptionStrings.CaptionStrings.StateConnecting;
                result.Image = Properties.Resources.status_connecting;

                _onlineSinceUtc = DateTime.UtcNow;

                return result;
            }

            if (core.Sip.Account.RegisterState == SipAccountState.Offline)
            {
                result.Color = Color.Gray;
                result.Image = core.Sip.Account?.PresenceStatus?.Icon ?? Properties.Resources.status_disconnected;

                var text = core.Sip.Account?.PresenceStatus?.Message ?? $"{CaptionStrings.CaptionStrings.StateDisconnected} {SipHelper.SipCodeDecode(core.Sip.Account.RegisterStateCode)}";
                result.Text = char.ToUpper(text[0]) + text.Substring(1);

                result.TimeElapsed = core.Sip.Account.RegisterStateDuration.ToFormattedString();

                return result;
            }

            if (core.Audio.PlaybackDevice == null || core.Audio.RecordingDevice == null)
            {
                result.Image = Properties.Resources.status_warning;
                result.Color = Color.Red;

                if (core.Audio.PlaybackDevice == null && core.Audio.RecordingDevice == null) result.Text = CaptionStrings.CaptionStrings.ErrorNoAudioDevices;
                else if (core.Audio.PlaybackDevice == null) result.Text = CaptionStrings.CaptionStrings.ErrorNoPlaybackDevice;
                else if (core.Audio.RecordingDevice == null) result.Text = CaptionStrings.CaptionStrings.ErrorNoRecordingDevice;

                return result;
            }

            if (core.Sip.Account?.PresenceStatus != null && core.Sip.Account.PresenceStatus.Code != PresenceStatusCode.Available)
            {
                result.Color = Color.Goldenrod;
                result.Image = core.Sip.Account.PresenceStatus?.Icon ?? Properties.Resources.status_connected;

                var text = core.Sip.Account.PresenceStatus?.Message ?? CaptionStrings.CaptionStrings.StatePause;
                result.Text = char.ToUpper(text[0]) + text.Substring(1);

                result.TimeElapsed = core.Sip?.Account?.PresenceStatusDuration.ToFormattedString();

                _onlineSinceUtc = DateTime.UtcNow;
            }
            else if (core.CallManager.ActiveCall != null)
            {
                switch (core.CallManager.ActiveCall.State)
                {
                    case CallState.ACTIVE:
                    case CallState.HOLDING:
                    case CallState.ALERTING:
                    case CallState.CONNECTING:
                        result.Color = Color.CornflowerBlue;
                        result.Text = CaptionStrings.CaptionStrings.StateOnThePhone;
                        result.Image = Properties.Resources.status_connected;
                        break;
                    case CallState.TERMINATED:
                        if (core.CallManager.ActiveCall.Check(
                            x => x.WasActive() || (
                            !x.IsIncoming && (
                                x.WasAlerting() ||
                                x.WasConnecting() ))))
                        {
                            _onlineSinceUtc = DateTime.UtcNow;
                        }
                        break;
                }
            }
            else
            {
                result.Color = Color.YellowGreen;
                result.Text = CaptionStrings.CaptionStrings.StateReady;
                result.Image = Properties.Resources.status_connected;

                result.TimeElapsed = (DateTime.UtcNow - _onlineSinceUtc).ToFormattedString();
            }

            return result;
        }
    }
}
