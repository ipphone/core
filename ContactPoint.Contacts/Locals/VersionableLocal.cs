using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Contacts.Locals
{
    internal class VersionableLocal : IVersionable
    {
        private readonly IAddressBookLocal _addressBookLocal;
        private bool _isChanged;

        public virtual bool IsChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; }
        }

        public string VersionKey { get; set; }

        public IVersionGenerator VersionGenerator
        {
            get { return _addressBookLocal.VersionGenerator; }
        }

        public VersionableLocal(IAddressBookLocal addressBookLocal)
        {
            _addressBookLocal = addressBookLocal;
        }

        public void IncrementVersion()
        {
            VersionKey = VersionGenerator.ConvertKeyToString(VersionGenerator.GenerateNextVersion(VersionGenerator.GetKeyFromString(VersionKey)));
            IsChanged = false;
        }

        public void ResetIsChanged()
        {
            IsChanged = false;
        }

        object IVersionable.VersionKey { get { return VersionGenerator.GetKeyFromString(VersionKey); } }
    }
}
