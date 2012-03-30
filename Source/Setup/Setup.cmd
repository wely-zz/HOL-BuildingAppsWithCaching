@ECHO off
%~d0
CD "%~dp0"

ECHO Install Visual Studio 2010 Code Snippets for the lab:
ECHO -------------------------------------------------------------------------------
CALL .\Scripts\InstallCodeSnippets.cmd
ECHO Done!
ECHO.
ECHO *******************************************************************************
ECHO.
CD "%~dp0"
ECHO Create SQL Azure Northwind database:
ECHO -------------------------------------------------------------------------------
CALL .\Scripts\SetupAzureDatabase.cmd
ECHO Done!
ECHO.
ECHO *******************************************************************************
ECHO.
@PAUSE
