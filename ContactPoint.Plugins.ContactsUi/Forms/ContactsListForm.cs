using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ContactPoint.BaseDesign.Components;
using ContactPoint.BaseDesign.Properties;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;
using ContactPoint.Plugins.ContactsUi.ViewModels;

namespace ContactPoint.Plugins.ContactsUi.Forms
{
  public partial class ContactsListForm : KryptonForm
    {
        private readonly IContactsManager _contactsManager;
        private readonly AdvancedBindingSource<ContactViewModel> _bindingSource;
        private readonly Dictionary<long, ContactViewModel> _contactViewModels = new Dictionary<long, ContactViewModel>();

        public ContactsListForm(IContactsManager contactsManager)
        {
            _contactsManager = contactsManager;

            InitializeComponent();

            ToolStripManager.Renderer = new ToolStripSystemRenderer();

            _bindingSource = new AdvancedBindingSource<ContactViewModel>();

            dataGridView.AutoGenerateColumns = false;

            dataGridView.Columns.Clear();
            dataGridView.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = Resources.ContactsListForm_Name, Name = "ShowedName", SortMode = DataGridViewColumnSortMode.NotSortable, Width = 250, DataPropertyName = "ShowedName" },
                new DataGridViewTextBoxColumn { HeaderText = Resources.ContactsListForm_Company, Name = Resources.ContactsListForm_Company, SortMode = DataGridViewColumnSortMode.NotSortable, Width = 100, DataPropertyName = Resources.ContactsListForm_Company },
                new DataGridViewTextBoxColumn { HeaderText = Resources.ContactsListForm_Phone_numbers, Name = "PhoneNumbersString", SortMode = DataGridViewColumnSortMode.NotSortable, Width = 250, DataPropertyName = "PhoneNumbersString" },
                //new DataGridViewTagsColumns { HeaderText = "Tags", Name = "Tags", SortMode = DataGridViewColumnSortMode.NotSortable, Width = 300, DataPropertyName = "TagLocals" },
                new DataGridViewTextBoxColumn { HeaderText = Resources.ContactsListForm_Note, Name = Resources.ContactsListForm_Note, SortMode = DataGridViewColumnSortMode.NotSortable, DataPropertyName = Resources.ContactsListForm_Note, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill }
                );

            dataGridView.DataSource = _bindingSource;

            dataGridView.CellDoubleClick += DataGridViewCellDoubleClick;
            dataGridView.CellContextMenuStripNeeded += DataGridViewCellContextMenuStripNeeded;

            buttonSpecAny1.Click += buttonSpecAny1_Click;

            textBoxSearch.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            _contactsManager.AddressBookReloaded += ContactsManagerOnAddressBookReloaded;             

