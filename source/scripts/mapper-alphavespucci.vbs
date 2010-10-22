Sub alphavespucci_onCreateMaps()

  If ((CreatePrimaryMaps = False) And (CreateSingleMaps = False) And (CreateLayeredMap = False) And (CreateSliceMaps = False)) Then
    Exit Sub
  End If

  DefaultSave = OUTPUT
  DatedSave = objFS.BuildPath(OUTPUT, "History")
  SlicedSave = objFS.BuildPath(CACHEDIR, "Slices")
  LayerCache = objFS.BuildPath(CACHEDIR, "Layers")
  SlicedSaveThumbs = objFS.BuildPath(SlicedSave, "Thumbs")

  DatedName = "mainmap-{YYYY}{MM}{DD}-{HH}"
  DatedName = Replace(DatedName, "{YYYY}", Year(Now()))
  DatedName = Replace(DatedName, "{MM}", StrPad(Month(Now()), 2))
  DatedName = Replace(DatedName, "{DD}", StrPad(Day(Now()), 2))
  DatedName = Replace(DatedName, "{HH}", StrPad(Hour(Now()), 2))

  WScript.Echo "Generating Alpha Vespucci maps..."

	If (CreateLayeredMap Or CreatePrimaryMaps Or CreateSingleMaps) Then
    CreateFolder DefaultSave
    CreateFolder DatedSave

    AlphaVespucciMap "obleft", "day", "mainmap", DefaultSave, True
    objFS.CopyFile objFS.BuildPath(OUTPUT, "mainmap.png"), objFS.BuildPath(DatedSave, DatedName & ".png")
    AlphaVespucciMap "obleft", "day", DatedName, DatedSave, False

    If (CreateLayeredMap Or CreateSingleMaps) Then
      AlphaVespucciMap "obleft", "night", "nightmap", DefaultSave, True
  	  AlphaVespucciMap "obleft", "cave", "caves", DefaultSave, True
  	  AlphaVespucciMap "obleft", "cavelimit 15", "surfacecaves", DefaultSave, True
  	  AlphaVespucciMap "obleft", "whitelist ""Diamond ore""", "resource-diamond", DefaultSave, True
  	  AlphaVespucciMap "obleft", "whitelist ""Redstone ore""", "resource-redstone", DefaultSave, True
  	  AlphaVespucciMap "obleft", "night -whitelist ""Torch""", "resource-torch", DefaultSave, True
  	  AlphaVespucciMap "flat", "day", "flatmap", DefaultSave, True
    End If
	End If


  If (CreateLayeredMap) Then
    CreateFolder DefaultSave
    CreateFolder LayerCache

    objFS.CopyFile objFS.BuildPath(OUTPUT, "mainmap.png"), objFS.BuildPath(LayerCache, "mainmap.png")
    objFS.CopyFile objFS.BuildPath(OUTPUT, "nightmap.png"), objFS.BuildPath(LayerCache, "nightmap.png")
    objFS.CopyFile objFS.BuildPath(OUTPUT, "caves.png"), objFS.BuildPath(LayerCache, "caves.png")
    objFS.CopyFile objFS.BuildPath(OUTPUT, "surfacecaves.png"), objFS.BuildPath(LayerCache, "surfacecaves.png")
    objFS.CopyFile objFS.BuildPath(OUTPUT, "resource-diamond.png"), objFS.BuildPath(LayerCache, "resource-diamond.png")
    objFS.CopyFile objFS.BuildPath(OUTPUT, "resource-redstone.png"), objFS.BuildPath(LayerCache, "resource-redstone.png")
    objFS.CopyFile objFS.BuildPath(OUTPUT, "resource-torch.png"), objFS.BuildPath(LayerCache, "resource-torch.png")

	  WScript.Echo "Generating layered viewer"
	  Set LayerConfFile = objFS.CreateTextFile(objFS.BuildPath(SERVERROOT, "mapper-layers.conf"), True)
	  LayerConfFile.WriteLine("title:World Map")
	  LayerConfFile.WriteLine("mainmap:Day Map;" & objFS.BuildPath(LayerCache, "mainmap.png"))
	  LayerConfFile.WriteLine("altmap:Night Map;" & objFS.BuildPath(LayerCache, "nightmap.png"))
	  LayerConfFile.WriteLine("dualmap:Torches;" & objFS.BuildPath(LayerCache, "resource-torch.png"))
	  LayerConfFile.WriteLine("overlay:Diamond Ore;" & objFS.BuildPath(LayerCache, "resource-diamond.png"))
	  LayerConfFile.WriteLine("overlay:Redstone Ore;" & objFS.BuildPath(LayerCache, "resource-redstone.png"))
	  LayerConfFile.WriteLine("overlay:Surface Caves;" & objFS.BuildPath(LayerCache, "surfacecaves.png"))
	  LayerConfFile.WriteLine("overlay:All Caves;" & objFS.BuildPath(LayerCache, "caves.png"))
	  LayerConfFile.WriteLine("outputfolder:" & OUTPUT)
	  LayerConfFile.Close
		AlphaVespucciTileViewer
	End If

  If (CreateSliceMaps = True) Then
    WScript.Echo "Generating some bonkers slices"
    CreateFolder SlicedSave

    For i = SLICE_MIN TO SLICE_MAX
      SlicedName = StrPad(i, 3)
      AlphaVespucciMap "obleft", "sliceUpto " & i, SlicedName, SlicedSave
			ResizeImage objFS.BuildPath(SlicedSave, SlicedName) & ".png", objFS.BuildPath(SlicedSaveThumbs, SlicedName) & ".png", SmallSize, SmallSize
    Next
		WScript.Echo("Make a video of the slices...")
    Animate SlicedSaveThumbs, objFS.BuildPath(OUTPUT, "slices.mpg")
		WScript.Echo(IMmsg)
  End If

