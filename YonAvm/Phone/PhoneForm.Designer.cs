namespace YonAvm
{
    partial class PhoneForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhoneForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonSendText = new System.Windows.Forms.Button();
            this.buttonStartHangupCall = new System.Windows.Forms.Button();
            this.PictureErr = new System.Windows.Forms.Panel();
            this.buttonHoldRetrieve = new System.Windows.Forms.Button();
            this.buttonRecordStartStop = new System.Windows.Forms.Button();
            this.buttonPlayStartStop = new System.Windows.Forms.Button();
            this.buttonTransfer = new System.Windows.Forms.Button();
            this.AddressBox = new System.Windows.Forms.ComboBox();
            this.buttonJoin = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxLines = new System.Windows.Forms.GroupBox();
            this.buttonLine5 = new System.Windows.Forms.Button();
            this.activeConnListbox = new System.Windows.Forms.ListBox();
            this.buttonLine6 = new System.Windows.Forms.Button();
            this.buttonLine1 = new System.Windows.Forms.Button();
            this.buttonLine4 = new System.Windows.Forms.Button();
            this.buttonLine3 = new System.Windows.Forms.Button();
            this.buttonLine2 = new System.Windows.Forms.Button();
            this.callDurationLabel = new System.Windows.Forms.Label();
            this.CallPanel = new System.Windows.Forms.GroupBox();
            this.DTFM6 = new System.Windows.Forms.Button();
            this.DTFMStar = new System.Windows.Forms.Button();
            this.DTFM2 = new System.Windows.Forms.Button();
            this.DTFM3 = new System.Windows.Forms.Button();
            this.DTFM9 = new System.Windows.Forms.Button();
            this.DTFM8 = new System.Windows.Forms.Button();
            this.DTFM1 = new System.Windows.Forms.Button();
            this.DTFM0 = new System.Windows.Forms.Button();
            this.DTFM5 = new System.Windows.Forms.Button();
            this.DTFMHash = new System.Windows.Forms.Button();
            this.DTFM4 = new System.Windows.Forms.Button();
            this.DTFM7 = new System.Windows.Forms.Button();
            this.muteSpeakerFlag = new System.Windows.Forms.CheckBox();
            this.spkVolumeBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.UInputLabel = new System.Windows.Forms.Label();
            this.micVolumeBar = new System.Windows.Forms.ProgressBar();
            this.micVolume = new System.Windows.Forms.TrackBar();
            this.spkVolume = new System.Windows.Forms.TrackBar();
            this.muteMicrophoneFlag = new System.Windows.Forms.CheckBox();
            this.notificationsListBox = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.lblversion2 = new System.Windows.Forms.Label();
            this.alertControl1 = new DevExpress.XtraBars.Alerter.AlertControl(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBoxLines.SuspendLayout();
            this.CallPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.micVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spkVolume)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(100, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(86, 54);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 32;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonSendText);
            this.groupBox1.Controls.Add(this.buttonStartHangupCall);
            this.groupBox1.Controls.Add(this.PictureErr);
            this.groupBox1.Controls.Add(this.buttonHoldRetrieve);
            this.groupBox1.Controls.Add(this.buttonRecordStartStop);
            this.groupBox1.Controls.Add(this.buttonPlayStartStop);
            this.groupBox1.Controls.Add(this.buttonTransfer);
            this.groupBox1.Controls.Add(this.AddressBox);
            this.groupBox1.Controls.Add(this.buttonJoin);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(382, 82);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            // 
            // buttonSendText
            // 
            this.buttonSendText.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSendText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonSendText.Location = new System.Drawing.Point(5, 44);
            this.buttonSendText.Name = "buttonSendText";
            this.buttonSendText.Size = new System.Drawing.Size(64, 30);
            this.buttonSendText.TabIndex = 5;
            this.buttonSendText.Text = "Message";
            this.buttonSendText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonSendText.UseVisualStyleBackColor = true;
            this.buttonSendText.Click += new System.EventHandler(this.sendTextBtn_Click);
            // 
            // buttonStartHangupCall
            // 
            this.buttonStartHangupCall.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowFrame;
            this.buttonStartHangupCall.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonStartHangupCall.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonStartHangupCall.Location = new System.Drawing.Point(264, 15);
            this.buttonStartHangupCall.Name = "buttonStartHangupCall";
            this.buttonStartHangupCall.Size = new System.Drawing.Size(114, 23);
            this.buttonStartHangupCall.TabIndex = 4;
            this.buttonStartHangupCall.Text = "Arama Başlat";
            this.buttonStartHangupCall.UseVisualStyleBackColor = true;
            this.buttonStartHangupCall.Click += new System.EventHandler(this.callStartHangupBtn_Click);
            // 
            // PictureErr
            // 
            this.PictureErr.Location = new System.Drawing.Point(171, 47);
            this.PictureErr.Name = "PictureErr";
            this.PictureErr.Size = new System.Drawing.Size(7, 27);
            this.PictureErr.TabIndex = 24;
            // 
            // buttonHoldRetrieve
            // 
            this.buttonHoldRetrieve.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonHoldRetrieve.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonHoldRetrieve.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonHoldRetrieve.Location = new System.Drawing.Point(295, 44);
            this.buttonHoldRetrieve.Name = "buttonHoldRetrieve";
            this.buttonHoldRetrieve.Size = new System.Drawing.Size(83, 30);
            this.buttonHoldRetrieve.TabIndex = 9;
            this.buttonHoldRetrieve.Text = "Beklet";
            this.buttonHoldRetrieve.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonHoldRetrieve.UseVisualStyleBackColor = true;
            this.buttonHoldRetrieve.Click += new System.EventHandler(this.holdRetreiveBtn_Click);
            // 
            // buttonRecordStartStop
            // 
            this.buttonRecordStartStop.Enabled = false;
            this.buttonRecordStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonRecordStartStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonRecordStartStop.Location = new System.Drawing.Point(237, 44);
            this.buttonRecordStartStop.Name = "buttonRecordStartStop";
            this.buttonRecordStartStop.Size = new System.Drawing.Size(57, 30);
            this.buttonRecordStartStop.TabIndex = 8;
            this.buttonRecordStartStop.Text = "Kayıt";
            this.buttonRecordStartStop.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonRecordStartStop.UseVisualStyleBackColor = true;
            this.buttonRecordStartStop.Click += new System.EventHandler(this.recordStartStopBtn_Click);
            // 
            // buttonPlayStartStop
            // 
            this.buttonPlayStartStop.Enabled = false;
            this.buttonPlayStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonPlayStartStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPlayStartStop.Location = new System.Drawing.Point(179, 44);
            this.buttonPlayStartStop.Name = "buttonPlayStartStop";
            this.buttonPlayStartStop.Size = new System.Drawing.Size(57, 30);
            this.buttonPlayStartStop.TabIndex = 7;
            this.buttonPlayStartStop.Text = "Oynat";
            this.buttonPlayStartStop.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonPlayStartStop.UseVisualStyleBackColor = true;
            this.buttonPlayStartStop.Click += new System.EventHandler(this.playStartStopButton_Click);
            // 
            // buttonTransfer
            // 
            this.buttonTransfer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonTransfer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonTransfer.Location = new System.Drawing.Point(70, 44);
            this.buttonTransfer.Name = "buttonTransfer";
            this.buttonTransfer.Size = new System.Drawing.Size(42, 30);
            this.buttonTransfer.TabIndex = 5;
            this.buttonTransfer.Text = "Aktar";
            this.buttonTransfer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonTransfer.UseVisualStyleBackColor = true;
            this.buttonTransfer.Click += new System.EventHandler(this.transferBtn_Click);
            // 
            // AddressBox
            // 
            this.AddressBox.FormattingEnabled = true;
            this.AddressBox.Location = new System.Drawing.Point(50, 17);
            this.AddressBox.Name = "AddressBox";
            this.AddressBox.Size = new System.Drawing.Size(213, 21);
            this.AddressBox.TabIndex = 3;
            // 
            // buttonJoin
            // 
            this.buttonJoin.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonJoin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonJoin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonJoin.Location = new System.Drawing.Point(113, 44);
            this.buttonJoin.Name = "buttonJoin";
            this.buttonJoin.Size = new System.Drawing.Size(57, 30);
            this.buttonJoin.TabIndex = 6;
            this.buttonJoin.Text = "Katıl";
            this.buttonJoin.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonJoin.UseVisualStyleBackColor = true;
            this.buttonJoin.Click += new System.EventHandler(this.buttonJoin_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(4, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Telefon:";
            // 
            // groupBoxLines
            // 
            this.groupBoxLines.Controls.Add(this.buttonLine5);
            this.groupBoxLines.Controls.Add(this.activeConnListbox);
            this.groupBoxLines.Controls.Add(this.buttonLine6);
            this.groupBoxLines.Controls.Add(this.buttonLine1);
            this.groupBoxLines.Controls.Add(this.buttonLine4);
            this.groupBoxLines.Controls.Add(this.buttonLine3);
            this.groupBoxLines.Controls.Add(this.buttonLine2);
            this.groupBoxLines.Controls.Add(this.callDurationLabel);
            this.groupBoxLines.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxLines.Location = new System.Drawing.Point(0, 136);
            this.groupBoxLines.Name = "groupBoxLines";
            this.groupBoxLines.Size = new System.Drawing.Size(382, 80);
            this.groupBoxLines.TabIndex = 34;
            this.groupBoxLines.TabStop = false;
            // 
            // buttonLine5
            // 
            this.buttonLine5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLine5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonLine5.Location = new System.Drawing.Point(262, 35);
            this.buttonLine5.Name = "buttonLine5";
            this.buttonLine5.Size = new System.Drawing.Size(53, 23);
            this.buttonLine5.TabIndex = 5;
            this.buttonLine5.Text = "Hat 5";
            this.buttonLine5.UseVisualStyleBackColor = true;
            this.buttonLine5.Click += new System.EventHandler(this.lineBtn_Click);
            // 
            // activeConnListbox
            // 
            this.activeConnListbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.activeConnListbox.FormattingEnabled = true;
            this.activeConnListbox.HorizontalScrollbar = true;
            this.activeConnListbox.Location = new System.Drawing.Point(6, 11);
            this.activeConnListbox.Name = "activeConnListbox";
            this.activeConnListbox.Size = new System.Drawing.Size(190, 54);
            this.activeConnListbox.TabIndex = 0;
            this.activeConnListbox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.activeConnListbox_MouseDoubleClick);
            // 
            // buttonLine6
            // 
            this.buttonLine6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLine6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonLine6.Location = new System.Drawing.Point(324, 35);
            this.buttonLine6.Name = "buttonLine6";
            this.buttonLine6.Size = new System.Drawing.Size(53, 23);
            this.buttonLine6.TabIndex = 6;
            this.buttonLine6.Text = "Hat 6";
            this.buttonLine6.UseVisualStyleBackColor = true;
            this.buttonLine6.Click += new System.EventHandler(this.lineBtn_Click);
            // 
            // buttonLine1
            // 
            this.buttonLine1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonLine1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLine1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonLine1.Location = new System.Drawing.Point(200, 11);
            this.buttonLine1.Name = "buttonLine1";
            this.buttonLine1.Size = new System.Drawing.Size(53, 23);
            this.buttonLine1.TabIndex = 1;
            this.buttonLine1.Text = "Hat 1";
            this.buttonLine1.UseVisualStyleBackColor = true;
            this.buttonLine1.Click += new System.EventHandler(this.lineBtn_Click);
            // 
            // buttonLine4
            // 
            this.buttonLine4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLine4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonLine4.Location = new System.Drawing.Point(200, 35);
            this.buttonLine4.Name = "buttonLine4";
            this.buttonLine4.Size = new System.Drawing.Size(53, 23);
            this.buttonLine4.TabIndex = 4;
            this.buttonLine4.Text = "Hat 4";
            this.buttonLine4.UseVisualStyleBackColor = true;
            this.buttonLine4.Click += new System.EventHandler(this.lineBtn_Click);
            // 
            // buttonLine3
            // 
            this.buttonLine3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLine3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonLine3.Location = new System.Drawing.Point(324, 11);
            this.buttonLine3.Name = "buttonLine3";
            this.buttonLine3.Size = new System.Drawing.Size(53, 23);
            this.buttonLine3.TabIndex = 3;
            this.buttonLine3.Text = "Hat 3";
            this.buttonLine3.UseVisualStyleBackColor = true;
            this.buttonLine3.Click += new System.EventHandler(this.lineBtn_Click);
            // 
            // buttonLine2
            // 
            this.buttonLine2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonLine2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLine2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonLine2.Location = new System.Drawing.Point(262, 11);
            this.buttonLine2.Name = "buttonLine2";
            this.buttonLine2.Size = new System.Drawing.Size(53, 23);
            this.buttonLine2.TabIndex = 2;
            this.buttonLine2.Text = "Hat 2";
            this.buttonLine2.UseVisualStyleBackColor = true;
            this.buttonLine2.Click += new System.EventHandler(this.lineBtn_Click);
            // 
            // callDurationLabel
            // 
            this.callDurationLabel.AutoSize = true;
            this.callDurationLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.callDurationLabel.Location = new System.Drawing.Point(204, 62);
            this.callDurationLabel.Name = "callDurationLabel";
            this.callDurationLabel.Size = new System.Drawing.Size(116, 13);
            this.callDurationLabel.TabIndex = 6;
            this.callDurationLabel.Text = "Çagrı Süresi: 00:00:00";
            // 
            // CallPanel
            // 
            this.CallPanel.Controls.Add(this.DTFM6);
            this.CallPanel.Controls.Add(this.DTFMStar);
            this.CallPanel.Controls.Add(this.DTFM2);
            this.CallPanel.Controls.Add(this.DTFM3);
            this.CallPanel.Controls.Add(this.DTFM9);
            this.CallPanel.Controls.Add(this.DTFM8);
            this.CallPanel.Controls.Add(this.DTFM1);
            this.CallPanel.Controls.Add(this.DTFM0);
            this.CallPanel.Controls.Add(this.DTFM5);
            this.CallPanel.Controls.Add(this.DTFMHash);
            this.CallPanel.Controls.Add(this.DTFM4);
            this.CallPanel.Controls.Add(this.DTFM7);
            this.CallPanel.Controls.Add(this.muteSpeakerFlag);
            this.CallPanel.Controls.Add(this.spkVolumeBar);
            this.CallPanel.Controls.Add(this.label1);
            this.CallPanel.Controls.Add(this.UInputLabel);
            this.CallPanel.Controls.Add(this.micVolumeBar);
            this.CallPanel.Controls.Add(this.micVolume);
            this.CallPanel.Controls.Add(this.spkVolume);
            this.CallPanel.Controls.Add(this.muteMicrophoneFlag);
            this.CallPanel.Location = new System.Drawing.Point(0, 336);
            this.CallPanel.Name = "CallPanel";
            this.CallPanel.Size = new System.Drawing.Size(421, 142);
            this.CallPanel.TabIndex = 35;
            this.CallPanel.TabStop = false;
            // 
            // DTFM6
            // 
            this.DTFM6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM6.Location = new System.Drawing.Point(371, 44);
            this.DTFM6.Name = "DTFM6";
            this.DTFM6.Size = new System.Drawing.Size(30, 30);
            this.DTFM6.TabIndex = 17;
            this.DTFM6.Tag = "6";
            this.DTFM6.Text = "6";
            this.DTFM6.UseVisualStyleBackColor = true;
            this.DTFM6.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFMStar
            // 
            this.DTFMStar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFMStar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFMStar.Location = new System.Drawing.Point(309, 106);
            this.DTFMStar.Name = "DTFMStar";
            this.DTFMStar.Size = new System.Drawing.Size(30, 30);
            this.DTFMStar.TabIndex = 21;
            this.DTFMStar.Tag = "10";
            this.DTFMStar.Text = "*";
            this.DTFMStar.UseVisualStyleBackColor = true;
            this.DTFMStar.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM2
            // 
            this.DTFM2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM2.Location = new System.Drawing.Point(340, 13);
            this.DTFM2.Name = "DTFM2";
            this.DTFM2.Size = new System.Drawing.Size(30, 30);
            this.DTFM2.TabIndex = 13;
            this.DTFM2.Tag = "2";
            this.DTFM2.Text = "2";
            this.DTFM2.UseVisualStyleBackColor = true;
            this.DTFM2.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM3
            // 
            this.DTFM3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM3.Location = new System.Drawing.Point(371, 13);
            this.DTFM3.Name = "DTFM3";
            this.DTFM3.Size = new System.Drawing.Size(30, 30);
            this.DTFM3.TabIndex = 14;
            this.DTFM3.Tag = "3";
            this.DTFM3.Text = "3";
            this.DTFM3.UseVisualStyleBackColor = true;
            this.DTFM3.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM9
            // 
            this.DTFM9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM9.Location = new System.Drawing.Point(371, 75);
            this.DTFM9.Name = "DTFM9";
            this.DTFM9.Size = new System.Drawing.Size(30, 30);
            this.DTFM9.TabIndex = 20;
            this.DTFM9.Tag = "9";
            this.DTFM9.Text = "9";
            this.DTFM9.UseVisualStyleBackColor = true;
            this.DTFM9.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM8
            // 
            this.DTFM8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM8.Location = new System.Drawing.Point(340, 75);
            this.DTFM8.Name = "DTFM8";
            this.DTFM8.Size = new System.Drawing.Size(30, 30);
            this.DTFM8.TabIndex = 19;
            this.DTFM8.Tag = "8";
            this.DTFM8.Text = "8";
            this.DTFM8.UseVisualStyleBackColor = true;
            this.DTFM8.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM1
            // 
            this.DTFM1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM1.Location = new System.Drawing.Point(309, 13);
            this.DTFM1.Name = "DTFM1";
            this.DTFM1.Size = new System.Drawing.Size(30, 30);
            this.DTFM1.TabIndex = 12;
            this.DTFM1.Tag = "1";
            this.DTFM1.Text = "1";
            this.DTFM1.UseVisualStyleBackColor = true;
            this.DTFM1.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM0
            // 
            this.DTFM0.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM0.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM0.Location = new System.Drawing.Point(340, 106);
            this.DTFM0.Name = "DTFM0";
            this.DTFM0.Size = new System.Drawing.Size(30, 30);
            this.DTFM0.TabIndex = 22;
            this.DTFM0.Tag = "0";
            this.DTFM0.Text = "0";
            this.DTFM0.UseVisualStyleBackColor = true;
            this.DTFM0.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM5
            // 
            this.DTFM5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM5.Location = new System.Drawing.Point(340, 44);
            this.DTFM5.Name = "DTFM5";
            this.DTFM5.Size = new System.Drawing.Size(30, 30);
            this.DTFM5.TabIndex = 16;
            this.DTFM5.Tag = "5";
            this.DTFM5.Text = "5";
            this.DTFM5.UseVisualStyleBackColor = true;
            this.DTFM5.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFMHash
            // 
            this.DTFMHash.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFMHash.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFMHash.Location = new System.Drawing.Point(371, 106);
            this.DTFMHash.Name = "DTFMHash";
            this.DTFMHash.Size = new System.Drawing.Size(30, 30);
            this.DTFMHash.TabIndex = 23;
            this.DTFMHash.Tag = "11";
            this.DTFMHash.Text = "#";
            this.DTFMHash.UseVisualStyleBackColor = true;
            this.DTFMHash.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM4
            // 
            this.DTFM4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM4.Location = new System.Drawing.Point(309, 44);
            this.DTFM4.Name = "DTFM4";
            this.DTFM4.Size = new System.Drawing.Size(30, 30);
            this.DTFM4.TabIndex = 15;
            this.DTFM4.Tag = "4";
            this.DTFM4.Text = "4";
            this.DTFM4.UseVisualStyleBackColor = true;
            this.DTFM4.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // DTFM7
            // 
            this.DTFM7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DTFM7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DTFM7.Location = new System.Drawing.Point(309, 75);
            this.DTFM7.Name = "DTFM7";
            this.DTFM7.Size = new System.Drawing.Size(30, 30);
            this.DTFM7.TabIndex = 18;
            this.DTFM7.Tag = "7";
            this.DTFM7.Text = "7";
            this.DTFM7.UseVisualStyleBackColor = true;
            this.DTFM7.Click += new System.EventHandler(this.DTFM_Click);
            // 
            // muteSpeakerFlag
            // 
            this.muteSpeakerFlag.AutoSize = true;
            this.muteSpeakerFlag.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.muteSpeakerFlag.Location = new System.Drawing.Point(5, 15);
            this.muteSpeakerFlag.Name = "muteSpeakerFlag";
            this.muteSpeakerFlag.Size = new System.Drawing.Size(108, 17);
            this.muteSpeakerFlag.TabIndex = 0;
            this.muteSpeakerFlag.Text = "Speaker Volume";
            this.muteSpeakerFlag.UseVisualStyleBackColor = true;
            this.muteSpeakerFlag.CheckStateChanged += new System.EventHandler(this.muteSoundCB_CheckedChanged);
            // 
            // spkVolumeBar
            // 
            this.spkVolumeBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.spkVolumeBar.Location = new System.Drawing.Point(5, 40);
            this.spkVolumeBar.Maximum = 30000;
            this.spkVolumeBar.Name = "spkVolumeBar";
            this.spkVolumeBar.Size = new System.Drawing.Size(260, 10);
            this.spkVolumeBar.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "User Input: ";
            // 
            // UInputLabel
            // 
            this.UInputLabel.AutoSize = true;
            this.UInputLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UInputLabel.Location = new System.Drawing.Point(70, 110);
            this.UInputLabel.Name = "UInputLabel";
            this.UInputLabel.Size = new System.Drawing.Size(16, 13);
            this.UInputLabel.TabIndex = 7;
            this.UInputLabel.Text = "...";
            // 
            // micVolumeBar
            // 
            this.micVolumeBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.micVolumeBar.Location = new System.Drawing.Point(5, 90);
            this.micVolumeBar.Maximum = 30000;
            this.micVolumeBar.Name = "micVolumeBar";
            this.micVolumeBar.Size = new System.Drawing.Size(260, 10);
            this.micVolumeBar.TabIndex = 5;
            // 
            // micVolume
            // 
            this.micVolume.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.micVolume.Location = new System.Drawing.Point(141, 60);
            this.micVolume.Maximum = 100;
            this.micVolume.Name = "micVolume";
            this.micVolume.Size = new System.Drawing.Size(131, 45);
            this.micVolume.TabIndex = 4;
            this.micVolume.TickFrequency = 10;
            this.micVolume.Scroll += new System.EventHandler(this.MicVolume_Scroll);
            // 
            // spkVolume
            // 
            this.spkVolume.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.spkVolume.Location = new System.Drawing.Point(141, 10);
            this.spkVolume.Maximum = 100;
            this.spkVolume.Name = "spkVolume";
            this.spkVolume.Size = new System.Drawing.Size(129, 45);
            this.spkVolume.TabIndex = 1;
            this.spkVolume.TickFrequency = 10;
            this.spkVolume.Scroll += new System.EventHandler(this.SpkVolume_Scroll);
            // 
            // muteMicrophoneFlag
            // 
            this.muteMicrophoneFlag.AutoSize = true;
            this.muteMicrophoneFlag.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.muteMicrophoneFlag.Location = new System.Drawing.Point(5, 65);
            this.muteMicrophoneFlag.Name = "muteMicrophoneFlag";
            this.muteMicrophoneFlag.Size = new System.Drawing.Size(130, 17);
            this.muteMicrophoneFlag.TabIndex = 3;
            this.muteMicrophoneFlag.Text = "Microphone Volume";
            this.muteMicrophoneFlag.UseVisualStyleBackColor = true;
            this.muteMicrophoneFlag.CheckStateChanged += new System.EventHandler(this.muteMicCB_CheckedChanged);
            // 
            // notificationsListBox
            // 
            this.notificationsListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.notificationsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notificationsListBox.FormattingEnabled = true;
            this.notificationsListBox.Location = new System.Drawing.Point(0, 216);
            this.notificationsListBox.Name = "notificationsListBox";
            this.notificationsListBox.Size = new System.Drawing.Size(382, 231);
            this.notificationsListBox.TabIndex = 36;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripSeparator1,
            this.toolStripMenuItem4,
            this.toolStripSeparator2,
            this.toolStripMenuItem3});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(112, 104);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(111, 22);
            this.toolStripMenuItem1.Text = "Ayarlar";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(111, 22);
            this.toolStripMenuItem2.Text = "Kayıt";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(108, 6);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(111, 22);
            this.toolStripMenuItem4.Text = "Bilgi";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(108, 6);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(111, 22);
            this.toolStripMenuItem3.Text = "Çıkış";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // lblversion2
            // 
            this.lblversion2.AutoSize = true;
            this.lblversion2.Location = new System.Drawing.Point(282, 21);
            this.lblversion2.Name = "lblversion2";
            this.lblversion2.Size = new System.Drawing.Size(38, 13);
            this.lblversion2.TabIndex = 38;
            this.lblversion2.Text = "label2";
            // 
            // alertControl1
            // 
            this.alertControl1.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 12F);
            this.alertControl1.AppearanceCaption.Options.UseFont = true;
            this.alertControl1.AppearanceText.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Italic);
            this.alertControl1.AppearanceText.Options.UseFont = true;
            this.alertControl1.AppearanceText.Options.UseTextOptions = true;
            this.alertControl1.AppearanceText.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.alertControl1.AppearanceText.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.alertControl1.ShowCloseButton = false;
            this.alertControl1.ShowPinButton = false;
            this.alertControl1.BeforeFormShow += new DevExpress.XtraBars.Alerter.AlertFormEventHandler(this.alertControl1_BeforeFormShow);
            this.alertControl1.FormLoad += new DevExpress.XtraBars.Alerter.AlertFormLoadEventHandler(this.alertControl1_FormLoad);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblversion2);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(382, 54);
            this.panel1.TabIndex = 41;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Left;
            //this.pictureBox2.Image = global::TicimaxWebServicesSample.Properties.Resources.Entegref__1_;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 54);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 41;
            this.pictureBox2.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PhoneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(382, 447);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.notificationsListBox);
            this.Controls.Add(this.CallPanel);
            this.Controls.Add(this.groupBoxLines);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("PhoneForm.IconOptions.Image")));
            this.MaximumSize = new System.Drawing.Size(838, 470);
            this.MinimizeBox = false;
            this.Name = "PhoneForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SIPPhoneForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PhoneForm_FormClosed);
            this.Load += new System.EventHandler(this.PhoneForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxLines.ResumeLayout(false);
            this.groupBoxLines.PerformLayout();
            this.CallPanel.ResumeLayout(false);
            this.CallPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.micVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spkVolume)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonSendText;
        private System.Windows.Forms.Button buttonStartHangupCall;
        private System.Windows.Forms.Panel PictureErr;
        private System.Windows.Forms.Button buttonHoldRetrieve;
        private System.Windows.Forms.Button buttonRecordStartStop;
        private System.Windows.Forms.Button buttonPlayStartStop;
        private System.Windows.Forms.Button buttonTransfer;
        private System.Windows.Forms.ComboBox AddressBox;
        private System.Windows.Forms.Button buttonJoin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBoxLines;
        private System.Windows.Forms.Button buttonLine5;
        private System.Windows.Forms.ListBox activeConnListbox;
        private System.Windows.Forms.Button buttonLine6;
        private System.Windows.Forms.Button buttonLine1;
        private System.Windows.Forms.Button buttonLine4;
        private System.Windows.Forms.Button buttonLine3;
        private System.Windows.Forms.Button buttonLine2;
        private System.Windows.Forms.Label callDurationLabel;
        private System.Windows.Forms.GroupBox CallPanel;
        private System.Windows.Forms.Button DTFM6;
        private System.Windows.Forms.Button DTFMStar;
        private System.Windows.Forms.Button DTFM2;
        private System.Windows.Forms.Button DTFM3;
        private System.Windows.Forms.Button DTFM9;
        private System.Windows.Forms.Button DTFM8;
        private System.Windows.Forms.Button DTFM1;
        private System.Windows.Forms.Button DTFM0;
        private System.Windows.Forms.Button DTFM5;
        private System.Windows.Forms.Button DTFMHash;
        private System.Windows.Forms.Button DTFM4;
        private System.Windows.Forms.Button DTFM7;
        private System.Windows.Forms.CheckBox muteMicrophoneFlag;
        private System.Windows.Forms.CheckBox muteSpeakerFlag;
        private System.Windows.Forms.ProgressBar spkVolumeBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label UInputLabel;
        private System.Windows.Forms.ProgressBar micVolumeBar;
        private System.Windows.Forms.TrackBar micVolume;
        private System.Windows.Forms.TrackBar spkVolume;
        private System.Windows.Forms.ListBox notificationsListBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.Label lblversion2;
        private DevExpress.XtraBars.Alerter.AlertControl alertControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Timer timer1;
    }
}

