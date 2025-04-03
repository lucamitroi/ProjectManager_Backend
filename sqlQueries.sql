--CREATE THE DATABASE
CREATE DATABASE ProjectManagerDatabase
GO

--USE THE DATABASE
USE ProjectManagerDatabase
GO

--CREATE THE SCHEMA
CREATE SCHEMA ProjectManagerSchema
GO

--CREATE THE USERS TABLE
CREATE TABLE ProjectManagerSchema.Users (
    UserId INT IDENTITY(1, 1) NOT NULL,
    [FirstName] NVARCHAR(50),
    [LastName] NVARCHAR(50),
    [Email] NVARCHAR(50)
)

--CREATE THE AUTH TABLE
CREATE TABLE ProjectManagerSchema.Auth (
    Email NVARCHAR(50),
    PasswordHash VARBINARY(MAX),
    PasswordSalt VARBINARY(MAX)
)

--CREATE THE PROJECTS TABLE
CREATE TABLE ProjectManagerSchema.Projects (
    ProjectId INT IDENTITY(1, 1),
    UserId INT,
    ProjectTitle NVARCHAR(100),
    ProjectDescription NVARCHAR(MAX),
    ProjectDate DATETIME,
)
GO

--CREATE CLUSTERED INDEX FOR ProjectManagerSchema.Projects
CREATE CLUSTERED INDEX cix_Projects_UserId_ProjectId ON ProjectManagerSchema.Projects(UserId, ProjectId)
GO

--CREATE THE TAKS TABLE
CREATE TABLE ProjectManagerSchema.Tasks (
    TaskId INT IDENTITY(1, 1),
    ProjectId INT, 
    TaskDescription NVARCHAR(MAX)    
)
GO

--CREATE CLUSTERED INDEX FOR ProjectManagerSchema.Tasks
CREATE CLUSTERED INDEX cix_Tasks_ProjectId_TaskId ON ProjectManagerSchema.Tasks(ProjectId, TaskId)
GO

--SHOW ALL PROJECTS AND ALL RELATED TASKS
SELECT 
    [Projects].[ProjectId], 
    [Projects].[ProjectTitle], 
    [Projects].[ProjectDescription], 
    [Projects].[ProjectDate], 
    [Tasks].[TaskId],
    [Tasks].[TaskDescription] 
FROM ProjectManagerSchema.Projects AS Projects 
    JOIN ProjectManagerSchema.Tasks AS Tasks 
        ON Tasks.ProjectId = Projects.UserId
GO


SELECT * FROM ProjectManagerSchema.Auth