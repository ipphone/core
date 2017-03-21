namespace ContactPoint.Plugins.CallsLog
{
    partial class CallLogForm
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
            System.Windows.Forms.ColumnHeader columnHeaderIcon;
            System.Windows.Forms.ColumnHeader columnHeaderNumber;
            System.Windows.Forms.ColumnHeader columnHeaderName;
            System.Windows.Forms.ColumnHeader columnHeaderDuration;
            System.Windows.Forms.ColumnHeader columnHeaderDate;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CallLogForm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageListIcons = new System.Windows.Forms.ImageList(this.components);
            columnHeaderIcon = new System.Windows.Forms.ColumnHeader();
            columnHeaderNumber = new System.Windows.Forms.ColumnHeader();
            columnHeaderName = new System.Windows.Forms.ColumnHeader();
            columnHeaderDuration = new System.Windows.Forms.ColumnHeader();
            columnHeaderDate = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // columnHeaderIcon
            // 
            columnHeaderIcon.Text = "";
            columnHeaderIcon.Width = 20;
            // 
            // columnHeaderNumber
            // 
            columnHeaderNumber.Text = "Номер";
            columnHeaderNumber.Width = 120;
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "Имя";
            columnHeaderName.Width = 220;
            // 
            // columnHeaderDuration
            // 
            columnHeaderDuration.Text = "Длительность";
            columnHeaderDuration.Width = 100;
            // 
            // columnHeaderDate
            // 
            columnHeaderDate.Text = "Дата/время";
            columnHeaderDate.Width = 160;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeaderIcon,
            columnHeaderNumber,
            columnHeaderName,
            columnHeaderDate,
            columnHeaderDuration});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(658, 409);
            this.listView1.SmallImageList = this.imageListIcons;
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // imageListIcons
            // 
            this.imageListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcons.ImageStream")));
            this.imageListIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListIcons.Images.SetKeyName(0, "delete.png");
            this.imageListIcons.Images.SetKeyName(1, "ic_call_log_list_incoming_call.png");
            this.imageListIcons.Images.SetKeyName(2, "ic_call_log_list_outgoing_call.png");
            // 
            // CallLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(682, 433);
            this.Controls.Add(this.listView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "CallLogForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "История звонков";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageListIcons;
    }
}
