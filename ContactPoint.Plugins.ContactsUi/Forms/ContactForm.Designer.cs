using ContactPoint.BaseDesign;
using ContactPoint.Plugins.ContactsUi.Controls;

namespace ContactPoint.Plugins.ContactsUi.Forms
{
    partial class ContactForm
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
            LineSeparator lineSeparator1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.labelShowedName = new System.Windows.Forms.Label();
            this.dropDownNewContactInfos = new ComponentFactory.Krypton.Toolkit.KryptonDropButton();
            this.kryptonPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.buttonOk = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.buttonCancel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.tagsListControl1 = new ContactPoint.Plugins.ContactsUi.Controls.TagsListControl();
            lineSeparator1 = new LineSeparator();
            this.SuspendLayout();
            // 
            // lineSeparator1
            // 
            lineSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            lineSeparator1.Location = new System.Drawing.Point(12, 71);
            lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            lineSeparator1.Name = "lineSeparator1";
            lineSeparator1.Size = new System.Drawing.Size(467, 2);
            lineSeparator1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Location = new System.Drawing.Point(12, 79);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(467, 488);
            this.tabControl1.TabIndex = 1;
            // 
            // labelShowedName
            // 
            this.labelShowedName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelShowedName.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelShowedName.Location = new System.Drawing.Point(12, 9);
            this.labelShowedName.Name = "labelShowedName";
            this.labelShowedName.Size = new System.Drawing.Size(467, 33);
            this.labelShowedName.TabIndex = 0;
            this.labelShowedName.Text = "label1";
            // 
            // dropDownNewContactInfos
            // 
            this.dropDownNewContactInfos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dropDownNewContactInfos.Location = new System.Drawing.Point(12, 573);
            this.dropDownNewContactInfos.Name = "dropDownNewContactInfos";
            this.dropDownNewContactInfos.Palette = this.kryptonPalette1;
            this.dropDownNewContactInfos.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.dropDownNewContactInfos.Size = new System.Drawing.Size(151, 25);
            this.dropDownNewContactInfos.Splitter = false;
            this.dropDownNewContactInfos.TabIndex = 2;
            this.dropDownNewContactInfos.Values.Text = "Add contact details";
            // 
            // kryptonPalette1
            // 
            this.kryptonPalette1.BasePaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(291, 573);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Palette = this.kryptonPalette1;
            this.buttonOk.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Values.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(387, 573);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Palette = this.kryptonPalette1;
            this.buttonCancel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Values.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tagsListControl1
            // 
            this.tagsListControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagsListControl1.ContactsManager = null;
            this.tagsListControl1.ContactViewModel = null;
            this.tagsListControl1.Location = new System.Drawing.Point(12, 45);
            this.tagsListControl1.Name = "tagsListControl1";
            this.tagsListControl1.Size = new System.Drawing.Size(467, 20);
            this.tagsListControl1.TabIndex = 5;
            // 
            // ContactForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(491, 610);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.dropDownNewContactInfos);
            this.Controls.Add(this.tagsListControl1);
            this.Controls.Add(this.labelShowedName);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(lineSeparator1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "ContactForm";
            this.Palette = this.kryptonPalette1;
            this.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.Text = "ContactForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label labelShowedName;
        private TagsListControl tagsListControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonDropButton dropDownNewContactInfos;
        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonOk;
        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette kryptonPalette1;

    }
}
