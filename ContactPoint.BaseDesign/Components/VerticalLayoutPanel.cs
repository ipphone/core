using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ContactPoint.BaseDesign.Components
{
    public class VerticalLayoutPanel : FlowLayoutPanel
    {
        [Browsable(true),
        DefaultValue(true)]
        public bool UseZebra
        {
            get; set;
        }

        [Browsable(true),
        DefaultValue(typeof(Color), "White")]
        public Color ZebraColor1
        {
            get; set;
        }

        [Browsable(true),
        DefaultValue(typeof(Color), "Gainsboro")]
        public Color ZebraColor2
        {
            get; set;
        }

        public VerticalLayoutPanel()
        {
            this.SuspendLayout();

            this.AutoScroll = true;
            this.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.UseZebra = true;

            this.ZebraColor1 = Color.White;
            this.ZebraColor2 = Color.Gainsboro;

            this.HScroll = false;
            this.HorizontalScroll.Enabled = false;
            this.HorizontalScroll.Visible = false;

            this.VScroll = true;
            this.VerticalScroll.Enabled = true;
            this.VerticalScroll.Visible = true;

            this.ResumeLayout(false);
        }

        #region Logic

        private void LayoutInternal()
        {
            foreach (Control c in this.Controls)
                c.Width = this.ClientSize.Width - (c.Margin.Left + c.Margin.Right);
        }

        private void UpdateControlsBg()
        {
            if (!this.UseZebra)
                return;

            int i = 0;

            foreach (Control c in this.Controls)
                c.BackColor = (i++) % 2 == 0 ? this.ZebraColor1 : this.ZebraColor2;
        }

        #endregion

        #region Overrides

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            this.UpdateControlsBg();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            this.UpdateControlsBg();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            this.LayoutInternal();
        }

        #endregion
    }
}
