Imports InTheHand.Net.Sockets
Imports InTheHand.Net.IrDA
Imports System.Net.Sockets
Imports InTheHand.Net
Imports System.Text
Imports NLog

Public Class IrDAPort
    Implements ICommPort



    '
    ' User selects the first peer!  Do this in your UI...
    '
    Const SelectPeerNum As Integer = 0

    '
    ' The Service Name to use.  We are using IrCOMM, which has the 
    ' Service Name "IrDA:IrCOMM" and it uses IrCOMM Cooked/9-wire mode 
    ' which we enable with setsockopt ( IRLMP_9WIRE_MODE ).
    '
    Const ServiceName As String = "IrDA:IrCOMM"

    Dim cli As IrDAClient
    Dim ep As IrDAEndPoint
    Dim m_enc As Encoding
    Dim strm As NetworkStream
    Dim logger As Logger

    Sub New()
        logger = LogManager.GetCurrentClassLogger

        cli = New IrDAClient()
        Try
            m_enc = Encoding.GetEncoding("x-IA5")
        Catch ex As ArgumentException
            m_enc = Encoding.ASCII
        End Try
        logger.Debug("Initializing IrDA Device.")
        logger.Debug("Using encoding: " & m_enc.EncodingName)

    End Sub


    Public Sub ClosePort() Implements ICommPort.ClosePort
        strm.Close()
        cli.Close()
    End Sub

    Public Sub OpenPort() Implements ICommPort.OpenPort
        LoadIrda()
        cli.Connect(ep)
    End Sub

    Public Function ReceiveDataFromPort() As String Implements ICommPort.ReceiveDataFromPort
        Dim buf = New Byte(256) {}
        strm = cli.GetStream()

        Dim numRead As Integer = strm.Read(buf, 0, buf.Length)
        If strm.CanRead() Then
            Return m_enc.GetString(buf, 0, numRead)
        End If

        Return Nothing
    End Function

    Public Sub SendDataToPort(cmd As String) Implements ICommPort.SendDataToPort
        Dim buf As Byte()
        If cli.Connected = False Then
            cli.Connect(ep)
        End If

        strm = cli.GetStream()

        If strm.CanWrite() Then
            buf = m_enc.GetBytes(cmd)
            strm.Write(buf, 0, buf.Length)
            strm.Flush()
        End If

    End Sub


    Private Sub LoadIrda()
        If cli Is Nothing Then
            cli = New IrDAClient
        End If
        ' Set IrCOMM Cooked/9-wire mode.
        cli.Client.SetSocketOption( _
            IrDASocketOptionLevel.IrLmp, _
            IrDASocketOptionName.NineWireMode, _
            1)      ' equivalent to 'true'

        Dim di As IrDADeviceInfo = selectIrdaPeer(cli)
        If (di Is Nothing) Then
            System.Console.WriteLine(String.Format("No peers discovered" & Environment.NewLine))
            Return
        End If
        ep = New IrDAEndPoint(di.DeviceAddress, ServiceName)

    End Sub


    '
    ' As above, we should add filtering/preferring based on Hint Bits to 
    ' this function.  See the C example.
    '
    ' Return: the selected device, or null if no devices were discovered.
    '
    Private Function selectIrdaPeer(cli As IrDAClient) As IrDADeviceInfo
        '--------------------------------------------
        ' Do the discovery
        '--------------------------------------------
        Dim aDi As IrDADeviceInfo() = cli.DiscoverDevices()
        If (aDi.Length = 0) Then
            Return Nothing    'None found
        End If

        '--------------------------------------------.
        ' Display the discovered devices
        '--------------------------------------------

        Dim i As Integer = 0
        For Each curDi As IrDADeviceInfo In aDi
            System.Console.WriteLine(String.Format("{0}: {1}, addr: {2}, type: {3}", _
                i, curDi.DeviceName, curDi.DeviceAddress, curDi.Hints))
            i = i + 1
            'Console.WriteLine("  [curDi.CharacterSet: {0}]", curDi.CharacterSet)
        Next

        '--------------------------------------------
        ' Select which device
        '--------------------------------------------
        Dim selectNum As Integer = SelectPeerNum    'Do this in your UI...

        System.Console.WriteLine("Selected #{0}" & ControlChars.Lf, selectNum)
        Return aDi(selectNum)  'The selected device.
    End Function

    Public Function BytesToRead() As Integer Implements ICommPort.BytesToRead
        Return cli.Available
    End Function

    Public Sub DiscardInBuffer() Implements ICommPort.DiscardInBuffer
        If strm Is Nothing Then
            strm = cli.GetStream()
        End If
        strm.Flush()
    End Sub

    Public Function IsOpen() As Boolean Implements ICommPort.IsOpen
        If cli Is Nothing Then
            Return False
        End If
        Return cli.Connected
    End Function

    Public Sub Dispose() Implements ICommPort.Dispose
        cli.Close()
        cli.Dispose()
    End Sub
End Class
