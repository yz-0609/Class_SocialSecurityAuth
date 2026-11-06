namespace SocialSecurityAuth
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cameraPanel = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.prompt = new System.Windows.Forms.Label();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.identifyButton = new System.Windows.Forms.Button();
            this.manageButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.socialNoValue = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.nameValue = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.unitValue = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.timeValue = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.resultValue = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.claimValue = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.recentGrid = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label20 = new System.Windows.Forms.Label();
            this.clockLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.deviceStatus = new System.Windows.Forms.Label();
            this.databaseStatus = new System.Windows.Forms.Label();
            this.identifyStatus = new System.Windows.Forms.Label();
            this.clockTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.cameraPanel.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentGrid)).BeginInit();
            this.recentGrid.SuspendLayout();
            this.panel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(181)))), ((int)(((byte)(255)))));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 65);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(18, 0, 18, 18);
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1048, 631);
            this.tableLayoutPanel1.TabIndex = 0;
            //
            // groupBox4
            //
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.groupBox4.Controls.Add(this.cameraPanel);
            this.groupBox4.Controls.Add(this.tableLayoutPanel6);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox4.Location = new System.Drawing.Point(21, 3);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(15);
            this.groupBox4.Size = new System.Drawing.Size(493, 607);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "人脸自动识别";
            //
            // cameraPanel
            //
            this.cameraPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cameraPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cameraPanel.Controls.Add(this.label7);
            this.cameraPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cameraPanel.Location = new System.Drawing.Point(15, 39);
            this.cameraPanel.Name = "cameraPanel";
            this.cameraPanel.Size = new System.Drawing.Size(463, 438);
            this.cameraPanel.TabIndex = 0;
            //
            // label7
            //
            this.label7.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(461, 436);
            this.label7.TabIndex = 0;
            this.label7.Text = "摄像头尚未连接";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // tableLayoutPanel6
            //
            this.tableLayoutPanel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.prompt, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.flowLayoutPanel8, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(15, 477);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize, 0F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(463, 115);
            this.tableLayoutPanel6.TabIndex = 1;
            //
            // prompt
            //
            this.prompt.AutoEllipsis = true;
            this.prompt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.prompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prompt.ForeColor = System.Drawing.SystemColors.ControlText;
            this.prompt.Location = new System.Drawing.Point(3, 0);
            this.prompt.Name = "prompt";
            this.prompt.Size = new System.Drawing.Size(457, 20);
            this.prompt.TabIndex = 0;
            this.prompt.Text = "正在准备系统，请稍候";
            this.prompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // flowLayoutPanel8
            //
            this.flowLayoutPanel8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanel8.AutoSize = true;
            this.flowLayoutPanel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.flowLayoutPanel8.Controls.Add(this.identifyButton);
            this.flowLayoutPanel8.Controls.Add(this.manageButton);
            this.flowLayoutPanel8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.flowLayoutPanel8.Location = new System.Drawing.Point(127, 42);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Padding = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.flowLayoutPanel8.Size = new System.Drawing.Size(209, 50);
            this.flowLayoutPanel8.TabIndex = 1;
            //
            // identifyButton
            //
            this.identifyButton.AutoSize = true;
            this.identifyButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.identifyButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.identifyButton.Location = new System.Drawing.Point(5, 9);
            this.identifyButton.Margin = new System.Windows.Forms.Padding(5);
            this.identifyButton.MinimumSize = new System.Drawing.Size(85, 34);
            this.identifyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.identifyButton.Name = "identifyButton";
            this.identifyButton.Size = new System.Drawing.Size(104, 34);
            this.identifyButton.TabIndex = 0;
            this.identifyButton.Text = "暂停识别";
            this.identifyButton.UseVisualStyleBackColor = true;
            this.identifyButton.Click += new System.EventHandler(this.ToggleRecognition);
            //
            // manageButton
            //
            this.manageButton.AutoSize = true;
            this.manageButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.manageButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.manageButton.Location = new System.Drawing.Point(119, 9);
            this.manageButton.Margin = new System.Windows.Forms.Padding(5);
            this.manageButton.MinimumSize = new System.Drawing.Size(85, 34);
            this.manageButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.manageButton.Name = "manageButton";
            this.manageButton.Size = new System.Drawing.Size(85, 34);
            this.manageButton.TabIndex = 1;
            this.manageButton.Text = "管理";
            this.manageButton.UseVisualStyleBackColor = true;
            this.manageButton.Click += new System.EventHandler(this.OpenManagement);
            //
            // tableLayoutPanel5
            //
            this.tableLayoutPanel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(181)))), ((int)(((byte)(255)))));
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.groupBox9, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.groupBox10, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(531, 3);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 310F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(496, 607);
            this.tableLayoutPanel5.TabIndex = 1;
            //
            // groupBox9
            //
            this.groupBox9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.groupBox9.Controls.Add(this.tableLayoutPanel11);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox9.Location = new System.Drawing.Point(3, 3);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(14);
            this.groupBox9.Size = new System.Drawing.Size(490, 254);
            this.groupBox9.TabIndex = 0;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "本次认证信息";
            //
            // tableLayoutPanel11
            //
            this.tableLayoutPanel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Controls.Add(this.label12, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.socialNoValue, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.label13, 0, 1);
            this.tableLayoutPanel11.Controls.Add(this.nameValue, 1, 1);
            this.tableLayoutPanel11.Controls.Add(this.label14, 0, 2);
            this.tableLayoutPanel11.Controls.Add(this.unitValue, 1, 2);
            this.tableLayoutPanel11.Controls.Add(this.label15, 0, 3);
            this.tableLayoutPanel11.Controls.Add(this.timeValue, 1, 3);
            this.tableLayoutPanel11.Controls.Add(this.label16, 0, 4);
            this.tableLayoutPanel11.Controls.Add(this.resultValue, 1, 4);
            this.tableLayoutPanel11.Controls.Add(this.label17, 0, 5);
            this.tableLayoutPanel11.Controls.Add(this.claimValue, 1, 5);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(14, 38);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 6;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(462, 202);
            this.tableLayoutPanel11.TabIndex = 0;
            //
            // label12
            //
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = false;
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.Location = new System.Drawing.Point(5, 8);
            this.label12.Margin = new System.Windows.Forms.Padding(4);
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label12.AutoEllipsis = true;
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 4);
            this.label12.TabIndex = 0;
            this.label12.Text = "社保编号：";
            //
            // socialNoValue
            //
            this.socialNoValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.socialNoValue.AutoSize = false;
            this.socialNoValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.socialNoValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.socialNoValue.Location = new System.Drawing.Point(115, 8);
            this.socialNoValue.Margin = new System.Windows.Forms.Padding(4);
            this.socialNoValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.socialNoValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.socialNoValue.AutoEllipsis = true;
            this.socialNoValue.Name = "socialNoValue";
            this.socialNoValue.Size = new System.Drawing.Size(31, 4);
            this.socialNoValue.TabIndex = 1;
            this.socialNoValue.Text = "—";
            //
            // label13
            //
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = false;
            this.label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.Location = new System.Drawing.Point(5, 28);
            this.label13.Margin = new System.Windows.Forms.Padding(4);
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label13.AutoEllipsis = true;
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 4);
            this.label13.TabIndex = 2;
            this.label13.Text = "姓名：";
            //
            // nameValue
            //
            this.nameValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nameValue.AutoSize = false;
            this.nameValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.nameValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.nameValue.Location = new System.Drawing.Point(115, 28);
            this.nameValue.Margin = new System.Windows.Forms.Padding(4);
            this.nameValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nameValue.AutoEllipsis = true;
            this.nameValue.Name = "nameValue";
            this.nameValue.Size = new System.Drawing.Size(31, 4);
            this.nameValue.TabIndex = 3;
            this.nameValue.Text = "—";
            //
            // label14
            //
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = false;
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.Location = new System.Drawing.Point(5, 48);
            this.label14.Margin = new System.Windows.Forms.Padding(4);
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label14.AutoEllipsis = true;
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 4);
            this.label14.TabIndex = 4;
            this.label14.Text = "所属单位：";
            //
            // unitValue
            //
            this.unitValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.unitValue.AutoSize = false;
            this.unitValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.unitValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.unitValue.Location = new System.Drawing.Point(115, 48);
            this.unitValue.Margin = new System.Windows.Forms.Padding(4);
            this.unitValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unitValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.unitValue.AutoEllipsis = true;
            this.unitValue.Name = "unitValue";
            this.unitValue.Size = new System.Drawing.Size(31, 4);
            this.unitValue.TabIndex = 5;
            this.unitValue.Text = "—";
            //
            // label15
            //
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label15.AutoSize = false;
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.label15.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label15.Location = new System.Drawing.Point(5, 68);
            this.label15.Margin = new System.Windows.Forms.Padding(4);
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label15.AutoEllipsis = true;
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(73, 4);
            this.label15.TabIndex = 6;
            this.label15.Text = "认证时间：";
            //
            // timeValue
            //
            this.timeValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.timeValue.AutoSize = false;
            this.timeValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.timeValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.timeValue.Location = new System.Drawing.Point(115, 68);
            this.timeValue.Margin = new System.Windows.Forms.Padding(4);
            this.timeValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.timeValue.AutoEllipsis = true;
            this.timeValue.Name = "timeValue";
            this.timeValue.Size = new System.Drawing.Size(31, 4);
            this.timeValue.TabIndex = 7;
            this.timeValue.Text = "—";
            //
            // label16
            //
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = false;
            this.label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.label16.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label16.Location = new System.Drawing.Point(5, 88);
            this.label16.Margin = new System.Windows.Forms.Padding(4);
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label16.AutoEllipsis = true;
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(73, 4);
            this.label16.TabIndex = 8;
            this.label16.Text = "认证结果：";
            //
            // resultValue
            //
            this.resultValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.resultValue.AutoSize = false;
            this.resultValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.resultValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.resultValue.Location = new System.Drawing.Point(115, 88);
            this.resultValue.Margin = new System.Windows.Forms.Padding(4);
            this.resultValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.resultValue.AutoEllipsis = true;
            this.resultValue.Name = "resultValue";
            this.resultValue.Size = new System.Drawing.Size(94, 4);
            this.resultValue.TabIndex = 9;
            this.resultValue.Text = "等待认证";
            //
            // label17
            //
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = false;
            this.label17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.label17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label17.Location = new System.Drawing.Point(5, 130);
            this.label17.Margin = new System.Windows.Forms.Padding(4);
            this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label17.AutoEllipsis = true;
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(73, 42);
            this.label17.TabIndex = 10;
            this.label17.Text = "领取记录：";
            //
            // claimValue
            //
            this.claimValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.claimValue.AutoSize = false;
            this.claimValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.claimValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.claimValue.Location = new System.Drawing.Point(115, 140);
            this.claimValue.Margin = new System.Windows.Forms.Padding(4);
            this.claimValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.claimValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.claimValue.AutoEllipsis = true;
            this.claimValue.Name = "claimValue";
            this.claimValue.Size = new System.Drawing.Size(31, 21);
            this.claimValue.TabIndex = 11;
            this.claimValue.Text = "—";
            //
            // groupBox10
            //
            this.groupBox10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.groupBox10.Controls.Add(this.recentGrid);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox10.Location = new System.Drawing.Point(3, 272);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(12);
            this.groupBox10.Size = new System.Drawing.Size(490, 332);
            this.groupBox10.TabIndex = 1;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "最近领取记录";
            //
            // recentGrid
            //
            this.recentGrid.AllowUserToAddRows = false;
            this.recentGrid.AllowUserToDeleteRows = false;
            this.recentGrid.AllowUserToResizeRows = false;
            this.recentGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.recentGrid.BackgroundColor = System.Drawing.Color.White;
            this.recentGrid.ColumnHeadersHeight = 32;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(228)))), ((int)(((byte)(251)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.recentGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.recentGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentGrid.Location = new System.Drawing.Point(12, 36);
            this.recentGrid.MultiSelect = false;
            this.recentGrid.Name = "recentGrid";
            this.recentGrid.ReadOnly = true;
            this.recentGrid.RowHeadersVisible = false;
            this.recentGrid.RowTemplate.Height = 28;
            this.recentGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.recentGrid.Size = new System.Drawing.Size(466, 284);
            this.recentGrid.TabIndex = 0;
            this.recentGrid.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.GridDataBindingComplete);
            //
            // hScrollBar18
            //
            //
            // vScrollBar19
            //
            //
            // panel2
            //
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(181)))), ((int)(((byte)(255)))));
            this.panel2.Controls.Add(this.label20);
            this.panel2.Controls.Add(this.clockLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(22, 15, 22, 10);
            this.panel2.Size = new System.Drawing.Size(1048, 65);
            this.panel2.TabIndex = 1;
            //
            // label20
            //
            this.label20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(181)))), ((int)(((byte)(255)))));
            this.label20.Dock = System.Windows.Forms.DockStyle.Left;
            this.label20.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold);
            this.label20.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label20.Location = new System.Drawing.Point(22, 15);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(420, 40);
            this.label20.TabIndex = 0;
            this.label20.Text = "社保身份认证系统";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // clockLabel
            //
            this.clockLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(181)))), ((int)(((byte)(255)))));
            this.clockLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.clockLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clockLabel.Location = new System.Drawing.Point(726, 15);
            this.clockLabel.Name = "clockLabel";
            this.clockLabel.Size = new System.Drawing.Size(300, 40);
            this.clockLabel.TabIndex = 1;
            this.clockLabel.Text = "2026年10月28日 11:35:43";
            this.clockLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // flowLayoutPanel3
            //
            this.flowLayoutPanel3.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel3.Controls.Add(this.deviceStatus);
            this.flowLayoutPanel3.Controls.Add(this.databaseStatus);
            this.flowLayoutPanel3.Controls.Add(this.identifyStatus);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 696);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.flowLayoutPanel3.Size = new System.Drawing.Size(1048, 30);
            this.flowLayoutPanel3.TabIndex = 2;
            //
            // deviceStatus
            //
            this.deviceStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.deviceStatus.AutoSize = true;
            this.deviceStatus.BackColor = System.Drawing.SystemColors.Control;
            this.deviceStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.deviceStatus.Location = new System.Drawing.Point(15, 8);
            this.deviceStatus.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.deviceStatus.Name = "deviceStatus";
            this.deviceStatus.Size = new System.Drawing.Size(136, 21);
            this.deviceStatus.TabIndex = 0;
            this.deviceStatus.Text = "设备：未连接";
            //
            // databaseStatus
            //
            this.databaseStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.databaseStatus.AutoSize = true;
            this.databaseStatus.BackColor = System.Drawing.SystemColors.Control;
            this.databaseStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.databaseStatus.Location = new System.Drawing.Point(161, 8);
            this.databaseStatus.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.databaseStatus.Name = "databaseStatus";
            this.databaseStatus.Size = new System.Drawing.Size(157, 21);
            this.databaseStatus.TabIndex = 1;
            this.databaseStatus.Text = "数据库：待连接";
            //
            // identifyStatus
            //
            this.identifyStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.identifyStatus.AutoSize = true;
            this.identifyStatus.BackColor = System.Drawing.SystemColors.Control;
            this.identifyStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.identifyStatus.Location = new System.Drawing.Point(328, 8);
            this.identifyStatus.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.identifyStatus.Name = "identifyStatus";
            this.identifyStatus.Size = new System.Drawing.Size(136, 21);
            this.identifyStatus.TabIndex = 2;
            this.identifyStatus.Text = "识别：未进行";
            //
            // clockTimer
            //
            this.clockTimer.Interval = 1000;
            this.clockTimer.Tick += new System.EventHandler(this.ClockTick);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(181)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1100, 760);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Font = new System.Drawing.Font("宋体", 10.5F);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MinimumSize = new System.Drawing.Size(1020, 720);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "基于人脸识别的社保身份认证系统";
            this.Shown += new System.EventHandler(this.OnShown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.cameraPanel.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.recentGrid)).EndInit();
            this.recentGrid.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel cameraPanel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label prompt;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel8;
        private System.Windows.Forms.Button identifyButton;
        private System.Windows.Forms.Button manageButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label socialNoValue;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label nameValue;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label unitValue;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label timeValue;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label resultValue;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label claimValue;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.DataGridView recentGrid;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label clockLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label deviceStatus;
        private System.Windows.Forms.Label databaseStatus;
        private System.Windows.Forms.Label identifyStatus;
        private System.Windows.Forms.Timer clockTimer;
    }
}
