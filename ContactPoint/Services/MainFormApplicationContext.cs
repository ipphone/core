using System.Windows.Forms;
using ContactPoint.BaseDesign;
using ContactPoint.Forms;

namespace ContactPoint.Services
{
    class MainFormApplicationContext : ApplicationContext
    {
        public MainForm ContactPointForm => (MainForm)MainForm;

        public MainFormApplicationContext() : base(new MainForm())
        {
            SyncUi.Initialize(MainForm);
        }
    }
}
