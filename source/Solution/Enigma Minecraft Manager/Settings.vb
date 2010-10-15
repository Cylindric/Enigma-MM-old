Public Class Settings

    Public Function GetSettingString(ByVal Key As String)
        Return My.Settings.Item(Key).ToString
    End Function

    Public Sub SaveSettingString(ByVal Key As String, ByVal Value As String)
        My.Settings.Item(Key) = Value
    End Sub

    Public Sub InitialiseDefaults()
        My.Settings.ServerRoot = ""
        My.Settings.ServerJar = "minecraft_server.jar"
    End Sub

End Class
