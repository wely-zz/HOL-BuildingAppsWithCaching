@echo off

setlocal 
%~d0
cd "%~dp0"

@REM  -------------------------------------------------------------
@REM  You can change server and database name here.
@REM  -------------------------------------------------------------

:select
SET /p sqlServer=Please enter the Windows Azure SQL Server Address (e.g: yourserver.database.windows.net):
SET /p User=Please enter the User to connect to your Windows Azure SQL Database (e.g: yourusername@yourserver):
SET /p Password=Please enter the Password to Connect to Windows Azure SQL Database:
SET dbName=Northwind2

echo.
echo ===========================
echo Northwind database setup 
echo ===========================
echo.
echo SQL Server: %sqlServer%
echo User: %User%
echo Password: %Password%
echo.

CHOICE /C YN /D Y /T 10 /M "Are these values correct"
IF ERRORLEVEL 2 GOTO select

echo IN PROGRESS: Dropping database '%dbName%' on '%sqlServer%' if it exists...
OSQL -S %sqlServer% -U %User% -P %Password% -b -n -d master -Q "DROP DATABASE [%dbName%]"

echo IN PROGRESS: Creating database '%dbName%' on '%sqlServer%'...
OSQL -S %sqlServer% -U %User% -P %Password% -b -n -d master -Q "CREATE DATABASE [%dbName%]"

echo IN PROGRESS: Creating tables in '%dbName%' database on '%sqlServer%'...
OSQL -S %sqlServer% -d "%dbName%" -U %User% -P %Password% -b -n -i "NorthwindProducts.sql"



echo =============================================================================
echo SUCCESS: '%dbName%' database created on '%sqlServer%'
echo REMEMBER change the connection string for each begin end solutions with your Windows Azure SQL Database account
echo =============================================================================
GOTO EXIT

:ERROR
echo.
echo ======================================
echo An error occured. 
echo Please review errors above.
echo ======================================
GOTO EXIT

:EXIT