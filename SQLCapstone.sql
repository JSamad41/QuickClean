-- --------------------------------------------------------------------------------
-- Name:  Anastasiia Efimova
-- Class: 
-- Abstract: 
-- --------------------------------------------------------------------------------

-- --------------------------------------------------------------------------------
-- Options
-- --------------------------------------------------------------------------------
USE dbSQL1;     
SET NOCOUNT ON; -- Report only errors

-- --------------------------------------------------------------------------------
--						Problem #1
-- --------------------------------------------------------------------------------

-- Drop Table Statements

IF OBJECT_ID ('TCleaningReviews')		IS NOT NULL DROP TABLE TCleaningReviews

IF OBJECT_ID ('TPropertyOrders')		IS NOT NULL DROP TABLE TPropertyOrders
IF OBJECT_ID ('TOrders')				IS NOT NULL DROP TABLE TOrders
IF OBJECT_ID ('TClientReviews')			IS NOT NULL DROP TABLE TClientReviews 
IF OBJECT_ID ('TPropertyImage')			IS NOT NULL DROP TABLE TPropertyImage
IF OBJECT_ID ('TOrderStatus')			IS NOT NULL DROP TABLE TOrderStatus
IF OBJECT_ID ('TTypeOfCleanings')		IS NOT NULL DROP TABLE TTypeOfCleanings
IF OBJECT_ID ('TCleanings')				IS NOT NULL DROP TABLE TCleanings
IF OBJECT_ID ('TClientPayments')		IS NOT NULL DROP TABLE TClientPayments
IF OBJECT_ID ('TPayments')				IS NOT NULL DROP TABLE TPayments
IF OBJECT_ID ('TProperties')			IS NOT NULL DROP TABLE TProperties
IF OBJECT_ID ('TClients')				IS NOT NULL DROP TABLE TClients
IF OBJECT_ID ('TStates')				IS NOT NULL DROP TABLE TStates

-- --------------------------------------------------------------------------------
--	Step #1 : Create table 
-- --------------------------------------------------------------------------------

CREATE TABLE TStates
(
	 intStateID			INTEGER			NOT NULL
	,strState  			VARCHAR(255)	NOT NULL
	,CONSTRAINT TStates_PK PRIMARY KEY ( intStateID )
)

CREATE TABLE TClientReviews
(
	 intClientReviewID	INTEGER			NOT NULL
	,intClientID		INTEGER			NOT NULL
	,strClientReview  	VARCHAR(255)	NOT NULL
	,CONSTRAINT TClientReviews_PK PRIMARY KEY ( intClientReviewID )
)

CREATE TABLE TCleaningReviews
(
	 intCleaningReviewID	INTEGER			NOT NULL
	,intCleaningID			INTEGER			NOT NULL
	,strCleaningReview  	VARCHAR(255)	NOT NULL
	,CONSTRAINT TCleaningReviews_PK PRIMARY KEY ( intCleaningReviewID )
)

CREATE TABLE TClientPayments
(
	 intClientPaymentID	INTEGER			NOT NULL
	,intClientID		INTEGER			NOT NULL
	,intPaymentID		INTEGER			NOT NULL
	,monAmount			MONEY			NOT NULL
	,dtmPaymentDate		DATETIME		NOT NULL
	,CONSTRAINT TClientPayments_PK PRIMARY KEY ( intClientPaymentID )
)

CREATE TABLE TPayments
(
	 intPaymentID		INTEGER			NOT NULL
	,strPaymentType		VARCHAR(255)	NOT NULL
	,CONSTRAINT TPayments_PK PRIMARY KEY ( intPaymentID )
)

CREATE TABLE TTypeOfCleanings
(
	 intTypeOfCleaningID	INTEGER			NOT NULL
	,strTypeOfCleaning		VARCHAR(255)	NOT NULL
	,CONSTRAINT TTypeOfCleanings_PK PRIMARY KEY ( intTypeOfCleaningID )
)

CREATE TABLE TOrderStatus
(
	 intOrderStatusID		INTEGER			NOT NULL
	,strOrderStatus			VARCHAR(255)	NOT NULL
	,CONSTRAINT TOrderStatus_PK PRIMARY KEY ( intOrderStatusID )
)

CREATE TABLE TPropertyImage
(
	 intPropertyImageID		INTEGER			NOT NULL
	,intPropertyID			INTEGER			NOT NULL
	,strFileName			VARCHAR(255)	NOT NULL
	,sngImageSize			FLOAT			NOT NULL
	,dtmDateAdded			DATETIME		NOT NULL
	,CONSTRAINT TPropertyImage_PK PRIMARY KEY ( intPropertyImageID )
)

