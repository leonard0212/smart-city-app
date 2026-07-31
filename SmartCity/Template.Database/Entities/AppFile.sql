CREATE TABLE [dbo].[AppFile]
(
    [Category] NVARCHAR(500) NULL, 
    [ContentType] NVARCHAR(500) NULL, 
    [Extension] NVARCHAR(50) NULL, 
    [Path] NVARCHAR(500) NULL, 
    [Name] NVARCHAR(500) NULL, 
    [UserId] UNIQUEIDENTIFIER NULL, 
    [Id] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedAt] DATETIME2 NULL, 
    CONSTRAINT [PK_AppFile] PRIMARY KEY ([Id])
)
