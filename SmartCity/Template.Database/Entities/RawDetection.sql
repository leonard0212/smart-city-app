CREATE TABLE [dbo].[RawDetection](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DetectionId] [uniqueidentifier] NOT NULL,
	[MainClass] [nvarchar](50) NULL,
	[SubClass] [nvarchar](50) NULL,
	[Lat] [float] NULL,
	[Lng] [float] NULL,
	[EdgeId] [uniqueidentifier] NULL,
	[ServiceBusMessageId] [nvarchar](50) NULL,
	[CreatedAt] [datetime2](7) NULL,
	[FileId] [uniqueidentifier] NULL,
	[ProcessStatus] [nvarchar](50) NULL,
	[PreviewFileId] [uniqueidentifier] NULL,
	[CropFileId] [uniqueidentifier] NULL,
	[ProcessedsFileId] [uniqueidentifier] NULL,
	[ProcessedExtensionData] [nvarchar](max) NULL,
 CONSTRAINT [PK_RawDetections] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[RawDetection] ADD  CONSTRAINT [DF_RawDetections_Id]  DEFAULT (newid()) FOR [Id]
GO

