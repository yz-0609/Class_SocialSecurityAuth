using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SocialSecurityAuth
{
    public static class Ui
    {
        public static void FormatGrid(DataGridView grid)
        {
            var headers = new Dictionary<string, string> {
                {"SocialNo","社保编号"},{"PersonName","姓名"},{"Gender","性别"},{"UnitName","所属单位"},{"HasFace","已采集人脸"},{"AuthStatus","认证状态"},
                {"CreatedAt","登记时间"},{"ClaimTime","领取时间"},
                {"Account","账号"},{"OperationTime","操作时间"},{"OperationContent","操作内容"},{"Result","结果"}
            };
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Name == "PersonId" || col.Name == "UnitId" || col.Name == "RecordId" || col.Name == "LogId" || col.Name == "AdminId") { col.Visible = false; continue; }
                string title; if (headers.TryGetValue(col.Name, out title)) col.HeaderText = title;
                if (col.ValueType == typeof(DateTime)) { col.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"; col.FillWeight = 160; col.MinimumWidth = 148; }
                else col.MinimumWidth = 70;
            }
        }
        public sealed class UnitOption
        {
            public int? Id { get; set; } public string Name { get; set; }
            public override string ToString() { return Name; }
        }
        // 三个查询下拉框都包含“全部”，不再传递额外的开关参数。
        public static void FillUnits(ComboBox combo, DataTable units)
        {
            int? selected = SelectedUnit(combo); combo.Items.Clear();
            combo.Items.Add(new UnitOption { Name = "全部", Id = null });
            foreach (DataRow r in units.Rows) combo.Items.Add(new UnitOption { Name = (string)r["UnitName"], Id = (int)r["UnitId"] });
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
            for (int i = 0; i < combo.Items.Count; i++) if (((UnitOption)combo.Items[i]).Id == selected) combo.SelectedIndex = i;
        }
        public static int? SelectedUnit(ComboBox combo) { var item = combo.SelectedItem as UnitOption; return item == null ? null : item.Id; }
        public static string ErrorText(Exception ex)
        {
            if (ex is SqlException) return "数据库操作失败，请检查 SQL Server 服务、连接配置和数据关联。\r\n" + ex.Message;
            if (ex is DllNotFoundException || ex is BadImageFormatException) return "人脸 SDK 或其运行依赖未能加载，请使用 x86 程序并保留完整 DLL。\r\n" + ex.Message;
            return ex.Message;
        }
        public static void Error(IWin32Window owner, Exception ex) { MessageBox.Show(owner, ErrorText(ex), "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
