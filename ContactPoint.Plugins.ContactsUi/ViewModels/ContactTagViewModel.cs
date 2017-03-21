using System.Drawing;
using System.Globalization;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Plugins.ContactsUi.ViewModels
{
    public class ContactTagViewModel : ViewModel
    {
        private readonly IContactTagLocal _tag;

        public string Name { get; set; }
        public Color Color { get; set; }

        public bool ReadOnly { get; private set; }

        public IContactTagLocal Tag
        {
            get { return _tag; }
        }

        public ContactTagViewModel(IContactTagLocal tag)
        {
            _tag = tag;

            Name = tag.Name;
            Color = ParseColor(tag.Color);
            ReadOnly = tag.AddressBook.ReadOnly;
        }

        public void SubmitTag()
        {
            _tag.Name = Name;
            _tag.Color = string.Format("#{0}{1}{2}", Color.R.ToString("X2"), Color.G.ToString("X2"), Color.B.ToString("X2"));

            _tag.Submit();
        }

        public static Color ParseColor(string color)
        {
            if (string.IsNullOrEmpty(color) || !color.StartsWith("#") || color.Length != 7) return Color.Black;

            var r = byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber);
            var g = byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber);
            var b = byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber);

            return Color.FromArgb(r, g, b);
        }
    }

    public class ContactApplyTagMessage
    {
        public ContactTagViewModel ContactTagViewModel { get; set; }

        public ContactApplyTagMessage(ContactTagViewModel viewModel)
        {
            ContactTagViewModel = viewModel;
        }
    }
}