CREATE TABLE TClients
(
	 intClientID			INTEGER			NOT NULL
	,strFirstName			VARCHAR(255)	NOT NULL
	,strLastName			VARCHAR(255)	NOT NULL
	,strAddress				VARCHAR(255)	NOT NULL
	,strCity				VARCHAR(255)	NOT NULL
	,intStateID				INTEGER			NOT NULL
	,strZip					VARCHAR(255)	NOT NULL
	,strPhoneNumber			VARCHAR(255)	NOT NULL
	,strEmal				VARCHAR(255)	NOT NULL
	,strPaymentInfo			VARCHAR(255)	NOT NULL
	,CONSTRAINT TClients_PK PRIMARY KEY ( intClientID )
)

CREATE TABLE TOrders
(
	 intOrderID				INTEGER			NOT NULL
	,intClientID			INTEGER			NOT NULL
	,intCleaningID			INTEGER			NOT NULL
	,intOrderStatusID		INTEGER			NOT NULL
	,intTypeOfCleaningID	INTEGER			NOT NULL
	,intClientPaymentID INTEGER			NOT NULL
	,dtmOrderDate			DATETIME		NOT NULL
	,sngNumberOfHours		FLOAT			NOT NULL
	,monRate				MONEY			NOT NULL
	,CONSTRAINT TOrders_PK PRIMARY KEY ( intOrderID )
)

CREATE TABLE TPropertyOrders
(
	 intPropertyOrderID		INTEGER			NOT NULL
	,intPropertyID			INTEGER			NOT NULL
	,intOrderID			    INTEGER			NOT NULL
	,CONSTRAINT TPropertyOrders_PK PRIMARY KEY ( intPropertyOrderID )
)

CREATE TABLE TProperties
(
	 intPropertyID			INTEGER			NOT NULL
	,intClientID			INTEGER			NOT NULL
	,strAddress				VARCHAR(255)	NOT NULL
	,strCity				VARCHAR(255)	NOT NULL
	,intStateID				INTEGER			NOT NULL
	,dblSquareFootage		FLOAT			NOT NULL
	,intNumberOfBedrooms	INTEGER			NOT NULL
	,intNumberOfBathrooms	INTEGER			NOT NULL
	,strMiscellaneous		VARCHAR(255)	NOT NULL
	,CONSTRAINT TProperties_PK PRIMARY KEY ( intPropertyID )
)

CREATE TABLE TCleanings
(
	 intCleaningID			INTEGER			NOT NULL
	,strCompanyName			VARCHAR(255)	NOT NULL
	,strAddress				VARCHAR(255)	NOT NULL
	,intCityID				INTEGER			NOT NULL
	,intStateID				INTEGER			NOT NULL
	,strZip					VARCHAR(255)	NOT NULL
	,strPhoneNumber			VARCHAR(255)	NOT NULL
	,strEmal				VARCHAR(255)	NOT NULL
	,CONSTRAINT TCleanings_PK PRIMARY KEY ( intCleaningID )
)

-- --------------------------------------------------------------------------------
--	Step #2 : Establish Referential Integrity 
-- --------------------------------------------------------------------------------
--
-- #	Child							Parent						Column
-- -	-----							------						---------
-- 1	TClients						TStates						intStateID
-- 2	TClientReviews					TClient						intClientID
-- 3	TClientPaymennt					TClient						intClientID
-- 4	TClientPaymennt					TPayments					intPaymentID
-- 5	TProperties						TClients					intClientID
-- 6	TPropertyImages					TProperties					intPropertyID
-- 7	TPropertyOrders					TProperties					intPropertyID
-- 8	TPropertyOrders					TTOrders					intOrderID
-- 9	TOrders							TClients					intClientID
--10	TOrders							TTypeOfCleans				intTypeOFCleanID
--11	TOrder							TClientPayments				intClientPayments
--12	TOrder							TOrderStatus				intOrderStatusID
--13	TOrder							TCleanings					intCleaningID
--14	TCleaninrReviews				TCleanings					intCleaningID

--1
ALTER TABLE TClients ADD CONSTRAINT TClients_TStates_FK 
FOREIGN KEY ( intStateID ) REFERENCES TStates (intStateID )

--2
ALTER TABLE TClientPayments ADD CONSTRAINT TClientPayments_TClients_FK 
FOREIGN KEY ( intClientID ) REFERENCES TClients (intClientID )

--3
ALTER TABLE TClientReviews ADD CONSTRAINT TClientReviews_TClients_FK 
FOREIGN KEY ( intClientID ) REFERENCES TClients (intClientID )

--4
ALTER TABLE TClientPayments ADD CONSTRAINT TClientPayments_TPayments_FK 
FOREIGN KEY ( intPaymentID ) REFERENCES TPayments (intPaymentID )

--5
ALTER TABLE TProperties ADD CONSTRAINT TProperties_TClients_FK 
FOREIGN KEY ( intClientID ) REFERENCES TClients (intClientID )

--6
ALTER TABLE TPropertyImage ADD CONSTRAINT TPropertyImage_TProperties_FK 
FOREIGN KEY ( intPropertyID ) REFERENCES TProperties (intPropertyID )

