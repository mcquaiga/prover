Imports System.IO.Ports
Public Class SerialPort
    Implements ICommPort



    Private WithEvents comm As System.IO.Ports.SerialPort
    Private _portName As String
    Private _baudRate As BaudRateEnum
    Private _timeOut As Integer

    Sub New(ByVal PortName As String, ByVal BaudRate As BaudRateEnum, Optional ByVal Timeout As Integer = 50)
        comm = New System.IO.Ports.SerialPort
        _portName = PortName
        _baudRate = BaudRate
        _timeOut = Timeout

        With comm
            .PortName = _portName
            .BaudRate = _baudRate
            .NewLine = "\\"
            .ReadTimeout = 200
            .WriteTimeout = 150
            .DtrEnable = True
            .RtsEnable = True
        End With
    End Sub

    Public Sub ClosePort() Implements ICommPort.ClosePort
        comm.Close()
    End Sub

    Public Sub OpenPort() Implements ICommPort.OpenPort
        If Not comm.IsOpen Then
            With comm
                'This will throw an exception if the port is already in use
                Try
                    .Open()
                Catch ex As Exception
                    'logger.Error(ex.Message)
                    Throw New CommInUseException(comm.PortName)
                End Try
            End With
        End If
    End Sub

    Public Function ReceiveDataFromPort() As String Implements ICommPort.ReceiveDataFromPort
        Return comm.ReadExisting()
    End Function

    Public Sub SendDataToPort(Command As String) Implements ICommPort.SendDataToPort
        comm.Write(Command)
    End Sub

    Public Function BytesToRead() As Integer Implements ICommPort.BytesToRead
        Return comm.BytesToRead()
    End Function

    Public Sub DiscardInBuffer() Implements ICommPort.DiscardInBuffer
        comm.DiscardInBuffer()
    End Sub

    Public Function IsOpen() As Boolean Implements ICommPort.IsOpen
        If comm Is Nothing Then Return False
        Return comm.IsOpen()
    End Function

    Public Sub Dispose() Implements ICommPort.Dispose
        comm.Close()
        comm.Dispose()

    End Sub
End Class
