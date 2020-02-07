
Public Class MiniATClass : Inherits miSerialProtocolClass
    Public Enum BaudRateEnum
        'Mini AT only supports the folllowing baud rates
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
        PulserBLimitAlarm = 70
        PulserCLimitAlarm = 71
        BatteryLowAlarm = 99
        BatteryCyclesAlarm = 100
        MemoryBatteryLow = 101
        IndexSW1Alarm = 102
        IndexSW2Alarm = 103
        MiscAlarm = 104
        PressureOutOfRange = 105
        TemperatureOutOfRange = 106
        AlarmOuput = 108
        PressureLowAlarm = 143
        TemperatureLowAlarm = 144
        PressureHighAlarm = 145
        TemperatureHighAlarm = 146
        FlowRateHighAlarm = 163
        RBXAlarm = 176
        DailyCorVolAlarm = 222
        PLogHighAlarm = 451
        PLogLowAlarm = 452
        FlowRateLowAlarm = 461
        ErrorItem = 439
    End Enum

    Sub New(CommPort As ICommPort)
        MyBase.New(CommPort)
        Me.Instrument = InstrumentTypeCode.MiniAT
    End Sub

    Public Shadows Sub Connect()
        Try
            MyBase.Connect()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

End Class
