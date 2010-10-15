Public Class Server

    Private mServerProcess As Process
    Private mServerStatus As Status
    Private mStatusMessage As String
    Private ioWriter As IO.StreamWriter

    Public Event InfoMessage(ByVal Message As String)
    Public Event LogMessage(ByVal Message As String)

    Public Enum Status
        Starting
        Running
        Busy
        Stopping
        Stopped
        Failed
    End Enum

    Public ReadOnly Property CurrentStatus() As Status
        Get
            Return mServerStatus
        End Get
    End Property

    Public ReadOnly Property IsRunning() As Boolean
        Get
            Return mServerStatus = Status.Running
        End Get
    End Property

    Public ReadOnly Property LastStatusMessage() As String
        Get
            Return mStatusMessage
        End Get
    End Property

    Public Sub Server()
        mServerStatus = Status.Stopped
    End Sub

    Public Sub StartServer()
        Dim cmdArgs As String
        cmdArgs = "-jar """ & Config.ServerJar & """"
        cmdArgs = cmdArgs & " nogui"

        ' Configure the main server process
        mServerProcess = New Process
        With mServerProcess.StartInfo
            .WorkingDirectory = Config.MinecraftRoot
            .FileName = Config.Java
            .Arguments = cmdArgs
            .UseShellExecute = False
            .CreateNoWindow = False
            .RedirectStandardError = True
            .RedirectStandardInput = True
            .RedirectStandardOutput = True
        End With
        mServerProcess.EnableRaisingEvents = True

        ' Wire up an event handler to catch messages out of the process
        AddHandler mServerProcess.OutputDataReceived, AddressOf LogOutputHandler
        AddHandler mServerProcess.ErrorDataReceived, AddressOf InfoOutputHandler
        AddHandler mServerProcess.Exited, AddressOf ServerExited

        ' Start the server process
        mServerStatus = Status.Starting
        mServerProcess.Start()

        ' Wire up the writer to send messages to the process
        ioWriter = mServerProcess.StandardInput
        ioWriter.AutoFlush = True

        ' Start listening for output
        mServerProcess.BeginOutputReadLine()
        mServerProcess.BeginErrorReadLine()

    End Sub

    Public Sub Shutdown()
        SendCommand("stop")
    End Sub

    Private Sub ForceShutdown()
        mServerProcess.Kill()
    End Sub

    Private mOnlineUserListReady As Boolean = False
    Private mOnlineUserList As String = ""

    Public Function OnlineUsers() As String
        mOnlineUserListReady = False
        SendCommand("list")
        Do Until mOnlineUserListReady
            Threading.Thread.Sleep(100)
        Loop
        Return mOnlineUserList
    End Function

    Public Sub SendCommand(ByVal Command As String)
        If mServerStatus = Status.Running Then
            ioWriter.WriteLine(Command)
        End If
    End Sub

    Private Sub LogOutputHandler(ByVal sender As Object, _
                              ByVal OutLine As DataReceivedEventArgs)

        If Not OutLine.Data Is Nothing Then
            RaiseEvent LogMessage(OutLine.Data)
        End If
    End Sub

    Private Sub InfoOutputHandler(ByVal sender As Object, _
                              ByVal OutLine As DataReceivedEventArgs)

        Dim T As String
        If Not OutLine.Data Is Nothing Then
            T = OutLine.Data

            If T.Contains(Config.SrvStringReady) Then
                mServerStatus = Status.Running

            ElseIf T.Contains(Config.SrvStringWarningPort) Then
                mServerStatus = Status.Failed
                mStatusMessage = T

            ElseIf T.Contains(Config.SrvStringOnlineUsers) Then
                mOnlineUserList = T.Substring(T.IndexOf(Config.SrvStringOnlineUsers) + Config.SrvStringOnlineUsers.Length)
                mOnlineUserListReady = True

            End If

            RaiseEvent InfoMessage(OutLine.Data)
        End If
    End Sub

    Private Sub ServerExited(ByVal sender As Object, _
                              ByVal args As System.EventArgs)
        mServerStatus = Status.Stopped
    End Sub

End Class
