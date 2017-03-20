// 
// 2004 Copyright Ashley van Gerven (ashley_van_gerven@hotmail.com)
// > Feel free to use/modify/delete this control
// > Feel free to remove this copyright notice (if you *need* to!)
// > Feel free to let me know of any cool things you do with this code (ie improvements etc)
// 
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace ContactPoint.Plugins.HotKeys.Ui
{
	public class ShortcutInput : System.Windows.Forms.UserControl
	{
		#region Public Properties
		public byte CharCode 
		{
			get { return (byte)((string)DdnChars.SelectedItem)[1]; }
			set 
			{
				foreach (object item in DdnChars.Items)
				{
					if (item.ToString() == " " + (char)value)
					{
						DdnChars.SelectedItem = item;
						return;
					}
				}
			}
		}


		public byte Win32Modifiers
		{
			get
			{
				byte toReturn = 0;
				if (CbxShift.Checked)
					toReturn += ModShift;
				if (CbxControl.Checked)
					toReturn += ModControl;
				if (CbxAlt.Checked)
					toReturn += ModAlt;
				return toReturn;
			}
		}


		public Keys Keys
		{
			get
			{
				Keys k = (Keys) CharCode;
				if (CbxShift.Checked)
					k |= Keys.Shift;
				if (CbxControl.Checked)
					k |= Keys.Control;
				if (CbxAlt.Checked)
					k |= Keys.Alt;
				return k;
			}
			set
			{
				Keys k = (Keys) value;
				if (((int)k & (int)Keys.Shift) == (int)Keys.Shift)
					Shift = true;
				if (((int)k & (int)Keys.Control) == (int)Keys.Control)
					Control = true;
				if (((int)k & (int)Keys.Alt) == (int)Keys.Alt)
					Alt = true;

				CharCode = ShortcutInput.CharCodeFromKeys(k);
			}
		}


		public bool Shift
		{
			get { return CbxShift.Checked; }
			set { CbxShift.Checked = value; }
		}


		public bool Control
		{
			get { return CbxControl.Checked; }
			set { CbxControl.Checked = value; }
		}


		public bool Alt
		{
			get { return CbxAlt.Checked; }
			set { CbxAlt.Checked = value; }
		}


		public byte MinModifiers = 0;


		public bool IsValid
		{
			get
			{
				byte ModCount = 0;
				ModCount += (byte)((Shift) ? 1 : 0);
				ModCount += (byte)((Control) ? 1 : 0);
				ModCount += (byte)((Alt) ? 1 : 0);
                if (ModCount < MinModifiers)
					return false;
				else
					return true;
			}
		}
		#endregion

		private const byte ModAlt = 1, ModControl = 2, ModShift = 4, ModWin = 8;
		private System.Windows.Forms.CheckBox CbxShift;
		private System.Windows.Forms.CheckBox CbxControl;
		private System.Windows.Forms.CheckBox CbxAlt;
		private System.Windows.Forms.ComboBox DdnChars;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		public ShortcutInput()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			for (int i=65; i<91; i++)
				DdnChars.Items.Add(" " + (char)i);

			for (int i=48; i<58; i++)
				DdnChars.Items.Add(" " + (char)i);
			
			DdnChars.SelectedIndex = 0;
		}

		/// <summary>
		/// Calculates the Win32 Modifiers total for a Keys enum
		/// </summary>
		/// <param name="k">An instance of the Keys enumaration</param>
		/// <returns>The Win32 Modifiers total as required by RegisterHotKey</returns>
		public static byte Win32ModifiersFromKeys(Keys k)
		{
			byte total = 0;

			if (((int)k & (int)Keys.Shift) == (int)Keys.Shift)
				total += ModShift;
			if (((int)k & (int)Keys.Control) == (int)Keys.Control)
				total += ModControl;
			if (((int)k & (int)Keys.Alt) == (int)Keys.Alt)
				total += ModAlt;
			if (((int)k & (int)Keys.LWin) == (int)Keys.LWin)
				total += ModWin;

			return total;
		}

		/// <summary>
		/// Calculates the character code of alphanumeric key of the Keys enum instance
		/// </summary>
		/// <param name="k">An instance of the Keys enumaration</param>
		/// <returns>The character code of the alphanumeric key</returns>
		public static byte CharCodeFromKeys(Keys k)
		{
            if (((int)k & (int)Keys.Shift) == (int)Keys.Shift)
                k ^= Keys.Shift;
            if (((int)k & (int)Keys.Control) == (int)Keys.Control)
                k ^= Keys.Control;
            if (((int)k & (int)Keys.Alt) == (int)Keys.Alt)
                k ^= Keys.Alt;
            if (((int)k & (int)Keys.LWin) == (int)Keys.LWin)
                k ^= Keys.LWin;

            return (byte)k;
        }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.CbxShift = new System.Windows.Forms.CheckBox();
            this.CbxControl = new System.Windows.Forms.CheckBox();
            this.CbxAlt = new System.Windows.Forms.CheckBox();
            this.DdnChars = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // CbxShift
            // 
            this.CbxShift.Location = new System.Drawing.Point(8, 2);
            this.CbxShift.Name = "CbxShift";
            this.CbxShift.Size = new System.Drawing.Size(56, 24);
            this.CbxShift.TabIndex = 0;
            this.CbxShift.Text = "Shift";
            // 
            // CbxControl
            // 
            this.CbxControl.Location = new System.Drawing.Point(64, 2);
            this.CbxControl.Name = "CbxControl";
            this.CbxControl.Size = new System.Drawing.Size(64, 24);
            this.CbxControl.TabIndex = 1;
            this.CbxControl.Text = "Control";
            // 
            // CbxAlt
            // 
            this.CbxAlt.Location = new System.Drawing.Point(136, 2);
            this.CbxAlt.Name = "CbxAlt";
            this.CbxAlt.Size = new System.Drawing.Size(40, 24);
            this.CbxAlt.TabIndex = 2;
            this.CbxAlt.Text = "Alt";
            // 
            // DdnChars
            // 
            this.DdnChars.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DdnChars.Location = new System.Drawing.Point(184, 2);
            this.DdnChars.Name = "DdnChars";
            this.DdnChars.Size = new System.Drawing.Size(40, 21);
            this.DdnChars.TabIndex = 4;
            // 
            // ShortcutInput
            // 
            this.Controls.Add(this.DdnChars);
            this.Controls.Add(this.CbxAlt);
            this.Controls.Add(this.CbxControl);
            this.Controls.Add(this.CbxShift);
            this.Name = "ShortcutInput";
            this.Size = new System.Drawing.Size(236, 26);
            this.ResumeLayout(false);

		}
		#endregion
	}
}
