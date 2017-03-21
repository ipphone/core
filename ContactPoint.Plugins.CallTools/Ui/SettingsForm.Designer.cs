using ContactPoint.BaseDesign;

namespace ContactPoint.Plugins.CallTools.Ui
{
    partial class SettingsForm
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
            System.Windows.Forms.Label AutoAnswerLabel;
            System.Windows.Forms.Label OneLineServiceLabel;
            System.Windows.Forms.Label ShowIncomingCallWindowLabel;
            this.lineSeparator1 = new LineSeparator();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.AutoAnswerCheckBox = new System.Windows.Forms.CheckBox();
            this.OneLineServiceCheckBox = new System.Windows.Forms.CheckBox();
            this.ShowIncomingCallWindowCheckBox = new System.Windows.Forms.CheckBox();
            this.DontHideCallWindowCheckBox = new System.Windows.Forms.CheckBox();
            this.DontHideCallWindowLabel = new System.Windows.Forms.Label();
            AutoAnswerLabel = new System.Windows.Forms.Label();
            OneLineServiceLabel = new System.Windows.Forms.Label();
            ShowIncomingCallWindowLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AutoAnswerLabel
            // 
            AutoAnswerLabel.AutoSize = true;
            AutoAnswerLabel.Location = new System.Drawing.Point(22, 33);
            AutoAnswerLabel.Name = "AutoAnswerLabel";
            AutoAnswerLabel.Size = new System.Drawing.Size(66, 13);
            AutoAnswerLabel.TabIndex = 1;
            AutoAnswerLabel.Text = "Auto answer";
            // 
            // OneLineServiceLabel
            // 
            OneLineServiceLabel.AutoSize = true;
            OneLineServiceLabel.Location = new System.Drawing.Point(22, 66);
            OneLineServiceLabel.Name = "OneLineServiceLabel";
            OneLineServiceLabel.Size = new System.Drawing.Size(87, 13);
            OneLineServiceLabel.TabIndex = 5;
            OneLineServiceLabel.Text = "One Line service";
            // 
            // ShowIncomingCallWindowLabel
            // 
            ShowIncomingCallWindowLabel.AutoSize = true;
            ShowIncomingCallWindowLabel.Location = new System.Drawing.Point(22, 101);
            ShowIncomingCallWindowLabel.Name = "ShowIncomingCallWindowLabel";
            ShowIncomingCallWindowLabel.Size = new System.Drawing.Size(137, 13);
            ShowIncomingCallWindowLabel.TabIndex = 12;
            ShowIncomingCallWindowLabel.Text = "Show incoming call window";
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.Location = new System.Drawing.Point(18, 183);
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            this.lineSeparator1.Size = new System.Drawing.Size(229, 2);
            this.lineSeparator1.TabIndex = 6;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(91, 197);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 7;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(172, 197);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // AutoAnswerCheckBox
            // 
            this.AutoAnswerCheckBox.AutoSize = true;
            this.AutoAnswerCheckBox.Location = new System.Drawing.Point(222, 32);
            this.AutoAnswerCheckBox.Name = "AutoAnswerCheckBox";
            this.AutoAnswerCheckBox.Size = new System.Drawing.Size(15, 14);
            this.AutoAnswerCheckBox.TabIndex = 9;
            this.AutoAnswerCheckBox.UseVisualStyleBackColor = true;
            // 
            // OneLineServiceCheckBox
            // 
            this.OneLineServiceCheckBox.AutoSize = true;
            this.OneLineServiceCheckBox.Location = new System.Drawing.Point(222, 66);
            this.OneLineServiceCheckBox.Name = "OneLineServiceCheckBox";
            this.OneLineServiceCheckBox.Size = new System.Drawing.Size(15, 14);
            this.OneLineServiceCheckBox.TabIndex = 11;
            this.OneLineServiceCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShowIncomingCallWindowCheckBox
            // 
            this.ShowIncomingCallWindowCheckBox.AutoSize = true;
            this.ShowIncomingCallWindowCheckBox.Location = new System.Drawing.Point(222, 101);
            this.ShowIncomingCallWindowCheckBox.Name = "ShowIncomingCallWindowCheckBox";
            this.ShowIncomingCallWindowCheckBox.Size = new System.Drawing.Size(15, 14);
            this.ShowIncomingCallWindowCheckBox.TabIndex = 13;
            this.ShowIncomingCallWindowCheckBox.UseVisualStyleBackColor = true;
            // 
            // DontHideCallWindowCheckBox
            // 
            this.DontHideCallWindowCheckBox.AutoSize = true;
            this.DontHideCallWindowCheckBox.Location = new System.Drawing.Point(222, 132);
            this.DontHideCallWindowCheckBox.Name = "DontHideCallWindowCheckBox";
            this.DontHideCallWindowCheckBox.Size = new System.Drawing.Size(15, 14);
            this.DontHideCallWindowCheckBox.TabIndex = 15;
            this.DontHideCallWindowCheckBox.UseVisualStyleBackColor = true;
            // 
            // DontHideCallWindowLabel
            // 
            this.DontHideCallWindowLabel.AutoSize = true;
            this.DontHideCallWindowLabel.Location = new System.Drawing.Point(22, 133);
            this.DontHideCallWindowLabel.Name = "DontHideCallWindowLabel";
            this.DontHideCallWindowLabel.Size = new System.Drawing.Size(113, 13);
            this.DontHideCallWindowLabel.TabIndex = 16;
            this.DontHideCallWindowLabel.Text = "Don\'t hide call window";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(269, 232);
            this.Controls.Add(this.DontHideCallWindowLabel);
            this.Controls.Add(this.DontHideCallWindowCheckBox);
            this.Controls.Add(this.ShowIncomingCallWindowCheckBox);
            this.Controls.Add(ShowIncomingCallWindowLabel);
            this.Controls.Add(this.OneLineServiceCheckBox);
            this.Controls.Add(this.AutoAnswerCheckBox);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.lineSeparator1);
            this.Controls.Add(OneLineServiceLabel);
            this.Controls.Add(AutoAnswerLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Call tools settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LineSeparator lineSeparator1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox AutoAnswerCheckBox;
        private System.Windows.Forms.CheckBox OneLineServiceCheckBox;
        private System.Windows.Forms.CheckBox ShowIncomingCallWindowCheckBox;
        private System.Windows.Forms.CheckBox DontHideCallWindowCheckBox;
        private System.Windows.Forms.Label DontHideCallWindowLabel;
    }
}