--7
ALTER TABLE TPropertyOrders ADD CONSTRAINT TPropertyOrders_TProperties_FK 
FOREIGN KEY ( intPropertyID ) REFERENCES TProperties (intPropertyID )

--8
ALTER TABLE TPropertyOrders ADD CONSTRAINT TPropertyOrders_TOrders_FK 
FOREIGN KEY ( intOrderID ) REFERENCES TOrders (intOrderID )

--9
ALTER TABLE TOrders ADD CONSTRAINT TOrders_TClients_FK 
FOREIGN KEY ( intClientID ) REFERENCES TClients (intClientID )

--10
ALTER TABLE TOrders ADD CONSTRAINT TOrders_TTypeOfCleanings_FK 
FOREIGN KEY ( intTypeOfCleaningID ) REFERENCES TTypeOfCleanings (intTypeOfCleaningID )

--11
ALTER TABLE TOrders ADD CONSTRAINT TOrders_TClientPayments_FK 
FOREIGN KEY ( intClientPaymentID ) REFERENCES TClientPayments (intClientPaymentID )

--12
ALTER TABLE TOrders ADD CONSTRAINT TOrders_TOrderStatus_FK 
FOREIGN KEY ( intOrderStatusID ) REFERENCES TOrderStatus (intOrderStatusID )

--13
ALTER TABLE TOrders ADD CONSTRAINT TOrders_TCleanings_FK 
FOREIGN KEY ( intCleaningID ) REFERENCES TCleanings (intCleaningID )

--14
ALTER TABLE TCleaningReviews ADD CONSTRAINT TCleaningReviews_TCleanings_FK 
FOREIGN KEY ( intCleaningID ) REFERENCES TCleanings (intCleaningID )

/*
-- --------------------------------------------------------------------------------
--	Step #3 : Add Data - INSERTS
-- --------------------------------------------------------------------------------


INSERT INTO TStates ( intStateID, strState )
VALUES				  (1, 'OH')
				     ,(2, 'KY')

INSERT INTO TCleanings ( intCleaningID, strCompanyName, strAddress,  intCityID, intStateID, strZip, strPhoneNumber, strEmal)
VALUES				  (1, 'Super Cleaners', '354 Main St.', 1, 1, '45236', '+19172644566', 'alicemcKenzie@gmail.com')
				     ,(2, 'Bubble Clean', '15 St. Washington', 2, 1, '437934', '+17657634687','bubbleclean@gmail.com')
				     ,(3, 'Luka Clianing', '2 Watsonvill', 3, 2, '41735', '+13456733276', 'lukacleaning@gmail.com')


INSERT INTO TProperties ( intPropertyID, intCleaningID, strServiceName, monCostofService)
VALUES			      (1, 1, 'Once',  100)
				     ,(2, 1, 'Weekly', 50)
					 ,(3, 1, 'By Weekly', 80)
					 ,(4, 1, 'Monthly', 150)
					 ,(5, 1, '< 1500 sqf', 40)
					 ,(6, 1, '< 2500 sqf', 50)
					 ,(7, 1, '< 3500 sqf', 60)
					 ,(8, 1, '> 3500 sqf', 80)
					 ,(9, 1, '1 bath', 30)
					 ,(10, 1, '2 bath', 50)
					 ,(11, 1, '3 bath', 70)
					 ,(12, 1, '4 bath', 90)
					 ,(13, 1, '5 bath', 110)



Insert Into  TCustomers (intCustomerID, strFirstName, strLastName, strAddress, intCityID, intStateID, strZip, strPhoneNumber, strEmal, strPaymentInfo)
VALUES				  (1, 'Max', 'Nill', '23 River Dr.', 2, 1, '45225', '513764274', 'maxnill89@mail.com', 'card')
					 ,(2, 'Katy', 'Lower', '45 Main St.', 3, 1, '45448', '51342744424', 'katylower@gmail.com', 'cash')
					 ,(3, 'Mike', 'Cortez', '1569 Windisch Rd.', 3, 1, '45069', '9173442745', 'mikecortez67@gmail.com', 'PayPal')
					 ,(4, 'Dakita', 'Fonsi', ' 722 E Columbia Ave', 1, 1, '45215', '5134652436', 'dakitafosi97@gmail.com', 'card')


INSERT INTO TOrders ( intOrderID, intCustomerID, strOrderNumber, strStatus, dtmOrderDate)
VALUES				  (1, 2, '0001', 'complete', 11/01/2021)
				     ,(2, 2, '0002', 'in process', 20/02/2022)
				     ,(3, 1, '0003', 'paid', 18/01/2021)
				     ,(4, 3, '0004', 'complete', 05/01/2022)
				     ,(5, 4, '0005', 'complete',  08/02/2022)


INSERT INTO  TServiceOrders ( intServiceOrderID, intServiceID,  intOrderID)
VALUES				(1, 1, 5)				 
				   ,(2, 1, 2)
				   ,(3, 3, 1)
				   ,(4, 2, 4)
				   ,(5, 3, 3)
				   */

				   
