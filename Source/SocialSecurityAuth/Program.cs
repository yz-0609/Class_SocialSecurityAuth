using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
namespace SocialSecurityAuth
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
            bool created;
            using (var instance = new Mutex(true, @"Local\SocialSecurityAuth_CourseProject", out created))
            {
                if (!created) { MessageBox.Show("程序已经运行，请切换到已打开的窗口。", "社保身份认证系统"); return; }
                Application.ThreadException += (s, e) => MessageBox.Show(Ui.ErrorText(e.Exception), "程序提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                using (var splash = new SplashForm()) splash.ShowDialog();
                Application.Run(new MainForm());
            }
        }
    }
}
