using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SocialSecurityAuth
{
    public static class Ui
    {
        public static readonly Color Blue = Color.FromArgb(50, 181, 255);
        public static readonly Color PanelColor = Color.FromArgb(226, 254, 255);
        public static readonly Font BodyFont = new Font("宋体", 10.5F);
        public static Button Button(string text, EventHandler click)
        { var b = new Button { Text = text, AutoSize = true, MinimumSize = new Size(85, 34), Margin = new Padding(5), UseVisualStyleBackColor = true }; if (click != null) b.Click += click; return b; }
        public static Label Label(string text)
        { return new Label { Text = text, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(5, 8, 5, 8) }; }
        public static FlowLayoutPanel Buttons(params Control[] controls)
        {
            var p = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 4, 0, 2) }; p.Controls.AddRange(controls); return p;
        }
        public static ComboBox FilterCombo() { return new ComboBox { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(3, 7, 8, 3) }; }
        public static DataGridView Grid()
        {
            var g = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false,
                MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White, RowHeadersVisible = false, BorderStyle = BorderStyle.FixedSingle, AutoGenerateColumns = true };
            g.RowTemplate.Height = 28; g.ColumnHeadersHeight = 32;
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(196, 228, 251); g.DefaultCellStyle.SelectionForeColor = Color.Black;
            g.DataBindingComplete += (s, e) => FormatGrid(g);
            return g;
        }
        public static void FormatGrid(DataGridView grid)
        {
            var headers = new Dictionary<string, string> {
                {"SocialNo","社保编号"},{"PersonName","姓名"},{"Gender","性别"},{"UnitName","所属单位"},{"HasFace","已采集人脸"},{"AuthStatus","认证状态"},
                {"CreatedAt","登记时间"},{"AuthTime","认证时间"},{"AuthResult","认证结果"},{"ClaimText","领取信息"},{"ClaimTime","领取时间"},{"FailureReason","失败说明"},
                {"Account","账号"},{"AdminName","姓名"},{"PermissionFlags","权限值"},{"OperationTime","操作时间"},{"OperationContent","操作内容"},{"Result","结果"}
            };
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Name == "PersonId" || col.Name == "UnitId" || col.Name == "RecordId" || col.Name == "LogId" || col.Name == "AdminId" || col.Name == "PermissionFlags") { col.Visible = false; continue; }
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
        public static void FillUnits(ComboBox combo, DataTable units, bool all)
        {
            int? selected = SelectedUnit(combo); combo.Items.Clear();
            if (all) combo.Items.Add(new UnitOption { Name = "全部", Id = null });
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
        public static Label CameraPlaceholder()
        { return new Label { Dock = DockStyle.Fill, Text = "摄像头尚未连接", TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.WhiteSmoke, ForeColor = Color.DimGray }; }
    }
}
