using System;
using System.Windows.Forms;
using ContactPoint.BaseDesign.Wpf;
using ContactPoint.Common;
using ExceptionReporter.Views;

namespace ContactPoint.Forms
{
    public partial class LoggerForm : Form
    {
        public LoggerForm()
        {
            InitializeComponent();

            buttonSipLog.Visible = Logger.LogLevel >= 2;

			var imageList = new ImageList();
			imageList.Images.Add(ContactPoint.Properties.Resources.info);
			imageList.Images.Add(ContactPoint.Properties.Resources.warning);
			imageList.Images.Add(ContactPoint.Properties.Resources.error);

			listViewMessages.SmallImageList = listViewMessages.LargeImageList = imageList;

            foreach (var item in Logger.Log)
                this.Log(item);

            Logger.MessageLogged += new Logger.MessageLoggedDelegate(Logger_MessageLogged);
        }

        private void Log(Logger.LogObject logObject)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Logger.MessageLoggedDelegate(Log), new object[] { logObject });
                return;
            }

            this.listViewMessages.Items.Insert(0, new ListViewItem(new string[] {
                "",
                logObject.Type.ToString(),
                logObject.DateTime.ToString("dd.MM.yyyy HH:mm:ss"),
                logObject.Message
            }, (int)logObject.Type) { Tag = logObject });
        }

        void Logger_MessageLogged(Logger.LogObject logObject)
        {
            this.Log(logObject);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoggerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.MessageLogged -= Logger_MessageLogged;
        }

        private void listViewMessages_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewMessages.SelectedItems.Count > 0)
            {
                var item = listViewMessages.SelectedItems[0];
                if (item == null) return;

                var logObject = item.Tag as Logger.ExceptionLogObject;
                if (logObject == null) return;

                new InternalExceptionView().ShowException(String.Format("Exception that was occured {0}", logObject.DateTime), logObject.Exception);
            }
        }

        private void buttonSipLog_Click(object sender, EventArgs e)
        {
            var viewModel = new SipLogViewModel();
            var logWindow = new SipLog() { DataContext = viewModel };

            logWindow.Closing += (o, args) => viewModel.Stop();
            logWindow.Show();
        }
    }
}
