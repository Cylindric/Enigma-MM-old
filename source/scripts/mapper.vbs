' -----------------------------------------------------------------------------
' Create master objects
' -----------------------------------------------------------------------------
Set objShell = WScript.CreateObject("WScript.Shell")
Set objNet = WScript.CreateObject("WScript.Network")
Set objFS = WScript.CreateObject("Scripting.FileSystemObject")
Set objIM = WScript.CreateObject("ImageMagickObject.MagickImage.1")

' -----------------------------------------------------------------------------
' Configuration
' -----------------------------------------------------------------------------
IncludeFile("common.vbs")

WORLD = objFS.BuildPath(SERVERROOT, "world")
WEBROOT = objFS.BuildPath(ROOT, "www")
OUTPUT = objFS.BuildPath(WEBROOT, "Maps")
PNGC = objFS.BuildPath(UTILPATH, "pngout.exe")
CONVERTER = "C:\Program Files\ImageMagick-6.6.4-Q16\convert.exe"
ANIMATOR = "C:\Program Files\ImageMagick-6.6.4-Q16\animate.exe"
VESPUCCIMAPPER = objFS.BuildPath(objFS.BuildPath(ROOT, "AlphaVespucci"), "AlphaVespucci.exe")
OVERVIEWMAPPER = objFS.BuildPath(objFS.BuildPath(ROOT, "Overviewer"), "gmap.py")

OPTIMISE = 0
SLICE_MIN = 1
SLICE_MAX = 127
ThumbSize = 120
SmallSize = 800

CreatePrimaryMaps = True
CreateSingleMaps = False
CreateLayeredMap = False
CreateGoogleMap = False
CreateSliceMaps = False
CreateHistoryAnim = False

IncludeFile("config.vbs")
IncludeFile("mapper-alphavespucci.vbs")
IncludeFile("mapper-overviewer.vbs")
IncludeFile("mapper-animatehistory.vbs")
IncludeFile("mapper-compressor.vbs")


If Wscript.Arguments.Count > 0 Then
	' Quick run-through to find any 'only' commands
  For i = 0 To Wscript.Arguments.Count - 1
		If (Left(LCase(WScript.Arguments(i)), 5) = "/only") Then
        CreatePrimaryMaps = False
				CreateSingleMaps = False
				CreateLayeredMap = False
				CreateOverviewerMap = False
				CreateSliceMaps = False
				CreateHistoryAnim = False
		End IF
	Next

  For i = 0 To Wscript.Arguments.Count - 1
    Select Case LCase(Wscript.Arguments(i))
      Case "/?", "/h", "/help", "-h", "--help"
        WScript.Echo("Available options:")
        WScript.Echo()
        WScript.Echo("To enable a feature:")
        WScript.Echo("  /PrimaryMaps")
        WScript.Echo("  /SingleMaps")
        WScript.Echo("  /LayeredMap")
        WScript.Echo("  /OverviewerMap")
        WScript.Echo("  /SliceMaps")
        WScript.Echo("  /HistoryAnim")
        WScript.Echo()
        WScript.Echo("To disable a feature:")
        WScript.Echo("  /NoPrimaryMaps")
        WScript.Echo("  /NoSingleMaps")
        WScript.Echo("  /NoLayeredMap")
        WScript.Echo("  /NoOverviewerMap")
        WScript.Echo("  /NoSliceMaps")
        WScript.Echo("  /NoHistoryAnim")
        WScript.Echo()
        WScript.Echo("To force only a single feature:")
        WScript.Echo("  /OnlyPrimaryMaps")
        WScript.Echo("  /OnlySingleMaps")
        WScript.Echo("  /OnlyLayeredMap")
        WScript.Echo("  /OnlyOverviewerMap")
        WScript.Echo("  /OnlySliceMaps")
        WScript.Echo("  /OnlyHistoryAnim")
        WScript.Echo()
        WScript.Quit

      Case "/primarymaps"
        CreatePrimaryMaps = True
      Case "/singlemaps"
        CreateSingleMaps = True
      Case "/layeredmap"
        CreateLayeredMap = True
      Case "/overviewermap"
        CreateOverviewerMap = True
      Case "/slicemaps"
        CreateSliceMaps = True
      Case "/historyanim"
        CreateHistoryAnim = True

      Case "/noprimarymaps"
        CreatePrimaryMaps = False
      Case "/nosinglemaps"
        CreateSingleMaps = False
      Case "/nolayeredmap"
        CreateLayeredMap = False
      Case "/nooverviewermap"
        CreateOverviewerMap = False
      Case "/noslicemaps"
        CreateSliceMaps = False
      Case "/nohistoryanim"
        CreateHistoryAnim = False

      Case "/onlyprimarymaps"
        CreatePrimaryMaps = True
      Case "/onlysinglemaps"
				CreateSingleMaps = True
      Case "/onlylayeredmap"
				CreateLayeredMap = True
      Case "/onlyoverviewermap"
				CreateOverviewerMap = True
      Case "/onlyslicemaps"
				CreateSliceMaps = True
      Case "/onlyhistoryanim"
				CreateHistoryAnim = True

    End Select
  Next
End If



' -----------------------------------------------------------------------------
' Startup
' -----------------------------------------------------------------------------
CheckPathExists ROOT
CheckPathExists SERVERROOT
CheckFileExists PYTHON
CheckFileExists COMPRESSOR

CheckPathExists WORLD
CheckPathExists WEBROOT
CheckFileExists COMPRESSOR
CheckFileExists CONVERTER
CheckFileExists ANIMATOR
CheckFileExists VESPUCCIMAPPER
CheckFileExists OVERVIEWMAPPER
If (OPTIMISE > 0) Then
  CheckFileExists PNGC
End If
CreateFolder CACHEDIR
CreateFolder OUTPUT

WScript.Echo("ROOT: " & ROOT)
WScript.Echo("SERVERROOT: " & SERVERROOT)
WScript.Echo("WORLD: " & WORLD)
WScript.Echo("WEBROOT: " & WEBROOT)
WScript.Echo("OUTPUT: " & OUTPUT)
WScript.Echo("CACHEDIR: " & CACHEDIR)
WScript.Echo("VESPUCCIMAPPER: " & VESPUCCIMAPPER)
WScript.Echo("OVERVIEWMAPPER: " & OVERVIEWMAPPER)
WScript.Echo()
WScript.Echo("")
WScript.Echo("CreatePrimaryMaps: " & CreatePrimaryMaps)
WScript.Echo("CreateSingleMaps: " & CreateSingleMaps)
WScript.Echo("CreateLayeredMap: " & CreateLayeredMap)
WScript.Echo("CreateOverviewerMap: " & CreateOverviewerMap)
WScript.Echo("CreateSliceMaps: " & CreateSliceMaps)
WScript.Echo("CreateHistoryAnim: " & CreateHistoryAnim)



' -----------------------------------------------------------------------------
' The main process
' -----------------------------------------------------------------------------
alphavespucci_onCreateMaps()
overviewer_onCreateMaps()
animatehistory_onCreateMaps()

compressor_afterCreateMaps()



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