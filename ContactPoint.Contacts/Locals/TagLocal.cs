using System;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Contacts.Locals
{
    internal class TagLocal : VersionableLocal, IContactTagLocal, IEntity
    {
        private readonly ContactsManager _contactsManager;
        private string _name = String.Empty;
        private string _color = String.Empty;

        public event Action<IContactTagLocal> Changed;

        public long Id { get; set; }
        public string Key { get; set; }
        public bool IsDeleted { get; set; }

        public string Name
        {
            get { return _name; }
            set { IsChanged |= _name != value; _name = value; }
        }

        public string Color
        {
            get { return _color; }
            set { IsChanged |= _color != value; _color = value; }
        }

        internal AddressBookLocal AddressBook { get; private set; }

        IAddressBookLocal IContactTagLocal.AddressBook { get { return AddressBook; } }

        public TagLocal(AddressBookLocal addressBook, ContactsManager contactsManager)
            : this(-1, addressBook, contactsManager)
        { }

        public TagLocal(long id, AddressBookLocal addressBook, ContactsManager contactsManager)
            : base(addressBook)
        {
            _contactsManager = contactsManager;
            Id = id;
            AddressBook = addressBook;
        }

        public TagLocal(IContactTag source, AddressBookLocal addressBook, ContactsManager contactsManager)
            : this(-1, addressBook, contactsManager)
        {
            Key = source.Key;

            UpdateFrom(source);
        }

        public void UpdateFrom(IContactTag source)
        {
            Name = source.Name;
            Color = source.Color;
            VersionKey = source.VersionKey == null ? String.Empty : source.VersionKey.ToString();
            Key = source.Key;

            IsChanged = false;
        }

        public void UpdateKey(string key)
        {
            if (string.IsNullOrEmpty(Key)) Key = key;
        }

        public void Submit()
        {
            Submit(true);
        }

        internal void Submit(bool repairDeleted)
        {
            if (!IsChanged && Id > 0) return;

            IncrementVersion();

            AddressBook.InsertOrUpdateTag(this);

            _contactsManager.InsertOrUpdateTag(this, repairDeleted);

            if (Changed != null) Changed.Invoke(this);
        }

        internal void RaiseChanged()
        {
            if (Changed != null) Changed.Invoke(this);
        }

        public void Remove()
        {
            _contactsManager.RemoveTag(this);
        }

        public override bool Equals(object obj)
        {
            var tag = obj as TagLocal;
            if (tag == null) return base.Equals(obj);

            return tag.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
