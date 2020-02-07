Public Class ECATClass
    Inherits miSerialProtocolClass

    Public Enum BaudRateEnum
        'ECAT only supports the folllowing baud rates
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
        PulserALimitAlarm = 69
        BatteryLowVoltAlarm = 99
        IndexSW1Alarm = 102
        IndexSW2Alarm = 103
        ADFaultAlarm = 104
        PressureOutOfRangeAlarm = 105
        TempOutOfRange = 106
        AlarmOuput = 108
    End Enum

    Sub New(CommPort As ICommPort)
        MyBase.New(CommPort)
        Me.Instrument = InstrumentTypeCode.ECAT
    End Sub

    Public Shadows Sub Connect()
        Try
            MyBase.Connect()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub


    Public Overloads Function ResetAlarms() As InstrumentErrorsEnum
        Dim myArray As Array
        Dim item As Integer

        myArray = System.Enum.GetValues(GetType(AlarmsEnum))

        For Each item In myArray
            If MyBase.RD(item) = AlarmValuesEnum.Alarm Then
                MyBase.WD(item, AlarmValuesEnum.NoAlarm)
            End If
        Next
    End Function

End Class
