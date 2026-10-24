using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SocialSecurityAuth;

namespace SocialSecurityAuth.Tests
{
    internal static class Program
    {
        private static int assertions;
        private static readonly string Script = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "00_CreateDatabase.sql");
        [STAThread]
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length > 0 && args[0] == "--initialize") { Database.Initialize(Database.ConnectionString, Script); Console.WriteLine("Project database initialized; no face or authentication records seeded."); return 0; }
                if (args.Length > 0 && args[0] == "--capture-ui") { CaptureUi(args.Length > 1 ? args[1] : "UiScreenshots"); return 0; }
                if (args.Length > 0 && args[0] == "--device-check") { CheckDevice(); return 0; }
                RunIntegrationTests(); Console.WriteLine("PASS: " + assertions + " assertions. Isolated test database cleaned up."); return 0;
            }
            catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
        }
        private static void Assert(bool value, string message)
        { if (!value) throw new Exception("ASSERT FAILED: " + message); assertions++; Console.WriteLine("PASS " + message); }
        private static void Throws<T>(Action work, string message) where T : Exception
        { try { work(); } catch (T) { Assert(true, message); return; } throw new Exception("Expected " + typeof(T).Name + ": " + message); }
        private static int UnitId(AppRepository repo, string name)
        { foreach (DataRow r in repo.GetUnits().Rows) if ((string)r["UnitName"] == name) return (int)r["UnitId"]; throw new Exception("Unit not found"); }
        private static PersonInput Person(string no, string name, string unit = "第一单位", int id = 0)
        { return new PersonInput { PersonId = id, SocialNo = no, Name = name, UnitName = unit, Gender = "男" }; }
        private static void RunIntegrationTests()
        {
            string catalog = "SocialSecurityAuth_Test_" + Guid.NewGuid().ToString("N");
            var cs = new SqlConnectionStringBuilder(Database.ConnectionString) { InitialCatalog = catalog };
            try
            {
                Database.Initialize(cs.ConnectionString, Script); Database.Initialize(cs.ConnectionString, Script);
                var repo = new AppRepository(cs.ConnectionString);
                Assert(repo.Login("admin", "wrong-password") == null, "wrong password rejected");
                AdminSession admin = repo.Login("admin", "Admin@123"); Assert(admin != null && admin.Can(Permission.All), "default admin authenticated");
                Assert(repo.GetAdmins(admin).Rows.Count == 1, "idempotent initialization does not duplicate admins");
                int alice = repo.SavePerson(admin, Person("T001", "李'测试"), new byte[] { 1, 2, 3, 4 });
                int bob = repo.SavePerson(admin, Person("T002", "王小明"));
                int carol = repo.SavePerson(admin, Person("T003", "陈老师", "第二单位"), new byte[] { 5, 6, 7, 8 });
                Assert((string)repo.GetPerson(alice)["PersonName"] == "李'测试", "Unicode and quoted text round trip via parameters");
                Assert(repo.GetFaces().Count == 2, "person templates persisted");
                repo.SavePerson(admin, Person("T002", "王小明修改", "新单位", bob));
                Assert((string)repo.GetPerson(bob)["UnitName"] == "新单位", "unit can be entered and personnel updated");
                Throws<InvalidOperationException>(() => repo.SavePerson(admin, Person("T001", "重复", "不应存在的单位")), "duplicate social number rejected");
                Assert(repo.GetUnits().Select("UnitName='不应存在的单位'").Length == 0, "failed registration rolls back unit insertion");
                Throws<ArgumentException>(() => repo.SavePerson(admin, Person("T004", "无效模板"), new byte[0]), "empty template rejected before saving");
                Assert(repo.GetPeople(admin, null).Rows.Count == 3, "failed registration does not leave incomplete person");
                var summary = repo.GetSummary(admin, null); Assert(summary.Collected == 2 && summary.Uncertified == 3, "initial statistics distinguish face collection and authentication");
                repo.RecordAuthentication(alice, false, "测试数据库中的已知人员失败");
                repo.RecordAuthentication(null, false, "测试数据库中的未知人员失败");
                Assert(repo.GetRecords(admin, null).Rows.Count == 0, "known and unknown failures do not create claim records");
                Assert(repo.GetSummary(admin, UnitId(repo, "第一单位")).Failed == 1, "known failed person counted in own unit");
                Assert(repo.GetSummary(admin, null).Failed == 1, "unknown failure not assigned to known person");
                repo.RecordAuthentication(alice, true, null);
                summary = repo.GetSummary(admin, null); Assert(summary.Passed == 1 && summary.Failed == 0 && summary.Uncertified == 2, "latest authentication decides person category");
                DataTable records = repo.GetRecords(admin, null); Assert(records.Rows.Count == 1, "only first successful claim is stored");
                DataRow passed = records.Select("AuthResult='通过'")[0]; Assert((string)passed["ClaimText"] == "已登记" && passed["ClaimTime"] != DBNull.Value, "successful authentication records claim atomically");
                Throws<ArgumentException>(() => repo.RecordAuthentication(null, true, null), "unknown person cannot pass authentication");
                Assert(repo.GetRecords(admin, UnitId(repo, "第一单位")).Rows.Count == 1, "unit query returns only first claims");
                repo.DeletePerson(admin, carol); Assert(repo.GetPerson(carol) == null && repo.GetFaces().Count == 1, "unreferenced person deletion also removes template");
                int viewerId = repo.SaveAdmin(admin, 0, "viewer", "记录查询员", "Viewer@123", Permission.Records);
                AdminSession viewer = repo.Login("viewer", "Viewer@123"); Assert(repo.GetRecords(viewer, null).Rows.Count == 1, "authorized read succeeds");
                Throws<UnauthorizedAccessException>(() => repo.GetPeople(viewer, null), "unauthorized people read blocked in repository");
                Throws<UnauthorizedAccessException>(() => repo.GetSummary(viewer, null), "unauthorized statistics blocked");
                Throws<UnauthorizedAccessException>(() => repo.SavePerson(viewer, Person("NOPE", "无权限", "不可新增")), "unauthorized mutation blocked");
                Throws<UnauthorizedAccessException>(() => repo.DeletePerson(viewer, alice), "unauthorized personnel deletion blocked");
                repo.SaveAdmin(admin, viewerId, "viewer", "记录查询员", "", Permission.None);
                Throws<UnauthorizedAccessException>(() => repo.GetRecords(viewer, null), "existing session respects revoked permissions");
                Throws<InvalidOperationException>(() => repo.SaveAdmin(admin, admin.AdminId, "admin", "管理员", "", Permission.People), "last account manager permission preserved");
                Throws<InvalidOperationException>(() => repo.DeleteAdmin(admin, admin.AdminId), "current admin cannot delete self");
                Throws<InvalidOperationException>(() => repo.DeleteAdmin(admin, viewerId), "admin audit history retained");
                int temp = repo.SaveAdmin(admin, 0, "temporary", "临时账户", "Temp@123", Permission.None);
                repo.DeleteAdmin(admin, temp); Assert(repo.GetAdmins(admin).Rows.Count == 2, "unreferenced admin deleted");
                repo.SaveAdmin(admin, admin.AdminId, "admin", "管理员", "Changed@123", Permission.All);
                Database.Initialize(cs.ConnectionString, Script);
                Assert(repo.Login("admin", "Admin@123") == null && repo.Login("admin", "Changed@123") != null, "reinitialization does not reset existing password");
                Assert(repo.GetLogs(admin).Rows.Count >= 12, "management operations have audit records");
                Assert(repo.GetStatistics(admin, null, StatisticsKind.Collected).Rows.Count == repo.GetSummary(admin, null).Collected, "statistics details agree with summary");
                CheckEnrollmentRegression(repo, admin, alice);
                CheckHistoryDeletionAndClaims(repo, admin, alice, cs.ConnectionString);
                CheckClaimPrompts(repo, admin);
                CheckClearRecords(repo, admin, cs.ConnectionString);
                Console.WriteLine("Test records used only in " + catalog + ", never in the project database.");
            }
            finally
            {
                SqlConnection.ClearAllPools();
                if (!System.Text.RegularExpressions.Regex.IsMatch(catalog, @"^SocialSecurityAuth_Test_[a-f0-9]{32}$")) throw new Exception("Unsafe cleanup target");
                cs.InitialCatalog = "master";
                using (var c = new SqlConnection(cs.ConnectionString))
                { c.Open(); using (var cmd = new SqlCommand("IF DB_ID(N'" + catalog + "') IS NOT NULL BEGIN ALTER DATABASE [" + catalog + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + catalog + "]; END", c)) cmd.ExecuteNonQuery(); }
            }
        }
        private static object Field(ManageForm form, string name)
        { return typeof(ManageForm).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(form); }
        private static object Invoke(ManageForm form, string name, params object[] args)
        { return typeof(ManageForm).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(form, args); }
        private static void CompleteTestEnrollment(ManageForm form, byte[] template)
        {
            // 走真实窗体完成回调；测试字节仅写入独立测试库，不调用摄像头。
            typeof(ManageForm).GetField("pendingPerson", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(form, Invoke(form, "ReadPerson"));
            Invoke(form, "SetEnrolling", true);
            Invoke(form, "OnEnrolled", null, new EnrollmentEventArgs { Passed = true, Template = template });
        }
        private static void CheckEnrollmentRegression(AppRepository repo, AdminSession admin, int existingId)
        {
            Throws<InvalidOperationException>(() => repo.SavePerson(admin, Person("WRONG-ID", "误覆盖", "不应新增", existingId), new byte[] { 99 }), "stale editor identity cannot overwrite another social number or template");
            Assert((string)repo.GetPerson(existingId)["SocialNo"] == "T001" && repo.GetFaces().Find(f => f.PersonId == existingId).Code[0] == 1, "rejected overwrite preserves original person and face");
            Assert(repo.GetUnits().Select("UnitName='不应新增'").Length == 0, "identity mismatch leaves no unit or partial changes");
            Application.EnableVisualStyles();
            using (var form = new ManageForm(repo, admin, null, true))
            {
                form.StartPosition = FormStartPosition.Manual; form.Location = new Point(-3000, -3000); form.ShowInTaskbar = false;
                form.Show(); Application.DoEvents();
                var no = (TextBox)Field(form, "personNo"); var name = (TextBox)Field(form, "personName");
                no.Text = "UI001"; name.Text = "连续注册第一人";
                CompleteTestEnrollment(form, new byte[] { 11, 12, 13, 14 });
                DataRow first = repo.GetPeople(admin, null).Select("SocialNo='UI001'")[0]; int firstId = (int)first["PersonId"];
                Assert((int)Field(form, "personId") == 0 && no.Text == "" && name.Text == "" && !no.ReadOnly, "successful enrollment resets editor for next person");
                Assert((bool)first["HasFace"], "first enrollment saves person and template");
                // 不点击新增，直接填写第二位，重现用户报告的操作顺序。
                no.Text = "UI002"; name.Text = "连续注册第二人";
                CompleteTestEnrollment(form, new byte[] { 21, 22, 23, 24 });
                DataTable people = repo.GetPeople(admin, null); DataRow second = people.Select("SocialNo='UI002'")[0]; int secondId = (int)second["PersonId"];
                Assert(firstId != secondId && people.Select("SocialNo='UI001'").Length == 1, "consecutive enrollments keep distinct personnel IDs");
                Assert(repo.GetFaces().Find(f => f.PersonId == firstId).Code[0] == 11 && repo.GetFaces().Find(f => f.PersonId == secondId).Code[0] == 21, "both templates survive second enrollment with original bytes");
                Assert((string)repo.GetPerson(firstId)["PersonName"] == "连续注册第一人", "second enrollment preserves first person details");
                var grid = (DataGridView)Field(form, "peopleGrid");
                int firstRow = -1;
                foreach (DataGridViewRow row in grid.Rows) if ((int)((DataRowView)row.DataBoundItem)["PersonId"] == firstId) firstRow = row.Index;
                Invoke(form, "SelectPerson", grid, new DataGridViewCellEventArgs(0, firstRow));
                Assert(no.ReadOnly && (int)Field(form, "personId") == firstId, "selecting existing person locks social number in edit mode");
                Invoke(form, "ClearPerson");
                Assert(!no.ReadOnly && (int)Field(form, "personId") == 0, "New clears existing identity and unlocks social number");
                no.Text = "UI003"; name.Text = "先保存后采集";
                Invoke(form, "SavePerson", null, EventArgs.Empty); int savedId = (int)Field(form, "personId");
                Assert(savedId > 0 && no.ReadOnly && !(bool)repo.GetPerson(savedId)["HasFace"], "saving basic details retains person for later face enrollment");
                CompleteTestEnrollment(form, new byte[] { 31, 32, 33, 34 });
                Assert((bool)repo.GetPerson(savedId)["HasFace"] && repo.GetPeople(admin, null).Select("SocialNo='UI003'").Length == 1 && (int)Field(form, "personId") == 0, "enrolling saved person keeps same ID then returns to new mode");
                form.Close();
            }
        }
        private static void ExecuteTestSql(string connectionString, string sql, int personId)
        {
            var cs = new SqlConnectionStringBuilder(connectionString);
            if (!System.Text.RegularExpressions.Regex.IsMatch(cs.InitialCatalog, @"^SocialSecurityAuth_Test_[a-f0-9]{32}$")) throw new Exception("Test SQL requires isolated database");
            using (var c = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, c))
            { c.Open(); cmd.Parameters.AddWithValue("@id", personId); cmd.ExecuteNonQuery(); }
        }
        private static void CheckClaimPrompts(AppRepository repo, AdminSession admin)
        {
            int person = repo.SavePerson(admin, Person("UI-CLAIM", "领取提示测试"));
            using (var form = new MainForm(repo, true))
            {
                form.StartPosition = FormStartPosition.Manual; form.Location = new Point(-3000, -3000); form.ShowInTaskbar = false;
                form.Show(); Application.DoEvents();
                MethodInfo callback = typeof(MainForm).GetMethod("OnIdentified", BindingFlags.Instance | BindingFlags.NonPublic);
                var claim = (Label)typeof(MainForm).GetField("claimValue", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(form);
                var result = (Label)typeof(MainForm).GetField("resultValue", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(form);
                callback.Invoke(form, new object[] { null, new IdentificationEventArgs { Passed = true, PersonId = person } });
                Assert(claim.Text == "首次领取已登记" && result.Text == "认证通过", "main window shows first successful claim");
                callback.Invoke(form, new object[] { null, new IdentificationEventArgs { Passed = true, PersonId = person } });
                Assert(claim.Text == "已领取，本次不重复登记" && result.Text == "认证通过", "main window allows repeat success and shows already claimed");
                Assert(repo.GetRecords(admin, null).Select("SocialNo='UI-CLAIM'").Length == 1, "main recognition callback stores only first successful claim");
                form.Close();
            }
        }
        private static void CheckHistoryDeletionAndClaims(AppRepository repo, AdminSession admin, int personId, string connectionString)
        {
            int originalRecords = repo.GetRecords(admin, null).Rows.Count;
            DataRow originalClaim = repo.GetRecords(admin, null).Select("SocialNo='T001'")[0];
            // 还原旧版混合流水结构并放入重复领取、重复成功和失败记录。
            ExecuteTestSql(connectionString, @"DROP INDEX UX_Auth_ClaimSocialNo ON dbo.AuthenticationRecord;
                ALTER TABLE dbo.AuthenticationRecord DROP CONSTRAINT CK_Authentication_FirstClaim;
                ALTER TABLE dbo.AuthenticationRecord DROP COLUMN ArchivedSocialNo,ArchivedPersonName,ArchivedUnitId,ArchivedUnitName,ClaimSocialNo;
                ALTER TABLE dbo.AuthenticationRecord ADD CONSTRAINT CK_Authentication_Result CHECK (
                 (Passed=1 AND PersonId IS NOT NULL AND FailureReason IS NULL AND ((ClaimRecorded=1 AND ClaimTime IS NOT NULL) OR (ClaimRecorded=0 AND ClaimTime IS NULL)))
                 OR (Passed=0 AND ClaimRecorded=0 AND ClaimTime IS NULL AND FailureReason IS NOT NULL));
                INSERT dbo.AuthenticationRecord(PersonId,AuthTime,Passed,ClaimRecorded,ClaimTime)
                 VALUES(@id,DATEADD(second,1,SYSDATETIME()),1,1,DATEADD(second,1,SYSDATETIME())),
                       (@id,DATEADD(second,2,SYSDATETIME()),1,1,DATEADD(second,2,SYSDATETIME())),
                       (@id,DATEADD(second,3,SYSDATETIME()),1,0,NULL);
                INSERT dbo.AuthenticationRecord(PersonId,AuthTime,Passed,ClaimRecorded,FailureReason)
                 VALUES(@id,DATEADD(second,4,SYSDATETIME()),0,0,N'旧版已知失败'),
                       (NULL,DATEADD(second,5,SYSDATETIME()),0,0,N'旧版未知失败');", personId);
            Database.Initialize(connectionString, Script); Database.Initialize(connectionString, Script);
            DataTable records = repo.GetRecords(admin, null);
            Assert(records.Rows.Count == originalRecords, "legacy upgrade removes failed and repeated authentication rows");
            Assert(records.Select("SocialNo='T001' AND ClaimText='已登记'").Length == 1, "legacy duplicate claims become one claim per social number");
            Assert(repo.GetSummary(admin, null).Failed == 1, "migration preserves latest failed status separately from claim history");
            DataRow firstClaim = records.Select("SocialNo='T001' AND ClaimText='已登记'")[0];
            Assert(Object.Equals(firstClaim["RecordId"], originalClaim["RecordId"]) && Object.Equals(firstClaim["ClaimTime"], originalClaim["ClaimTime"]), "legacy migration preserves earliest claim and original time");
            bool newClaim;
            long repeated = repo.RecordAuthentication(personId, true, null, out newClaim);
            DataRow repeatedRow = repo.GetRecords(admin, null).Select("RecordId=" + repeated)[0];
            Assert(!newClaim && repeated == (long)firstClaim["RecordId"] && repo.GetRecords(admin, null).Rows.Count == originalRecords && Object.Equals(repeatedRow["ClaimTime"], firstClaim["ClaimTime"]), "repeat success reuses first record without changing original claim time");
            repo.RecordAuthentication(personId, false, "首次领取后的识别失败");
            Assert(repo.GetSummary(admin, null).Failed == 1 && repo.GetRecords(admin, null).Rows.Count == originalRecords, "failure after claim updates statistics without adding or removing claim record");
            long recovered = repo.RecordAuthentication(personId, true, null, out newClaim);
            Assert(!newClaim && recovered == repeated && repo.GetSummary(admin, null).Passed == 1, "success after failure restores current state without another record");
            int parallelPerson = repo.SavePerson(admin, Person("CLAIM001", "并发领取测试"));
            int claimsCreated = 0;
            var tasks = new Task[8];
            for (int i = 0; i < tasks.Length; i++) tasks[i] = Task.Run(() => { bool created; repo.RecordAuthentication(parallelPerson, true, null, out created); if (created) Interlocked.Increment(ref claimsCreated); });
            Task.WaitAll(tasks);
            DataRow[] parallelRecords = repo.GetRecords(admin, null).Select("SocialNo='CLAIM001'");
            Assert(parallelRecords.Length == 1 && claimsCreated == 1, "eight concurrent successes create exactly one stored record");
            Assert(repo.GetRecords(admin, null).Select("SocialNo='CLAIM001' AND ClaimText='已登记'").Length == 1, "database stores only one concurrent claim");
            Throws<SqlException>(() => ExecuteTestSql(connectionString, @"INSERT dbo.AuthenticationRecord(PersonId,Passed,ClaimRecorded,ClaimTime,ClaimSocialNo) VALUES(@id,1,1,SYSDATETIME(),N'T001')", personId), "unique database index rejects duplicate claim even outside repository");
            Throws<SqlException>(() => ExecuteTestSql(connectionString, @"INSERT dbo.AuthenticationRecord(PersonId,Passed,ClaimRecorded) VALUES(NULL,1,0)", personId), "upgraded constraint still rejects unknown successful authentication");
            records = repo.GetRecords(admin, null); int beforeCount = records.Rows.Count;
            int passedBefore = repo.GetSummary(admin, null).Passed;
            int unitId = UnitId(repo, "第一单位"); int unitRecords = repo.GetRecords(admin, unitId).Rows.Count;
            repo.DeletePerson(admin, personId);
            Assert(repo.GetPerson(personId) == null && repo.GetPeople(admin, null).Select("SocialNo='T001'").Length == 0, "person with claim history can be deleted");
            Assert(repo.GetFaces().Find(f => f.PersonId == personId) == null, "deletion removes face template from recognition library");
            DataTable archived = repo.GetRecords(admin, null);
            Assert(archived.Rows.Count == beforeCount, "deleting person preserves all authentication and claim history");
            bool unchanged = true;
            foreach (DataRow row in records.Rows)
            {
                DataRow copy = archived.Select("RecordId=" + row["RecordId"])[0];
                foreach (DataColumn column in records.Columns) if (!Object.Equals(row[column], copy[column.ColumnName])) unchanged = false;
            }
            Assert(unchanged, "archived history retains names numbers units results claim flags and times");
            Assert(repo.GetRecords(admin, unitId).Rows.Count == unitRecords, "deleted member history still supports original unit filtering");
            Assert(repo.GetSummary(admin, null).Passed == passedBefore - 1, "deleted member removed from current personnel statistics");
            Assert(repo.GetLogs(admin).Select("OperationContent='删除人员：T001'").Length == 1, "history-bearing deletion is audited");
            Throws<InvalidOperationException>(() => repo.RecordAuthentication(personId, true, null), "stale face result cannot authenticate deleted member");
            int replacement = repo.SavePerson(admin, Person("T001", "重新登记的人员"), new byte[] { 41, 42, 43, 44 });
            Assert(replacement != personId, "deleted social number can be registered again");
            repo.RecordAuthentication(replacement, true, null, out newClaim);
            Assert(!newClaim && repo.GetRecords(admin, null).Select("SocialNo='T001' AND ClaimText='已登记'").Length == 1, "deleting and registering again cannot create another claim for same social number");
            Assert((string)repo.GetRecords(admin, null).Select("RecordId=" + firstClaim["RecordId"])[0]["PersonName"] == "李'测试", "reregistration does not change archived original identity");
            Database.Initialize(connectionString, Script);
            Assert(repo.GetRecords(admin, null).Rows.Count == beforeCount, "initialization remains idempotent after deletion and repeated recognition");
        }
        private static void CheckClearRecords(AppRepository repo, AdminSession admin, string connectionString)
        {
            int peopleCount = repo.GetPeople(admin, null).Rows.Count, faceCount = repo.GetFaces().Count, recordCount = repo.GetRecords(admin, null).Rows.Count;
            int passedBefore = repo.GetSummary(admin, null).Passed;
            repo.SaveAdmin(admin, 0, "people-only", "人员管理员", "People@123", Permission.People);
            AdminSession limited = repo.Login("people-only", "People@123");
            Throws<UnauthorizedAccessException>(() => repo.ClearRecords(limited), "clearing requires records permission in repository");
            Assert(repo.GetRecords(admin, null).Rows.Count == recordCount && repo.GetSummary(admin, null).Passed == passedBefore, "denied clear leaves records and current status unchanged");
            using (var form = new ManageForm(repo, admin, null, true))
            {
                form.StartPosition = FormStartPosition.Manual; form.Location = new Point(-3000, -3000); form.ShowInTaskbar = false;
                form.Show(); form.SelectPreviewTab("recordsTab"); Application.DoEvents();
                var button = (Button)Field(form, "clearRecordsButton");
                Assert(button.Visible && button.Enabled && button.Text == "清空认证领取记录", "records tab exposes clear records button");
                var filter = (ComboBox)Field(form, "recordUnit"); if (filter.Items.Count > 1) filter.SelectedIndex = 1;
                Invoke(form, "RefreshRecords");
                string imagePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Doc", "运行界面", "recordsTab_v1.0.3.png"));
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                using (var bmp = new Bitmap(form.Width, form.Height)) { form.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size)); bmp.Save(imagePath); }
                // 验证确认后的真实执行路径；不在测试中点击正式库的清空按钮。
                Invoke(form, "ClearRecordsAndRefresh");
                Assert(((DataGridView)Field(form, "recordsGrid")).Rows.Count == 0, "clear refreshes records grid immediately");
                form.Close();
            }
            Assert(repo.GetRecords(admin, null).Rows.Count == 0 && repo.GetRecentRecords().Rows.Count == 0, "clear removes all units and recent claim records regardless of filter");
            var summary = repo.GetSummary(admin, null);
            Assert(summary.Passed == 0 && summary.Failed == 0 && summary.Uncertified == peopleCount, "clear resets current personnel authentication status");
            Assert(repo.GetPeople(admin, null).Rows.Count == peopleCount && repo.GetFaces().Count == faceCount, "clear preserves personnel and face templates");
            Assert(repo.GetLogs(admin).Select("OperationContent LIKE '清空认证领取记录：%'").Length == 1, "clear writes management audit log");
            Database.Initialize(connectionString, Script);
            Assert(repo.GetRecords(admin, null).Rows.Count == 0 && repo.GetSummary(admin, null).Uncertified == peopleCount, "restart does not restore cleared records or stale authentication state");
            int person = (int)repo.GetPeople(admin, null).Select("SocialNo='T001'")[0]["PersonId"]; bool created;
            long first = repo.RecordAuthentication(person, true, null, out created);
            Assert(created && repo.GetRecords(admin, null).Rows.Count == 1, "first success after clear creates a new initial claim");
            long again = repo.RecordAuthentication(person, true, null, out created);
            Assert(!created && first == again && repo.GetRecords(admin, null).Rows.Count == 1, "success after renewed claim still does not add repeated records");
        }
        private static void Capture(Form form, string file)
        {
            form.StartPosition = FormStartPosition.Manual; form.Location = new Point(-3000, -3000); form.ShowInTaskbar = false;
            form.Show(); Application.DoEvents();
            using (var bmp = new Bitmap(form.Width, form.Height)) { form.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size)); bmp.Save(file); }
        }
        private static void CaptureUi(string path)
        {
            Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Directory.CreateDirectory(path);
            var repo = new AppRepository(Database.ConnectionString); repo.CheckConnection(); var admin = repo.Login("admin", "Admin@123");
            if (admin == null) throw new InvalidOperationException("UI 检查需使用初始化的管理账户；密码已修改时请通过实际程序检查。");
            using (var main = new MainForm(repo, true)) Capture(main, Path.Combine(path, "main.png"));
            using (var manage = new ManageForm(repo, admin, null, true))
            {
                Capture(manage, Path.Combine(path, "people.png"));
                foreach (string name in new[] { "recordsTab", "statisticsTab", "adminsTab", "logsTab" })
                { manage.SelectPreviewTab(name); Application.DoEvents(); using (var bmp = new Bitmap(manage.Width, manage.Height)) { manage.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size)); bmp.Save(Path.Combine(path, name + ".png")); } }
            }
            using (var login = new LoginForm(repo)) Capture(login, Path.Combine(path, "login.png"));
            using (var splash = new SplashForm()) Capture(splash, Path.Combine(path, "splash.png"));
            Console.WriteLine("UI captures saved; camera not opened and no authentication data simulated.");
        }
        private static void CheckDevice()
        {
            Application.EnableVisualStyles(); var repo = new AppRepository(Database.ConnectionString);
            using (var host = new Form { Width = 640, Height = 480, ShowInTaskbar = false, StartPosition = FormStartPosition.Manual, Location = new Point(-3000, -3000) })
            using (var face = new FaceService(repo))
            {
                face.StatusChanged += (s, e) => Console.WriteLine("DEVICE STATUS type=" + e.Type + " value=" + e.Value);
                host.Show(); Application.DoEvents(); face.Attach(host); Application.DoEvents();
                Console.WriteLine("SDK loaded; template count=" + face.TemplateCount);
                var wait = System.Diagnostics.Stopwatch.StartNew(); Image lastImage = null; int imageUpdates = 0;
                while (wait.ElapsedMilliseconds < 7000) {
                    Application.DoEvents(); Image current = LiveImage(host);
                    if (current != null && !Object.ReferenceEquals(lastImage, current)) { lastImage = current; imageUpdates++; }
                    Thread.Sleep(20);
                }
                Console.WriteLine("Live image updates=" + imageUpdates);
                if (imageUpdates < 2) throw new InvalidOperationException("SDK 已加载，但未确认持续收到摄像头画面，请在实际窗体中检查摄像头。");
                face.Unload(); Console.WriteLine("SDK unloaded successfully.");
                // 管理窗体关闭后，共享 SDK 控件必须仍能回到主窗口使用。
                var admin = repo.Login("admin", "Admin@123");
                if (admin == null) throw new InvalidOperationException("设备生命周期检查需使用初始化的管理账户。");
                using (var manage = new ManageForm(repo, admin, face, false))
                {
                    manage.StartPosition = FormStartPosition.Manual; manage.Location = new Point(-3000, -3000); manage.ShowInTaskbar = false;
                    manage.Show(); Application.DoEvents(); manage.Close();
                }
                face.Attach(host); face.Unload(); Console.WriteLine("SDK reload succeeded after management form disposal.");
                host.Close();
            }
        }
        private static Image LiveImage(Control root)
        {
            var box = root as PictureBox; if (box != null && box.Image != null) return box.Image;
            foreach (Control child in root.Controls) { Image image = LiveImage(child); if (image != null) return image; }
            return null;
        }
    }
}
