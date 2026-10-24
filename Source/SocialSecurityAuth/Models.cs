using System;
namespace SocialSecurityAuth
{
    [Flags]
    public enum Permission { None = 0, People = 1, Records = 2, Statistics = 4, Accounts = 8, Logs = 16, All = 31 }
    public sealed class AdminSession
    {
        public int AdminId { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public Permission Permissions { get; set; }
        public bool Can(Permission permission) { return (Permissions & permission) == permission; }
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
