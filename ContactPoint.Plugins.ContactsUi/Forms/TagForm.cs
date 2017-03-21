using System;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ContactPoint.Plugins.ContactsUi.ViewModels;

namespace ContactPoint.Plugins.ContactsUi.Forms
{
  public partial class TagForm : KryptonForm
    {
        private readonly ContactTagViewModel _contactTagViewModel;

        public TagForm(ContactTagViewModel contactTagViewModel)
        {
            _contactTagViewModel = contactTagViewModel;

            InitializeComponent();

            ToolStripManager.Renderer = new ToolStripSystemRenderer();

            colorPicker.SelectedColor = _contactTagViewModel.Color;
            textboxName.Text = _contactTagViewModel.Name;

            if (string.IsNullOrWhiteSpace(_contactTagViewModel.Name)) Text = "New tag";
            else Text = String.Format("Editing tag: {0}", _contactTagViewModel.Name);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            _contactTagViewModel.Name = textboxName.Text;
            _contactTagViewModel.Color = colorPicker.SelectedColor;

            _contactTagViewModel.SubmitTag();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
