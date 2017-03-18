using ContactPoint.BaseDesign;

namespace ContactPoint.NotifyControls
{
    partial class AudioDevicesNotifyControl
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
            LineSeparator lineSeparator1;
            this.linkLabelUseDevices = new System.Windows.Forms.LinkLabel();
            this.labelAction = new System.Windows.Forms.Label();
            this.labelDevicesList = new System.Windows.Forms.Label();
            this.linkLabelClose = new System.Windows.Forms.LinkLabel();
            lineSeparator1 = new LineSeparator();
            this.SuspendLayout();
            // 
            // lineSeparator1
            // 
            lineSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            lineSeparator1.Location = new System.Drawing.Point(3, 100);
            lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            lineSeparator1.Name = "lineSeparator1";
            lineSeparator1.Size = new System.Drawing.Size(338, 2);
            lineSeparator1.TabIndex = 0;
            // 
            // linkLabelUseDevices
            // 
            this.linkLabelUseDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelUseDevices.AutoSize = true;
            this.linkLabelUseDevices.Location = new System.Drawing.Point(9, 109);
            this.linkLabelUseDevices.Name = "linkLabelUseDevices";
            this.linkLabelUseDevices.Size = new System.Drawing.Size(55, 13);
            this.linkLabelUseDevices.TabIndex = 1;
            this.linkLabelUseDevices.TabStop = true;
            this.linkLabelUseDevices.Text = "linkLabel1";
            this.linkLabelUseDevices.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabelUseDevices.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelUseDevices_LinkClicked);
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelAction.Location = new System.Drawing.Point(9, 8);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(41, 13);
            this.labelAction.TabIndex = 2;
            this.labelAction.Text = "label1";
            // 
            // labelDevicesList
            // 
            this.labelDevicesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDevicesList.Location = new System.Drawing.Point(9, 32);
            this.labelDevicesList.Name = "labelDevicesList";
            this.labelDevicesList.Size = new System.Drawing.Size(323, 65);
            this.labelDevicesList.TabIndex = 3;
            this.labelDevicesList.Text = "label1";
            // 
            // linkLabelClose
            // 
            this.linkLabelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelClose.AutoSize = true;
            this.linkLabelClose.Location = new System.Drawing.Point(277, 109);
            this.linkLabelClose.Name = "linkLabelClose";
            this.linkLabelClose.Size = new System.Drawing.Size(55, 13);
            this.linkLabelClose.TabIndex = 4;
            this.linkLabelClose.TabStop = true;
            this.linkLabelClose.Text = "linkLabel1";
            this.linkLabelClose.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabelClose.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClose_LinkClicked);
            // 
            // AudioDevicesNotifyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.linkLabelClose);
            this.Controls.Add(this.labelDevicesList);
            this.Controls.Add(this.labelAction);
            this.Controls.Add(this.linkLabelUseDevices);
            this.Controls.Add(lineSeparator1);
            this.Name = "AudioDevicesNotifyControl";
            this.Size = new System.Drawing.Size(344, 132);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.Label labelDevicesList;
        protected System.Windows.Forms.LinkLabel linkLabelUseDevices;
        protected System.Windows.Forms.LinkLabel linkLabelClose;

    }
}
