' -----------------------------------------------------------------------------
' Create master objects
' -----------------------------------------------------------------------------
Set objShell = WScript.CreateObject("WScript.Shell")
Set objNet = WScript.CreateObject("WScript.Network")
Set objFS = WScript.CreateObject("Scripting.fileSystemObject")

' -----------------------------------------------------------------------------
' Configuration
' -----------------------------------------------------------------------------
IncludeFile("common.vbs")

BACKUPTEMP = objFS.BuildPath(CACHEDIR, "Backup")
BACKUPS = objFS.BuildPath(ROOT, "Backups")
PAUSEAUTOSAVE = True
BACKUPDUPLICATE = ""
BackupsToKeep = 5
BackupFormat = "7z" ' 7z or zip

IncludeFile("config.vbs")


' -----------------------------------------------------------------------------
' Startup
' -----------------------------------------------------------------------------
CheckPathExists ROOT
CheckPathExists SERVERROOT
CheckPathExists CACHEDIR
CheckFileExists PYTHON
CheckFileExists COMPRESSOR
If (PAUSEAUTOSAVE) Then
  CheckFileExists MULTIPLEXER
End If

WScript.Echo("ROOT: " & ROOT)
WScript.Echo("SERVERROOT: " & SERVERROOT)
WScript.Echo()



' -----------------------------------------------------------------------------
' The main process
' -----------------------------------------------------------------------------
If (PAUSEAUTOSAVE) Then
  WScript.Echo("Stopping Server auto-saves...")
  MinecraftCommand "save-off"

  WScript.Echo("Saving State...")
  MinecraftCommand "save-all"
End If

WScript.Echo("Copying Server...")
'CreateFolder BACKUPTEMP
ServerBackupPath = objFS.BuildPath(BACKUPTEMP, objFS.GetFileName(SERVERROOT))

cmd = "ROBOCOPY /MIR ""{SERVERROOT}"" ""{TEMPROOT}"""
cmd = Replace(cmd, "{SERVERROOT}", SERVERROOT)
cmd = Replace(cmd, "{TEMPROOT}", ServerBackupPath)
objShell.Run cmd, CommandWindowStyle, True

If (PAUSEAUTOSAVE) Then
  WScript.Echo("Restarting Server auto-saves...")
  MinecraftCommand "save-on"
End If


' Keep some old backups
BumpBackupFiles


WScript.Echo("Compressing Backup...")
BackupFile = objFS.BuildPath(Backups, "backup." & BackupFormat)
cmd = """{CMD}"" a ""{FILENAME}"" ""{CACHE}"" -mx9"
cmd = Replace(cmd, "{CACHE}", ServerBackupPath)
cmd = Replace(cmd, "{FILENAME}", BackupFile)
cmd = Replace(cmd, "{CMD}", COMPRESSOR)
objShell.Run cmd, CommandWindowStyle, True


If (BackupDuplicate <> "") Then
	objFS.CopyFile BackupFile, BackupDuplicate, True
End If



Sub BumpBackupFiles()

	For i = BackupsToKeep To 0 Step -1

		NextFilename = objFS.BuildPath(Backups, "backup-" & (i+1) & "." & BackupFormat)
		If (i = 0) Then
			Filename = objFS.BuildPath(Backups, "backup." & BackupFormat)
		Else
			Filename = objFS.BuildPath(Backups, "backup-" & i & "." & BackupFormat)
		End If

		If (objFS.FileExists(Filename)) Then
			If (i = BackupsToKeep) Then
				WScript.Echo "Deleting oldest file " & objFS.GetFileName(Filename)
				objFS.DeleteFile Filename
			Else
				WScript.Echo "Moving old files " & objFS.GetFileName(Filename) & " to " & objFS.GetFileName(NextFilename)
				objFS.MoveFile Filename, NextFilename
			End If
		End If

	Next

End Sub


' -----------------------------------------------------------------------------
' Helper Functions
' -----------------------------------------------------------------------------
Sub IncludeFile(Filename)
  Dim tsInc, strInc
	Const ForReading = 1
  If (objFS.FileExists(Filename)) Then
    Set fil = objFS.getFile(Filename)
    If fil.size > 0 Then
    	Set tsInc = objFS.OpenTextFile(Filename, ForReading)
    	strInc = tsInc.ReadAll
    	tsInc.Close
    	ExecuteGlobal strInc
    End If
  	Set tsInc = Nothing
  Else
    WScript.Echo("Include file not found: " & Filename)
    WScript.Exit(1)
  End If
End Sub