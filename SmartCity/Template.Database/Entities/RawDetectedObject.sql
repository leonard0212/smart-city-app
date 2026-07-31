CREATE TABLE [dbo].[RawDetectedObject](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[RawDetectionId] [uniqueidentifier] NOT NULL,
	[DetectionId] [uniqueidentifier] NULL,
	[Type] [nvarchar](50) NULL,
	[PointsOrder] [int] NULL,
	[PosX] [float] NULL,
	[PosY] [float] NULL,
	[Accuracy] [float] NULL,
	[ObjectName] [varchar](100) NULL,
	[CreatedAt] [datetime2](7) NULL,
	[GroupId] [int] NULL,
 CONSTRAINT [PK_RawDetectedObject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RawDetectedObject] ADD  CONSTRAINT [DF_RawDetectedObject_Id]  DEFAULT (newid()) FOR [Id]
GO

