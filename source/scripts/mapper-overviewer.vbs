Sub overviewer_onCreateMaps()
	If ((CreatePrimaryMaps) Or (CreateOverviewerMap)) Then
	  WScript.Echo "Generating Overview"
		OverviewTileViewer
		ExtractWarpLocations
	End If
End Sub



' -----------------------------------------------------------------------------
' Helper Functions
' -----------------------------------------------------------------------------
Sub OverviewTileViewer()
  OverViewerCache = objFS.BuildPath(CACHEDIR, "Overviewer")
  OverViewerOutput = objFS.BuildPath(OUTPUT, "Overviewer")
  CreateFolder OverViewerCache
  CreateFolder OverViewerOutput

	cmd = """{CMD}"" ""{OVERVIEWER}"" --cachedir ""{CACHEDIR}"" ""{WORLD}"" ""{OUTPUT}"""
  cmd = Replace(cmd, "{CMD}", PYTHON)
  cmd = Replace(cmd, "{OVERVIEWER}", OVERVIEWMAPPER)
  cmd = Replace(cmd, "{CACHEDIR}", OverViewerCache)
  cmd = Replace(cmd, "{WORLD}", WORLD)
  cmd = Replace(cmd, "{OUTPUT}", OverViewerOutput)

  WScript.Echo "Generating Overview map"
  objShell.Run cmd, CommandWindowStyle, True
End Sub



' Thanks to http://novylen.net/~minecraft/overviewer/ for the details!
Sub ExtractWarpLocations()
  WarpFileName = objFS.BuildPath(SERVERROOT, "warps.txt")
  MarkerFileName = objFS.BuildPath(OUTPUT, "Overviewer")
  MarkerFileName = objFS.BuildPath(MarkerFileName, "markers.js")

  WScript.Echo "Scanning for warps " & WarpFileName
  Set WarpFile = objFS.OpenTextFile(WarpFileName, ForReading, False)
  AllWarpData = WarpFile.ReadAll
  WarpFile.Close
  Set WarpFile = Nothing
  
  Markers = ""
  Markers = Markers & "var markerData=[" & vbCrLf

  Warps = Split(AllWarpData, vbCrLf)
  For i = 0 To UBound(Warps)
    ThisWarp = Trim(Warps(i))

    If (Len(ThisWarp) > 0) Then
      Warp = Split(ThisWarp, ":")
      WarpName = Warp(0)
      WarpX = Int(Warp(1))
      WarpY = Int(Warp(2))
      WarpZ = Int(Warp(3))
      WarpA = Warp(4)
      WarpB = Warp(5)
      WarpGroup = Warp(6)

      If (WarpGroup = "admins") Then
        'Skip admin warps
        WScript.Echo("Found admin warp: " & WarpName)
      Else
        WScript.Echo("Found warp: " & WarpName)

        MarkerEntry = "	{""msg"": """ & WarpName & """, ""y"": " & WarpY & ", ""z"": " & WarpZ & ", ""x"": " & WarpX & "}," & vbCrLf

        Markers = Markers & MarkerEntry
      End If
    End If

  Next

  Markers = Markers & "];" & vbCrLf
  
  WScript.Echo "Saving markers to " & MarkerFileName
  Set WarpFile = objFS.OpenTextFile(MarkerFileName, ForWriting, True)
  WarpFile.Write Markers
  WarpFile.Close
  Set WarpFile = Nothing

End Sub