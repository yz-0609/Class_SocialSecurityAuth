using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SocialSecurityAuth
{
    /// <summary>所有修改连同操作日志在同一事务提交；权限在数据层再次核验。</summary>
    public sealed class AppRepository
    {
        private readonly string connectionString;
        public AppRepository(string connectionString) { this.connectionString = connectionString; }
        private SqlConnection Open() { var c = new SqlConnection(connectionString); c.Open(); return c; }
        private static SqlParameter P(string name, object value) { return new SqlParameter(name, value ?? DBNull.Value); }
        private static SqlCommand Command(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        {
            var cmd = new SqlCommand(sql, c, tx); cmd.CommandTimeout = 10;
            cmd.Parameters.AddRange(parameters); return cmd;
        }
        private static DataTable Query(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        {
            using (var cmd = Command(c, tx, sql, parameters))
            using (var adapter = new SqlDataAdapter(cmd)) { var result = new DataTable(); adapter.Fill(result); return result; }
        }
        private static object Scalar(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        { using (var cmd = Command(c, tx, sql, parameters)) return cmd.ExecuteScalar(); }
        private static int Execute(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        { using (var cmd = Command(c, tx, sql, parameters)) return cmd.ExecuteNonQuery(); }
        private static string Required(string value, string label, int max)
        {
            value = (value ?? "").Trim();
            if (value.Length == 0 || value.Length > max) throw new ArgumentException(label + "不能为空，且不能超过 " + max + " 个字符。");
            return value;
        }
        private static void Demand(SqlConnection c, SqlTransaction tx, AdminSession actor, Permission permission)
        {
            if (actor == null) throw new UnauthorizedAccessException("请先登录管理账户。");
            object current = Scalar(c, tx, "SELECT PermissionFlags FROM dbo.Admin WHERE AdminId=@id", P("@id", actor.AdminId));
            if (current == null || (((Permission)Convert.ToInt32(current)) & permission) != permission)
                throw new UnauthorizedAccessException("当前账户没有此操作的权限，请重新登录或联系管理员。");
        }
        private static void Log(SqlConnection c, SqlTransaction tx, int adminId, string content, string result = "成功")
        {
            Execute(c, tx, "INSERT dbo.OperationLog(AdminId,OperationContent,Result) VALUES(@id,@content,@result)",
                P("@id", adminId), P("@content", content.Length > 200 ? content.Substring(0, 200) : content), P("@result", result));
        }
        private T Write<T>(AdminSession actor, Permission permission, Func<SqlConnection, SqlTransaction, T> work)
        {
            try
            {
                using (var c = Open()) using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
                { Demand(c, tx, actor, permission); T result = work(c, tx); tx.Commit(); return result; }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627) throw new InvalidOperationException("编号或账号已存在，请使用其他值。", ex);
                if (ex.Number == 547) throw new InvalidOperationException("数据有关联记录或不符合要求，未保存本次修改。", ex);
                throw;
            }
        }
        private DataTable Read(AdminSession actor, Permission permission, string sql, params SqlParameter[] parameters)
        { using (var c = Open()) { Demand(c, null, actor, permission); return Query(c, null, sql, parameters); } }
        public void CheckConnection() { using (var c = Open()) Scalar(c, null, "SELECT TOP(1) UnitId FROM dbo.Unit"); }
        public AdminSession Login(string account, string password)
        {
            using (var c = Open())
            {
                var rows = Query(c, null, "SELECT AdminId,Account,AdminName,PermissionFlags,PasswordHash,PasswordSalt FROM dbo.Admin WHERE Account=@account",
                    P("@account", (account ?? "").Trim()));
                if (rows.Rows.Count == 0) return null;
                DataRow row = rows.Rows[0];
                if (!PasswordHasher.Verify(password, (byte[])row["PasswordSalt"], (byte[])row["PasswordHash"])) return null;
                var session = new AdminSession { AdminId = (int)row["AdminId"], Account = (string)row["Account"], Name = (string)row["AdminName"], Permissions = (Permission)(int)row["PermissionFlags"] };
                Log(c, null, session.AdminId, "登录管理界面"); return session;
            }
        }
        public DataTable GetUnits() { using (var c = Open()) return Query(c, null, "SELECT UnitId,UnitName FROM dbo.Unit ORDER BY UnitName"); }
        private const string PeopleSelect = @"SELECT p.PersonId,p.SocialNo,p.PersonName,p.Gender,p.UnitId,u.UnitName,
            CAST(CASE WHEN f.PersonId IS NULL THEN 0 ELSE 1 END AS bit) HasFace,
            CASE WHEN p.LastAuthTime IS NULL THEN N'未认证' WHEN p.LastAuthPassed=1 THEN N'已通过' ELSE N'未通过' END AuthStatus,p.CreatedAt
            FROM dbo.Person p JOIN dbo.Unit u ON p.UnitId=u.UnitId
            LEFT JOIN dbo.FaceTemplate f ON p.PersonId=f.PersonId ";
        public DataTable GetPeople(AdminSession actor, int? unitId)
        { return Read(actor, Permission.People, PeopleSelect + " WHERE (@unit IS NULL OR p.UnitId=@unit) ORDER BY p.PersonId", P("@unit", unitId)); }
        public DataRow GetPerson(int personId)
        {
            using (var c = Open()) { var t = Query(c, null, PeopleSelect + " WHERE p.PersonId=@id", P("@id", personId)); return t.Rows.Count == 0 ? null : t.Rows[0]; }
        }
        public int SavePerson(AdminSession actor, PersonInput input, byte[] faceTemplate = null)
        {
            if (input == null) throw new ArgumentNullException("input");
            string no = Required(input.SocialNo, "社保编号", 30), name = Required(input.Name, "姓名", 50), unit = Required(input.UnitName, "所属单位", 100);
            if (input.Gender != "男" && input.Gender != "女") throw new ArgumentException("请选择性别。");
            if (faceTemplate != null && (faceTemplate.Length == 0 || faceTemplate.Length > 1048576)) throw new ArgumentException("人脸模板无效，未保存人员信息。");
            return Write(actor, Permission.People, (c, tx) => {
                // 已有档案的编号不可被下一位人员的输入替换。
                if (input.PersonId != 0)
                {
                    object originalNo = Scalar(c, tx, "SELECT SocialNo FROM dbo.Person WHERE PersonId=@id", P("@id", input.PersonId));
                    if (originalNo == null) throw new InvalidOperationException("人员已不存在，请刷新列表。");
                    if (!String.Equals((string)originalNo, no, StringComparison.Ordinal))
                        throw new InvalidOperationException("已有人员的社保编号不能修改。登记另一位人员请先点击“新增”。");
                }
                object found = Scalar(c, tx, "SELECT UnitId FROM dbo.Unit WHERE UnitName=@unit", P("@unit", unit));
                int unitId = found == null ? Convert.ToInt32(Scalar(c, tx, "INSERT dbo.Unit(UnitName) VALUES(@unit); SELECT CAST(SCOPE_IDENTITY() AS int)", P("@unit", unit))) : Convert.ToInt32(found);
                int id = input.PersonId;
                if (id == 0) id = Convert.ToInt32(Scalar(c, tx, "INSERT dbo.Person(SocialNo,PersonName,Gender,UnitId) VALUES(@no,@name,@sex,@unit); SELECT CAST(SCOPE_IDENTITY() AS int)", P("@no", no), P("@name", name), P("@sex", input.Gender), P("@unit", unitId)));
                else if (Execute(c, tx, "UPDATE dbo.Person SET PersonName=@name,Gender=@sex,UnitId=@unit WHERE PersonId=@id", P("@name", name), P("@sex", input.Gender), P("@unit", unitId), P("@id", id)) != 1) throw new InvalidOperationException("人员已不存在，请刷新列表。");
                if (faceTemplate != null)
                {
                    var code = new SqlParameter("@code", SqlDbType.VarBinary, -1) { Value = faceTemplate };
                    Execute(c, tx, "IF EXISTS(SELECT 1 FROM dbo.FaceTemplate WHERE PersonId=@id) UPDATE dbo.FaceTemplate SET FaceCode=@code,EnrolledAt=SYSDATETIME() WHERE PersonId=@id ELSE INSERT dbo.FaceTemplate(PersonId,FaceCode) VALUES(@id,@code)", P("@id", id), code);
                }
                Log(c, tx, actor.AdminId, (faceTemplate == null ? "保存人员：" : "人员与人脸注册：") + no); return id;
            });
        }
        public void DeletePerson(AdminSession actor, int personId)
        {
            Write(actor, Permission.People, (c, tx) => {
                object no = Scalar(c, tx, "SELECT SocialNo FROM dbo.Person WITH(UPDLOCK,HOLDLOCK) WHERE PersonId=@id", P("@id", personId));
                if (no == null) throw new InvalidOperationException("人员已不存在。");
                // 在同一事务中保存历史身份并解除关联，随后删除档案与模板。
                Execute(c, tx, @"UPDATE ar SET ArchivedSocialNo=p.SocialNo,ArchivedPersonName=p.PersonName,
                    ArchivedUnitId=p.UnitId,ArchivedUnitName=u.UnitName,PersonId=NULL
                    FROM dbo.AuthenticationRecord ar JOIN dbo.Person p ON ar.PersonId=p.PersonId
                    JOIN dbo.Unit u ON p.UnitId=u.UnitId WHERE p.PersonId=@id", P("@id", personId));
                Execute(c, tx, "DELETE dbo.FaceTemplate WHERE PersonId=@id; DELETE dbo.Person WHERE PersonId=@id", P("@id", personId));
                Log(c, tx, actor.AdminId, "删除人员：" + no); return 0;
            });
        }
        public List<FaceEntry> GetFaces()
        {
            using (var c = Open())
            {
                var t = Query(c, null, "SELECT PersonId,FaceCode FROM dbo.FaceTemplate ORDER BY PersonId"); var list = new List<FaceEntry>();
                foreach (DataRow r in t.Rows) list.Add(new FaceEntry { PersonId = (int)r["PersonId"], Code = (byte[])r["FaceCode"] }); return list;
            }
        }
        public long RecordAuthentication(int? personId, bool passed, string failureReason)
        { bool claimRecorded; return RecordAuthentication(personId, passed, failureReason, out claimRecorded); }
        public long RecordAuthentication(int? personId, bool passed, string failureReason, out bool claimRecorded)
        {
            claimRecorded = false;
            if (passed && !personId.HasValue) throw new ArgumentException("认证通过必须关联已登记人员。");
            if (!passed) Required(failureReason, "失败说明", 200);
            if (!personId.HasValue) return 0; // 未知人员失败只提示，不新增领取记录。
            using (var c = Open())
            using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
            {
                object found = Scalar(c, tx, "SELECT SocialNo FROM dbo.Person WITH(UPDLOCK,HOLDLOCK) WHERE PersonId=@id", P("@id", personId));
                if (found == null) throw new InvalidOperationException("人员已删除，请重新加载人脸库。");
                string socialNo = (string)found;
                Execute(c, tx, "UPDATE dbo.Person SET LastAuthTime=SYSDATETIME(),LastAuthPassed=@passed WHERE PersonId=@id", P("@passed", passed), P("@id", personId));
                if (!passed) { tx.Commit(); return 0; }
                object existing = Scalar(c, tx, "SELECT RecordId FROM dbo.AuthenticationRecord WHERE ClaimSocialNo=@no", P("@no", socialNo));
                if (existing != null)
                {
                    tx.Commit(); return Convert.ToInt64(existing);
                }
                long id = Convert.ToInt64(Scalar(c, tx, @"DECLARE @now datetime2=SYSDATETIME();
                    INSERT dbo.AuthenticationRecord(PersonId,AuthTime,Passed,FailureReason,ClaimRecorded,ClaimTime,ClaimSocialNo)
                    VALUES(@id,@now,1,NULL,1,@now,@no);
                    SELECT CAST(SCOPE_IDENTITY() AS bigint)", P("@id", personId), P("@no", socialNo)));
                tx.Commit(); claimRecorded = true; return id;
            }
        }
        private const string RecordSelect = @"SELECT ar.RecordId,COALESCE(p.SocialNo,ar.ArchivedSocialNo,N'—') SocialNo,
            COALESCE(p.PersonName,ar.ArchivedPersonName,N'未知人员') PersonName,COALESCE(u.UnitName,ar.ArchivedUnitName,N'—') UnitName,ar.AuthTime,
            CASE WHEN ar.Passed=1 THEN N'通过' ELSE N'未通过' END AuthResult,
            CASE WHEN ar.ClaimRecorded=1 THEN N'已登记' WHEN ar.Passed=1 THEN N'已领取（本次未重复登记）' ELSE N'未登记' END ClaimText,ar.ClaimTime,ar.FailureReason
            FROM dbo.AuthenticationRecord ar LEFT JOIN dbo.Person p ON ar.PersonId=p.PersonId LEFT JOIN dbo.Unit u ON p.UnitId=u.UnitId ";
        public DataTable GetRecords(AdminSession actor, int? unitId)
        { return Read(actor, Permission.Records, RecordSelect + "WHERE ar.Passed=1 AND ar.ClaimRecorded=1 AND (@unit IS NULL OR COALESCE(p.UnitId,ar.ArchivedUnitId)=@unit) ORDER BY ar.AuthTime DESC,ar.RecordId DESC", P("@unit", unitId)); }
        public DataTable GetRecentRecords()
        { using (var c = Open()) return Query(c, null, RecordSelect.Replace("SELECT ar.RecordId", "SELECT TOP(8) ar.RecordId") + "WHERE ar.Passed=1 AND ar.ClaimRecorded=1 ORDER BY ar.AuthTime DESC,ar.RecordId DESC"); }
        public int ClearRecords(AdminSession actor)
        {
            return Write(actor, Permission.Records, (c, tx) => {
                // 与认证保存保持先锁人员、后锁记录的顺序。
                Execute(c, tx, "UPDATE dbo.Person WITH(TABLOCKX) SET LastAuthTime=NULL,LastAuthPassed=NULL");
                int count = Execute(c, tx, "DELETE FROM dbo.AuthenticationRecord");
                Log(c, tx, actor.AdminId, "清空认证领取记录：" + count + " 条，重置认证状态"); return count;
            });
        }
        public DataTable GetStatistics(AdminSession actor, int? unitId, StatisticsKind kind)
        {
            return FilterStatistics(GetStatisticsSnapshot(actor, unitId), kind);
        }
        public DataTable GetStatisticsSnapshot(AdminSession actor, int? unitId)
        { return Read(actor, Permission.Statistics, PeopleSelect + "WHERE (@unit IS NULL OR p.UnitId=@unit) ORDER BY p.PersonId", P("@unit", unitId)); }
        public static DataTable FilterStatistics(DataTable snapshot, StatisticsKind kind)
        {
            string filter;
            switch (kind) { case StatisticsKind.Collected: filter = "HasFace = true"; break; case StatisticsKind.Passed: filter = "AuthStatus = '已通过'"; break; case StatisticsKind.Failed: filter = "AuthStatus = '未通过'"; break; default: filter = "AuthStatus = '未认证'"; break; }
            using (var view = new DataView(snapshot)) { view.RowFilter = filter; return view.ToTable(); }
        }
        public static StatisticsSummary Summarize(DataTable snapshot)
        {
            return new StatisticsSummary { Collected = snapshot.Select("HasFace = true").Length, Passed = snapshot.Select("AuthStatus = '已通过'").Length,
                Failed = snapshot.Select("AuthStatus = '未通过'").Length, Uncertified = snapshot.Select("AuthStatus = '未认证'").Length };
        }
        public StatisticsSummary GetSummary(AdminSession actor, int? unitId)
        {
            return Summarize(GetStatisticsSnapshot(actor, unitId));
        }
        public DataTable GetAdmins(AdminSession actor)
        { return Read(actor, Permission.Accounts, "SELECT AdminId,Account,AdminName,PermissionFlags FROM dbo.Admin ORDER BY AdminId"); }
        public int SaveAdmin(AdminSession actor, int id, string account, string name, string password, Permission permissions)
        {
            account = Required(account, "账号", 30); name = Required(name, "管理人员姓名", 50);
            if (id == 0 || !String.IsNullOrEmpty(password)) { if (String.IsNullOrEmpty(password) || password.Length < 6 || password.Length > 100) throw new ArgumentException("密码长度须为 6 至 100 个字符。"); }
            if (((int)permissions & ~31) != 0) throw new ArgumentException("权限设置无效。");
            return Write(actor, Permission.Accounts, (c, tx) => {
                if (id != 0 && (permissions & Permission.Accounts) == 0) EnsureAnotherManager(c, tx, id);
                byte[] salt = String.IsNullOrEmpty(password) ? null : PasswordHasher.NewSalt();
                byte[] hash = salt == null ? null : PasswordHasher.Hash(password, salt);
                if (id == 0) id = Convert.ToInt32(Scalar(c, tx, "INSERT dbo.Admin(Account,AdminName,PasswordHash,PasswordSalt,PermissionFlags) VALUES(@account,@name,@hash,@salt,@flags); SELECT CAST(SCOPE_IDENTITY() AS int)", P("@account", account), P("@name", name), P("@hash", hash), P("@salt", salt), P("@flags", (int)permissions)));
                else
                {
                    string sql = "UPDATE dbo.Admin SET Account=@account,AdminName=@name,PermissionFlags=@flags" + (salt == null ? "" : ",PasswordHash=@hash,PasswordSalt=@salt") + " WHERE AdminId=@id";
                    using (var cmd = Command(c, tx, sql, P("@account", account), P("@name", name), P("@flags", (int)permissions), P("@id", id)))
                    {
                        if (salt != null) { cmd.Parameters.Add(P("@hash", hash)); cmd.Parameters.Add(P("@salt", salt)); }
                        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("管理账户已不存在。");
                    }
                }
                Log(c, tx, actor.AdminId, "保存管理人员与权限：" + account); return id;
            });
        }
        private static void EnsureAnotherManager(SqlConnection c, SqlTransaction tx, int exceptId)
        {
            int others = Convert.ToInt32(Scalar(c, tx, "SELECT COUNT(*) FROM dbo.Admin WITH(UPDLOCK,HOLDLOCK) WHERE AdminId<>@id AND (PermissionFlags & 8)=8", P("@id", exceptId)));
            if (others == 0) throw new InvalidOperationException("必须保留至少一个具有管理人员与权限设置权限的账户。");
        }
        public void DeleteAdmin(AdminSession actor, int id)
        {
            Write(actor, Permission.Accounts, (c, tx) => {
                if (id == actor.AdminId) throw new InvalidOperationException("不能删除当前登录账户。");
                EnsureAnotherManager(c, tx, id);
                if (Convert.ToInt32(Scalar(c, tx, "SELECT COUNT(*) FROM dbo.OperationLog WHERE AdminId=@id", P("@id", id))) != 0) throw new InvalidOperationException("该账户已有操作记录，请保留以便追溯，可调整其权限。");
                if (Execute(c, tx, "DELETE dbo.Admin WHERE AdminId=@id", P("@id", id)) != 1) throw new InvalidOperationException("管理账户已不存在。");
                Log(c, tx, actor.AdminId, "删除管理人员，编号：" + id); return 0;
            });
        }
        public DataTable GetLogs(AdminSession actor)
        { return Read(actor, Permission.Logs, "SELECT l.LogId,l.OperationTime,a.Account,l.OperationContent,l.Result FROM dbo.OperationLog l JOIN dbo.Admin a ON l.AdminId=a.AdminId ORDER BY l.OperationTime DESC,l.LogId DESC"); }
        public void RecordFailedOperation(AdminSession actor, string description)
        {
            if (actor == null) return;
            using (var c = Open()) Log(c, null, actor.AdminId, description, "失败");
        }
    }
}
