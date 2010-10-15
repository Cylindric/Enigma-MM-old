' -----------------------------------------------------------------------------
' Requirements
' -----------------------------------------------------------------------------
' ImageMagick, including ImageMagickObject OLE option
'    Might have to manually register that OLE: (Need admin rights to setup)
'    regsvr32 "C:\Program Files\ImageMagick-6.6.4-Q16\ImageMagickObject.dll"


' -----------------------------------------------------------------------------
' Global constants
' -----------------------------------------------------------------------------
Const WindowStyleHide = 0
Const WindowStyleShow = 1
Const ForReading = 1
Const ForWriting = 2
Const ForAppending = 8

Const WIDTH = 0
Const HEIGHT = 1
Const OFF_X = 2
Const OFF_Y = 3

' -----------------------------------------------------------------------------
' Configuration
' -----------------------------------------------------------------------------
SCRIPTPATH = objFS.GetParentFolderName(Wscript.ScriptFullName)
UTILPATH = objFS.BuildPath(SCRIPTPATH, "Utils")
ROOT =  objFS.GetParentFolderName(SCRIPTPATH)
SERVERROOT = objFS.BuildPath(ROOT, "Server1")
CACHEDIR = objFS.BuildPath(ROOT, "Cache")
PYTHON = "C:\Program Files\Python27\python.exe"
COMPRESSOR = "C:\Program Files\7-Zip\7z.exe"
MULTIPLEXER = objFS.BuildPath(SERVERROOT, "multiplex_client.py")
CommandWindowStyle = WindowStyleShow
CheckEngine

' -----------------------------------------------------------------------------
' Image Functions
' -----------------------------------------------------------------------------
Function GetDimensions(Filename)
  im = objIM.Identify("-format", "%w" & vbTab & "%h", Filename)
  GetDimensions = Split(im, vbTab)
End Function

Sub CompressImages(Path, Recursive)
	Set objSourceFolder = objFS.GetFolder(Path)
	Set objSourceFiles = objSourceFolder.Files
	Set objSourceFolders = objSourceFolder.SubFolders

  ' Recurse into the folders
  If (Recursive = True) Then
  	For Each objFolder in objSourceFolder
  		CompressImages objFolder.Path, Recursive
  	Next
  End If

  ' Compress the files
	For Each objFile in objSourceFiles
		CompressImage objFile.Path
	Next
End Sub

Sub CompressImage(Filename)
  Dim cmd
  cmd = """{CMD}"" ""{FILENAME}"" /y /q /kt /k0 /s0"

  cmd = Replace(cmd, "{CMD}", PNGC)
  cmd = Replace(cmd, "{FILENAME}", Filename)

  WScript.Echo "Compressing " & objFS.GetFileName(Filename)
  objShell.Run cmd, CommandWindowStyle, True
End Sub

Sub ResizeImage(InFilename, OutFilename, Width, Height)
  Dim cmd
  If (Right(OutFilename, 4) = ".jpg") Then
    cmd = """{CMD}"" ""{INFILENAME}"" -resize {WIDTH}x{HEIGHT} -background white -flatten ""{OUTFILENAME}"""
  Else
    cmd = """{CMD}"" ""{INFILENAME}"" -resize {WIDTH}x{HEIGHT} ""{OUTFILENAME}"""
  End If

  cmd = Replace(cmd, "{CMD}", CONVERTER)
  cmd = Replace(cmd, "{INFILENAME}", InFilename)
  cmd = Replace(cmd, "{OUTFILENAME}", OutFilename)
  cmd = Replace(cmd, "{WIDTH}", Width)
  cmd = Replace(cmd, "{HEIGHT}", Height)

  WScript.Echo "Resizing " & objFS.GetFileName(InFilename)
  objShell.Run cmd, CommandWindowStyle, True
End Sub

Sub ConvertImage(InFilename, OutFilename)
  Dim cmd
  If (Right(OutFilename, 4) = ".jpg") Then
    cmd = """{CMD}"" ""{INFILENAME}"" -background white -flatten ""{OUTFILENAME}"""
  Else
    cmd = """{CMD}"" ""{INFILENAME}"" ""{OUTFILENAME}"""
  End If

  cmd = Replace(cmd, "{CMD}", CONVERTER)
  cmd = Replace(cmd, "{INFILENAME}", InFilename)
  cmd = Replace(cmd, "{OUTFILENAME}", OutFilename)

  WScript.Echo "Converting " & objFS.GetFileName(InFilename)
  objShell.Run cmd, CommandWindowStyle, True
End Sub

Sub ExtentImage(InFilename, OutFilename, Width, Height, PosX, PosY)
  extent = Width & "x" & Height
  extent = extent & PosX
  extent = extent & PosY

  WScript.Echo "Extenting " & objFS.GetFileName(InFilename)
  IMmsg = objIM.Convert("-extent", extent, InFilename, OutFilename)
End Sub

Sub AddText(Filename, Colour, PosX, PosY, Text)
  IMmsg = objIM.Convert("-fill", Colour, "-draw", "text " & PosX & "," & PosY & " """ & Text & """", Filename, Filename)
End Sub

Sub Animate(SourcePath, OutputFile)
  WScript.Echo "Animating " & objFS.GetFileName(OutputFile)
  objIM.Convert "-delay", 20, "-loop", 0, SourcePath & "\*.png", OutputFile
  DeleteFile objFS.BuildPath(objFS.GetParentFolderName(SCRIPTNAME), "ffmpeg2pass-0.log")
End Sub

' -----------------------------------------------------------------------------
' Helper Functions
' -----------------------------------------------------------------------------
Sub MinecraftCommand(Command)
  cmd = """{PYTHON}"" ""{MULTIPLEXER}"" ""{MCCMD}"""
  cmd = Replace(cmd, "{PYTHON}", PYTHON)
  cmd = Replace(cmd, "{MULTIPLEXER}", MULTIPLEXER)
  cmd = Replace(cmd, "{MCCMD}", Command)
  objShell.Run cmd, CommandWindowStyle, True
End Sub

Sub CheckPathExists(Path)
  If (Not objFS.FolderExists(Path)) Then
    WScript.Echo("Error!  Specified path does not exist")
    WScript.Echo(Path)
    WScript.Quit
  End If
End Sub

Sub CheckFileExists(Path)
  If (Not objFS.FileExists(Path)) Then
    WScript.Echo("Error!  Specified file does not exist")
    WScript.Echo(Path)
    WScript.Quit
  End If
End Sub

Sub CreateFolder(Path)
  If (Not objFS.FolderExists(Path)) Then
    objFS.CreateFolder Path
  End If
End Sub

Sub DeleteFile(Filename)
  If (objFS.FileExists(Filename)) Then
    objFS.DeleteFile Filename
  End If
End Sub

Sub CheckEngine
  Dim pcEngine
  pcEngine = LCase(Mid(WScript.FullName, InstrRev(WScript.FullName,"\")+1))
  If Not pcEngine="cscript.exe" Then
    objShell.Run "CSCRIPT.EXE """ & WScript.ScriptFullName & """"
    WScript.Quit
  End If
End Sub

Function StrPad(n, totalDigits)
  If totalDigits > len(n) Then
    StrPad = String(totalDigits-Len(n),"0") & n
  Else
    StrPad = n
  End If
End Function
