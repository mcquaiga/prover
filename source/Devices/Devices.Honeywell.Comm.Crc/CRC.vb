Public Module CRC
    Public Function CalcCRC(body As String) As String
        Dim x As Integer
        Dim y As Integer
        Dim inChar As Long
        Dim CRC As Long = 0
        Try
            For x = 1 To Len(body)
                inChar = Asc(Mid$(body, x, 1))
                CRC = CRC Xor (inChar * &H100)
                For y = 1 To 8
                    If CRC And 32768 Then
                        CRC = (CRC * 2) Xor &H1021
                    Else
                        CRC = CRC * 2
                    End If
                Next y
                CRC = CRC And 65535
            Next x
            Return Left$("0000", 4 - Len(Hex(CRC))) & Hex(CRC)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            CalcCRC = ""
        End Try
    End Function
End Module
