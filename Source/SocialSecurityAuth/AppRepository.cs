using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SocialSecurityAuth
{
    /// <summary>统一处理数据库操作、管理员登录校验和事务。</summary>
    public sealed class AppRepository
    {
        private readonly string connectionString;
        public AppRepository(string connectionString) { this.connectionString = connectionString; }
        private SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        // 通用 SQL 工具：参数、命令、数据表查询、单值查询和增删改。
        private static SqlParameter Parameter(string name, object value) { return new SqlParameter(name, value ?? DBNull.Value); }
        private static SqlCommand CreateCommand(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        {
            var cmd = new SqlCommand(sql, c, tx); cmd.CommandTimeout = 10;
            cmd.Parameters.AddRange(parameters); return cmd;
        }
        private static DataTable QueryTable(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        {
            using (var cmd = CreateCommand(c, tx, sql, parameters))
            using (var adapter = new SqlDataAdapter(cmd)) { var result = new DataTable(); adapter.Fill(result); return result; }
        }
        private static object GetValue(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        { using (var cmd = CreateCommand(c, tx, sql, parameters)) return cmd.ExecuteScalar(); }
        private static int ExecuteSql(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        { using (var cmd = CreateCommand(c, tx, sql, parameters)) return cmd.ExecuteNonQuery(); }
        private static string Required(string value, string label, int max)
        {
            value = (value ?? "").Trim();
            if (value.Length == 0 || value.Length > max) throw new ArgumentException(label + "不能为空，且不能超过 " + max + " 个字符。");
            return value;
        }
        // 单管理员方案：管理操作只检查已登录，并确认账号仍存在。
        private static void RequireAdmin(SqlConnection c, SqlTransaction tx, AdminSession actor)
        {
            if (actor == null) throw new UnauthorizedAccessException("请先登录管理账户。");
            object id = GetValue(c, tx, "SELECT AdminId FROM dbo.Admin WHERE AdminId=@id", Parameter("@id", actor.AdminId));
            if (id == null) throw new UnauthorizedAccessException("管理员账号已不存在，请重新登录。");
        }
        // 仅在注册人员、删除人员、清空领取记录的事务成功时保存日志。
        private static void Log(SqlConnection c, SqlTransaction tx, int adminId, string content)
        {
            ExecuteSql(c, tx, "INSERT dbo.OperationLog(AdminId,OperationContent,Result) VALUES(@id,@content,N'成功')",
                Parameter("@id", adminId), Parameter("@content", content.Length > 200 ? content.Substring(0, 200) : content));
        }
        // 三种管理写入（保存人员、删除人员、清空记录）都返回整数。
        private int RunTransaction(AdminSession actor, Func<SqlConnection, SqlTransaction, int> work)
        {
            try
            {
                using (var c = OpenConnection())
                using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
                {
                    RequireAdmin(c, tx, actor);
                    int result = work(c, tx);
                    tx.Commit();
                    return result;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627) throw new InvalidOperationException("编号或账号已存在，请使用其他值。", ex);
                if (ex.Number == 547) throw new InvalidOperationException("数据有关联记录或不符合要求，未保存本次修改。", ex);
                throw;
            }
        }
        private DataTable QueryForAdmin(AdminSession actor, string sql, params SqlParameter[] parameters)
        {
            using (var c = OpenConnection())
            {
                RequireAdmin(c, null, actor);
                return QueryTable(c, null, sql, parameters);
            }
        }
        public void CheckConnection() { using (var c = OpenConnection()) GetValue(c, null, "SELECT TOP(1) UnitId FROM dbo.Unit"); }
        public AdminSession Login(string account, string password)
        {
            using (var c = OpenConnection())
            {
                var rows = QueryTable(c, null, "SELECT AdminId,Account,PasswordHash,PasswordSalt FROM dbo.Admin WHERE Account=@account",
                    Parameter("@account", (account ?? "").Trim()));
                if (rows.Rows.Count == 0) return null;
                DataRow row = rows.Rows[0];
                if (!PasswordHasher.Verify(password, (byte[])row["PasswordSalt"], (byte[])row["PasswordHash"])) return null;
                return new AdminSession { AdminId = (int)row["AdminId"], Account = (string)row["Account"] };
            }
        }
        public DataTable GetUnits() { using (var c = OpenConnection()) return QueryTable(c, null, "SELECT UnitId,UnitName FROM dbo.Unit ORDER BY UnitName"); }
        private const string PeopleSelect = @"SELECT p.PersonId,p.SocialNo,p.PersonName,p.Gender,p.UnitId,u.UnitName,
            CAST(CASE WHEN f.PersonId IS NULL THEN 0 ELSE 1 END AS bit) HasFace,
            CASE WHEN p.LastAuthTime IS NULL THEN N'未认证' WHEN p.LastAuthPassed=1 THEN N'已通过' ELSE N'未通过' END AuthStatus,p.CreatedAt
            FROM dbo.Person p JOIN dbo.Unit u ON p.UnitId=u.UnitId
            LEFT JOIN dbo.FaceTemplate f ON p.PersonId=f.PersonId ";
        public DataTable GetPeople(AdminSession actor, int? unitId)
        { return QueryForAdmin(actor, PeopleSelect + " WHERE (@unit IS NULL OR p.UnitId=@unit) ORDER BY p.PersonId", Parameter("@unit", unitId)); }
        public DataRow GetPerson(int personId)
        {
            using (var c = OpenConnection()) { var t = QueryTable(c, null, PeopleSelect + " WHERE p.PersonId=@id", Parameter("@id", personId)); return t.Rows.Count == 0 ? null : t.Rows[0]; }
        }
        public int SavePerson(AdminSession actor, PersonInput input, byte[] faceTemplate = null)
        {
            if (input == null) throw new ArgumentNullException("input");
            string no = Required(input.SocialNo, "社保编号", 30), name = Required(input.Name, "姓名", 50), unit = Required(input.UnitName, "所属单位", 100);
            if (input.Gender != "男" && input.Gender != "女") throw new ArgumentException("请选择性别。");
            if (faceTemplate != null && (faceTemplate.Length == 0 || faceTemplate.Length > 1048576)) throw new ArgumentException("人脸模板无效，未保存人员信息。");
            return RunTransaction(actor, (c, tx) => {
                // 已有档案的编号不可被下一位人员的输入替换。
                if (input.PersonId != 0)
                {
                    object originalNo = GetValue(c, tx, "SELECT SocialNo FROM dbo.Person WHERE PersonId=@id", Parameter("@id", input.PersonId));
                    if (originalNo == null) throw new InvalidOperationException("人员已不存在，请刷新列表。");
                    if (!String.Equals((string)originalNo, no, StringComparison.Ordinal))
                        throw new InvalidOperationException("已有人员的社保编号不能修改。登记另一位人员请先点击“新增”。");
                }
                object found = GetValue(c, tx, "SELECT UnitId FROM dbo.Unit WHERE UnitName=@unit", Parameter("@unit", unit));
                int unitId = found == null ? Convert.ToInt32(GetValue(c, tx, "INSERT dbo.Unit(UnitName) VALUES(@unit); SELECT CAST(SCOPE_IDENTITY() AS int)", Parameter("@unit", unit))) : Convert.ToInt32(found);
                int id = input.PersonId;
                if (id == 0) id = Convert.ToInt32(GetValue(c, tx, "INSERT dbo.Person(SocialNo,PersonName,Gender,UnitId) VALUES(@no,@name,@sex,@unit); SELECT CAST(SCOPE_IDENTITY() AS int)", Parameter("@no", no), Parameter("@name", name), Parameter("@sex", input.Gender), Parameter("@unit", unitId)));
                else if (ExecuteSql(c, tx, "UPDATE dbo.Person SET PersonName=@name,Gender=@sex,UnitId=@unit WHERE PersonId=@id", Parameter("@name", name), Parameter("@sex", input.Gender), Parameter("@unit", unitId), Parameter("@id", id)) != 1) throw new InvalidOperationException("人员已不存在，请刷新列表。");
                if (faceTemplate != null)
                {
                    var code = new SqlParameter("@code", SqlDbType.VarBinary, -1) { Value = faceTemplate };
                    ExecuteSql(c, tx, "IF EXISTS(SELECT 1 FROM dbo.FaceTemplate WHERE PersonId=@id) UPDATE dbo.FaceTemplate SET FaceCode=@code,EnrolledAt=SYSDATETIME() WHERE PersonId=@id ELSE INSERT dbo.FaceTemplate(PersonId,FaceCode) VALUES(@id,@code)", Parameter("@id", id), code);
                }
                // 新建人员或采集注册人脸时记录；普通资料修改不记日志。
                if (input.PersonId == 0 || faceTemplate != null) Log(c, tx, actor.AdminId, "注册人员：" + no);
                return id;
            });
        }
        public void DeletePerson(AdminSession actor, int personId)
        {
            RunTransaction(actor, (c, tx) => {
                object no = GetValue(c, tx, "SELECT SocialNo FROM dbo.Person WITH(UPDLOCK,HOLDLOCK) WHERE PersonId=@id", Parameter("@id", personId));
                if (no == null) throw new InvalidOperationException("人员已不存在。");
                // 在同一事务中保存历史身份并解除关联，随后删除档案与模板。
                ExecuteSql(c, tx, @"UPDATE ar SET ArchivedPersonName=p.PersonName,
                    ArchivedUnitId=p.UnitId,ArchivedUnitName=u.UnitName,PersonId=NULL
                    FROM dbo.AuthenticationRecord ar JOIN dbo.Person p ON ar.PersonId=p.PersonId
                    JOIN dbo.Unit u ON p.UnitId=u.UnitId WHERE p.PersonId=@id", Parameter("@id", personId));
                ExecuteSql(c, tx, "DELETE dbo.FaceTemplate WHERE PersonId=@id; DELETE dbo.Person WHERE PersonId=@id", Parameter("@id", personId));
                Log(c, tx, actor.AdminId, "删除人员：" + no); return 0;
            });
        }
        public List<FaceEntry> GetFaces()
        {
            using (var c = OpenConnection())
            {
                var t = QueryTable(c, null, "SELECT PersonId,FaceCode FROM dbo.FaceTemplate ORDER BY PersonId"); var list = new List<FaceEntry>();
                foreach (DataRow r in t.Rows) list.Add(new FaceEntry { PersonId = (int)r["PersonId"], Code = (byte[])r["FaceCode"] }); return list;
            }
        }
        public long RecordAuthentication(int? personId, bool passed, string failureReason, out bool claimRecorded)
        {
            claimRecorded = false;
            if (passed && !personId.HasValue) throw new ArgumentException("认证通过必须关联已登记人员。");
            if (!passed) Required(failureReason, "失败说明", 200);
            if (!personId.HasValue) return 0; // 未知人员失败只提示，不新增领取记录。
            using (var c = OpenConnection())
            using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
            {
                object found = GetValue(c, tx, "SELECT SocialNo FROM dbo.Person WITH(UPDLOCK,HOLDLOCK) WHERE PersonId=@id", Parameter("@id", personId));
                if (found == null) throw new InvalidOperationException("人员已删除，请重新加载人脸库。");
                string socialNo = (string)found;
                ExecuteSql(c, tx, "UPDATE dbo.Person SET LastAuthTime=SYSDATETIME(),LastAuthPassed=@passed WHERE PersonId=@id", Parameter("@passed", passed), Parameter("@id", personId));
                if (!passed) { tx.Commit(); return 0; }
                object existing = GetValue(c, tx, "SELECT RecordId FROM dbo.AuthenticationRecord WHERE ClaimSocialNo=@no", Parameter("@no", socialNo));
                if (existing != null)
                {
                    tx.Commit(); return Convert.ToInt64(existing);
                }
                long id = Convert.ToInt64(GetValue(c, tx, @"INSERT dbo.AuthenticationRecord(PersonId,ClaimSocialNo,ClaimTime)
                    VALUES(@id,@no,SYSDATETIME());
                    SELECT CAST(SCOPE_IDENTITY() AS bigint)", Parameter("@id", personId), Parameter("@no", socialNo)));
                tx.Commit(); claimRecorded = true; return id;
            }
        }
        private const string RecordColumns = @"ar.RecordId,ar.ClaimSocialNo SocialNo,
            COALESCE(p.PersonName,ar.ArchivedPersonName) PersonName,
            COALESCE(u.UnitName,ar.ArchivedUnitName) UnitName,ar.ClaimTime
            FROM dbo.AuthenticationRecord ar LEFT JOIN dbo.Person p ON ar.PersonId=p.PersonId LEFT JOIN dbo.Unit u ON p.UnitId=u.UnitId ";
        private const string RecordOrder = " ORDER BY ar.ClaimTime DESC,ar.RecordId DESC";
        public DataTable GetRecords(AdminSession actor, int? unitId)
        { return QueryForAdmin(actor, "SELECT " + RecordColumns + "WHERE (@unit IS NULL OR COALESCE(p.UnitId,ar.ArchivedUnitId)=@unit)" + RecordOrder, Parameter("@unit", unitId)); }
        public DataTable GetRecentRecords()
        { using (var c = OpenConnection()) return QueryTable(c, null, "SELECT TOP(8) " + RecordColumns + RecordOrder); }
        public int ClearRecords(AdminSession actor)
        {
            return RunTransaction(actor, (c, tx) => {
                // 与认证保存保持先锁人员、后锁记录的顺序。
                ExecuteSql(c, tx, "UPDATE dbo.Person WITH(TABLOCKX) SET LastAuthTime=NULL,LastAuthPassed=NULL");
                int count = ExecuteSql(c, tx, "DELETE FROM dbo.AuthenticationRecord");
                Log(c, tx, actor.AdminId, "清空认证领取记录：" + count + " 条，重置认证状态"); return count;
            });
        }
        public DataTable GetStatisticsSnapshot(AdminSession actor, int? unitId)
        { return QueryForAdmin(actor, PeopleSelect + "WHERE (@unit IS NULL OR p.UnitId=@unit) ORDER BY p.PersonId", Parameter("@unit", unitId)); }
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
        public DataTable GetLogs(AdminSession actor)
        { return QueryForAdmin(actor, "SELECT l.LogId,l.OperationTime,a.Account,l.OperationContent,l.Result FROM dbo.OperationLog l JOIN dbo.Admin a ON l.AdminId=a.AdminId ORDER BY l.OperationTime DESC,l.LogId DESC"); }
    }
}
