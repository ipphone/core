using System.Windows.Forms;
using ContactPoint.BaseDesign;

namespace ContactPoint
{
    class ContactPointApplicationContext : ApplicationContext
    {
        public MainForm ContactPointForm => (MainForm)MainForm;

        public ContactPointApplicationContext() : base(new MainForm())
        {
            SyncUi.Initialize(MainForm);
        }
    }
}
