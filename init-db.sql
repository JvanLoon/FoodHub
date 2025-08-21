--CREATE DATABASE FoodCalc;
--GO
--USE FoodCalc;
--GO
--CREATE LOGIN sa WITH PASSWORD = 'aj7a!unEe#Ms*BqTE7WJ2a4tX1$^y7';
--GO
--CREATE USER sa  FOR LOGIN sa ;
--GO
--ALTER ROLE db_owner ADD MEMBER sa;
--GO


IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FoodCalc')
BEGIN
    CREATE DATABASE FoodCalc;
END
GO

USE FoodCalc;
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'foodhubuser')
BEGIN
    CREATE LOGIN foodhubuser WITH PASSWORD = 'aj7a!unEe#Ms*BqTE7WJ2a4tX1$^y7';
END
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'foodhubuser')
BEGIN
    CREATE USER foodhubuser FOR LOGIN foodhubuser;
    ALTER ROLE db_owner ADD MEMBER foodhubuser;
END
GO
