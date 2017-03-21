namespace ContactPoint.Plugins.ContactsUi.Controls
{
    partial class AddPhoneEmailControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonAddPhone = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.buttonAddEmail = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // buttonAddPhone
            // 
            this.buttonAddPhone.Location = new System.Drawing.Point(122, 3);
            this.buttonAddPhone.Name = "buttonAddPhone";
            this.buttonAddPhone.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            this.buttonAddPhone.TabIndex = 0;
            this.buttonAddPhone.Values.Text = "Add phone";
            this.buttonAddPhone.Click += new System.EventHandler(this.buttonAddPhone_Click);
            // 
            // buttonAddEmail
            // 
            this.buttonAddEmail.Location = new System.Drawing.Point(218, 3);
            this.buttonAddEmail.Name = "buttonAddEmail";
            this.buttonAddEmail.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            this.buttonAddEmail.TabIndex = 1;
            this.buttonAddEmail.Values.Text = "Add email";
            this.buttonAddEmail.Click += new System.EventHandler(this.buttonAddEmail_Click);
            // 
            // AddPhoneEmailControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.buttonAddEmail);
            this.Controls.Add(this.buttonAddPhone);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "AddPhoneEmailControl";
            this.Size = new System.Drawing.Size(340, 31);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonAddPhone;
        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonAddEmail;
    }
}
