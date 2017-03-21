using ContactPoint.Common;
using ContactPoint.Common.Contacts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ContactPoint.BaseDesign.Wpf.CoreDesign
{
    class PhoneViewModel : ViewModel
    {
        private string _searchText = String.Empty;
        private ICore _core;

        public ICall ActiveCall { get; set; }
        public ObservableCollection<ICall> CurrentCalls { get; private set; }
        public ObservableCollection<IContact> Contacts { get; private set; }
        public string SearchText 
        {
            get { return _searchText; }
            set
            {
                _searchText = value;

                NotifyPropertyChanged("SearchText");

                //Contacts = new ObservableCollection<IContact>(Contacts.Where(x => x.Key.Contains(value) || x.PhoneNumbers.Count(p => p.Number.Contains(value)) > 0));
            }
        }

        public PhoneViewModel(ICore core)
        {
            _core = core;

            CurrentCalls = new ObservableCollection<ICall>();
            Contacts = new ObservableCollection<IContact>();

            CurrentCalls.CollectionChanged += CurrentCalls_CollectionChanged;

            core.CallManager.OnIncomingCall += CallManager_OnIncomingCall;
            core.CallManager.OnCallRemoved += CallManager_OnCallRemoved;
        }

        void CurrentCalls_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ActiveCall = CurrentCalls.FirstOrDefault(x => x.State == CallState.ACTIVE);
        }

        void CallManager_OnIncomingCall(ICall call)
        {
            CurrentCalls.Add(call);
        }

        void CallManager_OnCallRemoved(ICall call, CallRemoveReason reason)
        {
            CurrentCalls.Remove(call);
        }
    }
}
