@ECHO OFF
SETLOCAL
:START

SET SCRIPTPATH=%~dp0
SET SCRIPTDRIVE=%SCRIPTPATH:~0,2%
SET BUILDVERSION=-%1
IF %1.==. SET BUILDVERSION=

::Set some common paths
SET ROOT=%SCRIPTPATH%\Deploy
SET MCSROOT=%ROOT%\EMMServer
SET MCROOT=%MCSROOT%\Minecraft
SET SMROOT=%MCSROOT%\ServerManager
SET AVROOT=%MCSROOT%\AlphaVespucci
SET CACHEROOT=%MCSROOT%\Cache
SET OVERVIEWERROOT=%MCSROOT%\Overviewer
SET MAPROOT=%MCSROOT%\Maps
SET BACKUPROOT=%MCSROOT%\Backups

:: Change to the script's path
%SCRIPTDRIVE%
CD "%SCRIPTPATH%"

:: Build the solution
:: "%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.EXE" "Enigma Minecraft Manager.sln"

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

::The readme file
XCOPY /Y ..\..\readme.txt %MCSROOT%

::The EMM core files
XCOPY /Y EMM\bin\Release\emm.dll "%SMROOT%"
XCOPY /Y Server\bin\Release\server.exe "%SMROOT%"

::The 3rd party stuff
XCOPY /Y EMM\libs\*.* "%SMROOT%"
XCOPY /Y LibNbt\bin\Release\LibNbt.dll "%SMROOT%"
XCOPY /Y LibNbt\bin\Release\LibNbt.txt "%SMROOT%"

:: Copy the config from the source folder, not the build folder, to ensure
:: we don't get any modified-for-test versions
XCOPY /Y EMM\*.conf "%SMROOT%"


: Create the archive
CD Deploy
..\Tools\7za.exe a -mx9 EMMServer%BUILDVERSION%.zip EMMServer\


:END
PAUSE
ENDLOCAL