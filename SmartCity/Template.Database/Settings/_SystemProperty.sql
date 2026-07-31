/****** Object:  Table [dbo].[_SystemProperty]    Script Date: 07/02/2024 11:57:21 AM ******/




CREATE TABLE [dbo].[_SystemProperty](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](350) NULL,
	[Value] [nvarchar](max) NULL,
 [CreatedAt] DATETIME NULL, 
    CONSTRAINT [PK__SystemProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


