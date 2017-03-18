using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ContactPoint.Common.Contacts;
using ContactPoint.Plugins.ContactsUi.Controls;
using ContactPoint.Plugins.ContactsUi.ViewModels;

namespace ContactPoint.Plugins.ContactsUi.Forms
{
  internal partial class ContactForm : KryptonForm
    {
        private readonly IContactsManager _contactsManager;
        private readonly ContactViewModel _contactViewModel;

        public ContactForm(IContactsManager contactsManager, ContactViewModel contactViewModel)
        {
            _contactsManager = contactsManager;
            _contactViewModel = contactViewModel;

            InitializeComponent();

            ToolStripManager.Renderer = new ToolStripSystemRenderer();

            tagsListControl1.ContactViewModel = contactViewModel;
            tagsListControl1.ContactsManager = _contactsManager;
            labelShowedName.Text = contactViewModel.ShowedName;

            ContactInfosCollectionChanged(null, null);

            _contactViewModel.ContactInfos.CollectionChanged += ContactInfosCollectionChanged;
            _contactViewModel.PropertyChanged += ContactViewModelPropertyChanged;
            _contactViewModel.SelectedContactInfo = _contactViewModel.ContactInfos.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(_contactViewModel.ShowedName)) Text = "New contact";
            else Text = String.Format("Editing contact: {0}", _contactViewModel.ShowedName);
        }

        private void ContactViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedContactInfo") SelectedContactInfoChanged();
            if (e.PropertyName == "FirstName" || e.PropertyName == "LastName" || e.PropertyName == "MiddleName") RefreshName();
        }

        private void RefreshName()
        {
            labelShowedName.Text = _contactViewModel.ShowedName;
        }

        private void SelectedContactInfoChanged()
        {
            foreach (var item in tabControl1.TabPages.OfType<ContactDetailsControl>())
                if (item.ContactInfoViewModel == _contactViewModel.SelectedContactInfo && tabControl1.SelectedTab != item)
                {
                    tabControl1.SelectedTab = item;

                    return;
                }
        }

        private void ContactInfosCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            tabControl1.SuspendLayout();
            tabControl1.TabPages.Clear();

            foreach (var contactInfo in _contactViewModel.ContactInfos)
                tabControl1.TabPages.Add(new ContactDetailsControl(_contactsManager, contactInfo));

            tabControl1.ResumeLayout(true);

            dropDownNewContactInfos.Visible = _contactViewModel.AddressBooksItems.Any();

            RefreshAddressBooksItems();
        }

        private void RefreshAddressBooksItems()
        {
            var menu = new ContextMenuStrip();
            foreach (var item in _contactViewModel.AddressBooksItems)
            {
                var itemLocal = item;
                menu.Items.Add(item.Text, null, (sender, args) => itemLocal.PerformExecute());
            }

            dropDownNewContactInfos.ContextMenuStrip = menu;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            _contactViewModel.SubmitContact();
            _contactViewModel.RefreshInfo();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _contactViewModel.CancelChanges();

            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
