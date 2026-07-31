CREATE TABLE [dbo].[RoadInfrastructureDetection](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Type] [nvarchar](50) NULL,
	[Dimension] [nvarchar](50) NULL,
	[Diameter] [float] NULL,
	[Depth] [float] NULL,
 CONSTRAINT [PK_RoadInfrastructureDetection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RoadInfrastructureDetection] ADD  CONSTRAINT [DF_RoadInfrastructureDetection_Id]  DEFAULT (newid()) FOR [Id]
GO