End Sub



' -----------------------------------------------------------------------------
' Helper Functions
' -----------------------------------------------------------------------------
Sub AlphaVespucciMap(RenderMode, MapType, FullName, SavePath, Thumbs)
  cmd = """{CMD}"" -{MODE} -{TYPE} -fullname ""{FULLNAME}"" -path ""{WORLD}"" -outputdir ""{OUTPUT}"" "' -minimal -nometer -notimer"

  cmd = Replace(cmd, "{CMD}", VESPUCCIMAPPER)
  cmd = Replace(cmd, "{MODE}", RenderMode)
  cmd = Replace(cmd, "{TYPE}", MapType)
  cmd = Replace(cmd, "{FULLNAME}", FullName)
  cmd = Replace(cmd, "{WORLD}", WORLD)
  cmd = Replace(cmd, "{OUTPUT}", SavePath)

  WScript.Echo "Generating " & FullName
  objShell.Run cmd, CommandWindowStyle, True
  ConvertImage objFS.BuildPath(SavePath, FullName & ".png"), objFS.BuildPath(SavePath, FullName & ".jpg")
  If (Thumbs = True) Then
    ResizeImage objFS.BuildPath(SavePath, FullName & ".png"), objFS.BuildPath(SavePath, FullName & "-small.jpg"), SmallSize, SmallSize
    ResizeImage objFS.BuildPath(SavePath, FullName & ".png"), objFS.BuildPath(SavePath, FullName & "-thumb.jpg"), ThumbSize, ThumbSize
  End If
End Sub


Sub AlphaVespucciTileViewer()
  cmd = "{CMD} -tilemode """ & objFS.BuildPath(SERVERROOT, "mapper-layers.conf") & """"
  cmd = Replace(cmd, "{CMD}", VESPUCCIMAPPER)
  WScript.Echo "Generating tile viewer"
  objShell.Run cmd, CommandWindowStyle, True
End Sub
