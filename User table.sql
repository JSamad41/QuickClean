
/****** Object:  Table [dbo].[CATEGORIES]    Script Date: 6/13/2020 2:38:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[USERS](
	[UID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](100) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
    [City] [nvarchar](100) NOT NULL,
    [State] [nvarchar](20) NOT NULL,
    [Zip] [nvarchar](20) NOT NULL,
    [PhoneNumber] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_USERS] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_UserID]  DEFAULT ('') FOR [UserID]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_FirstName]  DEFAULT ('') FOR [FirstName]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_LastName]  DEFAULT ('') FOR [LastName]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_Address]  DEFAULT ('') FOR [Address]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_City]  DEFAULT ('') FOR [City]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_State]  DEFAULT ('') FOR [State]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_Zip]  DEFAULT ('') FOR [Zip]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_PhoneNumber]  DEFAULT ('') FOR [PhoneNumber]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_Email]  DEFAULT ('') FOR [Email]
GO
ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_Password]  DEFAULT ('') FOR [Password]
GO
