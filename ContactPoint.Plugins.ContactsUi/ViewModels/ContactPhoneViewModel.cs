using ContactPoint.Common;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Plugins.ContactsUi.ViewModels
{
    internal class ContactPhoneViewModel : ViewModel
    {
        private readonly ICallManager _callManager;
        public long Id { get; private set; }
        public string Number { get; set; }
        public string Comment { get; set; }

        public ContactPhoneViewModel(ICallManager callManager, IContactPhoneLocal contactPhone)
        {
            _callManager = callManager;
            Id = contactPhone.Id;
            Number = contactPhone.Number;
            Comment = contactPhone.Comment;
        }

        public void UnWrap(IContactPhoneLocal contactPhone)
        {
            contactPhone.Number = Number;
            contactPhone.Comment = Comment;
        }

        public void Call()
        {
            _callManager.MakeCall(Number);
        }
    }
}
