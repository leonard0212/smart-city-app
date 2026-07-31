CREATE TABLE [dbo].[DetectionFile](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DetectionId] [uniqueidentifier] NULL,
	[FileId] [uniqueidentifier] NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK_DetectionFile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DetectionFile] ADD  CONSTRAINT [DF_DetectionFile_Id]  DEFAULT (newid()) FOR [Id]
GO