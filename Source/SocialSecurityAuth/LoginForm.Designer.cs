namespace SocialSecurityAuth
{
    partial class LoginForm
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
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.account = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanel3
            //
            this.tableLayoutPanel3.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.account, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.password, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel6, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(20);
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(400, 240);
            this.tableLayoutPanel3.TabIndex = 0;
            //
            // label4
            //
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(23, 23);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 48);
            this.label4.TabIndex = 0;
            this.label4.Text = "账号：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // account
            //
            this.account.BackColor = System.Drawing.SystemColors.Window;
            this.account.Dock = System.Windows.Forms.DockStyle.Fill;
            this.account.ForeColor = System.Drawing.SystemColors.WindowText;
            this.account.Location = new System.Drawing.Point(123, 32);
            this.account.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.account.MaxLength = 30;
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(254, 31);
            this.account.TabIndex = 1;
            this.account.Text = "admin";
            //
            // label5
            //
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(23, 77);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 48);
            this.label5.TabIndex = 2;
            this.label5.Text = "密码：";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // password
            //
            this.password.BackColor = System.Drawing.SystemColors.Window;
            this.password.Dock = System.Windows.Forms.DockStyle.Fill;
            this.password.ForeColor = System.Drawing.SystemColors.WindowText;
            this.password.Location = new System.Drawing.Point(123, 86);
            this.password.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.password.MaxLength = 100;
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(254, 31);
            this.password.TabIndex = 3;
            this.password.UseSystemPasswordChar = true;
            //
            // flowLayoutPanel6
            //
            this.flowLayoutPanel6.AutoSize = true;
            this.flowLayoutPanel6.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel3.SetColumnSpan(this.flowLayoutPanel6, 2);
            this.flowLayoutPanel6.Controls.Add(this.button1);
            this.flowLayoutPanel6.Controls.Add(this.button2);
            this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(23, 131);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flowLayoutPanel6.Size = new System.Drawing.Size(354, 86);
            this.flowLayoutPanel6.TabIndex = 4;
            this.flowLayoutPanel6.WrapContents = false;
            //
            // button1
            //
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Location = new System.Drawing.Point(5, 13);
            this.button1.Margin = new System.Windows.Forms.Padding(5);
            this.button1.MinimumSize = new System.Drawing.Size(85, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "登录";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Login);
            //
            // button2
            //
            this.button2.AutoSize = true;
            this.button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button2.BackColor = System.Drawing.SystemColors.Control;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Location = new System.Drawing.Point(100, 13);
            this.button2.Margin = new System.Windows.Forms.Padding(5);
            this.button2.MinimumSize = new System.Drawing.Size(85, 34);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 34);
            this.button2.TabIndex = 1;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.CancelLogin);
            //
            // LoginForm
            //
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(400, 240);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Font = new System.Drawing.Font("宋体", 10.5F);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "管理员登录";
            this.Shown += new System.EventHandler(this.LoginShown);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox account;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
