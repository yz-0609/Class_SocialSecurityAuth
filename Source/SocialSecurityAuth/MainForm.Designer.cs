using System.Drawing;
using System.Windows.Forms;
namespace SocialSecurityAuth
{
    public partial class MainForm
    {
        private Panel cameraPanel;
        private Label prompt, clockLabel, deviceStatus, databaseStatus, identifyStatus;
        private Label socialNoValue, nameValue, unitValue, timeValue, resultValue, claimValue;
        private DataGridView recentGrid;
        private Button identifyButton, manageButton;
        private Timer clockTimer;
        private void InitializeComponent()
        {
            Text = "基于人脸识别的社保身份认证系统"; Name = "MainForm";
            ClientSize = new Size(1048, 726); MinimumSize = new Size(960, 680); StartPosition = FormStartPosition.CenterScreen;
            Font = Ui.BodyFont; BackColor = Ui.Blue; AutoScaleMode = AutoScaleMode.Font;
            var header = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(22, 15, 22, 10) };
            var title = new Label { Dock = DockStyle.Left, Width = 420, Text = "社保身份认证系统", Font = new Font("宋体", 20F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            clockLabel = new Label { Dock = DockStyle.Right, Width = 300, TextAlign = ContentAlignment.MiddleRight }; header.Controls.Add(title); header.Controls.Add(clockLabel);
            var status = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 30, BackColor = SystemColors.Control, Padding = new Padding(10, 0, 0, 0) };
            deviceStatus = Ui.Label("设备：未连接"); databaseStatus = Ui.Label("数据库：待连接"); identifyStatus = Ui.Label("识别：未进行"); status.Controls.AddRange(new Control[] { deviceStatus, databaseStatus, identifyStatus });
            var columns = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(18, 0, 18, 18) };
            columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            var recognition = new GroupBox { Dock = DockStyle.Fill, Text = "人脸自动识别", BackColor = Ui.PanelColor, Padding = new Padding(15), Margin = new Padding(3, 3, 10, 3) };
            cameraPanel = new Panel { Dock = DockStyle.Fill, Name = "cameraPanel", BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle }; cameraPanel.Controls.Add(Ui.CameraPlaceholder());
            var actionArea = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 115, RowCount = 2, ColumnCount = 1 };
            prompt = new Label { Dock = DockStyle.Fill, Text = "正在准备系统，请稍候", TextAlign = ContentAlignment.MiddleCenter, AutoEllipsis = true };
            identifyButton = Ui.Button("暂停识别", ToggleRecognition); identifyButton.Name = "identifyButton";
            manageButton = Ui.Button("管理", OpenManagement); manageButton.Name = "manageButton";
            var buttons = Ui.Buttons(identifyButton, manageButton); buttons.Anchor = AnchorStyles.None; buttons.Dock = DockStyle.None;
            actionArea.Controls.Add(prompt, 0, 0); actionArea.Controls.Add(buttons, 0, 1); recognition.Controls.Add(cameraPanel); recognition.Controls.Add(actionArea);
            var right = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Margin = new Padding(7, 3, 3, 3) };
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 260)); right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var information = new GroupBox { Dock = DockStyle.Fill, Text = "本次认证信息", Padding = new Padding(14), BackColor = Ui.PanelColor };
            var info = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6 }; info.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            socialNoValue = Ui.Label("—"); nameValue = Ui.Label("—"); unitValue = Ui.Label("—"); timeValue = Ui.Label("—"); resultValue = Ui.Label("等待认证"); claimValue = Ui.Label("—");
            info.Controls.Add(Ui.Label("社保编号："), 0, 0); info.Controls.Add(socialNoValue, 1, 0);
            info.Controls.Add(Ui.Label("姓名："), 0, 1); info.Controls.Add(nameValue, 1, 1);
            info.Controls.Add(Ui.Label("所属单位："), 0, 2); info.Controls.Add(unitValue, 1, 2);
            info.Controls.Add(Ui.Label("认证时间："), 0, 3); info.Controls.Add(timeValue, 1, 3);
            info.Controls.Add(Ui.Label("认证结果："), 0, 4); info.Controls.Add(resultValue, 1, 4);
            info.Controls.Add(Ui.Label("领取记录："), 0, 5); info.Controls.Add(claimValue, 1, 5); information.Controls.Add(info);
            var recent = new GroupBox { Dock = DockStyle.Fill, Text = "最近领取记录", Padding = new Padding(12), BackColor = Ui.PanelColor, Margin = new Padding(3, 12, 3, 3) };
            recentGrid = Ui.Grid(); recentGrid.Name = "recentGrid"; recent.Controls.Add(recentGrid); right.Controls.Add(information, 0, 0); right.Controls.Add(recent, 0, 1);
            columns.Controls.Add(recognition, 0, 0); columns.Controls.Add(right, 1, 0);
            Controls.Add(columns); Controls.Add(header); Controls.Add(status);
            clockTimer = new Timer { Interval = 1000 }; clockTimer.Tick += (s, e) => clockLabel.Text = System.DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"); clockTimer.Start();
            clockLabel.Text = System.DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
        }
    }
}
