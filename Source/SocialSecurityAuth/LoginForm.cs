using System;
using System.Drawing;
using System.Windows.Forms;
namespace SocialSecurityAuth
{
    public partial class LoginForm : Form
    {
        private readonly AppRepository repository;
        public AdminSession Session { get; private set; }
        public LoginForm()
        {
            InitializeComponent();
        }
        public LoginForm(AppRepository repository) : this() { this.repository = repository; }
        private void LoginShown(object sender, EventArgs e) { password.Focus(); }
        private void CancelLogin(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
        private void Login(object sender, EventArgs e)
        {
            try
            {
                var source = repository ?? new AppRepository(Database.ConnectionString);
                Session = source.Login(account.Text, password.Text);
                if (Session == null) { MessageBox.Show(this, "账号或密码不正确。", "登录提示", MessageBoxButtons.OK, MessageBoxIcon.Information); password.Clear(); password.Focus(); return; }
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex) { Ui.Error(this, ex); }
        }
    }
}
