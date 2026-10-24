-- SQL Server 2016+。可重复执行；保留人员、人脸和首次领取，升级时移除多余认证流水。
IF DB_ID(N'SocialSecurityAuth') IS NULL
    EXEC(N'CREATE DATABASE [SocialSecurityAuth]');
GO
USE [SocialSecurityAuth];
GO
IF OBJECT_ID(N'dbo.Unit',N'U') IS NULL
CREATE TABLE dbo.Unit (
 UnitId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Unit PRIMARY KEY,
 UnitName nvarchar(100) NOT NULL CONSTRAINT UQ_Unit_Name UNIQUE
);
IF OBJECT_ID(N'dbo.Person',N'U') IS NULL
CREATE TABLE dbo.Person (
 PersonId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Person PRIMARY KEY,
 SocialNo nvarchar(30) NOT NULL CONSTRAINT UQ_Person_SocialNo UNIQUE,
 PersonName nvarchar(50) NOT NULL,
 Gender nvarchar(2) NOT NULL CONSTRAINT CK_Person_Gender CHECK(Gender IN (N'男',N'女')),
 UnitId int NOT NULL CONSTRAINT FK_Person_Unit REFERENCES dbo.Unit(UnitId),
 CreatedAt datetime2 NOT NULL CONSTRAINT DF_Person_Time DEFAULT(SYSDATETIME())
);
IF OBJECT_ID(N'dbo.FaceTemplate',N'U') IS NULL
CREATE TABLE dbo.FaceTemplate (
 PersonId int NOT NULL CONSTRAINT PK_FaceTemplate PRIMARY KEY CONSTRAINT FK_Face_Person REFERENCES dbo.Person(PersonId),
 FaceCode varbinary(max) NOT NULL CONSTRAINT CK_Face_NotEmpty CHECK(DATALENGTH(FaceCode)>0),
 EnrolledAt datetime2 NOT NULL CONSTRAINT DF_Face_Time DEFAULT(SYSDATETIME())
);
IF OBJECT_ID(N'dbo.Admin',N'U') IS NULL
CREATE TABLE dbo.Admin (
 AdminId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Admin PRIMARY KEY,
 Account nvarchar(30) NOT NULL CONSTRAINT UQ_Admin_Account UNIQUE,
 AdminName nvarchar(50) NOT NULL,
 PasswordHash varbinary(64) NOT NULL,
 PasswordSalt varbinary(32) NOT NULL,
 PermissionFlags int NOT NULL CONSTRAINT CK_Admin_Permission CHECK(PermissionFlags BETWEEN 0 AND 31)
);
IF OBJECT_ID(N'dbo.AuthenticationRecord',N'U') IS NULL
CREATE TABLE dbo.AuthenticationRecord (
 RecordId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuthenticationRecord PRIMARY KEY,
 PersonId int NULL CONSTRAINT FK_Authentication_Person REFERENCES dbo.Person(PersonId),
 AuthTime datetime2 NOT NULL CONSTRAINT DF_Authentication_Time DEFAULT(SYSDATETIME()),
 Passed bit NOT NULL,
 FailureReason nvarchar(200) NULL,
 ClaimRecorded bit NOT NULL,
 ClaimTime datetime2 NULL,
 ClaimSocialNo nvarchar(30) NULL,
 ArchivedSocialNo nvarchar(30) NULL,
 ArchivedPersonName nvarchar(50) NULL,
 ArchivedUnitId int NULL,
 ArchivedUnitName nvarchar(100) NULL,
 CONSTRAINT CK_Authentication_Result CHECK (
  (Passed=1 AND (PersonId IS NOT NULL OR (ArchivedSocialNo IS NOT NULL AND ArchivedPersonName IS NOT NULL AND ArchivedUnitId IS NOT NULL AND ArchivedUnitName IS NOT NULL)) AND FailureReason IS NULL
   AND ((ClaimRecorded=1 AND ClaimTime IS NOT NULL AND ClaimSocialNo IS NOT NULL) OR (ClaimRecorded=0 AND ClaimTime IS NULL AND ClaimSocialNo IS NULL)))
  OR (Passed=0 AND ClaimRecorded=0 AND ClaimTime IS NULL AND ClaimSocialNo IS NULL AND FailureReason IS NOT NULL))
);
GO
-- 升级已有数据库：人员删除后，历史记录保留删除时的身份信息。
IF COL_LENGTH(N'dbo.Person',N'LastAuthTime') IS NULL
 ALTER TABLE dbo.Person ADD LastAuthTime datetime2 NULL;
IF COL_LENGTH(N'dbo.Person',N'LastAuthPassed') IS NULL
 ALTER TABLE dbo.Person ADD LastAuthPassed bit NULL;
IF COL_LENGTH(N'dbo.AuthenticationRecord',N'ArchivedSocialNo') IS NULL
 ALTER TABLE dbo.AuthenticationRecord ADD ArchivedSocialNo nvarchar(30) NULL;
IF COL_LENGTH(N'dbo.AuthenticationRecord',N'ArchivedPersonName') IS NULL
 ALTER TABLE dbo.AuthenticationRecord ADD ArchivedPersonName nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.AuthenticationRecord',N'ArchivedUnitId') IS NULL
 ALTER TABLE dbo.AuthenticationRecord ADD ArchivedUnitId int NULL;
IF COL_LENGTH(N'dbo.AuthenticationRecord',N'ArchivedUnitName') IS NULL
 ALTER TABLE dbo.AuthenticationRecord ADD ArchivedUnitName nvarchar(100) NULL;
