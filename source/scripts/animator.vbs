' -----------------------------------------------------------------------------
' Create master objects
' -----------------------------------------------------------------------------
Set objShell = WScript.CreateObject("WScript.Shell")
Set objNet = WScript.CreateObject("WScript.Network")
Set objFS = WScript.CreateObject("Scripting.fileSystemObject")
Set objIM = WScript.CreateObject("ImageMagickObject.MagickImage.1")

' -----------------------------------------------------------------------------
' Configuration
' -----------------------------------------------------------------------------
SCRIPTNAME = Wscript.ScriptFullName
SERVERROOT =  objFS.GetParentFolderName(SCRIPTNAME)
ROOT = objFS.GetParentFolderName(SERVERROOT)
WEBROOT = objFS.BuildPath(ROOT, "www")
OUTPUT = objFS.BuildPath(WEBROOT, "maps")
CACHEDIR = objFS.BuildPath(ROOT, "MapCache")
ANIMATECACHE = objFS.BuildPath(CACHEDIR, "history")
SOURCEPATH = objFS.BuildPath(OUTPUT, "history")

WIDTH = 0
HEIGHT = 1
OFF_X = 2
OFF_Y = 3


' -----------------------------------------------------------------------------
' Startup
' -----------------------------------------------------------------------------
IncludeFile("common.vbs")
IncludeFile("config.vbs")
IncludeFile("animator-adjustments.vbs")


' for each file in source dir
Dim objSourceFolder
Dim objSourceFiles
Dim objFile
Set objSourceFolder = objFS.GetFolder(SOURCEPATH)
Set objSourceFiles = objSourceFolder.Files
Dim i, w, h, c, strType
Dim MaxWidth, MaxHeight
Dim IMmsg, IMvars
Dim adjust, extent
Dim FileCount, FileNumber

' Preprocess the adjustments
Dim dX, dY, dXnew, dYnew, dXthis, dYthis
dX = 0
dY = 0
For i = UBound(Adjustments) -1 To 0 Step -1
  dxThis = CInt(Adjustments(i, OFF_X))
  dyThis = CInt(Adjustments(i, OFF_Y))

  If (dxThis > 0) Then
    dXnew = dX + dxThis
  End If
  If (dyThis > 0) Then
    dYnew = dY + dyThis
  End If

  Adjustments(i, OFF_X) = CInt(dX)
  Adjustments(i, OFF_Y) = CInt(dY)
  dX = dXnew
  dY = dYnew
Next



' Get the basic image information
MaxWidth = 0
MaxHeight = 0
FileCount = 0

WScript.Echo("Scanning files...")
For Each objFile In objSourceFiles
  If InStr(UCase(objFile.Name), ".PNG") Then
    FileCount = FileCount + 1

    IMmsg = objIM.Identify("-format", "%w" & vbTab & "%h", objFile.Path)
    IMvars = Split(IMmsg, vbTab)
    w = IMvars(0)
    h = IMvars(1)

    If (w > MaxWidth) Then
     MaxWidth = w
    End If
    If (h > MaxHeight) Then
     MaxHeight = h
    End If

  End If
Next
WScript.Echo "Found: " & FileCount & " PNG files"
WScript.Echo "Max WxH: " & MaxWidth & "x" & MaxHeight & ""

' Process each image
Dim Found, Progress, ImgDate
Progress = CDbl(0.00)
For Each objFile in objSourceFiles
  If InStr(UCase(objFile.Name), ".PNG") Then
    Progress = CDbl(0.00)

    ImgDate = ""
    ImgDate = ImgDate & Mid(objFile.Name, 15, 2) ' dd
    ImgDate = ImgDate & "/" & Mid(objFile.Name, 13, 2) ' mm
    ImgDate = ImgDate & "/" & Mid(objFile.Name, 9, 4) ' yyyy
    ImgDate = ImgDate & " " & Mid(objFile.Name, 18, 2) ' hh
    ImgDate = ImgDate & ":00"

    IMmsg = objIM.Identify("-format", "%w" & vbTab & "%h", objFile.Path)
    IMvars = Split(IMmsg, vbTab)
    w = IMvars(0)
    h = IMvars(1)

    Found = False
    For i = 0 To UBound(Adjustments) - 1
      If ((CInt(w) = CInt(Adjustments(i, WIDTH))) And (CInt(h) = CInt(Adjustments(i, HEIGHT)))) Then
        Found = True
        extent = MaxWidth & "x" & MaxHeight
        extent = extent & "-" & Adjustments(i, OFF_X)
        extent = extent & "-" & Adjustments(i, OFF_Y)
        IMmsg = objIM.Convert("-extent", extent, objFile.Path, ANIMATECACHE & "/" & objFile.Name)
        IMmsg = objIM.Convert("-resize", "640x480", OutputPath & objFile.Name, ANIMATECACHE & "/" & objFile.Name)
        IMmsg = objIM.Convert("-fill", "#ff0000", "-draw", "text 5,15 """ & ImgDate & """", ANIMATECACHE & "/" & objFile.Name, ANIMATECACHE & "/" & objFile.Name)

        WScript.Echo("Extented image " & ImgDate & " - " & extent)
        Exit For
      End If
    Next

    If (Found = False) Then
      WScript.Echo("Dimensions not found in adjustments: " & w & "x" & h)
    End If

  End If
Next

IMmsg = objIM.Convert("-delay", 20, "-loop", 0, ANIMATECACHE & "/mainmap*.png", OUTPUT & "/animated.mpg")

' -----------------------------------------------------------------------------
' Finish
' -----------------------------------------------------------------------------
Set objShell = Nothing
Set objNet = Nothing
Set objFS = Nothing
Set objIM = Nothing
WScript.Echo "Done"





' =============================================================================


' -----------------------------------------------------------------------------
' Helper Functions
' -----------------------------------------------------------------------------
Sub IncludeFile(Filename)
  Dim tsInc, strInc
	Const ForReading = 1
	Set tsInc = objFS.OpenTextFile(Filename, ForReading)
	strInc = tsInc.ReadAll
	tsInc.Close
	Set tsInc = Nothing
	ExecuteGlobal strInc
End Sub