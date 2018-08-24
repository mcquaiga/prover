using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using SuperFactorCalculations;

namespace Prover.Core.Models.Instruments
{
    public class SuperFactorTest : BaseVerificationTest
    {
        public enum SuperFactorTable
        {
            NX19 = 0,
            AGA8 = 1
        }

        public SuperFactorTest(VerificationTest verificationTest)
        {
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsSuperFactor == true);
            VerificationTest = verificationTest;
        }

        private TemperatureTest TemperatureTest => VerificationTest.TemperatureTest;
        private PressureTest PressureTest => VerificationTest.PressureTest;

        public FactorCalculations SuperFactorCaculations { get; set; }

        //TODO: This will always have to be in Fahrenheit
        [NotMapped]
        public decimal GaugeTemp => (decimal) TemperatureTest.Gauge;

        //TODO: This will always have to be in PSI
        [NotMapped]
        public decimal? GaugePressure => PressureTest.GasGauge;

        [NotMapped]
        public decimal? EvcUnsqrFactor => PressureTest.Items.GetItem(ItemCodes.Pressure.UnsqrFactor).NumericValue;

        [NotMapped]
        public override decimal? ActualFactor
        {
            get
            {
                var fpv = (decimal?)CalculateFpv();
                return fpv.HasValue ? decimal.Round(fpv.Value, 4) : default(decimal?);
            }
        }

        public decimal SuperFactorSquared
        {
            get
            {
                var af = ActualFactor.HasValue ? (double) ActualFactor.Value : 0;
                return (decimal)Math.Pow(af, 2);
            }
        }

        public override decimal? PercentError
        {
            get
            {
                if (EvcUnsqrFactor == null || ActualFactor == null || (ActualFactor == 0)) return null;

                return decimal.Round((decimal) ((EvcUnsqrFactor - ActualFactor) / ActualFactor * 100), 2);                
            }
        }

        [NotMapped]
        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;

        private double? CalculateFpv()
        {
            if (!GaugePressure.HasValue) return null;

            var specGr = VerificationTest?.Instrument?.SpecGr();
            var co2 = VerificationTest?.Instrument?.CO2();
            var n2 = VerificationTest?.Instrument.N2();

            if (specGr.HasValue && co2.HasValue && n2.HasValue)
            {               
                var super = new FactorCalculations((double)specGr, (double) co2, (double) n2,
                    (double) GaugeTemp, (double) GaugePressure);
                return super.SuperFactor;               
            }

            return null;
        }
    }
}