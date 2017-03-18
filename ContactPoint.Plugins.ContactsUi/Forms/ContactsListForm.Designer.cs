namespace ContactPoint.Plugins.ContactsUi.Forms
{
    partial class ContactsListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactsListForm));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.buttonAddContact = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.buttonRemoveContact = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.textBoxSearch = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.buttonSpecAny1 = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 38);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(982, 500);
            this.dataGridView.TabIndex = 1;
            // 
            // buttonAddContact
            // 
            this.buttonAddContact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddContact.Location = new System.Drawing.Point(12, 544);
            this.buttonAddContact.Name = "buttonAddContact";
            this.buttonAddContact.Palette = this.kryptonPalette1;
            this.buttonAddContact.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.buttonAddContact.Size = new System.Drawing.Size(157, 25);
            this.buttonAddContact.TabIndex = 2;
            this.buttonAddContact.Values.Text = "Add contact";
            this.buttonAddContact.Click += new System.EventHandler(this.buttonAddContact_Click);
            // 
            // kryptonPalette1
            // 
            this.kryptonPalette1.BasePaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            // 
            // buttonRemoveContact
            // 
            this.buttonRemoveContact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemoveContact.Location = new System.Drawing.Point(175, 544);
            this.buttonRemoveContact.Name = "buttonRemoveContact";
            this.buttonRemoveContact.Palette = this.kryptonPalette1;
            this.buttonRemoveContact.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.buttonRemoveContact.Size = new System.Drawing.Size(157, 25);
            this.buttonRemoveContact.TabIndex = 3;
            this.buttonRemoveContact.Values.Text = "Remove contact";
            this.buttonRemoveContact.Click += new System.EventHandler(this.buttonRemoveContact_Click);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearch.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this.buttonSpecAny1});
            this.textBoxSearch.Location = new System.Drawing.Point(12, 12);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Palette = this.kryptonPalette1;
            this.textBoxSearch.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.textBoxSearch.Size = new System.Drawing.Size(982, 20);
            this.textBoxSearch.TabIndex = 0;
            this.textBoxSearch.WordWrap = false;
            this.textBoxSearch.TextChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
            // 
            // buttonSpecAny1
            // 
            this.buttonSpecAny1.ToolTipTitle = "Clear text";
            this.buttonSpecAny1.Type = ComponentFactory.Krypton.Toolkit.PaletteButtonSpecStyle.Close;
            this.buttonSpecAny1.UniqueName = "F1DC47AEDF644E06A1944324DBAC4515";
            // 
            // ContactsListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1006, 581);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.buttonRemoveContact);
            this.Controls.Add(this.buttonAddContact);
            this.Controls.Add(this.dataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ContactsListForm";
            this.Palette = this.kryptonPalette1;
            this.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.Text = "Phone book";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonAddContact;
        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonRemoveContact;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox textBoxSearch;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny buttonSpecAny1;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette kryptonPalette1;
    }
}
