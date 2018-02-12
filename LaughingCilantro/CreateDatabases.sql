-- "Idempotent": drops and recreates all tables into their final form.
-- Run this after ASP.NET MVC creates the basic tables (you  may have to register
-- a user first to force that to happen).

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Transactions'))
BEGIN
	DROP TABLE Transactions
END
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'FinancialAccounts'))
BEGIN
	DROP TABLE FinancialAccounts
END

CREATE TABLE FinancialAccounts (
	Id uniqueidentifier primary key default newid() NOT NULL,
	Name nvarchar(255) NOT NULL,
	OwnerId nvarchar(128) foreign key references AspNetUsers(Id) NOT NULL,
	AccountType varchar(64) NOT NULL
)

CREATE TABLE Transactions (
	Id nvarchar(255) primary key NOT NULL default newid(),
	ForeignId nvarchar(255),
	OriginalText nvarchar(255) NOT NULL,
	AccountId uniqueidentifier NOT NULL foreign key references FinancialAccounts(Id),
	Amount money NOT NULL,
	[TransactionDateUtc] datetime NOT NULL
)

CREATE UNIQUE INDEX ix_ForeignId_AccountId ON Transactions(ForeignId, AccountId)