using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ExceptionReporter.Core;

namespace ExceptionReporter.Views
{
	/// <summary>
	/// For exceptions within the ExceptionReporter - considering removing altogether
	/// </summary>
	public class InternalExceptionView : Form, IInternalExceptionView
	{
		private Button cmdOK;
		private TextBox txtExceptionMessage;
		private TabControl tabControl;
		private Label lblExceptionMessage;
		private Label lblGeneralMessage;
		private TabPage tpGeneral;
		private TabPage tpException;
		private Label lblExceptionType;
		private TextBox txtExceptionDetails;
        private PictureBox pictureBox1;
        private TextBox txtExceptionType;

	    public InternalExceptionView(ExceptionReportInfo reportInfo) : this()
	    {
	        ShowException(reportInfo.CustomMessage ?? reportInfo.MainException?.Message, reportInfo.MainException);
	    }

		public InternalExceptionView()
		{
			InitializeComponent();

			WireUpEvents();
		}

		private void WireUpEvents()
		{
			cmdOK.Click += cmdOK_Click;
		}

		private void InitializeComponent()
		{
            this.txtExceptionType = new System.Windows.Forms.TextBox();
            this.txtExceptionDetails = new System.Windows.Forms.TextBox();
            this.lblExceptionType = new System.Windows.Forms.Label();
            this.tpException = new System.Windows.Forms.TabPage();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.txtExceptionMessage = new System.Windows.Forms.TextBox();
            this.lblExceptionMessage = new System.Windows.Forms.Label();
            this.lblGeneralMessage = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.cmdOK = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tpException.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtExceptionType
            // 
            this.txtExceptionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionType.Location = new System.Drawing.Point(77, 74);
            this.txtExceptionType.Name = "txtExceptionType";
            this.txtExceptionType.ReadOnly = true;
            this.txtExceptionType.Size = new System.Drawing.Size(455, 20);
            this.txtExceptionType.TabIndex = 3;
            // 
            // txtExceptionDetails
            // 
            this.txtExceptionDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtExceptionDetails.Location = new System.Drawing.Point(9, 9);
            this.txtExceptionDetails.Multiline = true;
            this.txtExceptionDetails.Name = "txtExceptionDetails";
            this.txtExceptionDetails.ReadOnly = true;
            this.txtExceptionDetails.Size = new System.Drawing.Size(517, 292);
            this.txtExceptionDetails.TabIndex = 6;
            // 
            // lblExceptionType
            // 
            this.lblExceptionType.Location = new System.Drawing.Point(15, 77);
            this.lblExceptionType.Name = "lblExceptionType";
            this.lblExceptionType.Size = new System.Drawing.Size(40, 15);
            this.lblExceptionType.TabIndex = 2;
            this.lblExceptionType.Text = "Type:";
            // 
            // tpException
            // 
            this.tpException.Controls.Add(this.txtExceptionDetails);
            this.tpException.Location = new System.Drawing.Point(4, 22);
            this.tpException.Name = "tpException";
            this.tpException.Size = new System.Drawing.Size(535, 310);
            this.tpException.TabIndex = 1;
            this.tpException.Text = "Exception";
            this.tpException.ToolTipText = "Displays detailed information about the error that occurred";
            // 
            // tpGeneral
            // 
            this.tpGeneral.Controls.Add(this.pictureBox1);
            this.tpGeneral.Controls.Add(this.txtExceptionMessage);
            this.tpGeneral.Controls.Add(this.txtExceptionType);
            this.tpGeneral.Controls.Add(this.lblExceptionType);
            this.tpGeneral.Controls.Add(this.lblExceptionMessage);
            this.tpGeneral.Controls.Add(this.lblGeneralMessage);
            this.tpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.Size = new System.Drawing.Size(535, 310);
            this.tpGeneral.TabIndex = 0;
            this.tpGeneral.Text = "General";
            this.tpGeneral.ToolTipText = "Displays general information about the error that occurred";
            // 
            // txtExceptionMessage
            // 
            this.txtExceptionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionMessage.Location = new System.Drawing.Point(77, 100);
            this.txtExceptionMessage.Multiline = true;
            this.txtExceptionMessage.Name = "txtExceptionMessage";
            this.txtExceptionMessage.ReadOnly = true;
            this.txtExceptionMessage.Size = new System.Drawing.Size(455, 206);
            this.txtExceptionMessage.TabIndex = 5;
            // 
            // lblExceptionMessage
            // 
            this.lblExceptionMessage.Location = new System.Drawing.Point(15, 100);
            this.lblExceptionMessage.Name = "lblExceptionMessage";
            this.lblExceptionMessage.Size = new System.Drawing.Size(56, 14);
            this.lblExceptionMessage.TabIndex = 4;
            this.lblExceptionMessage.Text = "Message";
            // 
            // lblGeneralMessage
            // 
            this.lblGeneralMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGeneralMessage.Location = new System.Drawing.Point(77, 7);
            this.lblGeneralMessage.Name = "lblGeneralMessage";
            this.lblGeneralMessage.Size = new System.Drawing.Size(455, 60);
            this.lblGeneralMessage.TabIndex = 1;
            this.lblGeneralMessage.Text = "An internal exception has occurred within Exception Reporter";
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tpGeneral);
            this.tabControl.Controls.Add(this.tpException);
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(8, 7);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(543, 336);
            this.tabControl.TabIndex = 0;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdOK.Location = new System.Drawing.Point(469, 347);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(80, 25);
            this.cmdOK.TabIndex = 7;
            this.cmdOK.Text = "OK";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ExceptionReporter.Properties.Resources.warning__1_;
            this.pictureBox1.Location = new System.Drawing.Point(18, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(53, 60);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // InternalExceptionView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(556, 376);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.cmdOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InternalExceptionView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Exception Reporter";
            this.tpException.ResumeLayout(false);
            this.tpException.PerformLayout();
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.tabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}

		public void ShowException(string message, Exception ex)
		{
			lblGeneralMessage.Text = message;

			if (!(ex == null))
			{
				txtExceptionType.Text = ex.GetType().ToString();
				txtExceptionMessage.Text = ex.Message;
				txtExceptionDetails.Text = ExceptionHeirarchyToString(ex);
			}

			Show();
		}


		private static string ExceptionHeirarchyToString(Exception exception)
		{
			var stringBuilder = new StringBuilder();
		    using (var stringWriter = new StringWriter(stringBuilder))
		    {
		        var intCount = 0;
		        var current = exception;

		        while (current != null)
		        {
		            if (intCount == 0)
		            {
		                stringWriter.WriteLine("Top Level Exception");
		            }
		            else
		            {
		                stringWriter.WriteLine("Inner Exception " + intCount);
		            }
		            stringWriter.WriteLine("Type:        " + current.GetType());
		            stringWriter.WriteLine("Message:     " + current.Message);
		            stringWriter.WriteLine("Source:      " + current.Source);

                    if (current.StackTrace != null) stringWriter.WriteLine("Stack Trace: " + current.StackTrace.Trim());

		            stringWriter.WriteLine((String) null);

		            current = current.InnerException;
		            intCount++;
		        }
		    }
		    return stringBuilder.ToString();
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
