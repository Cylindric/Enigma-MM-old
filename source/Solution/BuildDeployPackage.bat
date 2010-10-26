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
SET AVROOT=%MCSROOT%\AlphaVespucci
SET CACHEROOT=%MCSROOT%\Cache
SET OVERVIEWERROOT=%MCSROOT%\Overviewer
SET MAPROOT=%MCSROOT%\Maps
SET BACKUPROOT=%MCSROOT%\Backups

:: Change to the script's path
%SCRIPTDRIVE%
CD %SCRIPTPATH%

:: Clean out the existing deploy package
ECHO Removing previous Deploy folder...
IF EXIST Deploy RD /S /Q Deploy

:: Create the folder hieararchy
ECHO Creating new Deploy folder...
MKDIR "%ROOT%"
IF NOT EXIST "%MCSROOT%" MKDIR "%MCSROOT%"
IF NOT EXIST "%MCROOT%" MKDIR "%MCROOT%"
IF NOT EXIST "%SMROOT%" MKDIR "%SMROOT%"
IF NOT EXIST "%AVROOT%" MKDIR "%AVROOT%"
IF NOT EXIST "%CACHEROOT%" MKDIR "%CACHEROOT%"
IF NOT EXIST "%OVERVIEWERROOT%" MKDIR "%OVERVIEWERROOT%"
IF NOT EXIST "%MAPROOT%" MKDIR "%MAPROOT%"
IF NOT EXIST "%BACKUPROOT%" MKDIR "%BACKUPROOT%"

::Copy the files we need to the package
ECHO Copying files...
XCOPY /Y ..\..\readme.txt %MCSROOT%
XCOPY /Y Build\ServerManager\*.exe "%SMROOT%"
XCOPY /Y Build\ServerManager\*.dll "%SMROOT%"
XCOPY /Y Build\ServerManager\settings.xml "%SMROOT%"
ERASE /F /Q "%SMROOT%\*.vshost.exe"

: Create the archive
CD Deploy
..\Tools\7za.exe a -mx9 MCServer%BUILDVERSION%.zip MCServer\


: Create the FTP script
ECHO {username}> deploy.ftp
ECHO {password}>> deploy.ftp
ECHO bin>> deploy.ftp
ECHO prompt>> deploy.ftp
ECHO lcd MCServer\ServerManager>> deploy.ftp
ECHO cd ServerManager>> deploy.ftp
ECHO mput *>> deploy.ftp
ECHO quit>> deploy.ftp

ECHO @ECHO OFF> deploy.bat
ECHO ftp -s:deploy.ftp {sitename}>> deploy.bat

:END
PAUSE
ENDLOCAL