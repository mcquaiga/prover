using Prover.Core.Extensions;
using SuperFactorCalculations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prover.Core.Models.Instruments
{
    public class SuperFactorTest : BaseVerificationTest
    {
        private int SPEC_GR_NUMBER = 53;
        private int N2_NUMBER = 54;
        private int CO2_NUMBER = 55;
        private int SUPER_TABLE_NUMBER = 147;
        private TemperatureTest TemperatureTest => VerificationTest.TemperatureTest;
        private PressureTest PressureTest => VerificationTest.PressureTest;

        public FactorCalculations SuperFactorCaculations { get; set; }

        public enum SuperFactorTable
        {
            NX19 = 0,
            AGA8 = 1
        }

        public SuperFactorTest()
        {
        }

        public SuperFactorTest(VerificationTest verificationTest)
        {
            VerificationTest = verificationTest;
        }

        //TODO: This will always have to be in Fahrenheit
        [NotMapped]
        public decimal GaugeTemp => (decimal)TemperatureTest.Gauge;

        //TODO: This will always have to be in PSI
        [NotMapped]
        public decimal? GaugePressure => PressureTest.GasGauge;

        [NotMapped]
        public decimal? EvcUnsqrFactor => PressureTest.ItemValues.EvcUnsqrFactor();

        [NotMapped]
        public override decimal? ActualFactor
        {
            get { return decimal.Round((decimal)CalculateFPV(), 4); }
        }

        private double CalculateFPV()
        {
            var super = new FactorCalculations((double)VerificationTest.Instrument.SpecGr().Value, (double)VerificationTest.Instrument.CO2().Value, (double)VerificationTest.Instrument.N2().Value, (double)GaugeTemp, (double)GaugePressure);
            return super.SuperFactor;
        }

        private double Fp
        {
            get { return 156.47 / (160.8 - 7.22 * (double)VerificationTest.Instrument.SpecGr().Value + (double)VerificationTest.Instrument.CO2().Value - 0.392 * (double)VerificationTest.Instrument.N2().Value); }
        }

        private double Ft
        {
            get { return 226.29 / (99.15 + 211.9 * (double)VerificationTest.Instrument.SpecGr().Value - (double)VerificationTest.Instrument.CO2().Value - 1.681 * (double)VerificationTest.Instrument.N2().Value); }
        }

        public decimal SuperFactorSquared
        {
            get { return (decimal)Math.Pow((double)ActualFactor, 2); }
        }

        public override decimal? PercentError
        {
            get
            {
                if (EvcUnsqrFactor == null || ActualFactor == 0) return null;
                return decimal.Round((decimal)(((EvcUnsqrFactor - ActualFactor) / ActualFactor) * 100), 2);
            }
        }

        public VerificationTest VerificationTest { get; private set; }
    }
}
