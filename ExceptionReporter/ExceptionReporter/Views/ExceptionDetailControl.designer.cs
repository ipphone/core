namespace ExceptionReporter.Views
{
	internal partial class ExceptionDetailControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtExceptionTabStackTrace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExceptionTabMessage = new System.Windows.Forms.TextBox();
            this.listviewExceptions = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Stack Trace";
            // 
            // txtExceptionTabStackTrace
            // 
            this.txtExceptionTabStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                                           | System.Windows.Forms.AnchorStyles.Left)
                                                                                          | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionTabStackTrace.BackColor = System.Drawing.SystemColors.Window;
            this.txtExceptionTabStackTrace.Location = new System.Drawing.Point(13, 226);
            this.txtExceptionTabStackTrace.Multiline = true;
            this.txtExceptionTabStackTrace.Name = "txtExceptionTabStackTrace";
            this.txtExceptionTabStackTrace.ReadOnly = true;
            this.txtExceptionTabStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExceptionTabStackTrace.Size = new System.Drawing.Size(598, 306);
            this.txtExceptionTabStackTrace.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Message";
            // 
            // txtExceptionTabMessage
            // 
            this.txtExceptionTabMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                                       | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionTabMessage.BackColor = System.Drawing.SystemColors.Window;
            this.txtExceptionTabMessage.Location = new System.Drawing.Point(13, 148);
            this.txtExceptionTabMessage.Multiline = true;
            this.txtExceptionTabMessage.Name = "txtExceptionTabMessage";
            this.txtExceptionTabMessage.ReadOnly = true;
            this.txtExceptionTabMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExceptionTabMessage.Size = new System.Drawing.Size(598, 52);
            this.txtExceptionTabMessage.TabIndex = 30;
            // 
            // listviewExceptions
            // 
            this.listviewExceptions.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listviewExceptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                                   | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewExceptions.FullRowSelect = true;
            this.listviewExceptions.HotTracking = true;
            this.listviewExceptions.HoverSelection = true;
            this.listviewExceptions.Location = new System.Drawing.Point(3, 3);
            this.listviewExceptions.Name = "listviewExceptions";
            this.listviewExceptions.Size = new System.Drawing.Size(608, 120);
            this.listviewExceptions.TabIndex = 29;
            this.listviewExceptions.UseCompatibleStateImageBehavior = false;
            this.listviewExceptions.View = System.Windows.Forms.View.Details;
            // 
            // ExceptionDetailControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtExceptionTabStackTrace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtExceptionTabMessage);
            this.Controls.Add(this.listviewExceptions);
            this.Name = "ExceptionDetailControl";
            this.Size = new System.Drawing.Size(614, 535);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtExceptionTabStackTrace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExceptionTabMessage;
        private System.Windows.Forms.ListView listviewExceptions;
    }
}
