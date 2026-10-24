using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocialSecurityAuth
{
    public partial class MainForm : Form
    {
        private readonly AppRepository repository;
        private readonly bool preview;
        private FaceService face;
        private bool databaseReady, initializing;
        public MainForm() : this(new AppRepository(Database.ConnectionString), false) { }
        public MainForm(AppRepository repository, bool preview)
        {
            this.repository = repository; this.preview = preview;
            InitializeComponent(); Shown += OnShown;
        }
        private async void OnShown(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            if (preview) { repository.CheckConnection(); databaseReady = true; databaseStatus.Text = "数据库：已连接"; identifyStatus.Text = "识别：界面检查"; RefreshRecent(); prompt.Text = "界面检查模式，未开启摄像头"; identifyButton.Enabled = false; return; }
            await InitializeSystem();
        }
        private async Task InitializeSystem()
        {
            if (initializing) return;
            initializing = true; identifyButton.Enabled = false; manageButton.Enabled = false;
            try
            {
                databaseStatus.Text = "数据库：正在连接";
                await Task.Run(() => Database.Initialize(Database.ConnectionString, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "00_CreateDatabase.sql")));
                if (IsDisposed) return;
                repository.CheckConnection(); databaseReady = true; databaseStatus.Text = "数据库：已连接"; RefreshRecent(); ConnectCamera();
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                databaseReady = false; databaseStatus.Text = "数据库：连接失败"; prompt.Text = "数据库未连接，请检查服务和配置后重试"; identifyButton.Text = "重新连接"; Ui.Error(this, ex);
            }
            finally { initializing = false; if (!IsDisposed) { identifyButton.Enabled = true; manageButton.Enabled = databaseReady; } }
        }
        private void ConnectCamera()
        {
            try
            {
                if (face == null) { face = new FaceService(repository); face.Identified += OnIdentified; face.Error += OnDeviceError; }
                face.Attach(cameraPanel); deviceStatus.Text = "设备：已连接"; StartRecognition();
            }
            catch (Exception ex)
            {
                deviceStatus.Text = "设备：未连接"; identifyStatus.Text = "识别：未进行"; identifyButton.Text = "重新连接";
                prompt.Text = "人脸设备不可用，可先进入管理界面维护信息";
                MessageBox.Show(this, Ui.ErrorText(ex), "设备连接提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void OnDeviceError(object sender, string text)
        {
            prompt.Text = text; deviceStatus.Text = "设备：异常"; identifyStatus.Text = "识别：未进行"; identifyButton.Text = "重新连接";
        }
        private void StartRecognition()
        {
            bool running = face.StartIdentification();
            identifyStatus.Text = running ? "识别：进行中" : "识别：等待注册";
            identifyButton.Text = running ? "暂停识别" : "重新连接";
            prompt.Text = running ? "请正对摄像头，系统正在自动识别" : "尚无人脸模板，请进入管理界面采集并注册";
        }
        private async void ToggleRecognition(object sender, EventArgs e)
        {
            if (!databaseReady) { await InitializeSystem(); return; }
            try
            {
                if (face == null || !face.DeviceAvailable || face.TemplateCount == 0) { ConnectCamera(); return; }
                if (face.Running) { face.Stop(); identifyStatus.Text = "识别：已暂停"; identifyButton.Text = "继续识别"; prompt.Text = "识别已暂停"; }
                else StartRecognition();
            }
            catch (Exception ex) { Ui.Error(this, ex); }
        }
        private void OnIdentified(object sender, IdentificationEventArgs args)
        {
            try
            {
                DataRow person = args.PersonId.HasValue ? repository.GetPerson(args.PersonId.Value) : null;
                bool passed = args.Passed && person != null;
                bool newClaim;
                repository.RecordAuthentication(person == null ? null : args.PersonId, passed, passed ? null : args.Reason, out newClaim);
                socialNoValue.Text = person == null ? "—" : (string)person["SocialNo"];
                nameValue.Text = person == null ? "未知人员" : (string)person["PersonName"];
                unitValue.Text = person == null ? "—" : (string)person["UnitName"];
                timeValue.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                resultValue.Text = passed ? "认证通过" : "认证未通过"; resultValue.ForeColor = passed ? Color.DarkGreen : Color.Firebrick;
                claimValue.Text = passed ? (newClaim ? "首次领取已登记" : "已领取，本次不重复登记") : "未登记";
                prompt.Text = passed ? (newClaim ? "认证通过，首次领取信息已记录" : "认证通过，该人员已领取，本次不新增领取记录") : "认证未通过，请联系管理人员";
                Play(passed ? "sbcg.wav" : "sbsb.wav"); RefreshRecent();
            }
            catch (Exception ex)
            {
                try { face.Stop(); } catch { }
                databaseReady = false; databaseStatus.Text = "数据库：写入失败"; identifyStatus.Text = "识别：已停止"; identifyButton.Text = "重新连接";
                resultValue.Text = "记录未保存"; claimValue.Text = "未登记"; prompt.Text = "认证记录未保存，请处理连接问题后重试"; Ui.Error(this, ex);
            }
        }
        private void RefreshRecent()
        {
            DataTable t = repository.GetRecentRecords();
            var view = new DataView(t).ToTable(false, "PersonName", "AuthResult", "AuthTime"); recentGrid.DataSource = view;
        }
        private void OpenManagement(object sender, EventArgs e)
        {
            if (!databaseReady) return;
            try
            {
                if (face != null) face.Unload();
                identifyStatus.Text = "识别：管理中"; deviceStatus.Text = "设备：切换管理";
                using (var login = new LoginForm(repository))
                {
                    if (login.ShowDialog(this) == DialogResult.OK)
                    {
                        using (var manage = new ManageForm(repository, login.Session, face, preview)) manage.ShowDialog(this);
                    }
                }
                RefreshRecent();
            }
            catch (Exception ex) { Ui.Error(this, ex); }
            finally { if (!preview && !IsDisposed) ConnectCamera(); }
        }
        public static void Play(string name)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sound", name);
            if (!File.Exists(path)) return;
            try { using (var sound = new SoundPlayer(path)) sound.Play(); } catch { /* 音频失败不能阻止业务记录。 */ }
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clockTimer.Stop(); clockTimer.Dispose(); if (face != null) face.Dispose(); base.OnFormClosed(e);
        }
    }
}
