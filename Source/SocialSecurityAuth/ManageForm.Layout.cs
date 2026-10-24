using System;
using System.Drawing;
using System.Windows.Forms;
namespace SocialSecurityAuth
{
    public partial class ManageForm
    {
        private TabControl tabs;
        private TabPage peopleTab, recordsTab, statisticsTab, adminsTab, logsTab;
        private DataGridView peopleGrid, recordsGrid, statisticsGrid, adminsGrid, logsGrid;
        private ComboBox peopleUnit, recordUnit, statisticsUnit, statisticsType, personUnit;
        private TextBox personNo, personName, adminAccount, adminName, adminPassword;
        private RadioButton male, female;
        private CheckBox[] permissionChecks;
        private Panel cameraPanel;
        private Label enrollmentStatus, summaryLabel, statisticsNote;
        private ToolStripStatusLabel status;
        private Button enrollButton, cancelEnrollButton, connectButton, newPersonButton, savePersonButton, deletePersonButton, clearRecordsButton;
        private void InitializeComponent()
        {
            Text = "社保身份认证系统 管理员界面"; Name = "ManageForm"; Font = Ui.BodyFont;
            BackColor = Ui.Blue; ClientSize = new Size(1110, 760); MinimumSize = new Size(1020, 720); StartPosition = FormStartPosition.CenterParent;
            var header = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(20, 14, 20, 12) };
            var title = new Label { Text = "管理员界面", Dock = DockStyle.Left, Width = 280, Font = new Font("宋体", 18F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            var back = Ui.Button("返回识别界面", (s, e) => Close()); back.Dock = DockStyle.Right;
            var account = new Label { Text = "当前管理员：" + (session == null ? "界面设计" : session.Account), Dock = DockStyle.Right, Width = 260, TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(0, 0, 18, 0) };
            header.Controls.Add(title); header.Controls.Add(account); header.Controls.Add(back);
            var strip = new StatusStrip(); status = new ToolStripStatusLabel("就绪"); strip.Items.Add(status);
            tabs = new TabControl { Dock = DockStyle.Fill, Name = "manageTabs", Padding = new Point(14, 8) };
            peopleTab = Page("人员登记与管理", "peopleTab"); recordsTab = Page("认证领取记录", "recordsTab"); statisticsTab = Page("数据统计", "statisticsTab"); adminsTab = Page("管理人员", "adminsTab"); logsTab = Page("操作记录", "logsTab");
            tabs.TabPages.AddRange(new[] { peopleTab, recordsTab, statisticsTab, adminsTab, logsTab }); tabs.Selecting += OnSelectingTab; tabs.SelectedIndexChanged += (s, e) => RefreshSelectedTab();
            BuildPeopleTab(); BuildRecordsTab(); BuildStatisticsTab(); BuildAdminsTab(); BuildLogsTab();
            var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(18, 0, 18, 18) }; content.Controls.Add(tabs);
            Controls.Add(content); Controls.Add(header); Controls.Add(strip);
        }
        private static TabPage Page(string text, string name) { return new TabPage { Text = text, Name = name, BackColor = Color.White, Padding = new Padding(14) }; }
        private static TableLayoutPanel Rows()
        {
            var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 49)); t.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); t.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); return t;
        }
        private void BuildPeopleTab()
        {
            var split = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); split.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330));
            var list = Rows(); peopleUnit = Ui.FilterCombo(); peopleUnit.Name = "peopleUnit";
            var filters = Ui.Buttons(Ui.Label("所属单位："), peopleUnit, Ui.Button("查询", (s, e) => Safe(RefreshPeople))); list.Controls.Add(filters, 0, 0);
            peopleGrid = Ui.Grid(); peopleGrid.Name = "peopleGrid"; peopleGrid.CellClick += SelectPerson; list.Controls.Add(peopleGrid, 0, 1);
            newPersonButton = Ui.Button("新增", (s, e) => ClearPerson()); newPersonButton.Name = "newPersonButton";
            savePersonButton = Ui.Button("保存修改", SavePerson); savePersonButton.Name = "savePersonButton";
            deletePersonButton = Ui.Button("删除", DeletePerson); deletePersonButton.Name = "deletePersonButton";
            list.Controls.Add(Ui.Buttons(newPersonButton, savePersonButton, deletePersonButton), 0, 2);
            var editor = new GroupBox { Text = "人员信息与人脸注册", Dock = DockStyle.Fill, BackColor = Ui.PanelColor, Padding = new Padding(12), Margin = new Padding(12, 3, 3, 3) };
            var fields = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 8 };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
            for (int i = 1; i <= 4; i++) fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 49)); fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            cameraPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle, Name = "enrollCamera" }; cameraPanel.Controls.Add(Ui.CameraPlaceholder()); fields.Controls.Add(cameraPanel, 0, 0); fields.SetColumnSpan(cameraPanel, 2);
            personNo = new TextBox { Dock = DockStyle.Fill, Name = "personNo", MaxLength = 30, Margin = new Padding(3, 8, 3, 3) };
            personName = new TextBox { Dock = DockStyle.Fill, Name = "personName", MaxLength = 50, Margin = new Padding(3, 8, 3, 3) };
            male = new RadioButton { Text = "男", AutoSize = true, Checked = true }; female = new RadioButton { Text = "女", AutoSize = true };
            personUnit = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDown, MaxLength = 100, Name = "personUnit", Margin = new Padding(3, 8, 3, 3) };
            fields.Controls.Add(Ui.Label("社保编号："), 0, 1); fields.Controls.Add(personNo, 1, 1); fields.Controls.Add(Ui.Label("姓名："), 0, 2); fields.Controls.Add(personName, 1, 2);
            fields.Controls.Add(Ui.Label("性别："), 0, 3); fields.Controls.Add(Ui.Buttons(male, female), 1, 3); fields.Controls.Add(Ui.Label("所属单位："), 0, 4); fields.Controls.Add(personUnit, 1, 4);
            enrollButton = Ui.Button("采集并注册", Enroll); enrollButton.Name = "enrollButton";
            cancelEnrollButton = Ui.Button("取消采集", CancelEnrollment); cancelEnrollButton.Enabled = false;
            var actions = Ui.Buttons(enrollButton, cancelEnrollButton); fields.Controls.Add(actions, 0, 5); fields.SetColumnSpan(actions, 2);
            connectButton = Ui.Button("连接设备", (s, e) => Safe(ConnectCamera)); var connection = Ui.Buttons(connectButton); fields.Controls.Add(connection, 0, 6); fields.SetColumnSpan(connection, 2);
            enrollmentStatus = new Label { Dock = DockStyle.Fill, Text = "人脸状态：未采集\r\n所属单位可选择，也可输入新单位。", AutoSize = false, Padding = new Padding(2, 6, 0, 0) }; fields.Controls.Add(enrollmentStatus, 0, 7); fields.SetColumnSpan(enrollmentStatus, 2);
            editor.Controls.Add(fields); split.Controls.Add(list, 0, 0); split.Controls.Add(editor, 1, 0); peopleTab.Controls.Add(split);
        }
        private void BuildRecordsTab()
        {
            var layout = Rows(); recordUnit = Ui.FilterCombo(); recordUnit.Name = "recordUnit";
            layout.Controls.Add(Ui.Buttons(Ui.Label("所属单位："), recordUnit, Ui.Button("查询", (s, e) => Safe(RefreshRecords))), 0, 0);
            recordsGrid = Ui.Grid(); recordsGrid.Name = "recordsGrid"; layout.Controls.Add(recordsGrid, 0, 1);
            clearRecordsButton = Ui.Button("清空认证领取记录", ClearRecords); clearRecordsButton.Name = "clearRecordsButton";
            layout.Controls.Add(Ui.Buttons(clearRecordsButton, Ui.Label("仅保存每人首次成功领取；清空作用于所有单位。")), 0, 2); recordsTab.Controls.Add(layout);
        }
        private void BuildStatisticsTab()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            statisticsUnit = Ui.FilterCombo(); statisticsUnit.Name = "statisticsUnit"; statisticsType = Ui.FilterCombo(); statisticsType.Name = "statisticsType"; statisticsType.Width = 160;
            statisticsType.Items.AddRange(new object[] { "已采集信息", "已通过认证", "认证未通过", "未认证" }); statisticsType.SelectedIndex = 0;
            layout.Controls.Add(Ui.Buttons(Ui.Label("所属单位："), statisticsUnit, Ui.Label("人员名单："), statisticsType, Ui.Button("查询", (s, e) => Safe(RefreshStatistics))), 0, 0);
            var totals = new GroupBox { Text = "数据汇总", Dock = DockStyle.Fill, BackColor = SystemColors.Control, Padding = new Padding(12, 8, 12, 8) };
            summaryLabel = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }; totals.Controls.Add(summaryLabel); layout.Controls.Add(totals, 0, 1);
            statisticsGrid = Ui.Grid(); statisticsGrid.Name = "statisticsGrid"; layout.Controls.Add(statisticsGrid, 0, 2);
            statisticsNote = new Label { Dock = DockStyle.Fill, Text = "已采集状态与认证结果可以交叉；认证结果按人员最近一次识别状态统计。", Padding = new Padding(4, 12, 0, 0) }; layout.Controls.Add(statisticsNote, 0, 3);
            statisticsTab.Controls.Add(layout);
        }
        private void BuildAdminsTab()
        {
            var split = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 }; split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48)); split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
            var list = Rows(); list.RowStyles[0].Height = 5;
            adminsGrid = Ui.Grid(); adminsGrid.Name = "adminsGrid"; adminsGrid.CellClick += SelectAdmin; list.Controls.Add(adminsGrid, 0, 1);
            list.Controls.Add(Ui.Buttons(Ui.Button("新增", (s, e) => ClearAdmin()), Ui.Button("删除", DeleteAdmin)), 0, 2);
            var editor = new GroupBox { Text = "管理人员信息与权限", Dock = DockStyle.Fill, BackColor = Ui.PanelColor, Padding = new Padding(18), Margin = new Padding(15, 3, 3, 3) };
            var rows = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 }; rows.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70)); rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            adminAccount = new TextBox { Dock = DockStyle.Fill, MaxLength = 30, Name = "adminAccount" }; adminName = new TextBox { Dock = DockStyle.Fill, MaxLength = 50, Name = "adminName" }; adminPassword = new TextBox { Dock = DockStyle.Fill, MaxLength = 100, UseSystemPasswordChar = true, Name = "adminPassword" };
            rows.Controls.Add(Ui.Label("账号："), 0, 0); rows.Controls.Add(adminAccount, 1, 0); rows.Controls.Add(Ui.Label("姓名："), 0, 1); rows.Controls.Add(adminName, 1, 1); rows.Controls.Add(Ui.Label("密码："), 0, 2); rows.Controls.Add(adminPassword, 1, 2);
            var hint = Ui.Label("修改时密码留空可保留原密码。"); rows.Controls.Add(hint, 0, 3); rows.SetColumnSpan(hint, 2);
            permissionChecks = new CheckBox[5]; string[] captions = { "社保人员管理与注册", "认证领取记录查询与清空", "数据统计", "管理人员与权限设置", "操作记录查询" };
            for (int i = 0; i < permissionChecks.Length; i++) { var cb = new CheckBox { Text = captions[i], AutoSize = true, Margin = new Padding(5, 10, 5, 6) }; permissionChecks[i] = cb; rows.Controls.Add(cb, 0, i + 4); rows.SetColumnSpan(cb, 2); }
            var save = Ui.Button("保存", SaveAdmin); save.Name = "saveAdminButton"; rows.Controls.Add(save, 0, 9); rows.SetColumnSpan(save, 2);
            editor.Controls.Add(rows); split.Controls.Add(list, 0, 0); split.Controls.Add(editor, 1, 0); adminsTab.Controls.Add(split);
        }
        private void BuildLogsTab()
        {
            var layout = Rows(); layout.Controls.Add(Ui.Buttons(Ui.Label("操作记录"), Ui.Button("刷新", (s, e) => Safe(RefreshLogs))), 0, 0);
            logsGrid = Ui.Grid(); logsGrid.Name = "logsGrid"; layout.Controls.Add(logsGrid, 0, 1); logsTab.Controls.Add(layout);
        }
    }
}
