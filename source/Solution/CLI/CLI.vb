Imports EnigmaMinecraftManager

Public Class CLI

    Private WithEvents mServer As Server

    Public Sub CLI(ByRef ServerInstance As Server)
        mServer = ServerInstance
    End Sub

    Public Sub WriteLine(ByVal Tag As String, ByVal Message As String)
        Console.WriteLine(Tag & ": " & Message)
    End Sub

    Public Sub WriteLine()
        WriteLine("Console", "")
    End Sub

    Public Sub WriteLine(ByVal Message As String)
        WriteLine("Console", Message)
    End Sub

    Private Sub LogMessage(ByVal Message As String) Handles mServer.LogMessage
        WriteLine("LOG", Message)
    End Sub

    Private Sub InfoMessage(ByVal Message As String) Handles mServer.InfoMessage
        WriteLine("INF", Message)
    End Sub

End Class
