namespace ContactPoint.Forms
{
    public partial class SettingsForm
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
            System.Windows.Forms.GroupBox groupBox1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.buttonLicenseUpload = new System.Windows.Forms.Button();
            this.textBoxLicensePath = new System.Windows.Forms.TextBox();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageSettingsSIP = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDomain = new System.Windows.Forms.TextBox();
            this.textBoxRegistrarAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDisplayName = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxAutoAnswerPeriod = new System.Windows.Forms.TextBox();
            this.checkBoxAutoAnswer = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxRTPPort = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxDtmfMode = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxECTail = new System.Windows.Forms.TextBox();
            this.checkBoxVAD = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.textBoxExpires = new System.Windows.Forms.TextBox();
            this.comboBoxSIPTransport = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxListenPort = new System.Windows.Forms.TextBox();
            this.tabPageSettingsAudio = new System.Windows.Forms.TabPage();
            this.groupBoxSpeakerphone = new System.Windows.Forms.GroupBox();
            this.comboBoxRecordingDevices = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxPlayRingback = new System.Windows.Forms.CheckBox();
            this.checkBoxPlayIncomingWhileActive = new System.Windows.Forms.CheckBox();
            this.checkBoxPlayIncomingSound = new System.Windows.Forms.CheckBox();
            this.comboBoxSpeakerphoneDevices = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBoxPlaybackDevices = new System.Windows.Forms.ComboBox();
            this.tabCodecsPage = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonDisable = new System.Windows.Forms.Button();
            this.buttonEnable = new System.Windows.Forms.Button();
            this.listBoxEnCodecs = new System.Windows.Forms.ListBox();
            this.listBoxDisCodecs = new System.Windows.Forms.ListBox();
            this.tabPagePlugins = new System.Windows.Forms.TabPage();
            this.label21 = new System.Windows.Forms.Label();
            this.groupBoxPluginInfo = new System.Windows.Forms.GroupBox();
            this.buttonPluginSettings = new System.Windows.Forms.Button();
            this.buttonPluginStop = new System.Windows.Forms.Button();
            this.buttonPluginStart = new System.Windows.Forms.Button();
            this.checkBoxAutoStartPlugin = new System.Windows.Forms.CheckBox();
            this.labelPluginVersion = new System.Windows.Forms.Label();
            this.labelPluginName = new System.Windows.Forms.Label();
            this.labelPluginID = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.listBoxPlugins = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonLog = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            this.tabControlSettings.SuspendLayout();
            this.tabPageSettingsSIP.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPageSettingsAudio.SuspendLayout();
            this.groupBoxSpeakerphone.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabCodecsPage.SuspendLayout();
            this.tabPagePlugins.SuspendLayout();
            this.groupBoxPluginInfo.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(this.buttonLicenseUpload);
            groupBox1.Controls.Add(this.textBoxLicensePath);
            groupBox1.Location = new System.Drawing.Point(8, 319);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(280, 46);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "License:";
            // 
            // buttonLicenseUpload
            // 
            this.buttonLicenseUpload.Location = new System.Drawing.Point(246, 16);
            this.buttonLicenseUpload.Name = "buttonLicenseUpload";
            this.buttonLicenseUpload.Size = new System.Drawing.Size(28, 23);
            this.buttonLicenseUpload.TabIndex = 1;
            this.buttonLicenseUpload.Text = "...";
            this.buttonLicenseUpload.UseVisualStyleBackColor = true;
            this.buttonLicenseUpload.Click += new System.EventHandler(this.buttonLicenseUpload_Click);
            // 
            // textBoxLicensePath
            // 
            this.textBoxLicensePath.Location = new System.Drawing.Point(6, 18);
            this.textBoxLicensePath.Name = "textBoxLicensePath";
            this.textBoxLicensePath.ReadOnly = true;
            this.textBoxLicensePath.Size = new System.Drawing.Size(234, 20);
            this.textBoxLicensePath.TabIndex = 0;
            // 
            // tabControlSettings
            // 
            this.tabControlSettings.Controls.Add(this.tabPageSettingsSIP);
            this.tabControlSettings.Controls.Add(this.tabPageSettingsAudio);
            this.tabControlSettings.Controls.Add(this.tabCodecsPage);
            this.tabControlSettings.Controls.Add(this.tabPagePlugins);
            this.tabControlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControlSettings.Location = new System.Drawing.Point(0, 0);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(304, 397);
            this.tabControlSettings.TabIndex = 0;
            // 
            // tabPageSettingsSIP
            // 
            this.tabPageSettingsSIP.Controls.Add(this.groupBox2);
            this.tabPageSettingsSIP.Controls.Add(this.groupBox6);
            this.tabPageSettingsSIP.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettingsSIP.Name = "tabPageSettingsSIP";
            this.tabPageSettingsSIP.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettingsSIP.Size = new System.Drawing.Size(296, 371);
            this.tabPageSettingsSIP.TabIndex = 1;
            this.tabPageSettingsSIP.Text = "SIP";
            this.tabPageSettingsSIP.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxDomain);
            this.groupBox2.Controls.Add(this.textBoxRegistrarAddress);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxPassword);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxUsername);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxDisplayName);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(290, 149);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "User Config";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Realm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "SIP Registrar";
            // 
            // textBoxDomain
            // 
            this.textBoxDomain.Location = new System.Drawing.Point(90, 97);
            this.textBoxDomain.Name = "textBoxDomain";
            this.textBoxDomain.Size = new System.Drawing.Size(184, 20);
            this.textBoxDomain.TabIndex = 3;
            this.textBoxDomain.Text = "*";
            this.textBoxDomain.ModifiedChanged += new System.EventHandler(this.reregistrationRequired_TextChanged);
            // 
            // textBoxRegistrarAddress
            // 
            this.textBoxRegistrarAddress.Location = new System.Drawing.Point(90, 123);
            this.textBoxRegistrarAddress.Name = "textBoxRegistrarAddress";
            this.textBoxRegistrarAddress.Size = new System.Drawing.Size(184, 20);
            this.textBoxRegistrarAddress.TabIndex = 4;
            this.textBoxRegistrarAddress.ModifiedChanged += new System.EventHandler(this.reregistrationRequired_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Password";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(90, 71);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(184, 20);
            this.textBoxPassword.TabIndex = 2;
            this.textBoxPassword.ModifiedChanged += new System.EventHandler(this.reregistrationRequired_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Username";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(90, 45);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(184, 20);
            this.textBoxUsername.TabIndex = 1;
            this.textBoxUsername.ModifiedChanged += new System.EventHandler(this.reregistrationRequired_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Display Name";
            // 
            // textBoxDisplayName
            // 
            this.textBoxDisplayName.Location = new System.Drawing.Point(90, 19);
            this.textBoxDisplayName.Name = "textBoxDisplayName";
            this.textBoxDisplayName.Size = new System.Drawing.Size(184, 20);
            this.textBoxDisplayName.TabIndex = 0;
            this.textBoxDisplayName.ModifiedChanged += new System.EventHandler(this.reregistrationRequired_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBoxAutoAnswerPeriod);
            this.groupBox6.Controls.Add(this.checkBoxAutoAnswer);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.textBoxRTPPort);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.comboBoxDtmfMode);
            this.groupBox6.Controls.Add(this.label19);
            this.groupBox6.Controls.Add(this.textBoxECTail);
            this.groupBox6.Controls.Add(this.checkBoxVAD);
            this.groupBox6.Controls.Add(this.label18);
            this.groupBox6.Controls.Add(this.textBoxExpires);
            this.groupBox6.Controls.Add(this.comboBoxSIPTransport);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.textBoxListenPort);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox6.Location = new System.Drawing.Point(3, 152);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(290, 216);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            // 
            // textBoxAutoAnswerPeriod
            // 
            this.textBoxAutoAnswerPeriod.Location = new System.Drawing.Point(202, 192);
            this.textBoxAutoAnswerPeriod.Name = "textBoxAutoAnswerPeriod";
            this.textBoxAutoAnswerPeriod.Size = new System.Drawing.Size(72, 20);
            this.textBoxAutoAnswerPeriod.TabIndex = 8;
            // 
            // checkBoxAutoAnswer
            // 
            this.checkBoxAutoAnswer.AutoSize = true;
            this.checkBoxAutoAnswer.Location = new System.Drawing.Point(10, 194);
            this.checkBoxAutoAnswer.Name = "checkBoxAutoAnswer";
            this.checkBoxAutoAnswer.Size = new System.Drawing.Size(182, 17);
            this.checkBoxAutoAnswer.TabIndex = 7;
            this.checkBoxAutoAnswer.Text = "Answer calls automatically in [ms]";
            this.checkBoxAutoAnswer.UseVisualStyleBackColor = true;
            this.checkBoxAutoAnswer.CheckedChanged += new System.EventHandler(this.checkBoxAutoAnswer_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Starting port number for RTP";
            // 
            // textBoxRTPPort
            // 
            this.textBoxRTPPort.Location = new System.Drawing.Point(189, 93);
            this.textBoxRTPPort.Name = "textBoxRTPPort";
            this.textBoxRTPPort.Size = new System.Drawing.Size(85, 20);
            this.textBoxRTPPort.TabIndex = 3;
            this.textBoxRTPPort.Text = "4000";
            this.textBoxRTPPort.TextChanged += new System.EventHandler(this.restartRequired_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(7, 69);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 13);
            this.label16.TabIndex = 20;
            this.label16.Text = "Dtmf Mode";
            // 
            // comboBoxDtmfMode
            // 
            this.comboBoxDtmfMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDtmfMode.FormattingEnabled = true;
            this.comboBoxDtmfMode.Items.AddRange(new object[] {
            "Inband (rfc2833)",
            "Transparent",
            "Out-of-band (INFO)"});
            this.comboBoxDtmfMode.Location = new System.Drawing.Point(90, 66);
            this.comboBoxDtmfMode.Name = "comboBoxDtmfMode";
            this.comboBoxDtmfMode.Size = new System.Drawing.Size(184, 21);
            this.comboBoxDtmfMode.TabIndex = 2;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(7, 148);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(133, 13);
            this.label19.TabIndex = 16;
            this.label19.Text = "Echo Cancelation Tail [ms]";
            // 
            // textBoxECTail
            // 
            this.textBoxECTail.Location = new System.Drawing.Point(202, 145);
            this.textBoxECTail.Name = "textBoxECTail";
            this.textBoxECTail.Size = new System.Drawing.Size(72, 20);
            this.textBoxECTail.TabIndex = 5;
            // 
            // checkBoxVAD
            // 
            this.checkBoxVAD.AutoSize = true;
            this.checkBoxVAD.Location = new System.Drawing.Point(10, 171);
            this.checkBoxVAD.Name = "checkBoxVAD";
            this.checkBoxVAD.Size = new System.Drawing.Size(170, 17);
            this.checkBoxVAD.TabIndex = 6;
            this.checkBoxVAD.Text = "Voice Activity Detection (VAD)";
            this.checkBoxVAD.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(7, 122);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(114, 13);
            this.label18.TabIndex = 15;
            this.label18.Text = "Registration timeout [s]";
            // 
            // textBoxExpires
            // 
            this.textBoxExpires.Location = new System.Drawing.Point(189, 119);
            this.textBoxExpires.Name = "textBoxExpires";
            this.textBoxExpires.Size = new System.Drawing.Size(85, 20);
            this.textBoxExpires.TabIndex = 4;
            this.textBoxExpires.Text = "3600";
            // 
            // comboBoxSIPTransport
            // 
            this.comboBoxSIPTransport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSIPTransport.FormattingEnabled = true;
            this.comboBoxSIPTransport.Items.AddRange(new object[] {
            "UDP",
            "TCP"});
            this.comboBoxSIPTransport.Location = new System.Drawing.Point(90, 39);
            this.comboBoxSIPTransport.Name = "comboBoxSIPTransport";
            this.comboBoxSIPTransport.Size = new System.Drawing.Size(184, 21);
            this.comboBoxSIPTransport.TabIndex = 1;
            this.comboBoxSIPTransport.Click += new System.EventHandler(this.restartRequired_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(7, 42);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(52, 13);
            this.label17.TabIndex = 12;
            this.label17.Text = "Transport";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 13);
            this.label12.TabIndex = 9;
            this.label12.Text = "Listening port";
            // 
            // textBoxListenPort
            // 
            this.textBoxListenPort.Location = new System.Drawing.Point(90, 13);
            this.textBoxListenPort.Name = "textBoxListenPort";
            this.textBoxListenPort.Size = new System.Drawing.Size(56, 20);
            this.textBoxListenPort.TabIndex = 0;
            this.textBoxListenPort.Text = "5060";
            this.textBoxListenPort.ModifiedChanged += new System.EventHandler(this.restartRequired_TextChanged);
            this.textBoxListenPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.numEvaluate_KeyPress);
            // 
            // tabPageSettingsAudio
            // 
            this.tabPageSettingsAudio.Controls.Add(this.groupBoxSpeakerphone);
            this.tabPageSettingsAudio.Controls.Add(this.groupBox5);
            this.tabPageSettingsAudio.Controls.Add(this.groupBox4);
            this.tabPageSettingsAudio.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettingsAudio.Name = "tabPageSettingsAudio";
            this.tabPageSettingsAudio.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettingsAudio.Size = new System.Drawing.Size(296, 371);
            this.tabPageSettingsAudio.TabIndex = 0;
            this.tabPageSettingsAudio.Text = "Audio";
            this.tabPageSettingsAudio.UseVisualStyleBackColor = true;
            // 
            // groupBoxSpeakerphone
            // 
            this.groupBoxSpeakerphone.Controls.Add(this.comboBoxRecordingDevices);
            this.groupBoxSpeakerphone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSpeakerphone.Location = new System.Drawing.Point(3, 112);
            this.groupBoxSpeakerphone.Name = "groupBoxSpeakerphone";
            this.groupBoxSpeakerphone.Size = new System.Drawing.Size(290, 124);
            this.groupBoxSpeakerphone.TabIndex = 2;
            this.groupBoxSpeakerphone.TabStop = false;
            this.groupBoxSpeakerphone.Text = "Recording";
            // 
            // comboBoxRecordingDevices
            // 
            this.comboBoxRecordingDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecordingDevices.FormattingEnabled = true;
            this.comboBoxRecordingDevices.Location = new System.Drawing.Point(11, 25);
            this.comboBoxRecordingDevices.Name = "comboBoxRecordingDevices";
            this.comboBoxRecordingDevices.Size = new System.Drawing.Size(199, 21);
            this.comboBoxRecordingDevices.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxPlayRingback);
            this.groupBox5.Controls.Add(this.checkBoxPlayIncomingWhileActive);
            this.groupBox5.Controls.Add(this.checkBoxPlayIncomingSound);
            this.groupBox5.Controls.Add(this.comboBoxSpeakerphoneDevices);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox5.Location = new System.Drawing.Point(3, 236);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(290, 132);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Options";
            // 
            // checkBoxPlayRingback
            // 
            this.checkBoxPlayRingback.AutoSize = true;
            this.checkBoxPlayRingback.Location = new System.Drawing.Point(11, 85);
            this.checkBoxPlayRingback.Name = "checkBoxPlayRingback";
            this.checkBoxPlayRingback.Size = new System.Drawing.Size(122, 17);
            this.checkBoxPlayRingback.TabIndex = 7;
            this.checkBoxPlayRingback.Text = "Play ringback sound";
            this.checkBoxPlayRingback.UseVisualStyleBackColor = true;
            // 
            // checkBoxPlayIncomingWhileActive
            // 
            this.checkBoxPlayIncomingWhileActive.AutoSize = true;
            this.checkBoxPlayIncomingWhileActive.Location = new System.Drawing.Point(29, 49);
            this.checkBoxPlayIncomingWhileActive.Name = "checkBoxPlayIncomingWhileActive";
            this.checkBoxPlayIncomingWhileActive.Size = new System.Drawing.Size(164, 17);
            this.checkBoxPlayIncomingWhileActive.TabIndex = 6;
            this.checkBoxPlayIncomingWhileActive.Text = "Play sound even if active call";
            this.checkBoxPlayIncomingWhileActive.UseVisualStyleBackColor = true;
            // 
            // checkBoxPlayIncomingSound
            // 
            this.checkBoxPlayIncomingSound.AutoSize = true;
            this.checkBoxPlayIncomingSound.Location = new System.Drawing.Point(11, 26);
            this.checkBoxPlayIncomingSound.Name = "checkBoxPlayIncomingSound";
            this.checkBoxPlayIncomingSound.Size = new System.Drawing.Size(157, 17);
            this.checkBoxPlayIncomingSound.TabIndex = 5;
            this.checkBoxPlayIncomingSound.Text = "Play sound for incoming call";
            this.checkBoxPlayIncomingSound.UseVisualStyleBackColor = true;
            this.checkBoxPlayIncomingSound.CheckedChanged += new System.EventHandler(this.checkBoxPlayIncomingSound_CheckedChanged);
            // 
            // comboBoxSpeakerphoneDevices
            // 
            this.comboBoxSpeakerphoneDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpeakerphoneDevices.FormattingEnabled = true;
            this.comboBoxSpeakerphoneDevices.Location = new System.Drawing.Point(265, 105);
            this.comboBoxSpeakerphoneDevices.Name = "comboBoxSpeakerphoneDevices";
            this.comboBoxSpeakerphoneDevices.Size = new System.Drawing.Size(19, 21);
            this.comboBoxSpeakerphoneDevices.TabIndex = 4;
            this.comboBoxSpeakerphoneDevices.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.comboBoxPlaybackDevices);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(290, 109);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Playback";
            // 
            // comboBoxPlaybackDevices
            // 
            this.comboBoxPlaybackDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPlaybackDevices.FormattingEnabled = true;
            this.comboBoxPlaybackDevices.Location = new System.Drawing.Point(11, 26);
            this.comboBoxPlaybackDevices.Name = "comboBoxPlaybackDevices";
            this.comboBoxPlaybackDevices.Size = new System.Drawing.Size(200, 21);
            this.comboBoxPlaybackDevices.TabIndex = 3;
            // 
            // tabCodecsPage
            // 
            this.tabCodecsPage.Controls.Add(this.label11);
            this.tabCodecsPage.Controls.Add(this.label10);
            this.tabCodecsPage.Controls.Add(this.buttonDisable);
            this.tabCodecsPage.Controls.Add(this.buttonEnable);
            this.tabCodecsPage.Controls.Add(this.listBoxEnCodecs);
            this.tabCodecsPage.Controls.Add(this.listBoxDisCodecs);
            this.tabCodecsPage.Location = new System.Drawing.Point(4, 22);
            this.tabCodecsPage.Name = "tabCodecsPage";
            this.tabCodecsPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabCodecsPage.Size = new System.Drawing.Size(296, 371);
            this.tabCodecsPage.TabIndex = 3;
            this.tabCodecsPage.Text = "Codecs";
            this.tabCodecsPage.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(169, 11);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(87, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Enabled codecs:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Disabled codecs:";
            // 
            // buttonDisable
            // 
            this.buttonDisable.Location = new System.Drawing.Point(135, 84);
            this.buttonDisable.Name = "buttonDisable";
            this.buttonDisable.Size = new System.Drawing.Size(29, 23);
            this.buttonDisable.TabIndex = 3;
            this.buttonDisable.Text = "<=";
            this.buttonDisable.UseVisualStyleBackColor = true;
            this.buttonDisable.Click += new System.EventHandler(this.buttonDisable_Click);
            // 
            // buttonEnable
            // 
            this.buttonEnable.Location = new System.Drawing.Point(135, 44);
            this.buttonEnable.Name = "buttonEnable";
            this.buttonEnable.Size = new System.Drawing.Size(29, 23);
            this.buttonEnable.TabIndex = 2;
            this.buttonEnable.Text = "=>";
            this.buttonEnable.UseVisualStyleBackColor = true;
            this.buttonEnable.Click += new System.EventHandler(this.buttonEnable_Click);
            // 
            // listBoxEnCodecs
            // 
            this.listBoxEnCodecs.FormattingEnabled = true;
            this.listBoxEnCodecs.Location = new System.Drawing.Point(169, 31);
            this.listBoxEnCodecs.Name = "listBoxEnCodecs";
            this.listBoxEnCodecs.Size = new System.Drawing.Size(120, 303);
            this.listBoxEnCodecs.TabIndex = 1;
            // 
            // listBoxDisCodecs
            // 
            this.listBoxDisCodecs.FormattingEnabled = true;
            this.listBoxDisCodecs.Location = new System.Drawing.Point(8, 31);
            this.listBoxDisCodecs.Name = "listBoxDisCodecs";
            this.listBoxDisCodecs.Size = new System.Drawing.Size(120, 303);
            this.listBoxDisCodecs.TabIndex = 0;
            // 
            // tabPagePlugins
            // 
            this.tabPagePlugins.Controls.Add(groupBox1);
            this.tabPagePlugins.Controls.Add(this.label21);
            this.tabPagePlugins.Controls.Add(this.groupBoxPluginInfo);
            this.tabPagePlugins.Controls.Add(this.listBoxPlugins);
            this.tabPagePlugins.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlugins.Name = "tabPagePlugins";
            this.tabPagePlugins.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePlugins.Size = new System.Drawing.Size(296, 371);
            this.tabPagePlugins.TabIndex = 5;
            this.tabPagePlugins.Text = "Plugins";
            this.tabPagePlugins.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(8, 3);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(44, 13);
            this.label21.TabIndex = 2;
            this.label21.Text = "Plugins:";
            // 
            // groupBoxPluginInfo
            // 
            this.groupBoxPluginInfo.Controls.Add(this.buttonPluginSettings);
            this.groupBoxPluginInfo.Controls.Add(this.buttonPluginStop);
            this.groupBoxPluginInfo.Controls.Add(this.buttonPluginStart);
            this.groupBoxPluginInfo.Controls.Add(this.checkBoxAutoStartPlugin);
            this.groupBoxPluginInfo.Controls.Add(this.labelPluginVersion);
            this.groupBoxPluginInfo.Controls.Add(this.labelPluginName);
            this.groupBoxPluginInfo.Controls.Add(this.labelPluginID);
            this.groupBoxPluginInfo.Controls.Add(this.label24);
            this.groupBoxPluginInfo.Controls.Add(this.label23);
            this.groupBoxPluginInfo.Controls.Add(this.label6);
            this.groupBoxPluginInfo.Enabled = false;
            this.groupBoxPluginInfo.Location = new System.Drawing.Point(8, 198);
            this.groupBoxPluginInfo.Name = "groupBoxPluginInfo";
            this.groupBoxPluginInfo.Size = new System.Drawing.Size(280, 115);
            this.groupBoxPluginInfo.TabIndex = 1;
            this.groupBoxPluginInfo.TabStop = false;
            this.groupBoxPluginInfo.Text = "Plugin information:";
            // 
            // buttonPluginSettings
            // 
            this.buttonPluginSettings.Location = new System.Drawing.Point(199, 84);
            this.buttonPluginSettings.Name = "buttonPluginSettings";
            this.buttonPluginSettings.Size = new System.Drawing.Size(75, 23);
            this.buttonPluginSettings.TabIndex = 11;
            this.buttonPluginSettings.Text = "Settings";
            this.buttonPluginSettings.UseVisualStyleBackColor = true;
            this.buttonPluginSettings.Click += new System.EventHandler(this.buttonPluginSettings_Click);
            // 
            // buttonPluginStop
            // 
            this.buttonPluginStop.Location = new System.Drawing.Point(90, 84);
            this.buttonPluginStop.Name = "buttonPluginStop";
            this.buttonPluginStop.Size = new System.Drawing.Size(75, 23);
            this.buttonPluginStop.TabIndex = 10;
            this.buttonPluginStop.Text = "Stop";
            this.buttonPluginStop.UseVisualStyleBackColor = true;
            this.buttonPluginStop.Click += new System.EventHandler(this.buttonPluginStop_Click);
            // 
            // buttonPluginStart
            // 
            this.buttonPluginStart.Location = new System.Drawing.Point(9, 84);
            this.buttonPluginStart.Name = "buttonPluginStart";
            this.buttonPluginStart.Size = new System.Drawing.Size(75, 23);
            this.buttonPluginStart.TabIndex = 9;
            this.buttonPluginStart.Text = "Start";
            this.buttonPluginStart.UseVisualStyleBackColor = true;
            this.buttonPluginStart.Click += new System.EventHandler(this.buttonPluginStart_Click);
            // 
            // checkBoxAutoStartPlugin
            // 
            this.checkBoxAutoStartPlugin.AutoSize = true;
            this.checkBoxAutoStartPlugin.Checked = true;
            this.checkBoxAutoStartPlugin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoStartPlugin.Location = new System.Drawing.Point(9, 64);
            this.checkBoxAutoStartPlugin.Name = "checkBoxAutoStartPlugin";
            this.checkBoxAutoStartPlugin.Size = new System.Drawing.Size(184, 17);
            this.checkBoxAutoStartPlugin.TabIndex = 8;
            this.checkBoxAutoStartPlugin.Text = "Start plugin when program started";
            this.checkBoxAutoStartPlugin.UseVisualStyleBackColor = true;
            this.checkBoxAutoStartPlugin.CheckedChanged += new System.EventHandler(this.checkBoxAutoStartPlugin_CheckedChanged);
            // 
            // labelPluginVersion
            // 
            this.labelPluginVersion.Location = new System.Drawing.Point(63, 47);
            this.labelPluginVersion.Name = "labelPluginVersion";
            this.labelPluginVersion.Size = new System.Drawing.Size(211, 13);
            this.labelPluginVersion.TabIndex = 7;
            this.labelPluginVersion.Text = "-";
            // 
            // labelPluginName
            // 
            this.labelPluginName.Location = new System.Drawing.Point(63, 31);
            this.labelPluginName.Name = "labelPluginName";
            this.labelPluginName.Size = new System.Drawing.Size(211, 13);
            this.labelPluginName.TabIndex = 5;
            this.labelPluginName.Text = "-";
            // 
            // labelPluginID
            // 
            this.labelPluginID.Location = new System.Drawing.Point(63, 15);
            this.labelPluginID.Name = "labelPluginID";
            this.labelPluginID.Size = new System.Drawing.Size(211, 13);
            this.labelPluginID.TabIndex = 4;
            this.labelPluginID.Text = "-";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(6, 47);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(45, 13);
            this.label24.TabIndex = 3;
            this.label24.Text = "Version:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 31);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(38, 13);
            this.label23.TabIndex = 2;
            this.label23.Text = "Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "ID:";
            // 
            // listBoxPlugins
            // 
            this.listBoxPlugins.FormattingEnabled = true;
            this.listBoxPlugins.Location = new System.Drawing.Point(8, 19);
            this.listBoxPlugins.Name = "listBoxPlugins";
            this.listBoxPlugins.Size = new System.Drawing.Size(280, 173);
            this.listBoxPlugins.TabIndex = 0;
            this.listBoxPlugins.SelectedIndexChanged += new System.EventHandler(this.listBoxPlugins_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonLog);
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Controls.Add(this.buttonOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 396);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(304, 36);
            this.panel2.TabIndex = 12;
            // 
            // buttonLog
            // 
            this.buttonLog.Location = new System.Drawing.Point(7, 4);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(75, 23);
            this.buttonLog.TabIndex = 2;
            this.buttonLog.Text = "Log";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.buttonLogger_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(222, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(141, 4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(304, 432);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tabControlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            this.tabControlSettings.ResumeLayout(false);
            this.tabPageSettingsSIP.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPageSettingsAudio.ResumeLayout(false);
            this.groupBoxSpeakerphone.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tabCodecsPage.ResumeLayout(false);
            this.tabCodecsPage.PerformLayout();
            this.tabPagePlugins.ResumeLayout(false);
            this.tabPagePlugins.PerformLayout();
            this.groupBoxPluginInfo.ResumeLayout(false);
            this.groupBoxPluginInfo.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabPageSettingsAudio;
        private System.Windows.Forms.TabPage tabPageSettingsSIP;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDomain;
        private System.Windows.Forms.TextBox textBoxRegistrarAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDisplayName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox comboBoxPlaybackDevices;
        private System.Windows.Forms.ComboBox comboBoxRecordingDevices;
        private System.Windows.Forms.TabPage tabCodecsPage;
        private System.Windows.Forms.ListBox listBoxDisCodecs;
        private System.Windows.Forms.ListBox listBoxEnCodecs;
        private System.Windows.Forms.Button buttonDisable;
        private System.Windows.Forms.Button buttonEnable;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxListenPort;
        private System.Windows.Forms.ComboBox comboBoxSIPTransport;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TabPage tabPagePlugins;
        private System.Windows.Forms.GroupBox groupBoxPluginInfo;
        private System.Windows.Forms.ListBox listBoxPlugins;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelPluginName;
        private System.Windows.Forms.Label labelPluginID;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button buttonPluginStop;
        private System.Windows.Forms.Button buttonPluginStart;
        private System.Windows.Forms.CheckBox checkBoxAutoStartPlugin;
        private System.Windows.Forms.Label labelPluginVersion;
        private System.Windows.Forms.Button buttonPluginSettings;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboBoxDtmfMode;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBoxECTail;
        private System.Windows.Forms.CheckBox checkBoxVAD;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBoxExpires;
        private System.Windows.Forms.Button buttonLog;
        private System.Windows.Forms.GroupBox groupBoxSpeakerphone;
        private System.Windows.Forms.ComboBox comboBoxSpeakerphoneDevices;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxRTPPort;
        private System.Windows.Forms.CheckBox checkBoxPlayIncomingWhileActive;
        private System.Windows.Forms.CheckBox checkBoxPlayIncomingSound;
        private System.Windows.Forms.CheckBox checkBoxPlayRingback;
        private System.Windows.Forms.TextBox textBoxAutoAnswerPeriod;
        private System.Windows.Forms.CheckBox checkBoxAutoAnswer;
        private System.Windows.Forms.TextBox textBoxLicensePath;
        private System.Windows.Forms.Button buttonLicenseUpload;
    }
}
