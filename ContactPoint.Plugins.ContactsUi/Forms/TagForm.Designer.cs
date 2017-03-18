namespace ContactPoint.Plugins.ContactsUi.Forms
{
    partial class TagForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagForm));
            this.kryptonPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.textboxName = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.colorPicker = new ComponentFactory.Krypton.Toolkit.KryptonColorButton();
            this.buttonCancel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.buttonOk = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(38, 13);
            label1.TabIndex = 1;
            label1.Text = "Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(89, 15);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(14, 13);
            label2.TabIndex = 2;
            label2.Text = "#";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 44);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(55, 13);
            label3.TabIndex = 4;
            label3.Text = "Tag color:";
            // 
            // kryptonPalette1
            // 
            this.kryptonPalette1.BasePaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            // 
            // textboxName
            // 
            this.textboxName.Location = new System.Drawing.Point(105, 12);
            this.textboxName.Name = "textboxName";
            this.textboxName.Palette = this.kryptonPalette1;
            this.textboxName.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.textboxName.Size = new System.Drawing.Size(190, 20);
            this.textboxName.TabIndex = 0;
            // 
            // colorPicker
            // 
            this.colorPicker.Location = new System.Drawing.Point(105, 38);
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.Palette = this.kryptonPalette1;
            this.colorPicker.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.colorPicker.SelectedColor = System.Drawing.Color.Black;
            this.colorPicker.Size = new System.Drawing.Size(190, 25);
            this.colorPicker.Splitter = false;
            this.colorPicker.TabIndex = 1;
            this.colorPicker.Values.Text = "Choose tag color";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(205, 79);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Palette = this.kryptonPalette1;
            this.buttonCancel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Values.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(105, 79);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Palette = this.kryptonPalette1;
            this.buttonOk.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.buttonOk.Size = new System.Drawing.Size(94, 25);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Values.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // TagForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(307, 116);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(label3);
            this.Controls.Add(this.colorPicker);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.textboxName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TagForm";
            this.Palette = this.kryptonPalette1;
            this.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.Text = "Edit tag";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPalette kryptonPalette1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox textboxName;
        private ComponentFactory.Krypton.Toolkit.KryptonColorButton colorPicker;
        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonOk;
    }
}
