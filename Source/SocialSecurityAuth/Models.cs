using System;
namespace SocialSecurityAuth
{
    // 登录后只需保留管理员编号和账号，用于身份校验、界面显示和操作日志。
    public sealed class AdminSession
    {
        public int AdminId { get; set; }
        public string Account { get; set; }
    }
    public sealed class PersonInput
    {
        public int PersonId { get; set; }
        public string SocialNo { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string UnitName { get; set; }
    }
    public sealed class FaceEntry
    {
        public int PersonId { get; set; }
        public byte[] Code { get; set; }
    }
    public enum StatisticsKind { Collected, Passed, Failed, Uncertified }
    public sealed class StatisticsSummary
    {
        public int Collected { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Uncertified { get; set; }
    }
    public sealed class IdentificationEventArgs : EventArgs
    {
        public bool Passed { get; set; }
        public int? PersonId { get; set; }
        public string Reason { get; set; }
    }
    public sealed class EnrollmentEventArgs : EventArgs
    {
        public bool Passed { get; set; }
        public byte[] Template { get; set; }
        public string Message { get; set; }
    }
    public sealed class DeviceStatusEventArgs : EventArgs
    {
        public int Type { get; set; }
        public int Value { get; set; }
    }
}
