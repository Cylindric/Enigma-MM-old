IncludeFile("animatehistory-adjustments.vbs")

Sub animatehistory_onCreateMaps()
  If (CreateHistoryAnim = False) Then
    Exit Sub
  End If
  
  AnimateHistoryCache = objFS.BuildPath(CACHEDIR, "History")
  AnimateHistoryThumbCache = objFS.BuildPath(AnimateHistoryCache, "Thumbs")
  AnimateHistoryOutput = objFS.BuildPath(OUTPUT, "History")
  CreateFolder AnimateHistoryOutput
  CreateFolder AnimateHistoryThumbCache
  CreateFolder AnimateHistoryOutput

  ' Preprocess the adjustments
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
  Set objSourceFolder = objFS.GetFolder(AnimateHistoryOutput)
  Set objSourceFiles = objSourceFolder.Files
  For Each objFile in objSourceFiles
    If InStr(UCase(objFile.Name), ".PNG") Then
      FileCount = FileCount + 1

      Dimensions = GetDimensions(objFile.Path)
      w = Dimensions(0)
      h = Dimensions(1)

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
  Dim Found, Progress, ImgDate, FileNum
  FileNum = 0
  Progress = CDbl(0.00)
  For Each objFile in objSourceFiles
    If InStr(UCase(objFile.Name), ".PNG") Then
      FileNum = FileNum + 1
      Progress = CDbl(CDbl(FileNum) / CDbl(FileCount))
      WScript.Echo(Round(Progress * 100) & "%")

      SourceFilename = objFile.Path
      CacheFilename = objFS.BuildPath(AnimateHistoryCache, objFile.Name)
      CacheThumbFilename = objFS.BuildPath(AnimateHistoryThumbCache, objFile.Name)


      ImgDate = ""
      ImgDate = ImgDate & Mid(objFile.Name, 15, 2) ' dd
      ImgDate = ImgDate & "/" & Mid(objFile.Name, 13, 2) ' mm
      ImgDate = ImgDate & "/" & Mid(objFile.Name, 9, 4) ' yyyy
      ImgDate = ImgDate & " " & Mid(objFile.Name, 18, 2) ' hh
      ImgDate = ImgDate & ":00"
      

      Dimensions = GetDimensions(SourceFilename)
      w = Dimensions(0)
      h = Dimensions(1)

      Found = False
      For i = 0 To UBound(Adjustments) - 1
        If ((CInt(w) = CInt(Adjustments(i, WIDTH))) And (CInt(h) = CInt(Adjustments(i, HEIGHT)))) Then
          Found = True
          PosX = "-" & Adjustments(i, OFF_X)
          PosY = "-" & Adjustments(i, OFF_Y)

          ' If cache file already exists, and is max size already, skip
          AlreadyProcessed = False
          If (objFS.FileExists(CacheFilename)) Then
            ExistingDimensions = GetDimensions(CacheFilename)
            If ((ExistingDimensions(0) = MaxWidth) And (ExistingDimensions(1) = MaxHeight)) Then
              AlreadyProcessed = True
            End If
          End If
          If (AlreadyProcessed = False) Then
            ExtentImage SourceFilename, CacheFilename, MaxWidth, MaxHeight, PosX, PosY
            ResizeImage CacheFilename, CacheThumbFilename, SmallSize, SmallSize
            AddText CacheThumbFilename, "#ff0000", 5, 15, ImgDate
          End If

          Exit For
        End If
      Next

      If (Found = False) Then
        WScript.Echo("Dimensions not found in adjustments: " & w & "x" & h)
      End If

    End If
  Next

  Animate AnimateHistoryThumbCache, objFS.BuildPath(OUTPUT, "history.mpg")

End Sub



' -----------------------------------------------------------------------------
' Helper Functions
' -----------------------------------------------------------------------------
