@ECHO off
%~d0
CD "%~dp0"

ECHO Install Visual Studio 2012 Code Snippets for the lab:
ECHO -------------------------------------------------------------------------------
CALL .\Scripts\InstallCodeSnippets.cmd
ECHO Done!
ECHO.
ECHO *******************************************************************************
ECHO.
CD "%~dp0"
ECHO Create Windows Azure SQL Database:
ECHO -------------------------------------------------------------------------------
CALL .\Scripts\SetupSqlDatabase.cmd
ECHO Done!
ECHO.
ECHO *******************************************************************************
ECHO.
@PAUSE
