using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace ContactPoint.BaseDesign.Components
{
    public class SelectableControl : UserControl
    {
        private bool _isSelected;
        private Color _backColor;
        private bool _isSelectable;

        public event EventHandler SelectionChanged;

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                this._backColor = value;

                UpdateBackColor();
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(Color), "LightBlue")]
        public Color SelectedColor
        {
            get; set;
        }

        [Browsable(true)]
        [DefaultValue(true)]
        public bool IsSelectable
        {
            get { return _isSelectable; }
            set
            {
                this._isSelectable = value;

                this.SetStyle(ControlStyles.Selectable, value);
            }
        }

        [Browsable(false)]
        public bool IsSelected
        {
            get { return this._isSelected; }
            set
            {
                bool raiseEvent = this._isSelected != value;

                this._isSelected = value;

                OnSelectionRedraw();

                if (raiseEvent && SelectionChanged != null)
                    SelectionChanged.Invoke(this, new EventArgs());
            }
        }

        public SelectableControl()
        {
            this.SelectedColor = Color.LightBlue;
            this.IsSelectable = true;

            this.Load += SelectableControl_Load;

            this.GotFocus += SelectableControl_GotFocus;
            this.LostFocus += SelectableControl_LostFocus;
        }

        protected virtual void OnSelectionRedraw()
        {
            UpdateBackColor();
        }

        private void UpdateBackColor()
        {
            base.BackColor = this.IsSelected && this.IsSelectable ? this.SelectedColor : this._backColor;
        }

        void SelectableControl_LostFocus(object sender, EventArgs e)
        {
            this.IsSelected = false;
        }

        void SelectableControl_GotFocus(object sender, EventArgs e)
        {
            this.IsSelected = true;
        }

        void SelectableControl_Load(object sender, EventArgs e)
        {
            foreach (Control control in Controls)
            {
                control.GotFocus += control_GotFocus;
                control.LostFocus += control_LostFocus;

                control.Click += new EventHandler(control_Click);
                control.DoubleClick += new EventHandler(control_DoubleClick);
            }
        }

        void control_DoubleClick(object sender, EventArgs e)
        {
            this.Focus();

            OnDoubleClick(e);
        }

        void control_Click(object sender, EventArgs e)
        {
            this.Focus();

            OnClick(e);
        }

        void control_LostFocus(object sender, EventArgs e)
        {
            this.IsSelected = false;
        }

        void control_GotFocus(object sender, EventArgs e)
        {
            this.IsSelected = true;
        }
    }
}
