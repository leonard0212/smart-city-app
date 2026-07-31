CREATE TABLE [dbo].[Account](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,		
	[CompanyName] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[Enabled] [bit] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedById] [uniqueidentifier] NULL,
	[DeletedById] [uniqueidentifier] NULL,
	[CreatedById] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_Id]  DEFAULT (newid()) FOR [Id]
GO

