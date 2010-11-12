Option Explicit
Dim objFS, objShell
Dim ScriptPath, DeployRoot, ServerRoot, EMMRoot
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
ServerRoot = objFS.BuildPath(DeployRoot, "EMMServer")
EMMRoot = objFS.BuildPath(ServerRoot, "ServerManager")


' This should only need creating once
CreateFolder DeployRoot

' Clean the deploy path
If (objFS.FolderExists(ServerRoot)) Then
	objFS.DeleteFolder(ServerRoot)
End If

' Create the folders
CreateFolder(ServerRoot)
CreateFolder(EMMRoot)
CreateFolder(objFS.BuildPath(ServerRoot, "AlphaVespucci"))
CreateFolder(objFS.BuildPath(ServerRoot, "Backups"))
CreateFolder(objFS.BuildPath(ServerRoot, "Cache"))
CreateFolder(objFS.BuildPath(ServerRoot, "Minecraft"))
CreateFolder(objFS.BuildPath(ServerRoot, "Overviewer"))

' Copy the built files

' The EMM core files
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "..\..\readme.txt")), ServerRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\bin\Release\*.dll")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\bin\Release\Libs\*.exe")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\bin\Release\Libs\*.txt")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "Server\bin\Release\server.exe")), EMMRoot & "\"

'The 3rd party stuff
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "LibNbt\bin\Release\LibNbt.dll")), EMMRoot & "\"
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "LibNbt\bin\Release\LibNbt.txt")), EMMRoot & "\"

' Copy the config from the source folder, not the build folder, to ensure
' we don't get any modified-for-test versions
objFS.CopyFile objFS.GetAbsolutePathName(objFS.BuildPath(ScriptPath, "EMM\*.conf")), EMMRoot


Dim cmd, zipexe, zipname, buildversion
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
