using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SocialSecurityAuth
{
    public partial class ManageForm : Form
    {
        private readonly AppRepository repository;
        private readonly AdminSession session;
        private readonly FaceService face;
        private readonly bool preview;
        private int personId, adminId;
        private bool ready, enrolling;
        private PersonInput pendingPerson;
        public ManageForm() : this(new AppRepository(Database.ConnectionString), null, null, true) { }
        public ManageForm(AppRepository repository, AdminSession session, FaceService face, bool preview)
        {
            this.repository = repository; this.session = session; this.face = face; this.preview = preview;
            InitializeComponent();
            Shown += OnShown; FormClosing += OnClosing;
        }
        private void OnShown(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || session == null) return;
            ApplyPermissions();
            if (face != null) { face.Enrolled += OnEnrolled; face.Error += OnDeviceError; }
            ready = true;
            Safe(() => { RefreshUnits(); RefreshSelectedTab(); if (session.Can(Permission.People)) { ClearPerson(); if (!preview) ConnectCamera(); } });
        }
        private void ApplyPermissions()
        {
            TabPage[] pages = { peopleTab, recordsTab, statisticsTab, adminsTab, logsTab };
            Permission[] required = { Permission.People, Permission.Records, Permission.Statistics, Permission.Accounts, Permission.Logs };
            for (int i = 0; i < pages.Length; i++) if (!session.Can(required[i])) tabs.TabPages.Remove(pages[i]);
            if (tabs.TabPages.Count == 0) status.Text = "当前账户没有管理权限，请返回。";
        }
        private void Safe(Action action)
        {
            try { action(); }
            catch (Exception ex) { status.Text = ex.Message; Ui.Error(this, ex); }
        }
        private void Mutation(string action, Action operation)
        {
            try { operation(); status.Text = action + "成功"; }
            catch (Exception ex)
            {
                try { repository.RecordFailedOperation(session, action + "：" + ex.Message); } catch { }
                status.Text = action + "未完成"; Ui.Error(this, ex);
            }
        }
        private void RefreshUnits()
        {
            DataTable units = repository.GetUnits();
            Ui.FillUnits(peopleUnit, units, true); Ui.FillUnits(recordUnit, units, true); Ui.FillUnits(statisticsUnit, units, true);
            string text = personUnit.Text; personUnit.Items.Clear(); foreach (DataRow r in units.Rows) personUnit.Items.Add((string)r["UnitName"]);
            personUnit.Text = text.Length > 0 ? text : units.Rows.Count > 0 ? (string)units.Rows[0]["UnitName"] : "";
        }
        private void RefreshSelectedTab()
        {
            if (!ready || tabs.SelectedTab == null) return;
            Safe(() => {
                if (tabs.SelectedTab == peopleTab) RefreshPeople();
                else if (tabs.SelectedTab == recordsTab) RefreshRecords();
                else if (tabs.SelectedTab == statisticsTab) RefreshStatistics();
                else if (tabs.SelectedTab == adminsTab) RefreshAdmins();
                else if (tabs.SelectedTab == logsTab) RefreshLogs();
            });
        }
        private void OnSelectingTab(object sender, TabControlCancelEventArgs e) { if (enrolling) { e.Cancel = true; status.Text = "请先完成或取消人脸采集。"; } }
        private void RefreshPeople()
        {
            peopleGrid.DataSource = repository.GetPeople(session, Ui.SelectedUnit(peopleUnit));
            if (peopleGrid.Columns.Contains("CreatedAt")) peopleGrid.Columns["CreatedAt"].Visible = false;
            if (peopleGrid.Columns.Contains("AuthStatus")) peopleGrid.Columns["AuthStatus"].Visible = false;
            peopleGrid.ClearSelection();
        }
        private void RefreshRecords() { recordsGrid.DataSource = repository.GetRecords(session, Ui.SelectedUnit(recordUnit)); }
        private void ClearRecords(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "确定清空所有单位的认证领取记录？此操作无法撤销。\r\n人员资料和人脸保留，认证状态将重置；下次识别成功可重新登记领取。", "清空确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            Mutation("清空认证领取记录", ClearRecordsAndRefresh);
        }
        private void ClearRecordsAndRefresh()
        {
            repository.ClearRecords(session); RefreshRecords();
            if (session.Can(Permission.People)) RefreshPeople();
            if (session.Can(Permission.Statistics)) RefreshStatistics();
            if (session.Can(Permission.Logs)) RefreshLogs();
        }
        private void RefreshStatistics()
        {
            int? unit = Ui.SelectedUnit(statisticsUnit);
            DataTable snapshot = repository.GetStatisticsSnapshot(session, unit);
            StatisticsSummary summary = AppRepository.Summarize(snapshot);
            summaryLabel.Text = String.Format("已采集信息：{0} 人   已通过认证：{1} 人   认证未通过：{2} 人   未认证：{3} 人", summary.Collected, summary.Passed, summary.Failed, summary.Uncertified);
            DataTable list = AppRepository.FilterStatistics(snapshot, (StatisticsKind)Math.Max(0, statisticsType.SelectedIndex)); statisticsGrid.DataSource = list;
            if (statisticsGrid.Columns.Contains("CreatedAt")) statisticsGrid.Columns["CreatedAt"].Visible = false;
            if (statisticsGrid.Columns.Contains("Gender")) statisticsGrid.Columns["Gender"].Visible = false;
            statisticsNote.Text = "当前名单共 " + list.Rows.Count + " 人；按人员最近一次识别状态统计，未知人员失败不归属单位。";
        }
        private void RefreshAdmins() { adminsGrid.DataSource = repository.GetAdmins(session); adminsGrid.ClearSelection(); }
        private void RefreshLogs() { logsGrid.DataSource = repository.GetLogs(session); }
        private void SelectPerson(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || enrolling) return;
            var row = ((DataRowView)peopleGrid.Rows[e.RowIndex].DataBoundItem).Row;
            personId = (int)row["PersonId"]; personNo.Text = (string)row["SocialNo"]; personName.Text = (string)row["PersonName"]; male.Checked = (string)row["Gender"] == "男"; female.Checked = !male.Checked; personUnit.Text = (string)row["UnitName"];
            personNo.ReadOnly = true;
            enrollmentStatus.Text = "正在编辑已有人员；登记下一位请点击“新增”。\r\n人脸状态：" + ((bool)row["HasFace"] ? "已采集" : "未采集");
        }
        private void ClearPerson()
        {
            personId = 0; personNo.ReadOnly = false; personNo.Clear(); personName.Clear(); male.Checked = true; peopleGrid.ClearSelection(); enrollmentStatus.Text = "当前为新增人员\r\n填写信息后可保存或采集并注册。"; personNo.Focus();
        }
        private PersonInput ReadPerson()
        {
            if (String.IsNullOrWhiteSpace(personNo.Text) || String.IsNullOrWhiteSpace(personName.Text) || String.IsNullOrWhiteSpace(personUnit.Text)) throw new ArgumentException("请填写社保编号、姓名和所属单位。");
            return new PersonInput { PersonId = personId, SocialNo = personNo.Text.Trim(), Name = personName.Text.Trim(), Gender = male.Checked ? "男" : "女", UnitName = personUnit.Text.Trim() };
        }
        private void SavePerson(object sender, EventArgs e)
        {
            Mutation("保存人员", () => {
                personId = repository.SavePerson(session, ReadPerson()); personNo.ReadOnly = true;
                RefreshUnits(); RefreshPeople();
                enrollmentStatus.Text = "正在编辑已保存人员；可继续采集人脸。\r\n登记下一位请点击“新增”。";
            });
        }
        private void DeletePerson(object sender, EventArgs e)
        {
            if (personId == 0) { MessageBox.Show(this, "请先从表格选择人员。"); return; }
            if (MessageBox.Show(this, "删除所选人员及其人脸模板？删除后不能再凭原人脸认证；已有认证领取记录仍会保留。", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            Mutation("删除人员", () => { repository.DeletePerson(session, personId); ClearPerson(); RefreshPeople(); if (!preview && face != null && face.Loaded) ConnectCamera(); });
        }
        private void ConnectCamera()
        {
            if (preview) { enrollmentStatus.Text = "界面检查模式，未开启摄像头"; enrollButton.Enabled = false; return; }
            if (face == null) throw new InvalidOperationException("SDK 尚未初始化，请返回主界面重新连接设备。");
            face.Attach(cameraPanel); enrollButton.Enabled = true; connectButton.Text = "重新连接"; status.Text = "设备已连接，已加载 " + face.TemplateCount + " 份人脸模板";
        }
        private void Enroll(object sender, EventArgs e)
        {
            Safe(() => {
                pendingPerson = ReadPerson();
                if (face == null || !face.DeviceAvailable) ConnectCamera();
                if (face == null || !face.DeviceAvailable) throw new InvalidOperationException("设备尚未连接。");
                SetEnrolling(true);
                try { face.StartEnrollment(); enrollmentStatus.Text = "正在采集，请正对摄像头"; }
                catch { SetEnrolling(false); pendingPerson = null; throw; }
            });
        }
        private void OnEnrolled(object sender, EnrollmentEventArgs args)
        {
            if (!enrolling || pendingPerson == null || IsDisposed) return;
            PersonInput captured = pendingPerson; pendingPerson = null; SetEnrolling(false);
            if (!args.Passed) { enrollmentStatus.Text = args.Message; MainForm.Play("zcsb.wav"); return; }
            Mutation("人脸注册", () => {
                repository.SavePerson(session, captured, args.Template); RefreshUnits(); RefreshPeople(); ClearPerson();
                enrollmentStatus.Text = captured.Name + "注册成功，模板已保存。\r\n现可直接填写下一位人员的信息。"; MainForm.Play("zccg.wav");
                // 重建模块与模板库，确保删除或重采集后没有残留索引。
                if (!preview && face != null) face.Attach(cameraPanel);
            });
        }
        private void SetEnrolling(bool value)
        {
            enrolling = value; cancelEnrollButton.Enabled = value;
            foreach (Control c in new Control[] { peopleGrid, personNo, personName, personUnit, male, female, newPersonButton, savePersonButton, deletePersonButton, enrollButton, connectButton, peopleUnit }) c.Enabled = !value;
        }
        private void OnDeviceError(object sender, string text)
        {
            pendingPerson = null; SetEnrolling(false); enrollButton.Enabled = false; enrollmentStatus.Text = text; status.Text = "设备异常，请重新连接";
        }
        private void CancelEnrollment(object sender, EventArgs e)
        { Safe(() => { try { if (face != null) face.Stop(); } finally { pendingPerson = null; SetEnrolling(false); enrollmentStatus.Text = "已取消采集"; } }); }
        private void ClearAdmin()
        { adminId = 0; adminAccount.Clear(); adminName.Clear(); adminPassword.Clear(); foreach (var c in permissionChecks) c.Checked = false; adminsGrid.ClearSelection(); adminAccount.Focus(); }
        private void SelectAdmin(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = ((DataRowView)adminsGrid.Rows[e.RowIndex].DataBoundItem).Row;
            adminId = (int)row["AdminId"]; adminAccount.Text = (string)row["Account"]; adminName.Text = (string)row["AdminName"]; adminPassword.Clear();
            int flags = (int)row["PermissionFlags"]; for (int i = 0; i < 5; i++) permissionChecks[i].Checked = (flags & (1 << i)) != 0;
        }
        private void SaveAdmin(object sender, EventArgs e)
        {
            Mutation("保存管理人员", () => {
                Permission permissions = Permission.None; for (int i = 0; i < 5; i++) if (permissionChecks[i].Checked) permissions |= (Permission)(1 << i);
                int saved = repository.SaveAdmin(session, adminId, adminAccount.Text, adminName.Text, adminPassword.Text, permissions);
                adminId = saved; adminPassword.Clear();
                if (saved == session.AdminId) { MessageBox.Show(this, "当前账户信息或权限已更新，请重新登录管理界面。"); Close(); return; }
                RefreshAdmins();
            });
        }
        private void DeleteAdmin(object sender, EventArgs e)
        {
            if (adminId == 0) { MessageBox.Show(this, "请先从表格选择管理人员。"); return; }
            if (MessageBox.Show(this, "删除选中的管理人员？", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            Mutation("删除管理人员", () => { repository.DeleteAdmin(session, adminId); ClearAdmin(); RefreshAdmins(); });
        }
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (face != null)
            {
                face.Enrolled -= OnEnrolled;
                face.Error -= OnDeviceError;
                try { face.Unload(); } catch (Exception ex) { status.Text = ex.Message; }
            }
            pendingPerson = null; enrolling = false;
        }
        // 仅供离线界面检查使用，不产生模拟认证数据、不调用摄像头。
        public void SelectPreviewTab(string name)
        {
            foreach (TabPage tab in tabs.TabPages) if (tab.Name == name) tabs.SelectedTab = tab;
        }
    }
}
