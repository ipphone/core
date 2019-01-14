using ContactPoint.BaseDesign;

namespace ContactPoint.Plugins.WebBrowser
{
    partial class WebBrowserSettings
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
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebBrowserSettings));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.lineSeparator1 = new LineSeparator();
            this.txtIncomingUrl = new System.Windows.Forms.TextBox();
            this.comboBoxWebBrowser = new System.Windows.Forms.ComboBox();
            this.txtTriggerHeaders = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(494, 230);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(413, 230);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lineSeparator1.Location = new System.Drawing.Point(12, 222);
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            this.lineSeparator1.Size = new System.Drawing.Size(557, 2);
            this.lineSeparator1.TabIndex = 10;
            // 
            // txtIncomingUrl
            // 
            this.txtIncomingUrl.Location = new System.Drawing.Point(12, 140);
            this.txtIncomingUrl.Name = "txtIncomingUrl";
            this.txtIncomingUrl.Size = new System.Drawing.Size(557, 20);
            this.txtIncomingUrl.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 124);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(97, 13);
            label4.TabIndex = 14;
            label4.Text = "Incoming call URL:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(12, 167);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(198, 26);
            label5.TabIndex = 15;
            label5.Text = "%number% - phone number\r\n%header_name% - value from sip header";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 24);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(75, 13);
            label1.TabIndex = 16;
            label1.Text = "WEB browser:";
            // 
            // comboBoxWebBrowser
            // 
            this.comboBoxWebBrowser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWebBrowser.FormattingEnabled = true;
            this.comboBoxWebBrowser.Location = new System.Drawing.Point(12, 40);
            this.comboBoxWebBrowser.Name = "comboBoxWebBrowser";
            this.comboBoxWebBrowser.Size = new System.Drawing.Size(254, 21);
            this.comboBoxWebBrowser.TabIndex = 0;
            // 
            // txtTriggerHeaders
            // 
            this.txtTriggerHeaders.Location = new System.Drawing.Point(12, 89);
            this.txtTriggerHeaders.Name = "txtTriggerHeaders";
            this.txtTriggerHeaders.Size = new System.Drawing.Size(557, 20);
            this.txtTriggerHeaders.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 73);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(177, 13);
            label2.TabIndex = 18;
            label2.Text = "Trigger headers (comma separated):";
            // 
            // WebBrowserSettings
            // 
            this.ClientSize = new System.Drawing.Size(581, 265);
            this.Controls.Add(this.txtTriggerHeaders);
            this.Controls.Add(label2);
            this.Controls.Add(this.comboBoxWebBrowser);
            this.Controls.Add(label1);
            this.Controls.Add(label5);
            this.Controls.Add(this.txtIncomingUrl);
            this.Controls.Add(label4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.lineSeparator1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WebBrowserSettings";
            this.ShowInTaskbar = false;
            this.Text = "WEB Browser plugin settings";
            this.Load += new System.EventHandler(this.WebBrowserSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private LineSeparator lineSeparator1;
        private System.Windows.Forms.TextBox txtIncomingUrl;
        private System.Windows.Forms.ComboBox comboBoxWebBrowser;
        private System.Windows.Forms.TextBox txtTriggerHeaders;

    }
}
