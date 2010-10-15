Imports EnigmaMinecraftManager
Imports EnigmaMinecraftManager.Server.Status

Module Main

    Dim WithEvents ServerProcess As Server
    Dim Con As CLI

    Public Sub Main()

        ' Start a console
        Con = New CLI

        Console.WriteLine("Initialising server...")
        ServerProcess = New Server

        Con.WriteLine("Starting server..")
        ServerProcess.StartServer()
        While Not ServerProcess.IsRunning
            Threading.Thread.Sleep(500)
            If ServerProcess.CurrentStatus = Failed Then
                Exit While
            End If
        End While

        If ServerProcess.CurrentStatus = Running Then
            Con.WriteLine("started.")
        Else
            Con.WriteLine("Failed to start server.")
            Con.WriteLine()
        End If

        Dim cmd As String = ""
        Dim key As ConsoleKeyInfo

        While ServerProcess.IsRunning
            If (Console.KeyAvailable) Then
                key = Console.ReadKey()
                Select Case key.Key
                    Case ConsoleKey.Backspace
                        cmd = cmd.Substring(0, cmd.Length() - 1)
                    Case ConsoleKey.Enter
                        ExecuteCommand(cmd)
                        cmd = ""
                    Case Else
                        cmd = cmd & key.KeyChar
                End Select
            Else
                Threading.Thread.Sleep(100)
            End If
        End While

    End Sub

    Private Sub ExecuteCommand(ByVal Command As String)
        If Command.Equals("list") Then
            Con.WriteLine("Online users: " & ServerProcess.OnlineUsers())
        Else
            ServerProcess.SendCommand(Command)
        End If
    End Sub

End Module
