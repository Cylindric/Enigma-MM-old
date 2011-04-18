Option Explicit
Dim objFS, objShell
Dim ScriptPath, DeployRoot, ServerRoot, EMMRoot, BuildRoot, SourceRoot
Dim cmd
Set objFS = CreateObject("Scripting.FileSystemObject")
Set objShell = WScript.CreateObject("WScript.Shell")

' Make sure we're running in CSCript
ForceCScript

' -----------------------------------------------------------------------------
' Global constants
' -----------------------------------------------------------------------------
Const WindowStyleHide = 0
Const WindowStyleShow = 1
Const ForReading = 1
Const ForWriting = 2
Const ForAppending = 8

' -----------------------------------------------------------------------------
' Configuration
' -----------------------------------------------------------------------------
ScriptPath = objFS.GetParentFolderName(Wscript.ScriptFullName)
DeployRoot = BuildPath(Array(objFS.GetParentFolderName(ScriptPath), "Deploy"))
SourceRoot = BuildPath(Array(ScriptPath, "Solution"))
BuildRoot = BuildPath(Array(ScriptPath, "Build"))
EMMRoot = BuildPath(Array(DeployRoot, "EMMServer"))

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
CreateFolder(BuildPath(Array(EMMRoot, "Backups")))
CreateFolder(BuildPath(Array(EMMRoot, "c10t")))
CreateFolder(BuildPath(Array(EMMRoot, "Cache")))
CreateFolder(BuildPath(Array(EMMRoot, "Maps")))
CreateFolder(BuildPath(Array(EMMRoot, "Minecraft")))
CreateFolder(BuildPath(Array(EMMRoot, "Overviewer")))
CreateFolder(BuildPath(Array(EMMRoot, "Plugins")))


' The EMM core files
CopyFile BuildPath(Array(BuildRoot, "\*.dll")), EMMRoot & "\"
CopyFile BuildPath(Array(BuildRoot, "\*.exe")), EMMRoot & "\"
CopyFile BuildPath(Array(BuildRoot, "\Plugins\*.dll")), EMMRoot & "\Plugins\"

' Copy the sample configs from the source folder, not the build folder, to ensure
' we don't get any modified-for-test versions
CopyFile BuildPath(Array(SourceRoot, "EMM", "EMM.sdf")), EMMRoot & "\"
CopyFile BuildPath(Array(SourceRoot, "EMM", "items.xml")), EMMRoot & "\"
CopyFile BuildPath(Array(SourceRoot, "EMM", "Scheduler", "*.xml")), EMMRoot & "\"
CopyFile BuildPath(Array(SourceRoot, "EMM", "Settings", "*.conf")), EMMRoot & "\"
CopyFile BuildPath(Array(SourceRoot, "Plugin.Overviewer", "*.conf")), EMMRoot & "\Plugins\"
CopyFile BuildPath(Array(SourceRoot, "Plugin.c10t", "*.conf")), EMMRoot & "\Plugins\"

' Remove any non-deploy files
DeleteFile BuildPath(Array(EMMRoot, "ItemExtractor.exe"))
DeleteFile BuildPath(Array(EMMRoot, "ItemExtractor.vshost.exe"))
DeleteFile BuildPath(Array(EMMRoot, "MinecraftSimulator.exe"))
DeleteFile BuildPath(Array(EMMRoot, "moq.dll"))
DeleteFile BuildPath(Array(EMMRoot, "nunit.framework.dll"))
DeleteFile BuildPath(Array(EMMRoot, "Server.vshost.exe"))
DeleteFile BuildPath(Array(EMMRoot, "Test.NullCommand.exe"))
DeleteFile BuildPath(Array(EMMRoot, "Tests.dll"))