            ReloadContacts();
        }

        void buttonSpecAny1_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = String.Empty;
        }

        private void ContactsManagerOnAddressBookReloaded(IAddressBookLocal addressBookLocal)
        {
            if (InvokeRequired) Invoke(new Action(ReloadContacts));
            else ReloadContacts();
        }

        void DataGridViewCellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView.Rows[e.RowIndex];

            var contactViewModel = row.DataBoundItem as ContactViewModel;
            if (contactViewModel == null) return;

            e.ContextMenuStrip = GenerateContextMenu(contactViewModel);
        }

        private ContextMenuStrip GenerateContextMenu(ContactViewModel contactViewModel)
        {
            if (contactViewModel == null) return null;

            var contextMenu = new ContextMenuStrip();

            contextMenu.Items.Add("Edit contact...", null, (sender, args) => ShowContactForm(contactViewModel));
            contextMenu.Items.Add("Remove contact", null, (sender, args) => DeleteContact(contactViewModel));

            if (contactViewModel.PhoneNumbersNotEmpty.Any())
            {
                contextMenu.Items.Add(new ToolStripSeparator());
                foreach (var item in contactViewModel.PhoneNumbersNotEmpty)
                {
                    var item1 = item;
                    contextMenu.Items.Add(string.Format("Call number {0} {1}", item.Number, string.IsNullOrWhiteSpace(item.Comment) ? string.Empty : string.Format("({0})", item.Comment)), null, (sender, args) => item1.Call());
                }
            }

            if (contactViewModel.EmailsNotEmpty.Any())
            {
                contextMenu.Items.Add(new ToolStripSeparator());
                foreach (var item in contactViewModel.EmailsNotEmpty)
                {
                    var item1 = item;
                    contextMenu.Items.Add(string.Format("Compose email for {0} {1}", item.Email, string.IsNullOrWhiteSpace(item.Comment) ? string.Empty : string.Format("({0})", item.Comment)), null, (sender, args) => item1.SendEmail());
                }
            }

            return contextMenu;
        }

        void DataGridViewCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView.Rows[e.RowIndex];

            var contactViewModel = row.DataBoundItem as ContactViewModel;
            if (contactViewModel == null) return;

            ShowContactForm(contactViewModel);
        }

        private void buttonAddContact_Click(object sender, EventArgs e)
        {
            var contactViewModel = new ContactViewModel(_contactsManager, _contactsManager.CreateContact());
            foreach (var item in _contactsManager.AddressBooks.Where(x => x.IsOnline && !x.ReadOnly))
                contactViewModel.ContactInfos.Add(new ContactInfoViewModel(_contactsManager, item.CreateContactInfo()));

            contactViewModel.SelectedContactInfo = contactViewModel.ContactInfos.FirstOrDefault();

            ShowContactForm(contactViewModel);
        }

        private void buttonRemoveContact_Click(object sender, EventArgs e)
        {
            foreach (var contactViewModel in dataGridView.SelectedRows.OfType<DataGridViewRow>().Select(x => x.DataBoundItem).OfType<ContactViewModel>().Where(x => x != null))
                DeleteContact(contactViewModel);

            ReloadContacts();
        }

        private void DeleteContact(ContactViewModel contactViewModel)
        {
            if (MessageBox.Show(string.Format("Delete '{0}'?", !string.IsNullOrWhiteSpace(contactViewModel.ShowedName) ? contactViewModel.ShowedName : "<empty>"), "Confirm delete of contact", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                contactViewModel.Contact.Remove();

                ReloadContacts();
            }
        }

        private void ShowContactForm(ContactViewModel contactViewModel)
        {
            var window = new ContactForm(_contactsManager, contactViewModel);
            window.Closed += ContactFormClosed;
            window.Show();
        }

        void ContactFormClosed(object sender, EventArgs e)
        {
            var form = sender as Form;
            if (form == null) return;

            form.Closed -= ContactFormClosed;

            if (form.DialogResult != DialogResult.OK) return;

            ReloadContacts();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            ReloadContacts();
        }

        public void ReloadContacts()
        {
            if (textBoxSearch.Text.Length > 0)
            {
                var filterParts = textBoxSearch.Text.Split(' ');

                dataGridView.DataSource = new ObservableCollection<ContactViewModel>(
                    _contactsManager.Contacts
                        .Where(x => filterParts.All(f => MatchTag(f, x) || MatchPhone(f, x) || MatchAll(f, x)))
                        .Select(GetContactViewModel)
                );
            }
            else
            {
                dataGridView.DataSource = new ObservableCollection<ContactViewModel>(_contactsManager.Contacts.Select(GetContactViewModel));
            }
        }

        private bool MatchAll(string pattern, IContact contact)
        {
            return contact.FirstName != null && contact.FirstName.IndexOf(pattern, 0, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                   contact.MiddleName != null && contact.MiddleName.IndexOf(pattern, 0, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                   contact.LastName != null && contact.LastName.IndexOf(pattern, 0, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                   contact.Company != null && contact.Company.IndexOf(pattern, 0, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private bool MatchPhone(string pattern, IContact contact)
        {
            if (pattern[0] < '0' || pattern[0] > '9') return false;

            return
                contact.ContactInfos.Any(
                    x =>
                    x.PhoneNumbers.Any(p => p.Number != null && p.Number.IndexOf(pattern, 0, StringComparison.CurrentCultureIgnoreCase) >= 0));
        }

        private bool MatchTag(string pattern, IContact contact)
        {
            if (pattern.StartsWith("#") || pattern.StartsWith("\\")) pattern = pattern.Substring(1);

            return
                contact.ContactInfos.Any(
                    x =>
                    x.Tags.Any(
                        y => y.Name != null && y.Name.IndexOf(pattern, 0, StringComparison.CurrentCultureIgnoreCase) >= 0));
        }

        private ContactViewModel GetContactViewModel(IContact contact)
        {
            ContactViewModel result;
            if (!_contactViewModels.ContainsKey(contact.Id))
            {
                result = new ContactViewModel(_contactsManager, contact);

                _contactViewModels.Add(contact.Id, result);
            }
            else result = _contactViewModels[contact.Id];

            return result;
        }
    }
}
