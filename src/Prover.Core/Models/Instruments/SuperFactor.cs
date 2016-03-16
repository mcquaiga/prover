using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{
    public class SuperFactor
    {
        private int SPEC_GR_NUMBER = 53;
        private int N2_NUMBER = 54;
        private int CO2_NUMBER = 55;
        private int SUPER_TABLE_NUMBER = 147;

        public enum SuperFactorTable
        {
            NX19 = 0,
            AGA8 = 1
        }

        public SuperFactor(Instrument instrument, TemperatureTest temperature, PressureTest pressure)
        {
            Instrument = instrument;
            TemperatureTest = temperature;
            PressureTest = pressure;
        }

        public Instrument Instrument { get; }
        public TemperatureTest TemperatureTest { get; }
        public PressureTest PressureTest { get; }

        public decimal? SpecGr => Instrument.Items.GetItem(SPEC_GR_NUMBER).GetNumericValue();
        public decimal? CO2 => Instrument.Items.GetItem(CO2_NUMBER).GetNumericValue();
        public decimal? N2 => Instrument.Items.GetItem(N2_NUMBER).GetNumericValue();
        public SuperFactorTable SuperTable => (SuperFactorTable)Instrument.Items.GetItem(SUPER_TABLE_NUMBER).GetNumericValue();

        //TODO: This will always have to be in Fahrenheit
        [NotMapped]
        public decimal? GaugeTemp
        {
            get
            {
                return (decimal)TemperatureTest.Gauge;
            }
        }

        //TODO: This will always have to be in PSI
        [NotMapped]
        public decimal? GaugePressure
        {
            get
            {
                return PressureTest.GasGauge;
            }
        }

        [NotMapped]
        public decimal? EVCUnsqrFactor
        {
            get
            {
                return PressureTest.EvcUnsqrFactor;
            }
        }
        
        [NotMapped]
        public decimal ActualFactor
        {
            get { return decimal.Round((decimal)CalculateFPV(), 4); }
        }

        private double CalculateFPV()
        {
            return CalculateFactorNX19();
        }

        private double Fp
        {
            get { return 156.47 / (160.8 - 7.22 * (double)SpecGr + (double)CO2 - 0.392 * (double)N2); }
        }

        private double Ft
        {
            get { return 226.29 / (99.15 + 211.9 * (double)SpecGr - (double)CO2 - 1.681 * (double)N2); }
        }

        public decimal SuperFactorSquared
        {
            get { return (decimal)Math.Pow((double)ActualFactor, 2); }
        }

        public decimal PercentError
        {
            get
            {
                return decimal.Round((decimal)(((EVCUnsqrFactor - ActualFactor) / ActualFactor) * 100), 2);
            }
        }

        public bool HasPassed
        {
            get { return (PercentError < 1 && PercentError > -1); }
        }

        private double CalculateFactorNX19()
        {
            // NX-19 calculation.
            // I would like to have all these variables as readonly properties within this class but I dont want throw off the calculations

            double Padj = 0;
            double Tadj = 0;
            double pi = 0;
            double tau = 0;
            double M = 0;
            double n = 0;
            double B = 0;
            double E = 0;
            double S = 0;
            double bb = 0;
            double D = 0;
            double Z = 0;
            double Y = 0;

            Z = (211.9 * (double)SpecGr);
            Y = (1.681 * (double)N2);

            Padj = (double)this.GaugePressure * Fp;
            Tadj = (double)(this.GaugeTemp + 460) * Ft - 460;

            pi = (Padj + 14.7) / 1000;
            tau = ((Tadj + 460) / 500);
            M = 0.0330378 * Math.Pow(tau, -2) - 0.0221323 * Math.Pow(tau, -3) + 0.0161353 * Math.Pow(tau, -5);
            n = (0.265827 * Math.Pow(tau, -2) + 0.0457697 * Math.Pow(tau, -4) - 0.133185 / tau) / M;
            B = (3 - M * n * n) / (9 * M * pi * pi);

            if ((tau >= 1.09 & tau <= 1.4 & pi >= 0 & pi <= 2))
            {
                E = 1 - 0.00075 * Math.Pow(pi, 2.3) * Math.Exp(-20 * (tau - 1.09)) - 0.0011 * Math.Sqrt(tau - 1.09) * pi * pi * Math.Pow((2.17 + 1.4 * Math.Sqrt(tau - 1.09) - pi), 2);
            }
            else {
                if ((tau >= 0.84 & tau <= 1.09 & pi >= 0 & pi <= 1.3))
                {
                    E = 1 - 0.00075 * Math.Pow(pi, 2.3) * (2 - Math.Exp(-20 * (1.09 - tau))) - 1.317 * Math.Pow((1.09 - tau), 4) * pi * (1.69 - pi * pi);
                }
                else {
                    if ((tau >= 0.88 & tau <= 1.09 & pi >= 1.3 & pi <= 2f))
                    {
                        E = 1 - 0.00075 * Math.Pow(pi, 2.3) * (2 - Math.Exp(-20 * (1.09 - tau))) + 0.455 * (200 * Math.Pow((1.09 - tau), 6) - 0.03249 * (1.09 - tau) + 2.0167 * Math.Pow((1.09 - tau), 2) - 18.028 * Math.Pow((1.09 - tau), 3) + 42.844 * Math.Pow((1.09 - tau), 4)) * (pi - 1.3) * (1.69 * Math.Pow(2, 1.25) - pi * pi);
                    }
                    else {
                        if ((tau >= 0.84 & tau <= 0.88 & pi >= 1.3 & pi <= 2f))
                        {
                            S = 1.25 + 80 * Math.Pow((0.88 - tau), 2);
                            E = 1 - 0.00075 * Math.Pow(pi, 2.3) * (2 - Math.Exp(-20 * (1.09 - tau))) + 0.455 * (200 * Math.Pow((1.09 - tau), 6) - 0.03249 * (1.09 - tau) + 2.0167 * Math.Pow((1.09 - tau), 2) - 18.028 * Math.Pow((1.09 - tau), 3) + 42.844 * Math.Pow((1.09 - tau), 4)) * (pi - 1.3) * (1.69 * Math.Pow(2, S) - pi * pi);
                        }
                        else {
                            if ((this.GaugeTemp >= 85 & this.GaugeTemp <= 240 & this.GaugePressure >= 0 & this.GaugePressure <= 2000))
                            {
                                E = 1 - 0.00075 * Math.Pow(pi, 2.3) * Math.Exp(-20 * (tau - 1.09)) - 0.0011 * Math.Sqrt(tau - 1.09) * pi * pi * Math.Pow((2.17 + 1.4 * Math.Sqrt(tau - 1.09) - pi), 2);
                            }
                            else {
                                if ((this.GaugeTemp >= -40 & this.GaugeTemp <= 85 & this.GaugePressure >= 0 & this.GaugePressure <= 1300))
                                {
                                    E = 1 - 0.00075 * Math.Pow(pi, 2.3) * (2 - Math.Exp(-20 * (1.09 - tau))) - 1.317 * Math.Pow((1.09 - tau), 4) * pi * (1.69 - pi * pi);
                                }
                                else {
                                    if ((this.GaugeTemp >= -20 & this.GaugeTemp <= 85 & this.GaugePressure >= 1300 & this.GaugePressure <= 2000))
                                    {
                                        E = 1 - 0.00075 * Math.Pow(pi, 2.3) * (2 - Math.Exp(-20 * (1.09 - tau))) + 0.455 * (200 * Math.Pow((1.09 - tau), 6) - 0.03249 * (1.09 - tau) + 2.0167 * Math.Pow((1.09 - tau), 2) - 18.028 * Math.Pow((1.09 - tau), 3) + 42.844 * Math.Pow((1.09 - tau), 4)) * (pi - 1.3) * (1.69 * Math.Pow(2, 1.25) - pi * pi);
                                    }
                                    else {
                                        if ((this.GaugeTemp >= -40 & this.GaugeTemp <= -20 & this.GaugePressure >= 1300 & this.GaugePressure <= 2000))
                                        {
                                            S = 1.25 + 80 * Math.Pow((0.88 - tau), 2);
                                            E = 1 - 0.00075 * Math.Pow(pi, 2.3) * (2 - Math.Exp(-20 * (1.09 - tau))) + 0.455 * (200 * Math.Pow((1.09 - tau), 6) - 0.03249 * (1.09 - tau) + 2.0167 * Math.Pow((1.09 - tau), 2) - 18.028 * Math.Pow((1.09 - tau), 3) + 42.844 * Math.Pow((1.09 - tau), 4)) * (pi - 1.3) * (1.69 * Math.Pow(2, S) - pi * pi);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            bb = (9 * n - 2 * M * Math.Pow(n, 3)) / (54 * M * Math.Pow(pi, 3)) - (E / (2 * M * Math.Pow(pi, 2)));

            D = bb + Math.Sqrt(Math.Pow(bb, 2) + Math.Pow(B, 3));
            D = Math.Pow(D, (1 / 3));

            return Math.Sqrt((B / D) - D + (n / (3 * pi))) / (1 + (0.00132 / Math.Pow(tau, 3.25)));
        }

    }
}
