Option Explicit
Dim objFS, objShell
Dim ScriptPath, DeployRoot, ServerRoot, EMMRoot, Config, ReBuild
Dim cmd
Set objFS = CreateObject("Scripting.FileSystemObject")
Set objShell = WScript.CreateObject("WScript.Shell")

' -----------------------------------------------------------------------------
' Global constants
' -----------------------------------------------------------------------------
Const WindowStyleHide = 0
Const WindowStyleShow = 1


' -----------------------------------------------------------------------------
' Configuration
' -----------------------------------------------------------------------------
ScriptPath = objFS.GetParentFolderName(Wscript.ScriptFullName)
DeployRoot = objFS.BuildPath(ScriptPath, "Deploy")
EMMRoot = objFS.BuildPath(DeployRoot, "EMMServer")
Rebuild = False
Config = "Debug"

' -----------------------------------------------------------------------------
' Build
' -----------------------------------------------------------------------------
If (Rebuild = True) Then
	Dim compiler
	compiler = """%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"""
	cmd = compiler & " /nologo /verbosity:q "
	cmd = cmd & """" & objFS.BuildPath(ScriptPath, "Enigma Minecraft Manager.sln") & """"
	objShell.Run cmd, WindowStyleShow, True
End If


' This should only need creating once
CreateFolder DeployRoot

' Clean the deploy path
If (objFS.FolderExists(EMMRoot)) Then
	WScript.Sleep(500)
	objFS.DeleteFolder(EMMRoot)
	WScript.Sleep(500)
End If

' Create the folders
CreateFolder(EMMRoot)
CreateFolder(objFS.BuildPath(EMMRoot, "AlphaVespucci"))
CreateFolder(objFS.BuildPath(EMMRoot, "Backups"))
CreateFolder(objFS.BuildPath(EMMRoot, "Cache"))
CreateFolder(objFS.BuildPath(EMMRoot, "Minecraft"))
CreateFolder(objFS.BuildPath(EMMRoot, "Overviewer"))
CreateFolder(objFS.BuildPath(EMMRoot, "Maps"))


' The EMM core files
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "..\..\readme.txt")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\bin\" & Config & "\*.dll")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\bin\" & Config & "\*.exe")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\bin\" & Config & "\*.txt")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\bin\" & Config & "\messages.xml")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "Server\bin\" & Config & "\server.exe")), EMMRoot & "\"

'The 3rd party stuff
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "LibNbt\bin\LibNbt.dll")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "LibNbt\bin\LibNbt.txt")), EMMRoot & "\"

' Copy the sample configs from the source folder, not the build folder, to ensure
' we don't get any modified-for-test versions
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\Settings\*.conf")), EMMRoot
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\Scheduler\*.xml")), EMMRoot


Dim zipexe, zipname, buildversion
buildversion = GetFileVersion(objFS.GetAbsolutePathName(objFS.BuildPath(EMMRoot, "emm.dll")))
zipexe = objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "Tools\7za.exe"))
If buildversion = "" Then
	zipname = objFS.GetAbsolutePathName(objFS.BuildPath(DeployRoot, "EMMServer.zip"))
Else
	zipname = objFS.GetAbsolutePathName(objFS.BuildPath(DeployRoot, "EMMServer-" & buildversion & ".zip"))
End If


cmd = zipexe
cmd = cmd & " a -mx9 "
cmd = cmd & zipname & " "
cmd = cmd & "EMMServer\"

objShell.CurrentDirectory = DeployRoot
objShell.Run cmd, WindowStyleShow, True

Function GetFileVersion(Filename)
	Dim f
  If (objFS.FileExists(Filename)) Then
		Set f = objFS.GetFile(Filename)
		GetFileVersion = objFS.GetFileVersion(f)
	Else
		GetFileVersion = ""
	End If
End Function

Sub CreateFolder(Path)
  If (Not objFS.FolderExists(Path)) Then
		WScript.Sleep(100)
    objFS.CreateFolder Path
		WScript.Sleep(100)
  End If
End Sub
