Public Class FactorCalculations
    Private sSpecGr As Double
    Private sCO2 As Double
    Private sN2 As Double
    Private sGaugeTemp As Double
    Private sGaugePressure As Double
    Private sEvcUnsqr As Double
    Private fpv As Double

    Public Enum SuperFactorItems
        SpecGR = 53
        N2 = 54
        CO2 = 55
    End Enum

    Public Enum SuperTables
        NX19 = 0
        AGA8 = 1
    End Enum

    Sub New(SpecGr As Double, CO2 As Double, N2 As Double, Temp As Double, GaugePressure As Double)
        Me.SpecGr = SpecGr
        Me.CO2 = CO2
        Me.N2 = N2
        Me.GaugeTemp = Temp
        Me.GaugePressure = GaugePressure
    End Sub

    Public Property SpecGr As Double
        Get
            Return sSpecGr
        End Get
        Set
            sSpecGr = value
        End Set
    End Property

    Public Property CO2 As Double
        Get
            Return sCO2
        End Get
        Set
            sCO2 = value
        End Set
    End Property

    Public Property N2 As Double
        Get
            Return sN2
        End Get
        Set
            sN2 = value
        End Set
    End Property

    'This will always have to be in Fahrenheit
    Public Property GaugeTemp As Double
        Get
            Return sGaugeTemp
        End Get
        Set
            sGaugeTemp = value
        End Set
    End Property

    'This will always have to be in PSI
    Public Property GaugePressure As Double
        Get
            Return sGaugePressure
        End Get
        Set
            sGaugePressure = value
        End Set
    End Property

    Public Property EVCUnsqrSuper As Double
        Get
            Return sEvcUnsqr
        End Get
        Set
            sEvcUnsqr = value
        End Set
    End Property

    Private ReadOnly Property Fp As Double
        Get
            Return 156.47/(160.8 - 7.22*Me.SpecGr + Me.CO2 - 0.392*Me.N2)
        End Get
    End Property

    Private ReadOnly Property Ft As Double
        Get
            Return 226.29/(99.15 + 211.9*Me.SpecGr - Me.CO2 - 1.681*Me.N2)
        End Get
    End Property

    Public ReadOnly Property SuperFactor As Double
        Get
            CalcFPV()
            Return fpv
        End Get
    End Property

    Public ReadOnly Property SuperFactorSquared As Double
        Get
            Return Math.Pow(SuperFactor, 2)
        End Get
    End Property

    Public Function CalculatePercentError() As Double
        Return ((EVCUnsqrSuper - SuperFactor)/SuperFactor)*100
    End Function

    Private Sub CalcFPV()
        ' NX-19 calculation.
        ' I would like to have all these variables as readonly properties within this class but I dont want throw off the calculations

        Dim Padj As Double
        Dim Tadj As Double
        Dim pi As Double
        Dim tau As Double
        Dim M As Double
        Dim n As Double
        Dim B As Double
        Dim E As Double
        Dim S As Double
        Dim bb As Double
        Dim D As Double
        Dim Z As Double
        Dim Y As Double

        Z = (211.9*Me.SpecGr)
        Y = (1.681*Me.N2)

        Padj = Me.GaugePressure*Fp
        Tadj = (Me.GaugeTemp + 460)*Ft - 460

        pi = (Padj + 14.7)/1000
        tau = (Tadj + 460)/500
        M = 0.0330378*tau^- 2 - 0.0221323*tau^- 3 + 0.0161353*tau^- 5
        n = (0.265827*tau^- 2 + 0.0457697*tau^- 4 - 0.133185/tau)/M
        B = (3 - M*n*n)/(9*M*pi*pi)

        If (tau >= 1.09 And tau <= 1.4 And pi >= 0 And pi <= 2) Then
            E = 1 - 0.00075*pi^2.3*Math.Exp(- 20*(tau - 1.09)) -
                0.0011*Math.Sqrt(tau - 1.09)*pi*pi*(2.17 + 1.4*Math.Sqrt(tau - 1.09) - pi)^2
        Else
            If (tau >= 0.84 And tau <= 1.09 And pi >= 0 And pi <= 1.3) Then
                E = 1 - 0.00075*pi^2.3*(2 - Math.Exp(- 20*(1.09 - tau))) - 1.317*(1.09 - tau)^4*pi*(1.69 - pi*pi)
            Else
                If (tau >= 0.88 And tau <= 1.09 And pi >= 1.3 And pi <= 2.0!) Then
                    E = 1 - 0.00075*pi^2.3*(2 - Math.Exp(- 20*(1.09 - tau))) +
                        0.455*
                        (200*(1.09 - tau)^6 - 0.03249*(1.09 - tau) + 2.0167*(1.09 - tau)^2 - 18.028*(1.09 - tau)^3 +
                         42.844*(1.09 - tau)^4)*(pi - 1.3)*(1.69*2^1.25 - pi*pi)
                Else
                    If (tau >= 0.84 And tau <= 0.88 And pi >= 1.3 And pi <= 2.0!) Then
                        S = 1.25 + 80*(0.88 - tau)^2
                        E = 1 - 0.00075*pi^2.3*(2 - Math.Exp(- 20*(1.09 - tau))) +
                            0.455*
                            (200*(1.09 - tau)^6 - 0.03249*(1.09 - tau) + 2.0167*(1.09 - tau)^2 - 18.028*(1.09 - tau)^3 +
                             42.844*(1.09 - tau)^4)*(pi - 1.3)*(1.69*2^S - pi*pi)
                    Else
                        If _
                            (Me.GaugeTemp >= 85 And Me.GaugeTemp <= 240 And Me.GaugePressure >= 0 And
                             Me.GaugePressure <= 2000) Then
                            E = 1 - 0.00075*pi^2.3*Math.Exp(- 20*(tau - 1.09)) -
                                0.0011*Math.Sqrt(tau - 1.09)*pi*pi*(2.17 + 1.4*Math.Sqrt(tau - 1.09) - pi)^2
                        Else
                            If _
                                (Me.GaugeTemp >= - 40 And Me.GaugeTemp <= 85 And Me.GaugePressure >= 0 And
                                 Me.GaugePressure <= 1300) Then
                                E = 1 - 0.00075*pi^2.3*(2 - Math.Exp(- 20*(1.09 - tau))) -
                                    1.317*(1.09 - tau)^4*pi*(1.69 - pi*pi)
                            Else
                                If _
                                    (Me.GaugeTemp >= - 20 And Me.GaugeTemp <= 85 And Me.GaugePressure >= 1300 And
                                     Me.GaugePressure <= 2000) Then
                                    E = 1 - 0.00075*pi^2.3*(2 - Math.Exp(- 20*(1.09 - tau))) +
                                        0.455*
                                        (200*(1.09 - tau)^6 - 0.03249*(1.09 - tau) + 2.0167*(1.09 - tau)^2 -
                                         18.028*(1.09 - tau)^3 + 42.844*(1.09 - tau)^4)*(pi - 1.3)*(1.69*2^1.25 - pi*pi)
                                Else
                                    If _
                                        (Me.GaugeTemp >= - 40 And Me.GaugeTemp <= - 20 And Me.GaugePressure >= 1300 And
                                         Me.GaugePressure <= 2000) Then
                                        S = 1.25 + 80*(0.88 - tau)^2
                                        E = 1 - 0.00075*pi^2.3*(2 - Math.Exp(- 20*(1.09 - tau))) +
                                            0.455*
                                            (200*(1.09 - tau)^6 - 0.03249*(1.09 - tau) + 2.0167*(1.09 - tau)^2 -
                                             18.028*(1.09 - tau)^3 + 42.844*(1.09 - tau)^4)*(pi - 1.3)*
                                            (1.69*2^S - pi*pi)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If

        bb = (9*n - 2*M*Math.Pow(n, 3))/(54*M*Math.Pow(pi, 3)) - (E/(2*M*Math.Pow(pi, 2)))

        D = bb + Math.Sqrt(Math.Pow(bb, 2) + Math.Pow(B, 3))
        D = Math.Pow(D, (1/3))

        fpv = Math.Sqrt((B/D) - D + (n/(3*pi)))/(1 + (0.00132/Math.Pow(tau, 3.25)))
        fpv = Format(fpv, "0.000000")
    End Sub
End Class
