CREATE TABLE [dbo].[Users](
	[UserId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[Password] [nvarchar](256) NOT NULL,
	[PasswordSalt] [nvarchar](256) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](3) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_CreatedAt]  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
GO