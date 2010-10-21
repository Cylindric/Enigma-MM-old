@ECHO OFF
SETLOCAL
:START

SET SCRIPTPATH=%~dp0
SET SCRIPTDRIVE=%SCRIPTPATH:~0,2%
SET BUILDVERSION=-%1
IF %1.==. SET BUILDVERSION=

::Set some common paths
SET ROOT=%SCRIPTPATH%\Deploy
SET MCSROOT=%ROOT%\MCServer
SET MCROOT=%MCSROOT%\Minecraft
SET SMROOT=%MCSROOT%\ServerManager

:: Change to the script's path
%SCRIPTDRIVE%
CD %SCRIPTPATH%

:: Clean out the existing deploy package
IF EXIST Deploy RMDIR /S /Q Deploy

:: Create the folder hieararchy
MKDIR "%ROOT%"
MKDIR "%MCSROOT%"
MKDIR "%MCROOT%"
MKDIR "%SMROOT%"

::Copy the files we need to the package
XCOPY /Y ..\..\readme.txt %MCSROOT%
XCOPY /Y Build\*.exe "%SMROOT%"
XCOPY /Y Build\*.dll "%SMROOT%"
XCOPY /Y Build\settings.xml "%SMROOT%"
ERASE /F /Q "%SMROOT%\*.vshost.exe"

: Create the archive
CD Deploy
..\Tools\7za.exe a -mx9 MCServer%BUILDVERSION%.zip MCServer\

:END
ENDLOCAL