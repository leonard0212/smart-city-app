CREATE TABLE [dbo].[AspNetUsers](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[ConcurrencyStamp] [varchar](255) NULL,
	[LockoutEnd] [datetime] NULL,
	[NormalizedEmail] [varchar](255) NULL,
	[NormalizedUserName] [varchar](255) NULL,
	[FirstName] [varchar](255) NULL,
	[LastName] [varchar](255) NULL,
	[Enabled] [bit] NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[CreatedById] [uniqueidentifier] NULL,
	[CreatedAt] [datetime] NULL,
 [LastSessionId] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_Id]  DEFAULT (newid()) FOR [Id]
GO

