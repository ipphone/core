using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContactPoint.Common;

namespace ContactPoint.BaseDesign.Wpf
{
    public class SipLogViewModel : ViewModel, IDisposable
    {
        private const string FilePath = @".\pjsip.log";

        private bool _isStopping = false;
        private DateTime _lastFileTime = DateTime.MinValue;
        private long _lastLength = 0;
        private string _raw = string.Empty;

        public ObservableCollection<SipLogItemViewModel> LogItems { get; set; }
        public string Raw
        {
            get { return _raw; }
            set { _raw = value; NotifyPropertyChanged("Raw"); }
        }

        public SipLogViewModel()
        {
            LogItems = new ObservableCollection<SipLogItemViewModel>();

            Task.Factory.StartNew(UpdateFromFileThread, TaskCreationOptions.PreferFairness);
        }

        public void Stop()
        {
            _isStopping = true;
        }

        void UpdateFromFileThread()
        {
            while (!_isStopping)
            {
                try
                {
                    var fileTime = File.GetLastWriteTime(FilePath);

                    if (fileTime > _lastFileTime)
                    {
                        var result = new StringBuilder(2048 + Raw.Length);
                        var buffer = new byte[2048];
                        _lastFileTime = fileTime;

                        using (var file = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            file.Seek(_lastLength + 1, SeekOrigin.Begin);
                            _lastLength = file.Length;

                            int n;
                            while ((n = file.Read(buffer, 0, 2048)) >= 2048)
                                result.Append(Encoding.ASCII.GetString(buffer, 0, n));
                        }

                        result.Insert(0, Raw);
                        Raw = result.ToString();
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e);

                    break;
                }

                Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public class SipLogItemViewModel : ViewModel
    {
        public DateTime DateTime { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
    }
}
