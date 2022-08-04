
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PROPERTIES](
	[PropertyID] [bigint] IDENTITY(1,1) NOT NULL,
	[IsActive] [char](1) NOT NULL,
	[OwnerUID] [bigint] NOT NULL,
	[StartDate] [datetime] NULL,
	[Address1] [nvarchar](500) NOT NULL,
	[Address2] [nvarchar](500) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[Zip] [nvarchar](20) NOT NULL,
	[squareFootage] [nvarchar](20) NOT NULL,
	[numberOfBedrooms] [nvarchar](20) NOT NULL,
	[numberOfBathrooms] [nvarchar](20) NOT NULL,
	[standardCleaning] [char](1) NOT NULL,
	[carpetCleaning] [char](1) NOT NULL,
	[baseboardCleaning] [char](1) NOT NULL,
	[laundryCleaning] [char](1) NOT NULL,
	[dishCleaning] [char](1) NOT NULL,
	[Details] [nvarchar](500) NOT NULL,
	[Compensation] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_PROPERTIES] PRIMARY KEY CLUSTERED 
(
	[PropertyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_Enabled]  DEFAULT ('Y') FOR [IsActive]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_OwnerUID]  DEFAULT ((0)) FOR [OwnerUID]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_Address1]  DEFAULT ('') FOR [Address1]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_Address2]  DEFAULT ('') FOR [Address2]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_City]  DEFAULT ('') FOR [City]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_State]  DEFAULT ('') FOR [State]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_Zip]  DEFAULT ('') FOR [Zip]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_squareFootage]  DEFAULT ('') FOR [squareFootage]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_numberOfBedrooms]  DEFAULT ('') FOR [numberOfBedrooms]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_numberOfBathrooms]  DEFAULT ('') FOR [numberOfBathrooms]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_standardCleaning]  DEFAULT ('Y') FOR [standardCleaning]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_carpetCleaning]  DEFAULT ('Y') FOR [carpetCleaning]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_baseboardCleaning]  DEFAULT ('Y') FOR [baseboardCleaning]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_laundryCleaning]  DEFAULT ('Y') FOR [laundryCleaning]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_dishCleaning]  DEFAULT ('Y') FOR [dishCleaning]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_Details]  DEFAULT ('') FOR [Details]
GO

ALTER TABLE [dbo].[PROPERTIES] ADD  CONSTRAINT [DF_PROPERTIES_Compensation]  DEFAULT ('') FOR [Compensation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PROPERTY_IMAGES](
	[PropertyImageID] [bigint] IDENTITY(1,1) NOT NULL,
	[PropertyID] [bigint] NOT NULL,
	[PrimaryImage] [nchar](1) NOT NULL,
	[Image] [varbinary](max) NOT NULL,
	[FileName] [nvarchar](1000) NOT NULL,
	[ImageSize] [bigint] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
 CONSTRAINT [PK_PROPERTY_IMAGES] PRIMARY KEY CLUSTERED 
(
	[PropertyImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PROPERTY_IMAGES] ADD  CONSTRAINT [DF_PROPERTY_IMAGES_PropertyID]  DEFAULT ((0)) FOR [PropertyID]
GO

ALTER TABLE [dbo].[PROPERTY_IMAGES] ADD  CONSTRAINT [DF_PROPERTY_IMAGES_PrimaryImage]  DEFAULT (N'N') FOR [PrimaryImage]
GO

ALTER TABLE [dbo].[PROPERTY_IMAGES] ADD  CONSTRAINT [DF_PROPERTY_IMAGES_FileName]  DEFAULT ('') FOR [FileName]
GO

ALTER TABLE [dbo].[PROPERTY_IMAGES] ADD  CONSTRAINT [DF_PROPERTY_IMAGES_ImageSize]  DEFAULT ((0)) FOR [ImageSize]
GO

ALTER TABLE [dbo].[PROPERTY_IMAGES] ADD  DEFAULT (getdate()) FOR [DateAdded]
GO


