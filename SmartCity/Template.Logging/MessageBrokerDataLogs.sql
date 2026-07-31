CREATE TABLE [dbo].[MessageBrokerDataLogs]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Application] NVARCHAR(500) NULL, 
    [Logged] DATETIME2 NULL, 
    [QueueName] NVARCHAR(500) NULL,
    [Body] NVARCHAR(MAX) NULL, 
    [ContentType] NVARCHAR(500) NULL, 
    [MessageId] NVARCHAR(500) NULL, 
    [CorrelationId] NVARCHAR(500) NULL, 
    [DeliveryCount] INT NULL, 
    [ExpiresAt] DATETIME2 NULL, 
    [Subject] NVARCHAR(MAX) NULL, 
    [To] NVARCHAR(500) NULL, 
    [Logger] NVARCHAR(500) NULL, 
    [SessionId] NVARCHAR(500) NULL
    
)