IF COL_LENGTH(N'dbo.AuthenticationRecord',N'ClaimSocialNo') IS NULL
 ALTER TABLE dbo.AuthenticationRecord ADD ClaimSocialNo nvarchar(30) NULL;
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.AuthenticationRecord') AND name=N'UX_Auth_ClaimSocialNo')
BEGIN
 IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.AuthenticationRecord') AND name=N'CK_Authentication_Result')
  ALTER TABLE dbo.AuthenticationRecord DROP CONSTRAINT CK_Authentication_Result;
 -- 旧数据保留全部认证记录，每个社保编号仅保留最早一次领取登记。
 UPDATE ar SET ClaimSocialNo=COALESCE(p.SocialNo,ar.ArchivedSocialNo)
  FROM dbo.AuthenticationRecord ar LEFT JOIN dbo.Person p ON ar.PersonId=p.PersonId
  WHERE ar.ClaimRecorded=1 AND ar.ClaimSocialNo IS NULL;
 ;WITH RankedClaims AS (
  SELECT RecordId,ROW_NUMBER() OVER(PARTITION BY ClaimSocialNo ORDER BY ClaimTime,AuthTime,RecordId) AS rn
  FROM dbo.AuthenticationRecord WHERE ClaimRecorded=1 AND ClaimSocialNo IS NOT NULL
 )
 UPDATE ar SET ClaimRecorded=0,ClaimTime=NULL,ClaimSocialNo=NULL
  FROM dbo.AuthenticationRecord ar JOIN RankedClaims r ON ar.RecordId=r.RecordId WHERE r.rn>1;
 ALTER TABLE dbo.AuthenticationRecord WITH CHECK ADD CONSTRAINT CK_Authentication_Result CHECK (
  (Passed=1 AND (PersonId IS NOT NULL OR (ArchivedSocialNo IS NOT NULL AND ArchivedPersonName IS NOT NULL AND ArchivedUnitId IS NOT NULL AND ArchivedUnitName IS NOT NULL)) AND FailureReason IS NULL
   AND ((ClaimRecorded=1 AND ClaimTime IS NOT NULL AND ClaimSocialNo IS NOT NULL) OR (ClaimRecorded=0 AND ClaimTime IS NULL AND ClaimSocialNo IS NULL)))
  OR (Passed=0 AND ClaimRecorded=0 AND ClaimTime IS NULL AND ClaimSocialNo IS NULL AND FailureReason IS NOT NULL));
 CREATE UNIQUE INDEX UX_Auth_ClaimSocialNo ON dbo.AuthenticationRecord(ClaimSocialNo) WHERE ClaimSocialNo IS NOT NULL;
END;
COMMIT TRANSACTION;
GO
-- 领取记录只保存首次成功；最新认证状态单独保存在人员表，仍可统计认证失败人员。
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.AuthenticationRecord') AND name=N'CK_Authentication_FirstClaim')
BEGIN
 UPDATE p SET LastAuthTime=a.AuthTime,LastAuthPassed=a.Passed FROM dbo.Person p
  CROSS APPLY(SELECT TOP(1) AuthTime,Passed FROM dbo.AuthenticationRecord ar WHERE ar.PersonId=p.PersonId ORDER BY AuthTime DESC,RecordId DESC) a;
 DELETE FROM dbo.AuthenticationRecord WHERE Passed=0 OR ClaimRecorded=0;
 IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.AuthenticationRecord') AND name=N'CK_Authentication_Result')
  ALTER TABLE dbo.AuthenticationRecord DROP CONSTRAINT CK_Authentication_Result;
 ALTER TABLE dbo.AuthenticationRecord WITH CHECK ADD CONSTRAINT CK_Authentication_FirstClaim CHECK (
  Passed=1 AND ClaimRecorded=1 AND ClaimTime IS NOT NULL AND ClaimSocialNo IS NOT NULL AND FailureReason IS NULL
  AND (PersonId IS NOT NULL OR (ArchivedSocialNo IS NOT NULL AND ArchivedPersonName IS NOT NULL AND ArchivedUnitId IS NOT NULL AND ArchivedUnitName IS NOT NULL)));
END;
COMMIT TRANSACTION;
GO
IF OBJECT_ID(N'dbo.OperationLog',N'U') IS NULL
CREATE TABLE dbo.OperationLog (
 LogId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_OperationLog PRIMARY KEY,
 AdminId int NOT NULL CONSTRAINT FK_Operation_Admin REFERENCES dbo.Admin(AdminId),
 OperationTime datetime2 NOT NULL CONSTRAINT DF_Operation_Time DEFAULT(SYSDATETIME()),
 OperationContent nvarchar(200) NOT NULL,
 Result nvarchar(20) NOT NULL
);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.AuthenticationRecord') AND name=N'IX_Auth_PersonTime')
 CREATE INDEX IX_Auth_PersonTime ON dbo.AuthenticationRecord(PersonId,AuthTime DESC,RecordId DESC);
IF NOT EXISTS(SELECT 1 FROM dbo.Unit WHERE UnitName=N'第一单位') INSERT dbo.Unit(UnitName) VALUES(N'第一单位');
IF NOT EXISTS(SELECT 1 FROM dbo.Unit WHERE UnitName=N'第二单位') INSERT dbo.Unit(UnitName) VALUES(N'第二单位');
-- 首次运行由程序生成加盐密码散列并创建 admin 账户；脚本不存放明文密码。
GO
