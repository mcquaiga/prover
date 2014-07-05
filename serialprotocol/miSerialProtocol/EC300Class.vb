Imports System.Windows.Forms
Imports InTheHand.Net.IrDA

Public Class EC300Class : Inherits miSerialProtocolClass

    Public Enum BaudRateEnum
        b300 = 300
        b600 = 600
        b1200 = 1200
        b2400 = 2400
        b4800 = 4800
        b9600 = 9600
        b19200 = 19200
        b38400 = 38400
    End Enum
    Public Enum AlarmsEnum
        BatteryLowAlarm = 99
        IndexSW1Alarm = 102
        IndexSW2Alarm = 103
        ADAlarm = 104
        AlarmOuput = 108
        PressureLowAlarm = 143
        TemperatureLowAlarm = 144
        PressureHighAlarm = 145
        TemperatureHighAlarm = 146
        DailyCorVolAlarm = 222
        REIAlarm = 435
        RBXAlarm = 176
        TestAlarm = 112
    End Enum

    'Serial communication with the TCI is alot slower then the rest of the instruments
    'The computer is picking up 4 or 5 bytes of information from a larger string of information and is missing the rest

    'I've been receiving errors from the infared software about the device being used by another application.
    'This error appears to happen on the 3rd attempt to access the port
    'At any rate, after the error message, the port does not work until I restart the computer.
    'When this protocol tries to access the port it is freezing the application. e

    Sub New(CommPort As ICommPort)
        MyBase.New(CommPort)
        Me.Instrument = InstrumentTypeCode.EC300
    End Sub

    Public Overrides Sub Connect()
        Dim instr_Code As String = Instrument
        Dim return_code As InstrumentErrorsEnum

        Try
            OpenCommPort()
            ClearCommBuffer()

            'Override the write timeout from the OpenCommPort method
            'The infared connection is a little slower then the serial port.


            If CommState = CommStateEnum.UnlinkedIdle Then
                MessageState = MessageStateEnum.OK_Idle
                CommState = CommStateEnum.WakingItUp
                'After send EOT and ENQ we can expect a one character response from the instrument
                'Wake up the instrument
                _commPort.SendDataToPort(Chr(CommCharEnum.EOT))
                System.Threading.Thread.Sleep(150)
                _commPort.SendDataToPort(Chr(CommCharEnum.ENQ))
                RcvDataFromComm()
                return_code = GetReturnCode()
                'Attempt to connect if there was no error or if we get an Invalid Enquiry(30) back from the instrument, which means the instrument is stuck
                ' in a connection already
                If return_code = InstrumentErrorsEnum.NoError Or return_code = InstrumentErrorsEnum.InvalidEnquiryError Then
                    instr_Code = "99"
                    CommState = CommStateEnum.SigningOn
                    System.Threading.Thread.Sleep(100)
                    SendDataToComm("SN," & i_AccessCode & Chr(CommCharEnum.STX) & "vq" & instr_Code & Chr(CommCharEnum.ETX))
                    return_code = GetReturnCode()
                    If return_code = InstrumentErrorsEnum.NoError Then
                        CommState = CommStateEnum.LinkedIdle
                    Else
                        Throw New InstrumentCommunicationException(return_code)
                    End If
                ElseIf return_code = InstrumentErrorsEnum.NoData Then
                    'No data recieved, is an instrument hooked up?
                    If MessageBox.Show("No Response from Instrument." & vbNewLine & "Try Again?", "Connection Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) = DialogResult.Retry Then
                        Me.Connect()
                    Else
                        Throw New CommExceptions("No Response From Instrument.")
                    End If
                End If
            ElseIf CommState = CommStateEnum.LinkedIdle Then
                'Already Connected to instrument, don't need to do anything
                Console.Write("Already Connected.")
            Else
                Throw New InstrumentBusyException(CommState)
            End If
        Catch ex As CommInUseException
            Throw New CommInUseException(comm.PortName)
        Catch ex As CommExceptions
            Me.Dispose()
            Throw New CommExceptions(ex.Message)
        Catch ex As TimeoutException
            If MessageBox.Show("Connection Timed out" & vbNewLine & "Try Again?", "Failed Connection.", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) = DialogResult.Retry Then
                CommState = CommStateEnum.UnlinkedIdle
                Me.Connect()
            Else
                Throw New CommExceptions("Connection cancelled.")
            End If
        End Try
    End Sub


End Class
