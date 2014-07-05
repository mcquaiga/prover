Public Interface ICommPort

    Function IsOpen() As Boolean
    Sub OpenPort()
    Sub ClosePort()
    Sub Dispose()
    Function BytesToRead() As Integer
    Function ReceiveDataFromPort() As String
    Sub SendDataToPort(Command As String)
    Sub DiscardInBuffer()
End Interface
