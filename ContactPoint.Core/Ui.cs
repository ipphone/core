using System;
using ContactPoint.Common;

namespace ContactPoint.Core
{
    public class Ui : IUi
    {
        private static IUi _implementation;

        public static IUi Current { get; }

        public static void SetCurrent(IUi ui)
        {
            _implementation = ui;
        }

        static Ui()
        {
            Current = new Ui();
        }

        public void ActivateCall(ICall call)
        {
            try
            {
                _implementation.ActivateCall(call);
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Can't activate call.");
            }
        }

        public string GetPhoneNumber()
        {
            return _implementation.GetPhoneNumber();
        }

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            return _implementation.BeginInvoke(method, args);
        }

        public object EndInvoke(IAsyncResult result)
        {
            return _implementation.EndInvoke(result);
        }

        public object Invoke(Delegate method, object[] args)
        {
            return _implementation.Invoke(method, args);
        }

        public bool InvokeRequired => _implementation.InvokeRequired;
    }
}
