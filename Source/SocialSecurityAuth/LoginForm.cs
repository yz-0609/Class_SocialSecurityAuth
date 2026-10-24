using System;
using System.Drawing;
using System.Windows.Forms;
namespace SocialSecurityAuth
{
    public sealed class LoginForm : Form
    {
        private readonly AppRepository repository;
        private readonly TextBox account = new TextBox { Width = 210, MaxLength = 30, Text = "admin" };
        private readonly TextBox password = new TextBox { Width = 210, MaxLength = 100, UseSystemPasswordChar = true };
        public AdminSession Session { get; private set; }
        public LoginForm(AppRepository repository)
        {
            this.repository = repository; Text = "管理员登录"; ClientSize = new Size(350, 205); Font = Ui.BodyFont;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            var fields = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2, RowCount = 3 };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            fields.Controls.Add(Ui.Label("账号："), 0, 0); fields.Controls.Add(account, 1, 0); fields.Controls.Add(Ui.Label("密码："), 0, 1); fields.Controls.Add(password, 1, 1);
            var login = Ui.Button("登录", Login); var cancel = Ui.Button("取消", (s, e) => { DialogResult = DialogResult.Cancel; Close(); });
            var buttons = Ui.Buttons(login, cancel); fields.Controls.Add(buttons, 0, 2); fields.SetColumnSpan(buttons, 2); Controls.Add(fields);
            AcceptButton = login; CancelButton = cancel; Shown += (s, e) => password.Focus();
        }
        private void Login(object sender, EventArgs e)
        {
            try
            {
                Session = repository.Login(account.Text, password.Text);
                if (Session == null) { MessageBox.Show(this, "账号或密码不正确。", "登录提示", MessageBoxButtons.OK, MessageBoxIcon.Information); password.Clear(); password.Focus(); return; }
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex) { Ui.Error(this, ex); }
        }
    }
}
