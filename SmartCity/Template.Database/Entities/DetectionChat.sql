CREATE TABLE [dbo].[DetectionChat](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DetectioId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[CreatedAt] [datetime] NULL,
	[Text] [nvarchar](max) NULL,
 CONSTRAINT [PK_DetectionChat] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[DetectionChat] ADD  CONSTRAINT [DF_DetectionChat_Id]  DEFAULT (newid()) FOR [Id]
GO
