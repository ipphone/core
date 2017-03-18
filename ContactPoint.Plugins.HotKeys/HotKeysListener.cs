using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ContactPoint.Common;
using ContactPoint.Plugins.HotKeys.Actions;

namespace ContactPoint.Plugins.HotKeys
{
    internal class HotKeysListener : IService
    {
        private HotKeysPlugin _plugin;
        private List<KeyCombination> _combinations = new List<KeyCombination>();

        public IList<KeyCombination> Combinations
        {
            get { return _combinations; }
        }

        public HotKeysListener(HotKeysPlugin plugin)
        {
            _plugin = plugin;

            // Hardcoded staff because no time
            // Anyway it is a creation of Answer\Drop\Hold command for HotKeys
            // Be careful in plugin used hardcoded order of elements in this list!!!
            _combinations.Add(new KeyCombination(new UserAction()
            {
                Name = "Answer call",
                Command = x =>
                              {
                                  if (x.CallManager.ActiveCall != null) x.CallManager.AnswerCall(x.CallManager.ActiveCall);
                                  else
                                  {
                                      var call = FindNextCall();

                                      if (call != null)
                                      {
                                          Core.Ui.Current.ActivateCall(call);

                                          call.Answer();
                                      }
                                  }
                              }
            }, _plugin.PluginManager.Core)
            {
                Combination = _plugin.PluginManager.Core.SettingsManager.GetValueOrSetDefault<int>("HotKeysPluginAnswerKey", 131121) // Means "Ctrl + 1"
            });

            
            _combinations.Add(new KeyCombination(new UserAction()
            {
                Name = "Drop call",
                Command = x => { if (x.CallManager.ActiveCall != null) x.CallManager.DropCall(x.CallManager.ActiveCall); }
            }, _plugin.PluginManager.Core)
            {
                Combination = _plugin.PluginManager.Core.SettingsManager.GetValueOrSetDefault<int>("HotKeysPluginDropKey", 131122) // Means "Ctrl + 2"
            });
            
            
            _combinations.Add(new KeyCombination(new UserAction()
            {
                Name = "Hold call",
                Command = x => { if (x.CallManager.ActiveCall != null) x.CallManager.ToggleHoldCall(x.CallManager.ActiveCall); }
            }, _plugin.PluginManager.Core)
            {
                Combination = _plugin.PluginManager.Core.SettingsManager.GetValueOrSetDefault<int>("HotKeysPluginHoldKey", 131123) // Means "Ctrl + 3"
            });

            //foreach (var c in Combinations)
            //    (_plugin.PluginManager.Core.SyncObject as Control).Controls.Add(c);
        }

        private ICall FindNextCall()
        {
            ICall nextCall = null;
            Int64 lastPartTime = -1, lastPartId = -1;

            foreach (var call in _plugin.PluginManager.Core.CallManager)
                if (call != null)
                {
                    if (nextCall == null) nextCall = call;

                    // We will try to compare unique ids of call and found call with minimal number.
                    // This is because if call is in queue it can leave queue and enter queue again
                    // and it is better to pick it up as soon as possible.
                    var unid = call.Headers["x-unid"];
                    if (unid != null)
                    {
                        // Asterisk unique id. Example: 1349177069.455
                        var uniqueId = unid.Value;

                        var uniqueIdParts = uniqueId.Split('.');

                        if (uniqueIdParts.Length == 2)
                        {
                            Int64 partTime, partId;

                            if (Int64.TryParse(uniqueIdParts[0], out partTime) && Int64.TryParse(uniqueIdParts[1], out partId))
                            {
                                if (partTime < lastPartTime || (partTime == lastPartTime && partId < lastPartId))
                                {
                                    nextCall = call;

                                    lastPartTime = partTime;
                                    lastPartId = partId;
                                }
                                else if (nextCall == call)
                                {
                                    lastPartTime = partTime;
                                    lastPartId = partId;
                                }
                            }
                        }
                    }
                }

            return nextCall;
        }

        #region IService Members

        public event ServiceStartedDelegate Started;

        public event ServiceStoppedDelegate Stopped;

        public void Start()
        {
            foreach (var c in Combinations)
                c.Start();

            IsStarted = true;

            if (Started != null)
                Started(this);
        }

        public void Stop()
        {
            foreach (var c in Combinations)
                c.Stop();

            IsStarted = false;

            if (Stopped != null)
                Stopped(this, "Normal stop");
        }

        public bool IsStarted
        {
            get;
            private set;
        }

        #endregion
    }
}
