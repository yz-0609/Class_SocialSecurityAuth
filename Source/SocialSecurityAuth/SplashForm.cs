using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
namespace SocialSecurityAuth
{
    public sealed class SplashForm : Form
    {
        private readonly Timer timer = new Timer { Interval = 2000 };
        public SplashForm()
        {
            Text = "课程设计"; ClientSize = new Size(640, 380); BackColor = Ui.Blue; Font = Ui.BodyFont;
            StartPosition = FormStartPosition.CenterScreen; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelColor, Padding = new Padding(30), Margin = new Padding(18) };
            Padding = new Padding(18); Controls.Add(panel);
            var title = new Label { Text = "基于人脸识别的\r\n社保身份认证系统", Font = new Font("宋体", 21F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 110 };
            var identity = new Label { Text = "班级：" + ConfigurationManager.AppSettings["StudentClass"] + "\r\n\r\n学号：" + ConfigurationManager.AppSettings["StudentNumber"] + "\r\n\r\n姓名：" + ConfigurationManager.AppSettings["StudentName"], Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            panel.Controls.Add(identity); panel.Controls.Add(title);
            Shown += (s, e) => timer.Start(); timer.Tick += (s, e) => { timer.Stop(); Close(); };
            FormClosed += (s, e) => timer.Dispose();
        }
    }
}
