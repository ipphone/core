using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ContactPoint.Common.SIP;

namespace ContactPoint.Core.SIP
{
    internal class SipCodec : ISipCodec
    {
        private string _name = "";
        private SIP _sip;
        private int _priority = 0;

        public SipCodec(SIP sip, string name)
        {
            this._sip = sip;
            this._name = name;
        }

        private void RaiseEnabledChangedEvent()
        {
            if (this.EnabledChanged != null)
                this.EnabledChanged(this);
        }

        #region ISipCodec Members

        public event Action<ISipCodec> EnabledChanged;

        public string Name
        {
            get { return this._name; }
        }

        public bool Enabled
        {
            get { return this._priority > 0 ? true : false; }
            set
            {
                if (this.Enabled != value)
                {
                    this._priority = value == true ? 128 : 0;

                    // Save into settings
                    this._sip.Core.SettingsManager[this.Name] = value;

                    RaiseEnabledChangedEvent();
                }
            }
        }

        public int Priority
        {
            get { return this._priority; }
        }

        #endregion
    }
}
