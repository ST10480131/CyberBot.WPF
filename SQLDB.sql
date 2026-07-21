CREATE DATABASE CyberBotDB;
GO

USE CyberBotDB;
GO

CREATE TABLE ActivityLog
(
    ActivityId INT IDENTITY(1,1) PRIMARY KEY,
    ActivityType NVARCHAR(50),
    Description NVARCHAR(255),
    ActivityDate DATETIME DEFAULT GETDATE()
);