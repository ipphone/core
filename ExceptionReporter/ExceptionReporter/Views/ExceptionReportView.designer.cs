namespace ExceptionReporter.Views
{
	public partial class ExceptionReportView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionReportView));
            this.listviewAssemblies = new System.Windows.Forms.ListView();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.picGeneral = new System.Windows.Forms.PictureBox();
            this.txtExceptionMessage = new System.Windows.Forms.TextBox();
            this.lblExplanation = new System.Windows.Forms.Label();
            this.txtUserExplanation = new System.Windows.Forms.TextBox();
            this.lblRegion = new System.Windows.Forms.Label();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.lblApplication = new System.Windows.Forms.Label();
            this.txtApplicationName = new System.Windows.Forms.TextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.tabExceptions = new System.Windows.Forms.TabPage();
            this.tabAssemblies = new System.Windows.Forms.TabPage();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.webBrowserConfig = new System.Windows.Forms.WebBrowser();
            this.tabSysInfo = new System.Windows.Forms.TabPage();
            this.lblMachine = new System.Windows.Forms.Label();
            this.txtMachine = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.treeEnvironment = new System.Windows.Forms.TreeView();
            this.tabContact = new System.Windows.Forms.TabPage();
            this.lblContactMessageTop = new System.Windows.Forms.Label();
            this.txtFax = new System.Windows.Forms.TextBox();
            this.lblFax = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblWebSite = new System.Windows.Forms.Label();
            this.urlWeb = new System.Windows.Forms.LinkLabel();
            this.lblEmail = new System.Windows.Forms.Label();
            this.urlEmail = new System.Windows.Forms.LinkLabel();
            this.btnSave = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnEmail = new System.Windows.Forms.Button();
            this.lblProgressMessage = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnDetailToggle = new System.Windows.Forms.Button();
            this.txtExceptionMessageLarge = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lessDetailPanel = new System.Windows.Forms.Panel();
            this.lessDetail_optionsPanel = new System.Windows.Forms.Panel();
            this.lblContactCompany = new System.Windows.Forms.Label();
            this.btnSimpleEmail = new System.Windows.Forms.Button();
            this.btnSimpleDetailToggle = new System.Windows.Forms.Button();
            this.btnSimpleCopy = new System.Windows.Forms.Button();
            this.txtExceptionMessageLarge2 = new System.Windows.Forms.TextBox();
            this.lessDetail_alertIcon = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxScreenshot = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGeneral)).BeginInit();
            this.tabAssemblies.SuspendLayout();
            this.tabConfig.SuspendLayout();
            this.tabSysInfo.SuspendLayout();
            this.tabContact.SuspendLayout();
            this.lessDetailPanel.SuspendLayout();
            this.lessDetail_optionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lessDetail_alertIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // listviewAssemblies
            // 
            this.listviewAssemblies.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listviewAssemblies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listviewAssemblies.FullRowSelect = true;
            this.listviewAssemblies.HotTracking = true;
            this.listviewAssemblies.HoverSelection = true;
            this.listviewAssemblies.Location = new System.Drawing.Point(0, 0);
            this.listviewAssemblies.Name = "listviewAssemblies";
            this.listviewAssemblies.Size = new System.Drawing.Size(364, 131);
            this.listviewAssemblies.TabIndex = 21;
            this.listviewAssemblies.UseCompatibleStateImageBehavior = false;
            this.listviewAssemblies.View = System.Windows.Forms.View.Details;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabExceptions);
            this.tabControl.Controls.Add(this.tabAssemblies);
            this.tabControl.Controls.Add(this.tabConfig);
            this.tabControl.Controls.Add(this.tabSysInfo);
            this.tabControl.Controls.Add(this.tabContact);
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(6, 6);
            this.tabControl.MinimumSize = new System.Drawing.Size(200, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(372, 157);
            this.tabControl.TabIndex = 6;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.picGeneral);
            this.tabGeneral.Controls.Add(this.txtExceptionMessage);
            this.tabGeneral.Controls.Add(this.lblExplanation);
            this.tabGeneral.Controls.Add(this.txtUserExplanation);
            this.tabGeneral.Controls.Add(this.lblRegion);
            this.tabGeneral.Controls.Add(this.txtRegion);
            this.tabGeneral.Controls.Add(this.lblDate);
            this.tabGeneral.Controls.Add(this.txtDate);
            this.tabGeneral.Controls.Add(this.lblTime);
            this.tabGeneral.Controls.Add(this.txtTime);
            this.tabGeneral.Controls.Add(this.lblApplication);
            this.tabGeneral.Controls.Add(this.txtApplicationName);
            this.tabGeneral.Controls.Add(this.lblVersion);
            this.tabGeneral.Controls.Add(this.txtVersion);
            this.tabGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(364, 131);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // picGeneral
            // 
            this.picGeneral.Image = ((System.Drawing.Image)(resources.GetObject("picGeneral.Image")));
            this.picGeneral.Location = new System.Drawing.Point(8, 7);
            this.picGeneral.Name = "picGeneral";
            this.picGeneral.Size = new System.Drawing.Size(64, 64);
            this.picGeneral.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picGeneral.TabIndex = 25;
            this.picGeneral.TabStop = false;
            // 
            // txtExceptionMessage
            // 
            this.txtExceptionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionMessage.BackColor = System.Drawing.Color.White;
            this.txtExceptionMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExceptionMessage.Location = new System.Drawing.Point(78, 7);
            this.txtExceptionMessage.Multiline = true;
            this.txtExceptionMessage.Name = "txtExceptionMessage";
            this.txtExceptionMessage.ReadOnly = true;
            this.txtExceptionMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExceptionMessage.Size = new System.Drawing.Size(271, 68);
            this.txtExceptionMessage.TabIndex = 0;
            this.txtExceptionMessage.Text = "No message";
            // 
            // lblExplanation
            // 
            this.lblExplanation.AutoSize = true;
            this.lblExplanation.Location = new System.Drawing.Point(6, 191);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Size = new System.Drawing.Size(334, 13);
            this.lblExplanation.TabIndex = 14;
            this.lblExplanation.Text = "Please enter a brief explanation of events leading up to this exception";
            // 
            // txtUserExplanation
            // 
            this.txtUserExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserExplanation.BackColor = System.Drawing.Color.Cornsilk;
            this.txtUserExplanation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserExplanation.Location = new System.Drawing.Point(8, 210);
            this.txtUserExplanation.Multiline = true;
            this.txtUserExplanation.Name = "txtUserExplanation";
            this.txtUserExplanation.Size = new System.Drawing.Size(341, 0);
            this.txtUserExplanation.TabIndex = 6;
            // 
            // lblRegion
            // 
            this.lblRegion.AutoSize = true;
            this.lblRegion.Location = new System.Drawing.Point(254, 127);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Size = new System.Drawing.Size(41, 13);
            this.lblRegion.TabIndex = 7;
            this.lblRegion.Text = "Region";
            // 
            // txtRegion
            // 
            this.txtRegion.BackColor = System.Drawing.Color.Snow;
            this.txtRegion.Location = new System.Drawing.Point(310, 124);
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.ReadOnly = true;
            this.txtRegion.Size = new System.Drawing.Size(141, 20);
            this.txtRegion.TabIndex = 3;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(14, 159);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(30, 13);
            this.lblDate.TabIndex = 9;
            this.lblDate.Text = "Date";
            // 
            // txtDate
            // 
            this.txtDate.BackColor = System.Drawing.Color.Snow;
            this.txtDate.Location = new System.Drawing.Point(78, 156);
            this.txtDate.Name = "txtDate";
            this.txtDate.ReadOnly = true;
            this.txtDate.Size = new System.Drawing.Size(152, 20);
            this.txtDate.TabIndex = 4;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(254, 159);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(30, 13);
            this.lblTime.TabIndex = 11;
            this.lblTime.Text = "Time";
            // 
            // txtTime
            // 
            this.txtTime.BackColor = System.Drawing.Color.Snow;
            this.txtTime.Location = new System.Drawing.Point(310, 156);
            this.txtTime.Name = "txtTime";
            this.txtTime.ReadOnly = true;
            this.txtTime.Size = new System.Drawing.Size(141, 20);
            this.txtTime.TabIndex = 5;
            // 
            // lblApplication
            // 
            this.lblApplication.AutoSize = true;
            this.lblApplication.Location = new System.Drawing.Point(14, 94);
            this.lblApplication.Name = "lblApplication";
            this.lblApplication.Size = new System.Drawing.Size(59, 13);
            this.lblApplication.TabIndex = 3;
            this.lblApplication.Text = "Application";
            // 
            // txtApplicationName
            // 
            this.txtApplicationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtApplicationName.BackColor = System.Drawing.Color.Snow;
            this.txtApplicationName.Location = new System.Drawing.Point(78, 92);
            this.txtApplicationName.Name = "txtApplicationName";
            this.txtApplicationName.ReadOnly = true;
            this.txtApplicationName.Size = new System.Drawing.Size(271, 20);
            this.txtApplicationName.TabIndex = 1;
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(14, 127);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(48, 16);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "Version";
            // 
            // txtVersion
            // 
            this.txtVersion.BackColor = System.Drawing.Color.Snow;
            this.txtVersion.Location = new System.Drawing.Point(78, 124);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(152, 20);
            this.txtVersion.TabIndex = 2;
            // 
            // tabExceptions
            // 
            this.tabExceptions.Location = new System.Drawing.Point(4, 22);
            this.tabExceptions.Name = "tabExceptions";
            this.tabExceptions.Size = new System.Drawing.Size(364, 131);
            this.tabExceptions.TabIndex = 1;
            this.tabExceptions.Text = "Exceptions";
            this.tabExceptions.UseVisualStyleBackColor = true;
            // 
            // tabAssemblies
            // 
            this.tabAssemblies.Controls.Add(this.listviewAssemblies);
            this.tabAssemblies.Location = new System.Drawing.Point(4, 22);
            this.tabAssemblies.Name = "tabAssemblies";
            this.tabAssemblies.Size = new System.Drawing.Size(364, 131);
            this.tabAssemblies.TabIndex = 6;
            this.tabAssemblies.Text = "Assemblies";
            this.tabAssemblies.UseVisualStyleBackColor = true;
            // 
            // tabConfig
            // 
            this.tabConfig.Controls.Add(this.webBrowserConfig);
            this.tabConfig.Location = new System.Drawing.Point(4, 22);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Size = new System.Drawing.Size(364, 131);
            this.tabConfig.TabIndex = 5;
            this.tabConfig.Text = "Configuration";
            this.tabConfig.UseVisualStyleBackColor = true;
            // 
            // webBrowserConfig
            // 
            this.webBrowserConfig.AllowNavigation = false;
            this.webBrowserConfig.AllowWebBrowserDrop = false;
            this.webBrowserConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserConfig.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserConfig.Location = new System.Drawing.Point(0, 0);
            this.webBrowserConfig.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserConfig.Name = "webBrowserConfig";
            this.webBrowserConfig.Size = new System.Drawing.Size(364, 131);
            this.webBrowserConfig.TabIndex = 21;
            this.webBrowserConfig.WebBrowserShortcutsEnabled = false;
            // 
            // tabSysInfo
            // 
            this.tabSysInfo.Controls.Add(this.lblMachine);
            this.tabSysInfo.Controls.Add(this.txtMachine);
            this.tabSysInfo.Controls.Add(this.lblUsername);
            this.tabSysInfo.Controls.Add(this.txtUserName);
            this.tabSysInfo.Controls.Add(this.treeEnvironment);
            this.tabSysInfo.Location = new System.Drawing.Point(4, 22);
            this.tabSysInfo.Name = "tabSysInfo";
            this.tabSysInfo.Size = new System.Drawing.Size(364, 131);
            this.tabSysInfo.TabIndex = 3;
            this.tabSysInfo.Text = "System";
            this.tabSysInfo.UseVisualStyleBackColor = true;
            // 
            // lblMachine
            // 
            this.lblMachine.AutoSize = true;
            this.lblMachine.Location = new System.Drawing.Point(5, 15);
            this.lblMachine.Name = "lblMachine";
            this.lblMachine.Size = new System.Drawing.Size(46, 13);
            this.lblMachine.TabIndex = 16;
            this.lblMachine.Text = "Machine";
            // 
            // txtMachine
            // 
            this.txtMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMachine.BackColor = System.Drawing.SystemColors.Control;
            this.txtMachine.Location = new System.Drawing.Point(59, 12);
            this.txtMachine.Name = "txtMachine";
            this.txtMachine.ReadOnly = true;
            this.txtMachine.Size = new System.Drawing.Size(270, 21);
            this.txtMachine.TabIndex = 0;
            // 
            // lblUsername
            // 
            this.lblUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(351, 15);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Username";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.BackColor = System.Drawing.SystemColors.Control;
            this.txtUserName.Location = new System.Drawing.Point(412, 14);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(169, 21);
            this.txtUserName.TabIndex = 1;
            // 
            // treeEnvironment
            // 
            this.treeEnvironment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeEnvironment.BackColor = System.Drawing.SystemColors.Window;
            this.treeEnvironment.HotTracking = true;
            this.treeEnvironment.Location = new System.Drawing.Point(8, 40);
            this.treeEnvironment.Name = "treeEnvironment";
            this.treeEnvironment.Size = new System.Drawing.Size(573, 301);
            this.treeEnvironment.TabIndex = 2;
            // 
            // tabContact
            // 
            this.tabContact.Controls.Add(this.lblContactMessageTop);
            this.tabContact.Controls.Add(this.txtFax);
            this.tabContact.Controls.Add(this.lblFax);
            this.tabContact.Controls.Add(this.txtPhone);
            this.tabContact.Controls.Add(this.lblPhone);
            this.tabContact.Controls.Add(this.lblWebSite);
            this.tabContact.Controls.Add(this.urlWeb);
            this.tabContact.Controls.Add(this.lblEmail);
            this.tabContact.Controls.Add(this.urlEmail);
            this.tabContact.Location = new System.Drawing.Point(4, 22);
            this.tabContact.Name = "tabContact";
            this.tabContact.Size = new System.Drawing.Size(364, 131);
            this.tabContact.TabIndex = 4;
            this.tabContact.Text = "Contact";
            this.tabContact.UseVisualStyleBackColor = true;
            // 
            // lblContactMessageTop
            // 
            this.lblContactMessageTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContactMessageTop.Location = new System.Drawing.Point(8, 24);
            this.lblContactMessageTop.MinimumSize = new System.Drawing.Size(200, 0);
            this.lblContactMessageTop.Name = "lblContactMessageTop";
            this.lblContactMessageTop.Size = new System.Drawing.Size(533, 24);
            this.lblContactMessageTop.TabIndex = 27;
            this.lblContactMessageTop.Text = "The following details can be used to obtain support for this application.";
            // 
            // txtFax
            // 
            this.txtFax.BackColor = System.Drawing.SystemColors.Control;
            this.txtFax.Location = new System.Drawing.Point(72, 168);
            this.txtFax.MinimumSize = new System.Drawing.Size(200, 0);
            this.txtFax.Name = "txtFax";
            this.txtFax.ReadOnly = true;
            this.txtFax.Size = new System.Drawing.Size(249, 21);
            this.txtFax.TabIndex = 3;
            // 
            // lblFax
            // 
            this.lblFax.AutoSize = true;
            this.lblFax.Location = new System.Drawing.Point(18, 168);
            this.lblFax.Name = "lblFax";
            this.lblFax.Size = new System.Drawing.Size(25, 13);
            this.lblFax.TabIndex = 34;
            this.lblFax.Text = "Fax";
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(72, 142);
            this.txtPhone.MinimumSize = new System.Drawing.Size(200, 0);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.ReadOnly = true;
            this.txtPhone.Size = new System.Drawing.Size(249, 21);
            this.txtPhone.TabIndex = 2;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(16, 144);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(37, 13);
            this.lblPhone.TabIndex = 32;
            this.lblPhone.Text = "Phone";
            // 
            // lblWebSite
            // 
            this.lblWebSite.AutoSize = true;
            this.lblWebSite.Location = new System.Drawing.Point(16, 80);
            this.lblWebSite.Name = "lblWebSite";
            this.lblWebSite.Size = new System.Drawing.Size(29, 13);
            this.lblWebSite.TabIndex = 30;
            this.lblWebSite.Text = "Web";
            // 
            // urlWeb
            // 
            this.urlWeb.ActiveLinkColor = System.Drawing.Color.Orange;
            this.urlWeb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.urlWeb.AutoSize = true;
            this.urlWeb.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.urlWeb.Location = new System.Drawing.Point(72, 77);
            this.urlWeb.Margin = new System.Windows.Forms.Padding(5);
            this.urlWeb.MinimumSize = new System.Drawing.Size(200, 0);
            this.urlWeb.Name = "urlWeb";
            this.urlWeb.Size = new System.Drawing.Size(200, 18);
            this.urlWeb.TabIndex = 1;
            this.urlWeb.TabStop = true;
            this.urlWeb.Text = "NA";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(16, 56);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(31, 13);
            this.lblEmail.TabIndex = 28;
            this.lblEmail.Text = "Email";
            // 
            // urlEmail
            // 
            this.urlEmail.ActiveLinkColor = System.Drawing.Color.Orange;
            this.urlEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.urlEmail.AutoSize = true;
            this.urlEmail.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.urlEmail.Location = new System.Drawing.Point(72, 53);
            this.urlEmail.Margin = new System.Windows.Forms.Padding(5);
            this.urlEmail.MinimumSize = new System.Drawing.Size(200, 0);
            this.urlEmail.Name = "urlEmail";
            this.urlEmail.Size = new System.Drawing.Size(200, 18);
            this.urlEmail.TabIndex = 0;
            this.urlEmail.TabStop = true;
            this.urlEmail.Text = "NA";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(151, 166);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 32);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar.Location = new System.Drawing.Point(5, 182);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(141, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 53;
            // 
            // btnEmail
            // 
            this.btnEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEmail.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEmail.Image = ((System.Drawing.Image)(resources.GetObject("btnEmail.Image")));
            this.btnEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmail.Location = new System.Drawing.Point(228, 166);
            this.btnEmail.Name = "btnEmail";
            this.btnEmail.Size = new System.Drawing.Size(72, 32);
            this.btnEmail.TabIndex = 1;
            this.btnEmail.Text = "Email";
            this.btnEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblProgressMessage
            // 
            this.lblProgressMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgressMessage.AutoSize = true;
            this.lblProgressMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblProgressMessage.Location = new System.Drawing.Point(3, 166);
            this.lblProgressMessage.Name = "lblProgressMessage";
            this.lblProgressMessage.Size = new System.Drawing.Size(150, 13);
            this.lblProgressMessage.TabIndex = 52;
            this.lblProgressMessage.Text = "Loading system information...";
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
            this.btnCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopy.Location = new System.Drawing.Point(74, 166);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(72, 32);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = "Copy";
            this.btnCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnDetailToggle
            // 
            this.btnDetailToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDetailToggle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDetailToggle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDetailToggle.Location = new System.Drawing.Point(-3, 166);
            this.btnDetailToggle.Name = "btnDetailToggle";
            this.btnDetailToggle.Size = new System.Drawing.Size(72, 32);
            this.btnDetailToggle.TabIndex = 4;
            this.btnDetailToggle.Text = "Less Detail";
            // 
            // txtExceptionMessageLarge
            // 
            this.txtExceptionMessageLarge.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionMessageLarge.BackColor = System.Drawing.Color.White;
            this.txtExceptionMessageLarge.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExceptionMessageLarge.Location = new System.Drawing.Point(6, 6);
            this.txtExceptionMessageLarge.Multiline = true;
            this.txtExceptionMessageLarge.Name = "txtExceptionMessageLarge";
            this.txtExceptionMessageLarge.ReadOnly = true;
            this.txtExceptionMessageLarge.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExceptionMessageLarge.Size = new System.Drawing.Size(371, 154);
            this.txtExceptionMessageLarge.TabIndex = 5;
            this.txtExceptionMessageLarge.Text = "No message";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(305, 166);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 32);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            // 
            // lessDetailPanel
            // 
            this.lessDetailPanel.BackColor = System.Drawing.Color.White;
            this.lessDetailPanel.Controls.Add(this.lessDetail_optionsPanel);
            this.lessDetailPanel.Controls.Add(this.txtExceptionMessageLarge2);
            this.lessDetailPanel.Controls.Add(this.lessDetail_alertIcon);
            this.lessDetailPanel.Controls.Add(this.label1);
            this.lessDetailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lessDetailPanel.Location = new System.Drawing.Point(0, 0);
            this.lessDetailPanel.Name = "lessDetailPanel";
            this.lessDetailPanel.Size = new System.Drawing.Size(384, 202);
            this.lessDetailPanel.TabIndex = 54;
            // 
            // lessDetail_optionsPanel
            // 
            this.lessDetail_optionsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.lessDetail_optionsPanel.Controls.Add(this.checkBoxScreenshot);
            this.lessDetail_optionsPanel.Controls.Add(this.lblContactCompany);
            this.lessDetail_optionsPanel.Controls.Add(this.btnSimpleEmail);
            this.lessDetail_optionsPanel.Controls.Add(this.btnSimpleDetailToggle);
            this.lessDetail_optionsPanel.Controls.Add(this.btnSimpleCopy);
            this.lessDetail_optionsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lessDetail_optionsPanel.Location = new System.Drawing.Point(0, 123);
            this.lessDetail_optionsPanel.Name = "lessDetail_optionsPanel";
            this.lessDetail_optionsPanel.Padding = new System.Windows.Forms.Padding(8);
            this.lessDetail_optionsPanel.Size = new System.Drawing.Size(384, 79);
            this.lessDetail_optionsPanel.TabIndex = 26;
            // 
            // lblContactCompany
            // 
            this.lblContactCompany.AutoSize = true;
            this.lblContactCompany.ForeColor = System.Drawing.Color.SlateGray;
            this.lblContactCompany.Location = new System.Drawing.Point(13, 11);
            this.lblContactCompany.Name = "lblContactCompany";
            this.lblContactCompany.Size = new System.Drawing.Size(0, 13);
            this.lblContactCompany.TabIndex = 3;
            // 
            // btnSimpleEmail
            // 
            this.btnSimpleEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSimpleEmail.FlatAppearance.BorderSize = 0;
            this.btnSimpleEmail.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.btnSimpleEmail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(228)))), ((int)(((byte)(255)))));
            this.btnSimpleEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimpleEmail.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSimpleEmail.Image = ((System.Drawing.Image)(resources.GetObject("btnSimpleEmail.Image")));
            this.btnSimpleEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSimpleEmail.Location = new System.Drawing.Point(267, 36);
            this.btnSimpleEmail.Name = "btnSimpleEmail";
            this.btnSimpleEmail.Size = new System.Drawing.Size(106, 32);
            this.btnSimpleEmail.TabIndex = 1;
            this.btnSimpleEmail.Text = "Отправить";
            this.btnSimpleEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSimpleDetailToggle
            // 
            this.btnSimpleDetailToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSimpleDetailToggle.FlatAppearance.BorderSize = 0;
            this.btnSimpleDetailToggle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.btnSimpleDetailToggle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(228)))), ((int)(((byte)(255)))));
            this.btnSimpleDetailToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimpleDetailToggle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSimpleDetailToggle.Image = ((System.Drawing.Image)(resources.GetObject("btnSimpleDetailToggle.Image")));
            this.btnSimpleDetailToggle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSimpleDetailToggle.Location = new System.Drawing.Point(12, 36);
            this.btnSimpleDetailToggle.Name = "btnSimpleDetailToggle";
            this.btnSimpleDetailToggle.Size = new System.Drawing.Size(96, 32);
            this.btnSimpleDetailToggle.TabIndex = 4;
            this.btnSimpleDetailToggle.Text = "Подробнее";
            this.btnSimpleDetailToggle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSimpleCopy
            // 
            this.btnSimpleCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSimpleCopy.FlatAppearance.BorderSize = 0;
            this.btnSimpleCopy.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(230)))));
            this.btnSimpleCopy.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(228)))), ((int)(((byte)(255)))));
            this.btnSimpleCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimpleCopy.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSimpleCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnSimpleCopy.Image")));
            this.btnSimpleCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSimpleCopy.Location = new System.Drawing.Point(136, 36);
            this.btnSimpleCopy.Name = "btnSimpleCopy";
            this.btnSimpleCopy.Size = new System.Drawing.Size(117, 32);
            this.btnSimpleCopy.TabIndex = 3;
            this.btnSimpleCopy.Text = "Скопировать";
            this.btnSimpleCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtExceptionMessageLarge2
            // 
            this.txtExceptionMessageLarge2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionMessageLarge2.BackColor = System.Drawing.Color.White;
            this.txtExceptionMessageLarge2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtExceptionMessageLarge2.Location = new System.Drawing.Point(86, 62);
            this.txtExceptionMessageLarge2.Multiline = true;
            this.txtExceptionMessageLarge2.Name = "txtExceptionMessageLarge2";
            this.txtExceptionMessageLarge2.ReadOnly = true;
            this.txtExceptionMessageLarge2.Size = new System.Drawing.Size(283, 55);
            this.txtExceptionMessageLarge2.TabIndex = 0;
            this.txtExceptionMessageLarge2.Text = "No message";
            // 
            // lessDetail_alertIcon
            // 
            this.lessDetail_alertIcon.Image = ((System.Drawing.Image)(resources.GetObject("lessDetail_alertIcon.Image")));
            this.lessDetail_alertIcon.Location = new System.Drawing.Point(14, 13);
            this.lessDetail_alertIcon.Name = "lessDetail_alertIcon";
            this.lessDetail_alertIcon.Size = new System.Drawing.Size(64, 64);
            this.lessDetail_alertIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.lessDetail_alertIcon.TabIndex = 25;
            this.lessDetail_alertIcon.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(84, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 23);
            this.label1.TabIndex = 14;
            this.label1.Text = "Operation Failed";
            // 
            // checkBoxScreenshot
            // 
            this.checkBoxScreenshot.AutoSize = true;
            this.checkBoxScreenshot.Checked = true;
            this.checkBoxScreenshot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScreenshot.Location = new System.Drawing.Point(161, 12);
            this.checkBoxScreenshot.Name = "checkBoxScreenshot";
            this.checkBoxScreenshot.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxScreenshot.Size = new System.Drawing.Size(211, 17);
            this.checkBoxScreenshot.TabIndex = 5;
            this.checkBoxScreenshot.Text = "Прикрепить к отчету снимок экрана";
            this.checkBoxScreenshot.UseVisualStyleBackColor = true;
            // 
            // ExceptionReportView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 202);
            this.Controls.Add(this.lessDetailPanel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDetailToggle);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnEmail);
            this.Controls.Add(this.lblProgressMessage);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtExceptionMessageLarge);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ExceptionReportView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGeneral)).EndInit();
            this.tabAssemblies.ResumeLayout(false);
            this.tabConfig.ResumeLayout(false);
            this.tabSysInfo.ResumeLayout(false);
            this.tabSysInfo.PerformLayout();
            this.tabContact.ResumeLayout(false);
            this.tabContact.PerformLayout();
            this.lessDetailPanel.ResumeLayout(false);
            this.lessDetailPanel.PerformLayout();
            this.lessDetail_optionsPanel.ResumeLayout(false);
            this.lessDetail_optionsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lessDetail_alertIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listviewAssemblies;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.Label lblExplanation;
        private System.Windows.Forms.TextBox txtUserExplanation;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.TextBox txtRegion;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Label lblApplication;
        private System.Windows.Forms.TextBox txtApplicationName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TabPage tabExceptions;
        private System.Windows.Forms.TabPage tabAssemblies;
        private System.Windows.Forms.TabPage tabConfig;
        private System.Windows.Forms.TabPage tabSysInfo;
        private System.Windows.Forms.Label lblMachine;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TreeView treeEnvironment;
        private System.Windows.Forms.TabPage tabContact;
        private System.Windows.Forms.Label lblContactMessageTop;
        private System.Windows.Forms.TextBox txtFax;
        private System.Windows.Forms.Label lblFax;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.Label lblWebSite;
        private System.Windows.Forms.LinkLabel urlWeb;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.LinkLabel urlEmail;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnEmail;
        private System.Windows.Forms.Label lblProgressMessage;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.TextBox txtExceptionMessage;
        private System.Windows.Forms.PictureBox picGeneral;
        private System.Windows.Forms.Button btnDetailToggle;
        private System.Windows.Forms.TextBox txtExceptionMessageLarge;
        private System.Windows.Forms.WebBrowser webBrowserConfig;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel lessDetailPanel;
        private System.Windows.Forms.PictureBox lessDetail_alertIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel lessDetail_optionsPanel;
        private System.Windows.Forms.TextBox txtExceptionMessageLarge2;
        private System.Windows.Forms.Label lblContactCompany;
        private System.Windows.Forms.Button btnSimpleEmail;
        private System.Windows.Forms.Button btnSimpleCopy;
        private System.Windows.Forms.Button btnSimpleDetailToggle;
        private System.Windows.Forms.CheckBox checkBoxScreenshot;
    }
}
