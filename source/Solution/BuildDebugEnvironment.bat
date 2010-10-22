@ECHO OFF
SETLOCAL
:START

SET SCRIPTPATH=%~dp0
SET SCRIPTDRIVE=%SCRIPTPATH:~0,2%


::Set some common paths
SET REPOSITORY=D:\Minecraft\MCServerRoot
SET ROOT=%SCRIPTPATH%
SET MCSROOT=%ROOT%\Build
SET MCROOT=%MCSROOT%\Minecraft
SET SMROOT=%MCSROOT%\ServerManager
SET AVROOT=%MCSROOT%\AlphaVespucci


:: Change to the script's path
%SCRIPTDRIVE%
CD %SCRIPTPATH%


:: Create the folder hieararchy
IF NOT EXIST "%MCSROOT%" MKDIR "%MCSROOT%"
IF NOT EXIST "%MCROOT%" MKDIR "%MCROOT%"
IF NOT EXIST "%SMROOT%" MKDIR "%SMROOT%"
IF NOT EXIST "%AVROOT%" MKDIR "%AVROOT%"


::Copy the files we need to the package
XCOPY /Y "%REPOSITORY%\Minecraft\minecraft_server.jar" "%MCROOT%"
XCOPY /Y "%REPOSITORY%\AlphaVespucci\*" "%AVROOT%"

:END
PAUSE
ENDLOCAL