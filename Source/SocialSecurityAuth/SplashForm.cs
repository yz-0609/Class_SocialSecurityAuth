using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
namespace SocialSecurityAuth
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
        }
        private void SplashShown(object sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            studentInfoLabel.Text = "班级：" + ConfigurationManager.AppSettings["StudentClass"] + "\r\n\r\n学号：" + ConfigurationManager.AppSettings["StudentNumber"] + "\r\n\r\n姓名：" + ConfigurationManager.AppSettings["StudentName"];
            timer.Start();
        }
        private void SplashTimerTick(object sender, EventArgs e) { timer.Stop(); Close(); }
    }
}
