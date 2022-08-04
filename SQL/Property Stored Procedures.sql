SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DELETE_PROPERTY]
@id bigint
AS
BEGIN
	SET NOCOUNT ON;

	delete from PROPERTY_IMAGES where PropertyID=@id
	delete from [PROPERTIES] where PropertyID=@id
	return @@rowcount
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DELETE_PROPERTY_IMAGE]
@id bigint
AS
BEGIN
	SET NOCOUNT ON;

	delete from PROPERTY_IMAGES where PropertyImageID=@id

	return @@rowcount
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[INSERT_PROPERTY_IMAGE] 
@property_image_id bigint = null output
,@property_id bigint
,@primary_image nchar(1)
,@image varbinary(max)
,@file_name nvarchar(1000)
,@image_size bigint
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[PROPERTY_IMAGES]
				([PropertyID]
				,[PrimaryImage]
				,[Image]
				,[FileName]
				,[ImageSize])
			VALUES
				(@property_id
				,@primary_image
				,@image
				,@file_name
				,@image_size)

	select @property_image_id=@@identity
	return 1


END


GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[INSERT_PROPERTIES] 
@id bigint = null output
,@owner_uid bigint
,@start_date datetime
,@address1 nvarchar(500)
,@address2 nvarchar(500)
,@city nvarchar(100)
,@state nvarchar(100)
,@zip nvarchar(20)
,@squareFootage nvarchar(20)
,@numberOfBedrooms nvarchar(20)
,@numberOfBathrooms nvarchar(20)
,@standardCleaning char(1)
,@carpetCleaning char(1)
,@baseboardCleaning char(1)
,@laundryCleaning char(1)
,@dishCleaning char(1)
,@details nvarchar(500)
,@compensation nvarchar(20)
,@is_active char(1)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[PROPERTIES]
		([OwnerUID]
		,[StartDate]
		,[Address1]
		,[Address2]
		,[City]
		,[State]
		,[Zip]
		,[squareFootage]
		,[numberOfBedrooms]
		,[numberOfBathrooms]
		,[standardCleaning]
		,[carpetCleaning]
		,[baseboardCleaning]
		,[laundryCleaning]
		,[dishCleaning]
		,[Details]
		,[Compensation]
		,[IsActive])
	VALUES
		(@owner_uid
		,@start_date
		,@address1
		,@address2
		,@city
		,@state
		,@zip
		,@squareFootage
		,@numberOfBedrooms
		,@numberOfBathrooms
		,@standardCleaning
		,@carpetCleaning
		,@baseboardCleaning
		,@laundryCleaning
		,@dishCleaning
		,@details
		,@compensation
		,@is_active)

	select @id=@@IDENTITY
	return 1 --success

END
GO

SET ANSI_NULLS ON

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_PROPERTY_IMAGES]
@property_id bigint = null
,@property_image_id bigint = null
,@primary_only char(1) = 'N'
AS
BEGIN
	SET NOCOUNT ON;

	if @property_id is not null
	begin
		if @primary_only = 'Y'
		begin
			select *
			from PROPERTY_IMAGES
			where [PropertyID] = @property_id
			and PrimaryImage = 'Y'
			order by DateAdded desc
		end else
		begin
			select *
			from PROPERTY_IMAGES
			where [PropertyID] = @property_id
			and PrimaryImage = 'N'
			order by DateAdded desc
		end
	end
	else
	begin
		select *
		from PROPERTY_IMAGES
		where PropertyImageID = @property_image_id
	end
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_PROPERTIES]
@id bigint = null
,@uid bigint = null
,@location_title nvarchar(1000) = null
AS
BEGIN
	SET NOCOUNT ON;

	if @id is not null --a specific property
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email
		from [PROPERTIES] e, USERS u
		where PropertyID=@id
		and e.[OwnerUID] = u.[UID]
	else if @uid is not null
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email
		from [PROPERTIES] e, USERS u
		where [OwnerUID] = @uid
		and e.[OwnerUID] = u.[UID]
		and e.StartDate >= getdate()
		order by e.StartDate
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_PROPERTIES_ACTIVE]
AS
BEGIN
	SET NOCOUNT ON;

	create table #t (PropertyID bigint, Rating int)

	insert into #t (PropertyID, Rating)
	select e.PropertyID, 0
	from [PROPERTIES] e
	where e.IsActive='Y' --only active propertys
	and getdate() < e.StartDate --only PROPERTIES are are in the future

	declare @avg int, @property_id bigint

	declare c cursor for
	select PropertyID from #t
	open c

	fetch next from c into @property_id

	while @@fetch_status = 0
		begin
			select @avg = avg(Rating) from PROPERTY_RATINGS where PropertyID = @property_id
			if @avg is not null and @avg > 0
				update #t set Rating = @avg where PropertyID = @property_id
			fetch next from c into @property_id
		end
	close c
	deallocate c

	select e.*, u.FirstName, u.LastName, #t.Rating as AvgRating
	from [PROPERTIES] e, USERS u, #t
	where e.PropertyID = #t.PropertyID
	and e.OwnerUID = u.[UID]
	and e.IsActive='Y' --only active PROPERTIES
	and getdate() < e.StartDate --only PROPERTIES are are in the future
	order by StartDate

	drop table #t

END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_PROPERTY] 
@id bigint
,@owner_uid bigint
,@start datetime
,@address1 nvarchar(500)
,@address2 nvarchar(500)
,@city nvarchar(100)
,@state nvarchar(100)
,@zip nvarchar(20)
,@squareFootage nvarchar(20)
,@numberOfBedrooms nvarchar(20)
,@numberOfBathrooms nvarchar(20)
,@standardCleaning char(1)
,@carpetCleaning char(1)
,@baseboardCleaning char(1)
,@laundryCleaning char(1)
,@dishCleaning char(1)
,@details nvarchar(500)
,@compensation nvarchar(20)
,@is_active char(1)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[PROPERTIES]
	   SET [OwnerUID] = @owner_uid
		  ,[StartDate] = @start
		  ,[Address1] = @address1
		  ,[Address2] = @address2
		  ,[City] = @city
		  ,[State] = @state
		  ,[Zip] = @zip
		  ,[squareFootage] = @squareFootage
		  ,[numberOfBedrooms] = @numberOfBedrooms
		  ,[numberOfBathrooms] = @numberOfBathrooms
		  ,[standardCleaning] = @standardCleaning
		  ,[carpetCleaning] = @carpetCleaning
		  ,[baseboardCleaning] = @baseboardCleaning
		  ,[laundryCleaning] = @laundryCleaning
		  ,[dishCleaning] = @dishCleaning
		  ,[Details] = @details
		  ,[Compensation] = @compensation
		  ,[IsActive] = @is_active
	WHERE PropertyID = @id

	 return 1
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_PROPERTY_IMAGE] 
@property_image_id bigint = null output
,@primary_image nchar(1)
,@image varbinary(max)
,@file_name nvarchar(1000)
,@image_size bigint
AS
BEGIN
	SET NOCOUNT ON;

	update [dbo].[PROPERTY_IMAGES]
		set [FileName] = @file_name
			,[PrimaryImage] = @primary_image
			,[Image] = @image
			,[ImageSize] = @image_size
	where PropertyImageID = @property_image_id
	return 1

END

GO
