using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ContactPoint.BaseDesign.BaseNotifyControls;
using ContactPoint.Contacts.Updater;
using Action = System.Action;

namespace ContactPoint.Contacts
{
    internal partial class AddressBookUpdatingNotifyControl : NotifyControl
    {
        private readonly UpdateTask _task;

        public AddressBookUpdatingNotifyControl(UpdateTask task)
        {
            _task = task;

            InitializeComponent();

            if (_task.IsFinished)
            {
                Close();

                return;
            }

            task.Finished += TaskFinished;
            task.StateChanged += TaskStateChanged;

            labelName.Text = string.Format("Updating {0}", task.AddressBook.Name);
            labelState.Text = task.CurrentStateString;
        }

        void TaskStateChanged()
        {
            if (InvokeRequired) Invoke(new Action(RefreshContent));
            else RefreshContent();
        }

        void TaskFinished(UpdateTask obj)
        {
            _task.Finished -= TaskFinished;
            _task.StateChanged -= TaskStateChanged;

            Close();
        }

        void RefreshContent()
        {
            labelState.Text = _task.CurrentStateString;
        }
    }
}
