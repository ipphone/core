using System;
using System.Drawing;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace ContactPoint.Controls
{
    class MainFormShadeControl : TransparentPanel
    {
        public event EventHandler ButtonClicked;

        public override Color BackColor
        {
            get { return Color.Transparent; }
            set { }
        }

        public override DockStyle Dock
        {
            get { return DockStyle.Fill; }
            set { }
        }

        public MainFormShadeControl()
        {
            #region Button initialization

            var button = new KryptonButton
            {
                ButtonStyle = ButtonStyle.Standalone,
                Location = new Point(76, 131),
                Name = "button"
            };

            button.OverrideDefault.Back.Color1 = Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            button.OverrideDefault.Back.Color2 = Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            button.OverrideDefault.Back.ColorAlign = PaletteRectangleAlign.Control;
            button.OverrideDefault.Back.ColorStyle = PaletteColorStyle.Linear;
            button.OverrideDefault.Border.Color1 = Color.LightGray;
            button.OverrideDefault.Border.Color2 = Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            button.OverrideDefault.Border.ColorStyle = PaletteColorStyle.Linear;
            button.OverrideDefault.Border.DrawBorders = ((PaletteDrawBorders)((((PaletteDrawBorders.Top | PaletteDrawBorders.Bottom)
                        | PaletteDrawBorders.Left)
                        | PaletteDrawBorders.Right)));
            button.OverrideDefault.Border.Rounding = 6;
            button.OverrideDefault.Content.LongText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.OverrideDefault.Content.LongText.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            button.OverrideDefault.Content.ShortText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.OverrideDefault.Content.ShortText.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            button.OverrideFocus.Content.LongText.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            button.OverrideFocus.Content.ShortText.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            button.PaletteMode = PaletteMode.Global;
            button.Size = new Size(70, 66);
            button.StateCommon.Back.Color1 = Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            button.StateCommon.Back.Color2 = Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            button.StateCommon.Back.ColorAlign = PaletteRectangleAlign.Control;
            button.StateCommon.Back.ColorStyle = PaletteColorStyle.Linear;
            button.StateCommon.Border.Color1 = Color.LightGray;
            button.StateCommon.Border.Color2 = Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            button.StateCommon.Border.ColorStyle = PaletteColorStyle.Linear;
            button.StateCommon.Border.DrawBorders = ((PaletteDrawBorders)((((PaletteDrawBorders.Top | PaletteDrawBorders.Bottom)
                        | PaletteDrawBorders.Left)
                        | PaletteDrawBorders.Right)));
            button.StateCommon.Border.Rounding = 6;
            button.StateCommon.Content.LongText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StateCommon.Content.LongText.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            button.StateCommon.Content.ShortText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StateCommon.Content.ShortText.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            button.StateDisabled.Back.Color1 = Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            button.StateDisabled.Back.Color2 = Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            button.StateDisabled.Back.ColorAlign = PaletteRectangleAlign.Control;
            button.StateDisabled.Back.ColorStyle = PaletteColorStyle.Solid;
            button.StateDisabled.Border.Color1 = Color.LightGray;
            button.StateDisabled.Border.Color2 = Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            button.StateDisabled.Border.ColorStyle = PaletteColorStyle.Linear;
            button.StateDisabled.Border.DrawBorders = ((PaletteDrawBorders)((((PaletteDrawBorders.Top | PaletteDrawBorders.Bottom)
                        | PaletteDrawBorders.Left)
                        | PaletteDrawBorders.Right)));
            button.StateDisabled.Border.Rounding = 6;
            button.StateDisabled.Content.LongText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StateDisabled.Content.ShortText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StateNormal.Back.Color1 = Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            button.StateNormal.Back.Color2 = Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            button.StateNormal.Back.ColorStyle = PaletteColorStyle.Linear;
            button.StateNormal.Border.Color1 = Color.LightGray;
            button.StateNormal.Border.Color2 = Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            button.StateNormal.Border.ColorStyle = PaletteColorStyle.Linear;
            button.StateNormal.Border.DrawBorders = ((PaletteDrawBorders)((((PaletteDrawBorders.Top | PaletteDrawBorders.Bottom)
                        | PaletteDrawBorders.Left)
                        | PaletteDrawBorders.Right)));
            button.StateNormal.Border.Rounding = 6;
            button.StateNormal.Content.LongText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StateNormal.Content.ShortText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StatePressed.Back.Color1 = Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            button.StatePressed.Back.Color2 = Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            button.StatePressed.Back.ColorAlign = PaletteRectangleAlign.Control;
            button.StatePressed.Back.ColorStyle = PaletteColorStyle.Linear;
            button.StatePressed.Border.Color1 = Color.LightGray;
            button.StatePressed.Border.Color2 = Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            button.StatePressed.Border.ColorStyle = PaletteColorStyle.Linear;
            button.StatePressed.Border.DrawBorders = ((PaletteDrawBorders)((((PaletteDrawBorders.Top | PaletteDrawBorders.Bottom)
                        | PaletteDrawBorders.Left)
                        | PaletteDrawBorders.Right)));
            button.StatePressed.Border.Rounding = 6;
            button.StatePressed.Content.LongText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StatePressed.Content.ShortText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StateTracking.Back.Color1 = Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            button.StateTracking.Back.Color2 = Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            button.StateTracking.Back.ColorStyle = PaletteColorStyle.Linear;
            button.StateTracking.Border.Color1 = Color.LightGray;
            button.StateTracking.Border.Color2 = Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            button.StateTracking.Border.ColorStyle = PaletteColorStyle.Linear;
            button.StateTracking.Border.DrawBorders = ((PaletteDrawBorders)((((PaletteDrawBorders.Top | PaletteDrawBorders.Bottom)
                        | PaletteDrawBorders.Left)
                        | PaletteDrawBorders.Right)));
            button.StateTracking.Border.Rounding = 6;
            button.StateTracking.Content.LongText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.StateTracking.Content.ShortText.Color1 = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            button.TabIndex = 24;
            button.TabStop = false;
            button.Values.ExtraText = "";
            button.Values.Image = Properties.Resources.play;
            button.Values.ImageStates.ImageCheckedNormal = null;
            button.Values.ImageStates.ImageCheckedPressed = null;
            button.Values.ImageStates.ImageCheckedTracking = null;
            button.Values.Text = "";
            button.Visible = true;
            button.Click += new EventHandler(OnButtonClick);

            #endregion

            button.TabStop = false;

            Opacity = 70;
            FillColor = Color.Black;
            Visible = true;

            Controls.Add(button);
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            ButtonClicked?.Invoke(sender, e);
        }
    }
}
