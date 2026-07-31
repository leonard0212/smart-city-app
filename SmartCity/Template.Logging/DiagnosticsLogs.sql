CREATE TABLE [dbo].[DiagnosticsLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](500) NOT NULL,
	[Logged] [datetime2](7) NOT NULL,
	[Level] [nvarchar](50) NOT NULL,
	[Logger] [nvarchar](500) NULL,
	[Username] [nvarchar](500) NULL,
	[Ip] [nvarchar](50) NULL,
	[HttpMethod] [nvarchar](50) NULL,
	[Url] [nvarchar](max) NULL,
	[ElapsedMilliseconds] [int] NULL,
	[CorrelationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_DiagnosticsLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
