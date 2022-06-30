
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_USER_PROPERTY_LIKES]
@uid bigint = null
AS
BEGIN
	SET NOCOUNT ON;

	select *
	from PROPERTY_LIKES
	where [UID] = @uid
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_USER_PROPERTY_RATINGS]
@uid bigint = null
AS
BEGIN
	SET NOCOUNT ON;

	select *
	from PROPERTY_RATINGS
	where [UID] = @uid
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TOGGLE_PROPERTY_LIKE]
@uid bigint
,@property_id bigint
AS
BEGIN
	SET NOCOUNT ON;

	declare @count int

	--check to see if the like is already there
	select @count=count(*)
	from PROPERTY_LIKES 
	where [UID] = @uid 
	and PropertyID = @property_id 

	if @count=0 --the user has not already liked this Property; like it now
	begin
		insert into PROPERTY_LIKES ([UID], PropertyID)
		values(@uid, @property_id)
		return 1 --inserted
	end
	else --the user already liked this Property; remove the like
	begin
		delete from PROPERTY_LIKES 
		where [UID] = @uid 
		and PropertyID = @property_id 
		return 0 --removed
	end
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_PROPERTY_RATING]
@rating_id bigint output
,@uid bigint
,@property_id bigint
,@rating tinyint
AS
BEGIN
	SET NOCOUNT ON;

	declare @count int

	--check to see if the like is already there
	select @count=count(*)
	from PROPERTY_RATINGS 
	where [UID] = @uid 
	and PropertyID = @property_id 

	if @count=0 --the user has not rated this Property; rate it now
	begin
		insert into PROPERTY_RATINGS ([UID], PropertyID, Rating)
		values(@uid, @property_id, @rating)
		set @rating_id = @@IDENTITY
		return 1 --new rating added
	end
	else --the user previously rated this Property; update it now
	begin
		update [dbo].[PROPERTY_RATINGS]
		set [Rating] = @rating
		where [UID] = @uid
		and PropertyID = @property_id

		select @rating_id = RateID from PROPERTY_RATINGS where [UID]=@uid and PropertyID=@property_id
		return 2 --existing rating updated
	end
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SELECT_PROPERTIES]
@id bigint = null
,@uid bigint = null
,@location_title nvarchar(1000) = null
AS
BEGIN
	SET NOCOUNT ON;

	if @id is not null --a specific Property
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email, TotalLikes=(select count(*) from PROPERTY_LIKES where PropertyID=@id)
		from [PROPERTIES] e, USERS u
		where PropertyID=@id
		and e.[OwnerUID] = u.[UID]
	else if @uid is not null
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email, TotalLikes=0
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
ALTER PROCEDURE [dbo].[SELECT_PROPERTIES_ACTIVE]
AS
BEGIN
	SET NOCOUNT ON;

	create table #t (PropertyID bigint, Rating int, Likes int)

	insert into #t (PropertyID, Rating)
	select e.PropertyID, 0
	from [PROPERTIES] e
	where e.IsActive='Y' --only active Property
	and getdate() < e.StartDate --only Property are are in the future

	declare @avg int, @property_id bigint, @count bigint

	declare c cursor for
	select PropertyID from #t
	open c

	fetch next from c into @property_id

	while @@fetch_status = 0
		begin
			select @avg = avg(Rating) from PROPERTY_RATINGS where PropertyID = @property_id
			if @avg is not null and @avg > 0
				update #t set Rating = @avg where PropertyID = @property_id
			select @count=count(*) from PROPERTY_LIKES where PropertyID=@property_id
			update #t set Likes = @count where PropertyID = @property_id
			fetch next from c into @property_id
		end
	close c
	deallocate c

	select e.*, u.FirstName, u.LastName, #t.Rating as AvgRating, #t.Likes as TotalLikes
	from [PROPERTIES] e, USERS u, #t
	where e.PropertyID = #t.PropertyID
	and e.OwnerUID = u.[UID]
	and e.IsActive='Y' --only active Property
	and getdate() < e.StartDate --only Property are are in the future
	order by StartDate

	drop table #t

END

GO
