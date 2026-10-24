using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
namespace SocialSecurityAuth
{
    public static class Database
    {
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["SocialSecurityAuth"].ConnectionString; }
        }
        public static void Initialize(string connectionString, string scriptPath)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            string catalog = builder.InitialCatalog;
            if (catalog != "SocialSecurityAuth" && !Regex.IsMatch(catalog, @"^SocialSecurityAuth_Test_[a-f0-9]{32}$"))
                throw new InvalidOperationException("初始化仅允许创建本项目数据库，请检查连接配置。");
            builder.InitialCatalog = "master";
            string sql = File.ReadAllText(scriptPath).Replace("SocialSecurityAuth", catalog);
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                foreach (string batch in Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    if (String.IsNullOrWhiteSpace(batch)) continue;
                    using (var command = new SqlCommand(batch, connection)) { command.CommandTimeout = 30; command.ExecuteNonQuery(); }
                }
            }
            // 仅空账户表初始化，已存在账户不会被覆盖或重设密码。
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                using (var command = new SqlCommand("IF NOT EXISTS(SELECT 1 FROM dbo.Admin WITH(UPDLOCK,HOLDLOCK)) INSERT dbo.Admin(Account,AdminName,PasswordHash,PasswordSalt,PermissionFlags) VALUES(N'admin',N'管理员',@hash,@salt,31)", connection, transaction))
                {
                    byte[] salt = PasswordHasher.NewSalt();
                    command.Parameters.Add("@salt", SqlDbType.VarBinary, 32).Value = salt;
                    command.Parameters.Add("@hash", SqlDbType.VarBinary, 64).Value = PasswordHasher.Hash("Admin@123", salt);
                    command.ExecuteNonQuery(); transaction.Commit();
                }
            }
        }
    }
}
