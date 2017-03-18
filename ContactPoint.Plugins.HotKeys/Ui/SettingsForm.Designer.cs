using ContactPoint.BaseDesign;

namespace ContactPoint.Plugins.HotKeys.Ui
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.lineSeparator1 = new LineSeparator();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.shortcutInputHold = new ShortcutInput();
            this.shortcutInputDrop = new ShortcutInput();
            this.shortcutInputAnswer = new ShortcutInput();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 24);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(64, 13);
            label1.TabIndex = 1;
            label1.Text = "Answer call:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 57);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(52, 13);
            label2.TabIndex = 3;
            label2.Text = "Drop call:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 90);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(51, 13);
            label3.TabIndex = 5;
            label3.Text = "Hold call:";
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.Location = new System.Drawing.Point(15, 121);
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            this.lineSeparator1.Size = new System.Drawing.Size(329, 2);
            this.lineSeparator1.TabIndex = 6;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(188, 135);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 7;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(269, 135);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // shortcutInputHold
            // 
            this.shortcutInputHold.Alt = false;
            this.shortcutInputHold.CharCode = ((byte)(65));
            this.shortcutInputHold.Control = false;
            this.shortcutInputHold.Keys = System.Windows.Forms.Keys.A;
            this.shortcutInputHold.Location = new System.Drawing.Point(119, 83);
            this.shortcutInputHold.Name = "shortcutInputHold";
            this.shortcutInputHold.Shift = false;
            this.shortcutInputHold.Size = new System.Drawing.Size(236, 26);
            this.shortcutInputHold.TabIndex = 4;
            // 
            // shortcutInputDrop
            // 
            this.shortcutInputDrop.Alt = false;
            this.shortcutInputDrop.CharCode = ((byte)(65));
            this.shortcutInputDrop.Control = false;
            this.shortcutInputDrop.Keys = System.Windows.Forms.Keys.A;
            this.shortcutInputDrop.Location = new System.Drawing.Point(119, 51);
            this.shortcutInputDrop.Name = "shortcutInputDrop";
            this.shortcutInputDrop.Shift = false;
            this.shortcutInputDrop.Size = new System.Drawing.Size(236, 26);
            this.shortcutInputDrop.TabIndex = 2;
            // 
            // shortcutInputAnswer
            // 
            this.shortcutInputAnswer.Alt = false;
            this.shortcutInputAnswer.CharCode = ((byte)(65));
            this.shortcutInputAnswer.Control = false;
            this.shortcutInputAnswer.Keys = System.Windows.Forms.Keys.A;
            this.shortcutInputAnswer.Location = new System.Drawing.Point(119, 19);
            this.shortcutInputAnswer.Name = "shortcutInputAnswer";
            this.shortcutInputAnswer.Shift = false;
            this.shortcutInputAnswer.Size = new System.Drawing.Size(236, 26);
            this.shortcutInputAnswer.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(365, 176);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.lineSeparator1);
            this.Controls.Add(label3);
            this.Controls.Add(this.shortcutInputHold);
            this.Controls.Add(label2);
            this.Controls.Add(this.shortcutInputDrop);
            this.Controls.Add(label1);
            this.Controls.Add(this.shortcutInputAnswer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Hot keys settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ShortcutInput shortcutInputAnswer;
        private ShortcutInput shortcutInputDrop;
        private ShortcutInput shortcutInputHold;
        private LineSeparator lineSeparator1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}
