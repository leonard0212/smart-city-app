CREATE TABLE [dbo].[OutRequestDataLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](500) NOT NULL,
	[Endpoint] [nvarchar](max) NOT NULL,
	[Logged] [datetime2](7) NOT NULL,
	[Logger] [nvarchar](500) NULL,
	[Username] [nvarchar](500) NULL,
	[Ip] [nvarchar](50) NULL,
	[HttpMethod] [nvarchar](max) NULL,
	[Url] [nvarchar](max) NULL,
	[Request] [nvarchar](max) NULL,
	[Response] [nvarchar](max) NULL,
	[CorrelationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_OutRequestDataLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
