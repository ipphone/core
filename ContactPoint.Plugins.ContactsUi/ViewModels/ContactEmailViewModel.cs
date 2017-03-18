using System.Diagnostics;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Plugins.ContactsUi.ViewModels
{
    internal class ContactEmailViewModel : ViewModel
    {
        public long Id { get; private set; }
        public string Email { get; set; }
        public string Comment { get; set; }

        public ContactEmailViewModel(IContactEmailLocal contactEmail)
        {
            Id = contactEmail.Id;
            Email = contactEmail.Email;
            Comment = contactEmail.Comment;
        }

        public void UnWrap(IContactEmailLocal contactEmail)
        {
            contactEmail.Email = Email;
            contactEmail.Comment = Comment;
        }

        public void SendEmail()
        {
            Process.Start(string.Format("mailto:{0}", Email));
        }
    }

    internal class ContactEmailRemoveMessage
    {
        public ContactEmailViewModel ContactEmailViewModel { get; private set; }

        public ContactEmailRemoveMessage(ContactEmailViewModel viewModel)
        {
            ContactEmailViewModel = viewModel;
        }
    }
}
