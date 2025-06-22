CREATE DATABASE MiniAccountingDB;
GO

USE MiniAccountingDB;
GO

CREATE TABLE Accounts (
    AccountId INT PRIMARY KEY IDENTITY,
    AccountCode VARCHAR(20) NOT NULL,
    AccountName NVARCHAR(100) NOT NULL,
    ParentAccountId INT NULL,
    AccountType NVARCHAR(50) NOT NULL, -- Asset/Liability/Equity/Revenue/Expense etc.
	
    FOREIGN KEY (ParentAccountId) REFERENCES Accounts(AccountId)
);
GO

CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) PRIMARY KEY,
    UserName NVARCHAR(256) NOT NULL,
    NormalizedUserName NVARCHAR(256) NOT NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL
);
GO

CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) PRIMARY KEY,
    Name NVARCHAR(256) NOT NULL,
    NormalizedName NVARCHAR(256) NOT NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL
);
GO

CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);
GO

-- Predefined Roles
INSERT INTO AspNetRoles (Id, Name, NormalizedName) 
VALUES 
    ('1', 'Admin', 'ADMIN'),
    ('2', 'Accountant', 'ACCOUNTANT'),
    ('3', 'Viewer', 'VIEWER');
GO

CREATE TABLE Vouchers (
    VoucherId INT IDENTITY(1,1) PRIMARY KEY,
    VoucherType VARCHAR(10) NOT NULL,
    VoucherDate DATE NOT NULL,
    ReferenceNo VARCHAR(50) NOT NULL,
    CreatedBy NVARCHAR(450) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id)
);
GO

CREATE TABLE VoucherDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VoucherId INT NOT NULL,
    AccountIdValue INT NOT NULL,
    Debit DECIMAL(18,2) NOT NULL DEFAULT 0,
    Credit DECIMAL(18,2) NOT NULL DEFAULT 0,
    Description NVARCHAR(200) NULL,
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(VoucherId) ON DELETE CASCADE,
    FOREIGN KEY (AccountIdValue) REFERENCES Accounts(AccountId)
);
Go

CREATE TYPE VoucherDetailsType AS TABLE
(
    AccountIdNum INT,
    Debit DECIMAL(18,2),
    Credit DECIMAL(18,2),
    Description NVARCHAR(200)
);
GO


CREATE TABLE Modules (
    ModuleId INT IDENTITY(1,1) PRIMARY KEY,
    ModuleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Predefined Modules
INSERT INTO Modules (ModuleName) 
VALUES 
    ('UserManagement'),
    ('ChartOfAccounts'),
    ('VoucherEntry');
Go

CREATE TABLE RolePermissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId NVARCHAR(450) NOT NULL,
    ModuleId INT NOT NULL,
    AccessValue INT NOT NULL, 
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id),
    FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId)
);
GO

INSERT INTO RolePermissions(RoleId, ModuleId, AccessValue) 
VALUES 
        ('1', 1 , 0),
	('1', 1 , 1),
	('1', 2 , 0),
	('1', 2 , 1),
	('1', 3 , 0),
	('1', 3 , 1),
	('2', 3 , 0),
	('2', 3 , 1),
	('2', 2 , 0),
	('2', 2 , 1),
	('3', 3 , 0),
	('3', 2 , 0)    
GO





















