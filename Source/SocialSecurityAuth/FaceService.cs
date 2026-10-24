using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using FacePlatformCtrl;
using FacePlatformAx;

namespace SocialSecurityAuth
{
    /// <summary>沿用课堂 BioFPCtrl，不替换识别算法；全程序共用一个摄像头控件。</summary>
    public sealed class FaceService : IDisposable
    {
        private enum Mode { Idle, Identifying, Enrolling }
        private readonly AppRepository repository;
        private readonly BioFPCtrl control;
        private readonly Dictionary<int, int> people = new Dictionary<int, int>();
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private long lastCallback = -2000;
        private Mode mode;
        private int generation;
        private int moduleGeneration;
        private bool disposed;
        private bool deviceFault;
        public bool Loaded { get; private set; }
        public bool DeviceAvailable { get { return Loaded && !deviceFault; } }
        public bool Running { get { return mode == Mode.Identifying; } }
        public int TemplateCount { get { return people.Count; } }
        public event EventHandler<IdentificationEventArgs> Identified;
        public event EventHandler<EnrollmentEventArgs> Enrolled;
        public event EventHandler<string> Error;
        public event EventHandler<DeviceStatusEventArgs> StatusChanged;
        public FaceService(AppRepository repository)
        {
            this.repository = repository;
            control = new BioFPCtrl { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke, Name = "bioFPCtrl1" };
            control.IdentifyNotify += OnIdentify;
            control.EnrollNotify += OnEnroll;
            control.StatusNotify += OnStatus;
        }
        private static void Check(int result, string action)
        {
            // SDK 使用非负返回值表示成功，错误码为负数；不能把返回 0 当成失败。
            if (result < 0) throw new InvalidOperationException(action + "失败，SDK 返回 0x" + unchecked((uint)result).ToString("X8") + "。");
        }
        public void Attach(Control host)
        {
            if (disposed) throw new ObjectDisposedException("FaceService");
            Unload();
            host.Controls.Add(control); control.BringToFront();
            // ModuleLoad 使用控件句柄，必须在控件已经归属当前窗体后调用。
            IntPtr handle = control.Handle;
            try
            {
                deviceFault = false; Check(control.ModuleLoad(), "加载人脸模块"); Loaded = true; LoadTemplates();
            }
            catch { Unload(); throw; }
        }
        private void LoadTemplates()
        {
            people.Clear();
            List<FaceEntry> entries = repository.GetFaces();
            if (entries.Count == 0) return; // 全新模块没有模板，不向 SDK 传入空数组。
            int size = control.GetFaceCodeLen();
            if (size <= 0 || size > 1048576) throw new InvalidOperationException("SDK 返回的人脸特征长度无效。");
            byte[] packed = new byte[checked(entries.Count * size)]; uint[] ids = new uint[entries.Count];
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Code.Length != size) throw new InvalidOperationException("人员编号 " + entries[i].PersonId + " 的模板长度与当前 SDK 不一致，请重新采集。");
                Buffer.BlockCopy(entries[i].Code, 0, packed, i * size, size);
                ids[i] = (uint)i; people.Add(i, entries[i].PersonId);
            }
            Check(control.SetFaceCode((uint)entries.Count, packed, ids), "加载人脸模板");
        }
        public bool StartIdentification()
        {
            if (!DeviceAvailable) throw new InvalidOperationException("人脸设备尚未连接或采集异常，请重新连接。");
            Stop();
            if (people.Count == 0) return false;
            mode = Mode.Identifying; lastCallback = clock.ElapsedMilliseconds - 2000;
            try { Check(control.Identify(), "启动人脸识别"); return true; }
            catch { mode = Mode.Idle; throw; }
        }
        public void StartEnrollment()
        {
            if (!DeviceAvailable) throw new InvalidOperationException("人脸设备尚未连接或采集异常，请重新连接。");
            Stop(); mode = Mode.Enrolling;
            try { Check(control.Enroll(), "启动人脸采集"); }
            catch { mode = Mode.Idle; throw; }
        }
        public void Stop()
        {
            generation++; bool wasActive = mode != Mode.Idle; mode = Mode.Idle;
            if (Loaded && wasActive) Check(control.Cancel(), "停止采集或识别");
        }
        public void Unload()
        {
            generation++; moduleGeneration++; mode = Mode.Idle; people.Clear();
            bool wasLoaded = Loaded; Loaded = false;
            try
            {
                if (wasLoaded) { try { control.Cancel(); } finally { Check(control.ModuleUnload(), "卸载人脸模块"); } }
            }
            finally
            {
                // 管理窗体关闭时不能连同共享的 SDK 控件一并 Dispose。
                if (control.Parent != null) control.Parent.Controls.Remove(control);
            }
        }
        private void Post(int token, Action action)
        {
            if (disposed || !control.IsHandleCreated) return;
            try { control.BeginInvoke(new Action(() => { if (!disposed && token == generation) action(); })); }
            catch (InvalidOperationException) { /* 退出时窗口句柄可能已经销毁。 */ }
        }
        private void OnIdentify(object sender, int result, int faceIndex)
        {
            if (mode != Mode.Identifying) return;
            int token = generation;
            if (result <= 0 && result != BioFPCtrl.BioFP_ERROR_IDENTIFY_FAILED) return;
            if (clock.ElapsedMilliseconds - lastCallback < 2000) return;
            lastCallback = clock.ElapsedMilliseconds;
            int person; bool known = people.TryGetValue(faceIndex, out person);
            var args = new IdentificationEventArgs {
                Passed = result > 0 && known,
                PersonId = known ? (int?)person : null,
                Reason = result > 0 && !known ? "识别索引未映射到有效人员，请重新加载人脸库。" : "人脸认证未通过"
            };
            Post(token, () => { var h = Identified; if (h != null) h(this, args); });
        }
        private void OnStatus(object sender, int type, int value)
        {
            if (disposed || !control.IsHandleCreated) return;
            int token = moduleGeneration;
            try
            {
                control.BeginInvoke(new Action(() => {
                    if (disposed || !Loaded || token != moduleGeneration) return;
                    var h = StatusChanged; if (h != null) h(this, new DeviceStatusEventArgs { Type = type, Value = value });
                    bool failed = type == BioFP.BioFP_DEVICE_ERR_OPEN || type == BioFP.BioFP_DEVICE_ERR_INVALID_STATE
                        || type == BioFP.BioFP_DEVICE_EVENT_SURPRISE_REMOVE || value == BioFP.BioFP_ERROR_LICENSE;
                    if (failed && !deviceFault)
                    {
                        deviceFault = true; mode = Mode.Idle; generation++;
                        var error = Error; if (error != null) error(this, "摄像头或 SDK 状态异常，请检查设备后重新连接。（" + type + ", " + value + "）");
                    }
                }));
            }
            catch (InvalidOperationException) { }
        }
        private void OnEnroll(object sender, int result, int faceIndex, uint quality, byte[] template, Bitmap image)
        {
            if (mode != Mode.Enrolling) return;
            int token = generation;
            byte[] copy = template == null ? null : (byte[])template.Clone();
            Post(token, () => {
                mode = Mode.Idle;
                bool passed = result > 0 && copy != null && copy.Length == control.GetFaceCodeLen();
                var args = new EnrollmentEventArgs {
                    Passed = passed, Template = passed ? copy : null,
                    Message = passed ? "人脸采集成功" : result == BioFPCtrl.BioFP_ERROR_ENROLL_DUPLICATE ? "人脸已存在，请选择原有人员，不要重复注册。" : "人脸采集未成功，请调整位置后重试。"
                };
                var h = Enrolled; if (h != null) h(this, args);
            });
        }
        public void Dispose()
        {
            if (disposed) return;
            try { Unload(); }
            catch (Exception ex) { var h = Error; if (h != null) h(this, ex.Message); }
            finally { disposed = true; control.IdentifyNotify -= OnIdentify; control.EnrollNotify -= OnEnroll; control.StatusNotify -= OnStatus; control.Dispose(); }
        }
    }
}
