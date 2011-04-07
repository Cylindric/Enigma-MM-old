Dim oShell
Set oShell = CreateObject("Wscript.Shell")
If Instr(1, WScript.FullName, "CScript", vbTextCompare) = 0 Then
    oShell.Run "cscript """ & WScript.ScriptFullName & """", 1, False
    WScript.Quit
End If

Const ForReading = 1, ForWriting = 2, ForAppending = 8

Const wikiFileName = "data_values.wiki.txt"
Const itemFileName = "items.xml"
Const DefaultQty = 1
const DefaultMax = 32
Const BlockQty = 64
Const BlockMax = 256


' These items will not be present in the list
Dim ignoredItems()
Redim ignoredItems(0)
AddIgnore(7) 'bedrock - just because it's dangerous
AddIgnore(26) 'bed - this is the block, the item should be used instead
AddIgnore(91) 'cake - this is the block, the item should be used instead


' These items will be included in the list, but be "disabled" - max=0
Dim forbiddenItems()
Redim forbiddenItems(0)
AddForbidden(0) 'air
AddForbidden(7) 'bedrock
AddForbidden(8) 'water
AddForbidden(9) 'water (stationary)
AddForbidden(10) 'lava
AddForbidden(11) 'lava (stationary)
AddForbidden(26) 'bed
AddForbidden(43) 'double slab
AddForbidden(51) 'fire
AddForbidden(52) 'monster spawner
AddForbidden(62) 'burning furnace
AddForbidden(74) 'redstone ore (glowing)
AddForbidden(76) 'redstone torch (on)
AddForbidden(90) 'portal
AddForbidden(46) 'tnt
AddForbidden(94) 'redstone repeater (on)
AddForbidden(95) 'locked chest

Dim blockItems()
Redim blockItems(0)
AddBlockItem("arrow")
AddBlockItem("bedrock")
AddBlockItem("coalore")
AddBlockItem("cobblestone")
AddBlockItem("cobblestonestairs")
AddBlockItem("diamondore")
AddBlockItem("dirt")
AddBlockItem("doubleslabs")
AddBlockItem("farmland")
AddBlockItem("fence")
AddBlockItem("glass")
AddBlockItem("glowingredstoneore")
AddBlockItem("goldore")
AddBlockItem("grass")
AddBlockItem("gravel")
AddBlockItem("ice")
AddBlockItem("ironore")
AddBlockItem("ladders")
AddBlockItem("lapislazuliore")
AddBlockItem("lava")
AddBlockItem("leaves")
AddBlockItem("mossstone")
AddBlockItem("netherrack")
AddBlockItem("obsidian")
AddBlockItem("rails")
AddBlockItem("redstoneore")
AddBlockItem("sand")
AddBlockItem("sandstone")
AddBlockItem("slabs")
AddBlockItem("snow")
AddBlockItem("soulsand")
AddBlockItem("sponge")
AddBlockItem("stationarylava")
AddBlockItem("stationarywater")
AddBlockItem("stick")
AddBlockItem("stone")
AddBlockItem("tnt")
AddBlockItem("torch")
AddBlockItem("water")
AddBlockItem("wood")
AddBlockItem("woodenplank")
AddBlockItem("woodenstairs")
AddBlockItem("wool")

Dim fso
Set fso = CreateObject("Scripting.FileSystemObject")

Dim wikiFile
Set wikiFile = fso.OpenTextFile(wikiFileName, ForReading, True)

Dim itemFile
Set itemFile = fso.OpenTextFile(itemFileName, ForWriting, True)


itemFile.WriteLine("<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>")
itemFile.WriteLine("<?xml-stylesheet type='text/xsl' href='items.xsl'?>")
itemFile.WriteLine("<items xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">")

' Scroll to Block IDs
Dim ContinueStatus
ContinueStatus = False
Do Until wikiFile.AtEndOfStream
    strNextLine = wikiFile.Readline
    If (strNextLine = "== Block IDs ==") Then
        Wscript.Echo "Found Block IDs"
        ContinueStatus = True
        Exit Do
    End If
Loop
If (ContinueStatus = False) Then
    Wscript.Quit
End If


' Loop until Item IDs
Dim parts
Dim id, code, qty, max, counter
Do Until wikiFile.AtEndOfStream
    strNextLine = wikiFile.Readline
    If (strNextLine = "== Inventory Slot Number ==") Then
        ContinueStatus = True
        Exit Do
    End If

    parts = Split(strNextLine, "|")
    If (UBound(parts) >= 9) Then
        id = 0
        code = ""
        name = ""
        qty = DefaultQty
        max = DefaultMax

        Select Case (UBound(parts))
            Case 9, 10, 11, 12
                id = parts(5)
                name = parts(9)

            Case Else
                WScript.Echo "found item (" & UBound(parts) & "): " & Join(parts, "###")
                For Each part In parts
                    WScript.Echo "> " & part
                Next
        End Select

        id = CInt(CleanString(id))
        name = CleanString(name)
        code = Codify(name)
        qty = GetItemQty(code)
        max = GetItemMax(code)

        ' Set any ignored items to zero id to force a skip
        For Each itemID In ignoredItems
            If (id = itemID) Then
                id = 0
                qty = 0
                max = 0
                Exit For
            End If
        Next

        ' Set any forbidden items to zero qty and max
        For Each itemID In forbiddenItems
            If (id = itemID) Then
                qty = 0
                max = 0
                Exit For
            End If
        Next

        ' If the item still has an Id, save it
        If (id > 0) Then
            WriteItem id, code, name, qty, max
        End If

    End If
Loop


itemFile.WriteLine("</items>")

itemFile.Close
wikiFile.Close


Function isBlock(code)
    isBlock = False

    If ((Right(code, 5) = "block")) Then
        isBlock = True
    End If

    If (isBlock = False) Then
        For Each itemCode In blockItems
            If (code = itemCode) Then
                isBlock = True
                Exit For
            End If
        Next
    End If
End Function

Function GetItemQty(code)
    GetItemQty = DefaultQty
    If (isBlock(code) = True) Then GetItemQty = BlockQty
End Function


Function GetItemMax(code)
    GetItemMax = DefaultMax
    If (isBlock(code)  = True) Then GetItemMax = BlockMax
End Function


Function CleanString(input)
    Dim output
    output = input
    output = Replace(output, "[[", "")
    output = Replace(output, "]]", "")
    output = Replace(output, "<span style='color:red'>", "")
    output = Replace(output, "<span style='color:green'>", "")
    output = Replace(output, "</span>", "")
    If (InStr(output, "<sup")) Then
        output = Left(output, InStr(output, "<sup")-1)
    End If
    If (InStr(output, "#")) Then
        output = Right(output, len(output)-InStr(output, "#"))
    End If
    output = LTrim(RTrim(output))
    CleanString = output
End Function


Function Codify(input)
    Dim output
    output = input
    output = LCase(output)
    output = Replace(output, " ", "")
    output = Replace(output, "(", "")
    output = Replace(output, ")", "")
    output = Replace(output, """", "")

    Codify = output
End Function


Function WriteItem(id, code, name, quantity, max)
    itemFile.WriteLine("	<item>")
	itemFile.WriteLine("		<id>" & id & "</id>")
	itemFile.WriteLine("		<code>" & code & "</code>")
	itemFile.WriteLine("		<name>" & name & "</name>")
	itemFile.WriteLine("		<quantity>" & quantity & "</quantity>")
	itemFile.WriteLine("		<max>" & max & "</max>")
	itemFile.WriteLine("	</item>")
End Function

Function AddIgnore(id)
    Dim size
    size = 0
    size = UBound(ignoredItems)
    Redim Preserve ignoredItems(size+1)
    ignoredItems(UBound(ignoredItems)) = id
End Function

Function AddBlockItem(code)
    Dim size
    size = 0
    size = UBound(blockItems)
    Redim Preserve blockItems(size+1)
    blockItems(UBound(blockItems)) = code
End Function

Function AddForbidden(id)
    Dim size
    size = 0
    size = UBound(forbiddenItems)
    Redim Preserve forbiddenItems(size+1)
    forbiddenItems(UBound(forbiddenItems)) = id
End Function
