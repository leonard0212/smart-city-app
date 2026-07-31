
CREATE TABLE [dbo].[Detection](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Category] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[NeedIntervention] [nvarchar](50) NULL,
	[Severity] [nvarchar](50) NULL,
	[Lat] [float] NULL,
	[Lng] [float] NULL,
	[ResolutionStatus] [nvarchar](50) NULL,
	[ReportingDate] [datetime] NULL,
	[EdgeId] [uniqueidentifier] NULL,
	[RawDataId] [uniqueidentifier] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[Enabled] [bit] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedById] [uniqueidentifier] NULL,
	[DeletedById] [uniqueidentifier] NULL,
	[CreatedById] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Detection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Detection] ADD  CONSTRAINT [DF_Detection_Id]  DEFAULT (newid()) FOR [Id]
GO