Sub compressor_afterCreateMaps()
	If (OPTIMISE >= 1) Then
	  WScript.Echo "Compressing simple images"
    CompressImages OUTPUT, False
	End IF

	If (OPTIMISE >= 2) Then
	  WScript.Echo "Compressing layer map images"
    CompressImages objFS.BuildPath(OUTPUT, "World Map"), True
	End IF

	If (OPTIMISE >= 3) Then
	  WScript.Echo "Compressing google map images"
    CompressImages objFS.BuildPath(OUTPUT, "Overview"), True
	End If
End Sub



' -----------------------------------------------------------------------------
' Helper Functions
' -----------------------------------------------------------------------------
