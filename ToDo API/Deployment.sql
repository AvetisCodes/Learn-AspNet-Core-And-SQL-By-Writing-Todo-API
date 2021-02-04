CREATE DATABASE [ToDoDB]
GO

USE [ToDoDB]
GO

CREATE TABLE ApplicationUser (
	ApplicationUserId INT NOT NULL IDENTITY(1, 1),
	Username VARCHAR(20) NOT NULL,
	NormalizedUsername VARCHAR(20) NOT NULL,
	Email VARCHAR(30) NOT NULL,
	NormalizedEmail VARCHAR(30) NOT NULL,
	Fullname VARCHAR(30) NULL,
	PasswordHash NVARCHAR(MAX) NOT NULL,
	PRIMARY KEY(ApplicationUserId)
)

CREATE INDEX [IX_ApplicationUser_NormalizedUsername] ON [dbo].[ApplicationUser] ([NormalizedUsername])
GO

CREATE INDEX [IX_ApplicationUser_NormalizedEmail] ON [dbo].[ApplicationUser] ([NormalizedEmail])
GO

CREATE TABLE [dbo].[Category](
	[CategoryId] INT IDENTITY(1,1) NOT NULL,
	[Description] VARCHAR(50) NOT NULL,
	PRIMARY KEY(CategoryId)
)
GO

CREATE TABLE [dbo].[UserToDo](
	[UserToDoId] INT NOT NULL IDENTITY(1,1),
	[CategoryId] INT NOT NULL,
	[ApplicationUserId] INT NOT NULL,
	[Title] VARCHAR(50) NOT NULL,
	[Description] VARCHAR(150) NOT NULL,
	[PulishDate] DATETIME NOT NULL DEFAULT (GETDATE()),
	[UpdateDate] DATETIME NOT NULL DEFAULT (GETDATE()),
	[IsComplete] BIT NOT NULL DEFAULT CONVERT(BIT, 0),
	FOREIGN KEY(ApplicationUserId) REFERENCES [dbo].[ApplicationUser] (ApplicationUserId),
	FOREIGN KEY(CategoryId) REFERENCES [dbo].[Category] (CategoryId),
	PRIMARY KEY(UserToDoId)
)
GO

CREATE TYPE [dbo].[AccountType] AS TABLE(
	[Username] VARCHAR(20) NOT NULL,
	[NormalizedUsername] VARCHAR(20) NOT NULL,
	[Email] VARCHAR(30) NOT NULL,
	[NormalizedEmail] VARCHAR(30) NOT NULL,
	[Fullname] VARCHAR(30) NULL,
	[PasswordHash] NVARCHAR(max) NOT NULL
)
GO

CREATE TYPE [dbo].[UserToDoType] AS TABLE(
	[UserToDoId] INT NOT NULL,
	[CategoryId] INT NOT NULL,
	[Title] VARCHAR(50) NOT NULL,
	[Description] VARCHAR(150) NOT NULL,
	[IsComplete] BIT NOT NULL
)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Account_GetByUsername]
	@NormalizedUsername VARCHAR(20)
AS

	SELECT 
		[ApplicationUserId],
		[Username],
		[NormalizedUsername],
		[Email],
		[NormalizedEmail],
		[Fullname],
		[PasswordHash]
	FROM 
		[dbo].[ApplicationUser] t1
	WHERE
		t1.[NormalizedUsername] = @NormalizedUsername

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Account_Insert]
	@Account AccountType READONLY
AS
	
	INSERT INTO
		[dbo].[ApplicationUser]
		(
			[Username],
			[NormalizedUsername],
			[Email],
			[NormalizedEmail],
			[Fullname],
			[PasswordHash]
		)
	SELECT
		[Username],
		[NormalizedUsername],
		[Email],
		[NormalizedEmail],
		[Fullname],
		[PasswordHash]
	FROM
		@Account;

	SELECT CAST(SCOPE_IDENTITY() AS INT);
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Category_GetAll]
AS
	
	SELECT
		[CategoryId],
		[Description]
	FROM
		[dbo].[Category]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UserToDo_Delete]
	@UserToDoId INT
AS

	DELETE FROM [dbo].[UserToDo] WHERE [UserToDoId] = @UserToDoId

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UserToDo_Get]
	@UserToDoId INT
AS

	SELECT 
		t1.[UserToDoId],
		t1.[CategoryId],
		t1.[ApplicationUserId],
		t1.[Title],
		t1.[Description],
		t1.[PulishDate],
		t1.[UpdateDate],
		t1.[IsComplete]
	FROM 
		[dbo].[UserToDo] t1
	WHERE
		t1.[UserToDoId] = @UserToDoId

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UserToDo_GetByUserId]
	@ApplicationUserId INT,
	@IsComplete BIT
AS

	SELECT 
		t1.[UserToDoId],
		t1.[CategoryId],
		t1.[ApplicationUserId],
		t1.[Title],
		t1.[Description],
		t1.[PulishDate],
		t1.[UpdateDate],
		t1.[IsComplete]
	FROM 
		[dbo].[UserToDo] t1
	WHERE
		t1.[ApplicationUserId] = @ApplicationUserId AND
		t1.[IsComplete] = @IsComplete

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UserToDo_Upsert]
	@UserToDo UserToDoType READONLY,
	@ApplicationUserId INT
AS
	MERGE INTO [dbo].[UserToDo] TARGET
	USING (
		SELECT
			[UserToDoId],
			@ApplicationUserId [ApplicationUserId],
			[CategoryId],
			[Title],
			[Description],
			[IsComplete]
		FROM
			@UserToDo
	) AS SOURCE
	ON
	(
		TARGET.[UserToDoId] = SOURCE.[UserToDoId] AND TARGET.[ApplicationUserId] = SOURCE.[ApplicationUserId]
	)
	WHEN MATCHED THEN
		UPDATE SET
			TARGET.[CategoryId] = SOURCE.[CategoryId],
			TARGET.[Title] = SOURCE.[Title],
			TARGET.[Description] = SOURCE.[Description],
			TARGET.[IsComplete] = SOURCE.[IsComplete],
			TARGET.[UpdateDate] = GETDATE()
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			[ApplicationUserId],
			[CategoryId],
			[Title],
			[Description]
		)
		VALUES (
			SOURCE.[ApplicationUserId],
			SOURCE.[CategoryId],
			SOURCE.[Title],
			SOURCE.[Description]
		);

	SELECT CAST(SCOPE_IDENTITY() AS INT);
GO

INSERT INTO [dbo].[Category] ([Description]) VALUES ('Personal')
INSERT INTO [dbo].[Category] ([Description]) VALUES ('Business')
INSERT INTO [dbo].[Category] ([Description]) VALUES ('Career')
INSERT INTO [dbo].[Category] ([Description]) VALUES ('Misc')
GO
