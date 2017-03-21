using System;
using System.Linq;
using System.Windows.Forms;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;
using ContactPoint.Plugins.ContactsUi.ViewModels;

namespace ContactPoint.Plugins.ContactsUi.Controls
{
  internal partial class TagsListControl : UserControl
    {
        private ContactViewModel _contactViewModel;

        public TagsListControl()
        {
            InitializeComponent();

            panelContainer.MouseClick += OnMouseClick;
        }

        public IContactsManager ContactsManager { get; set; }

        public ContactViewModel ContactViewModel
        {
            get { return _contactViewModel; }
            set 
            { 
                _contactViewModel = value;
                RefreshData();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button != MouseButtons.Right) return;

            OnMouseRightClick(this, e);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            OnMouseRightClick(sender, e);
        }

        private void OnMouseRightClick(object sender, MouseEventArgs e)
        {
            if (ContactsManager == null) return;

            var contextMenu = new ContextMenuStrip();

            foreach (var tag in ContactsManager.Tags)
            {
                var tagCopy = tag;
                var item = new ToolStripMenuItem(string.Format("#{0}", tag.Name), null, (o, args) =>
                    {
                        _contactViewModel.AddTag(tagCopy);
                        RefreshData();
                    }) {ForeColor = ContactTagViewModel.ParseColor(tag.Color)};

                contextMenu.Items.Add(item);
            }

            contextMenu.Items.Add(new ToolStripSeparator());

            var label = sender as TagLabel;
            if (label != null && !label.TagViewModel.ReadOnly)
                contextMenu.Items.Add(string.Format("Remove #{0}", label.TagViewModel.Name), null, (o, args) =>
                    {
                        if (MessageBox.Show(string.Format("Remove tag '{0}' for '{1}'?", label.TagViewModel.Name, _contactViewModel.ShowedName), "Confirm tag delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            _contactViewModel.RemoveTag(label.TagViewModel);
                            RefreshData();
                        }
                    });

            var createItem = new ToolStripMenuItem("Create new tag");
            foreach (var item in ContactsManager.AddressBooks.Where(x => !x.ReadOnly))
                createItem.DropDownItems.Add(new ToolStripMenuItem(string.Format("Create tag for {0}...", item.Name), null, CreateNewTag) { Tag = item });

            contextMenu.Items.Add(createItem);
            contextMenu.Show(Cursor.Position);
        }

        private void CreateNewTag(object sender, EventArgs eventArgs)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            var addressBook = menuItem.Tag as IAddressBookLocal;
            if (addressBook == null) return;

            _contactViewModel.AddNewTag(addressBook);
        }

        private void RefreshData()
        {
            panelContainer.SuspendLayout();
            panelContainer.Controls.Clear();

            if (_contactViewModel != null)
                foreach (var tag in _contactViewModel.Tags)
                {
                    var tagLabel = new TagLabel(this, tag);

                    panelContainer.Controls.Add(tagLabel);
                }

            panelContainer.ResumeLayout(true);
        }

        private class TagLabel : Label
        {
            private readonly TagsListControl _container;
            private readonly ContactTagViewModel _tagViewModel;

            public ContactTagViewModel TagViewModel
            {
                get { return _tagViewModel; }
            }

            public TagLabel(TagsListControl container, ContactTagViewModel tagViewModel)
            {
                _container = container;
                _tagViewModel = tagViewModel;

                Text = string.Format("#{0}", _tagViewModel.Name);
                ForeColor = _tagViewModel.Color;
                AutoSize = true;
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);

                if (e.Button != MouseButtons.Right) return;

                _container.OnMouseRightClick(this, e);
            }
        }
    }
}
