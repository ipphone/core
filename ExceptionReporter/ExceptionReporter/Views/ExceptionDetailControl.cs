using System;
using System.Drawing;
using System.Windows.Forms;

namespace ExceptionReporting.WinForms.Views
{
	internal partial class ExceptionDetailControl : UserControl
    {
        public ExceptionDetailControl()
        {
            InitializeComponent();

            WireUpEvents();
        }

        public void SetControlBackgrounds(Color color)
        {
            listviewExceptions.BackColor =
                txtExceptionTabMessage.BackColor =
                txtExceptionTabStackTrace.BackColor = color;
        }

        //TODO Label='EH' - move this logic out (is duplicated almost entirely (without ListView) in ExceptionReportBuilder)
        public void PopulateExceptionTab(Exception rootException)
        {
            TheException = rootException;
            listviewExceptions.Clear();
            listviewExceptions.Columns.Add("Level", 100, HorizontalAlignment.Left);
            listviewExceptions.Columns.Add("Exception Type", 150, HorizontalAlignment.Left);
            listviewExceptions.Columns.Add("Target Site / Method", 150, HorizontalAlignment.Left);

            var listViewItem = new ListViewItem
            {
                Text = "Top Level"
            };
            listViewItem.SubItems.Add(rootException.GetType().ToString());

            AddTargetSite(listViewItem, rootException);
            listViewItem.Tag = "0";
            listviewExceptions.Items.Add(listViewItem);
            listViewItem.Selected = true;

            int index = 0;
            Exception currentException = rootException;

            while (currentException.InnerException != null)
            {
                index++;
                currentException = currentException.InnerException;
                listViewItem = new ListViewItem
                {
                    Text = string.Format("Inner Exception {0}", index)
                };
                listViewItem.SubItems.Add(currentException.GetType().ToString());
                AddTargetSite(listViewItem, currentException);
                listViewItem.Tag = index.ToString();
                listviewExceptions.Items.Add(listViewItem);
            }

            txtExceptionTabStackTrace.Text = rootException.StackTrace;
            txtExceptionTabMessage.Text = rootException.Message;
        }

        private static void AddTargetSite(ListViewItem listViewItem, Exception exception)
        {
            //TargetSite can be null (http://msdn.microsoft.com/en-us/library/system.exception.targetsite.aspx)
            if (exception.TargetSite != null)
            {
                listViewItem.SubItems.Add(exception.TargetSite.ToString());
            }
        }

        private void WireUpEvents()
        {
            listviewExceptions.SelectedIndexChanged += ExceptionsSelectedIndexChanged;
        }

        private void ExceptionsSelectedIndexChanged(object sender, EventArgs e)
        {
            Exception displayException = TheException;
            foreach (ListViewItem listViewItem in listviewExceptions.Items)
            {
                if (!listViewItem.Selected) continue;
                for (int count = 0; count < int.Parse(listViewItem.Tag.ToString()); count++)
                {
                    displayException = displayException.InnerException;
                }
            }

            txtExceptionTabStackTrace.Text = string.Empty;
            txtExceptionTabMessage.Text = string.Empty;

            if (displayException == null)
            {
                displayException = TheException;
            }
            if (displayException == null)
            {
                return;
            }

            txtExceptionTabStackTrace.Text = displayException.StackTrace;
            txtExceptionTabMessage.Text = displayException.Message;
        }

        public Exception TheException{get; set;}
    }

}