using System;

namespace ContactPoint.BaseDesign.Components
{
    public class SelectableVerticalLayoutPanel : VerticalLayoutPanel
    {
        private SelectableControl _selectedControl;

        public event EventHandler SelectionChanged;

        public SelectableControl SelectedControl
        {
            get { return this._selectedControl; }
        }

        public SelectableVerticalLayoutPanel()
        {
            this.ControlAdded += SelectableVerticalLayoutPanel_ControlAdded;
            this.ControlRemoved += SelectableVerticalLayoutPanel_ControlRemoved;
        }

        void SelectableVerticalLayoutPanel_ControlAdded(object sender, System.Windows.Forms.ControlEventArgs e)
        {
            var control = e.Control as SelectableControl;

            if (control != null)
                control.SelectionChanged += control_SelectionChanged;
        }

        void SelectableVerticalLayoutPanel_ControlRemoved(object sender, System.Windows.Forms.ControlEventArgs e)
        {
            var control = e.Control as SelectableControl;

            if (control != null)
                control.SelectionChanged -= control_SelectionChanged;
        }

        void control_SelectionChanged(object sender, EventArgs e)
        {
            var control = sender as SelectableControl;

            if (control != null && control.IsSelected)
            {
                this._selectedControl = control;

                if (SelectionChanged != null)
                    SelectionChanged.Invoke(this, e);
            }
        }
    }
}
