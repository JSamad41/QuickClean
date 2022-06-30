
CREATE PROCEDURE [dbo].[INSERT_USER] 
@uid bigint = null output
,@user_id nvarchar(100)
,@password nvarchar(20)
,@first_name nvarchar(100)
,@last_name nvarchar(100)
,@address nvarchar(100)
,@city nvarchar(100)
,@state nvarchar(50)
,@zip nvarchar(50)
,@phone_number nvarchar(50)
,@email nvarchar(255)
AS
BEGIN
	-------------------------------------------------------------------------------
	--Spring 2021
	-------------------------------------------------------------------------------
	SET NOCOUNT ON;

	declare @count tinyint

	--Don't allow more than one Email address in this table
	select @count=count(*) from USERS where Email=@email 
	if @count >0 return -1 --email already exists

	--Don't allow more than one UserID
	select @count=count(*) from USERS where UserID=@user_id
	if @count >0 return -2 --user id already exists

	INSERT INTO [dbo].[USERS]
			   ([UserID]
			   ,[Password]
			   ,[FirstName]
			   ,[LastName]
			   ,[Address]
			   ,[City]
			   ,[State]
			   ,[Zip]
			   ,[PhoneNumber]
			   ,[Email])
		 VALUES
			   (@user_id
			   ,@password
			   ,@first_name
			   ,@last_name
			   ,@address
			   ,@city
			   ,@state
			   ,@zip
			   ,@phone_number
			   ,@email)
	select @uid=@@IDENTITY
	return 1

END

GO

CREATE PROCEDURE [dbo].[LOGIN]
@user_id nvarchar(100)
,@password nvarchar(20)
AS
BEGIN
	-------------------------------------------------------------------------------
	--Spring 2021
	-------------------------------------------------------------------------------
	SET NOCOUNT ON;

	select *
	from USERS
	where UserID = @user_id
	and [Password] = @password

END

GO

CREATE PROCEDURE [dbo].[SELECT_USERS]
@uid bigint = null
AS
BEGIN
	-------------------------------------------------------------------------------
	--Spring 2021
	-------------------------------------------------------------------------------
	SET NOCOUNT ON;

	if @uid is not null
	begin --find the user based on the @uid
		select *
		from USERS u
		where u.[UID] = @uid
	end
	else
	begin
		select *
		from USERS u
		order by LastName, FirstName
	end
END

GO

CREATE PROCEDURE [dbo].[UPDATE_USER] 
@uid bigint
,@user_id nvarchar(100)
,@password nvarchar(20)
,@first_name nvarchar(100)
,@last_name nvarchar(100)
,@address nvarchar(100)
,@city nvarchar(100)
,@state nvarchar(50)
,@zip nvarchar(50)
,@phone_number nvarchar(50)
,@email nvarchar(255)
AS
BEGIN
	-------------------------------------------------------------------------------
	--Spring 2021
	-------------------------------------------------------------------------------

	SET NOCOUNT ON;

	UPDATE [dbo].[USERS]
	   SET [UserID] = @user_id
		  ,[Password] = @password
		  ,[FirstName] = @first_name
		  ,[LastName] = @last_name
		  ,[Address] = @address
		  ,[City] = @city
		  ,[State] = @state
		  ,[Zip] = @zip
		  ,[PhoneNumber] = @phone_number
		  ,[Email] = @email
	 WHERE [UID] = @uid
	 return 1
END

GO

