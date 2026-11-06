namespace SocialSecurityAuth
{
    partial class SplashForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.studentInfoLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            //
            // panel1
            //
            this.panel1.Text = "";
            this.panel1.Location = new System.Drawing.Point(18, 18);
            this.panel1.Size = new System.Drawing.Size(604, 344);
            this.panel1.MinimumSize = new System.Drawing.Size(0, 0);
            this.panel1.MaximumSize = new System.Drawing.Size(0, 0);
            this.panel1.AutoSize = false;
            this.panel1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left);
            this.panel1.Dock = (System.Windows.Forms.DockStyle.Fill);
            this.panel1.Margin = new System.Windows.Forms.Padding(18, 18, 18, 18);
            this.panel1.Padding = new System.Windows.Forms.Padding(30, 30, 30, 30);
            this.panel1.BackColor = System.Drawing.Color.FromArgb(255, 226, 254, 255);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Enabled = true;
            this.panel1.TabIndex = 0;
            this.panel1.TabStop = false;
            this.panel1.Name = "panel1";
            this.panel1.BorderStyle = (System.Windows.Forms.BorderStyle.None);
            this.panel1.Controls.Add(this.studentInfoLabel);
            this.panel1.Controls.Add(this.label3);
            //
            // studentInfoLabel
            //
            this.studentInfoLabel.Text = "班级：202407Z班\r\n\r\n学号：20242005\r\n\r\n姓名：杨泽";
            this.studentInfoLabel.Location = new System.Drawing.Point(30, 140);
            this.studentInfoLabel.Size = new System.Drawing.Size(544, 174);
            this.studentInfoLabel.MinimumSize = new System.Drawing.Size(0, 0);
            this.studentInfoLabel.MaximumSize = new System.Drawing.Size(0, 0);
            this.studentInfoLabel.AutoSize = false;
            this.studentInfoLabel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left);
            this.studentInfoLabel.Dock = (System.Windows.Forms.DockStyle.Fill);
            this.studentInfoLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.studentInfoLabel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.studentInfoLabel.BackColor = System.Drawing.Color.FromArgb(255, 226, 254, 255);
            this.studentInfoLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.studentInfoLabel.Enabled = true;
            this.studentInfoLabel.TabIndex = 0;
            this.studentInfoLabel.TabStop = false;
            this.studentInfoLabel.Name = "studentInfoLabel";
            this.studentInfoLabel.TextAlign = (System.Drawing.ContentAlignment.MiddleCenter);
            this.studentInfoLabel.AutoEllipsis = false;
            //
            // label3
            //
            this.label3.Text = "基于人脸识别的\r\n社保身份认证系统";
            this.label3.Location = new System.Drawing.Point(30, 30);
            this.label3.Size = new System.Drawing.Size(544, 110);
            this.label3.MinimumSize = new System.Drawing.Size(0, 0);
            this.label3.MaximumSize = new System.Drawing.Size(0, 0);
            this.label3.AutoSize = false;
            this.label3.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left);
            this.label3.Dock = (System.Windows.Forms.DockStyle.Top);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.label3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.label3.BackColor = System.Drawing.Color.FromArgb(255, 226, 254, 255);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Enabled = true;
            this.label3.TabIndex = 1;
            this.label3.TabStop = false;
            this.label3.Font = new System.Drawing.Font("宋体", 21.0F, (System.Drawing.FontStyle.Bold), System.Drawing.GraphicsUnit.Point);
            this.label3.Name = "label3";
            this.label3.TextAlign = (System.Drawing.ContentAlignment.MiddleCenter);
            this.label3.AutoEllipsis = false;
            //
            // SplashForm
            //
            this.Text = "课程设计";
            this.StartPosition = (System.Windows.Forms.FormStartPosition.CenterScreen);
            this.ClientSize = new System.Drawing.Size(640, 380);
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.MaximumSize = new System.Drawing.Size(0, 0);
            this.AutoSize = false;
            this.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left);
            this.Dock = (System.Windows.Forms.DockStyle.None);
            this.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.Padding = new System.Windows.Forms.Padding(18, 18, 18, 18);
            this.BackColor = System.Drawing.Color.FromArgb(255, 50, 181, 255);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Enabled = true;
            this.TabIndex = 0;
            this.TabStop = true;
            this.Font = new System.Drawing.Font("宋体", 10.5F, (System.Drawing.FontStyle.Regular), System.Drawing.GraphicsUnit.Point);
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.Name = "SplashForm";
            this.FormBorderStyle = (System.Windows.Forms.FormBorderStyle.FixedDialog);
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.timer.Interval = 2000;
            this.timer.Tick += new System.EventHandler(this.SplashTimerTick);
            this.Shown += new System.EventHandler(this.SplashShown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label studentInfoLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timer;
    }
}