' Concatenate and clean out the license files
Dim file
file = BuildPath(Array(EMMROOT, "readme.txt"))
CreateFile file
WriteLine file, "##### Contents"
WriteLine file, "##1## Readme"
WriteLine file, "##2## LibNbt License"
WriteLine file, "##3## Ionic Zip License"
WriteLine file, "#####"
WriteLine file, ""
WriteLine file, ""
WriteLine file, "################################################################################"
WriteLine file, "##1## Readme"
WriteLine file, "################################################################################"
ConcatenateFiles file, BuildPath(Array(SourceRoot, "..", "..", "readme.txt"))
WriteLine file, ""
WriteLine file, ""
WriteLine file, "################################################################################"
WriteLine file, "##2## LibNbt License"
WriteLine file, "################################################################################"
ConcatenateFiles file, BuildPath(Array(SourceRoot, "LibNbt", "LibNbt.txt"))
WriteLine file, ""
WriteLine file, ""
WriteLine file, "################################################################################"
WriteLine file, "##3## Ionic Zip License"
WriteLine file, "################################################################################"
ConcatenateFiles file, BuildPath(Array(SourceRoot, "EMM", "Ionic.txt"))


Dim zipexe, zipname, buildversion
buildversion = GetFileVersion(BuildPath(Array(EMMRoot, "emm.dll")))
zipexe = objFS.GetAbsolutePathName(BuildPath(Array(SourceRoot, "Tools", "7za.exe")))
If buildversion = "" Then
	zipname = objFS.GetAbsolutePathName(BuildPath(Array(DeployRoot, "EMMServer.zip")))
Else
	zipname = objFS.GetAbsolutePathName(BuildPath(Array(DeployRoot, "EMMServer-" & buildversion & ".zip")))
End If

DeleteFile zipname

cmd = zipexe
cmd = cmd & " a -mx9 "
cmd = cmd & zipname & " "
cmd = cmd & "EMMServer\"

objShell.CurrentDirectory = DeployRoot
WScript.Echo "Creating new zip file..."
objShell.Run cmd, WindowStyleHide, True





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

Function CopyFile(Source, Destination)
	WScript.Echo "Copying " & Source & " to " & Destination & "."
	objFS.CopyFile Source, Destination
End Function

Function DeleteFile(Filename)
	If objFS.FileExists(Filename) Then
		WScript.Echo "Deleting " & Filename & "."
		objFS.DeleteFile Filename
	End If
End Function

Sub ForceCScript
	Dim args : args = ""
	Dim i
	If Right(LCase(WScript.FullName), 11) = "wscript.exe" Then
		For i=0 To WScript.Arguments.Count - 1
			args = args & WScript.Arguments(i) & " "
		Next
		objShell.run objShell.ExpandEnvironmentStrings("%comspec%") & _
			" /c cscript.exe //nologo """ & WScript.ScriptFullName & """" & args
		WScript.Quit
	End If
End Sub

Sub CreateFile(TargetFileName)
  WScript.Echo "Creating file " & TargetFileName
  objFS.CreateTextFile TargetFileName, True
End Sub

Sub WriteLine(TargetFileName, Line)
  Dim TargetFile
  Set TargetFile = objFS.OpenTextFile(TargetFileName, ForAppending)
  TargetFile.WriteLine(Line)
  TargetFile.Close
End Sub

Sub ConcatenateFiles(TargetFileName, SecondFileName)
  Dim TargetFile, SecondFile
  Dim txt
	WScript.Echo "Appending " & SecondFilename & " to " & TargetFileName
  Set TargetFile = objFS.OpenTextFile(TargetFileName, ForAppending)
  Set SecondFile = objFS.OpenTextFile(SecondFileName, ForReading)
  txt = SecondFile.ReadAll
  TargetFile.Write txt
  TargetFile.Close
  SecondFile.Close
End Sub

Function BuildPath(rArgs)
  Dim c
  BuildPath = rArgs(0)
  For c = 1 To UBound(rArgs)
    BuildPath = objFS.BuildPath(BuildPath, rArgs(c))
    BuildPath = objFS.GetAbsolutePathName(BuildPath)
  Next
End Function
