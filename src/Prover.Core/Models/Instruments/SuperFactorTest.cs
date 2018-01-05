using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using SuperFactorCalculations;

namespace Prover.Core.Models.Instruments
{
    public sealed class SuperFactorTest : BaseVerificationTest
    {
        protected override decimal PassTolerance => 0.1m;
        
        public SuperFactorTest(VerificationTest verificationTest)
        {
            if (verificationTest == null)
                throw new NullReferenceException(nameof(verificationTest));

            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsSuperFactor == true).ToList();
            VerificationTest = verificationTest;          
        }

        private TemperatureTest TemperatureTest => VerificationTest.TemperatureTest;
        private PressureTest PressureTest => VerificationTest.PressureTest;

        [NotMapped]
        public decimal GaugeTemp => TemperatureTest.GaugeFahrenheit;

        [NotMapped]
        public decimal? GaugePressure => PressureTest?.GasPressurePsi;

        [NotMapped]
        public decimal? EvcUnsqrFactor => PressureTest.Items.GetItem(ItemCodes.Pressure.UnsqrFactor).NumericValue;

        public override decimal? ActualFactor => CalculateFpv();
        public override decimal? EvcFactor => EvcUnsqrFactor;
        public decimal? SuperFactorSquared => ActualFactor.HasValue ? (decimal?) Math.Pow((double) ActualFactor, 2) : null;

        [NotMapped]
        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;

        private decimal? CalculateFpv()
        {
            if (!GaugePressure.HasValue)
                return null;

            var super = new FactorCalculations(
                (double) VerificationTest.Instrument.SpecGr().Value,
                (double) VerificationTest.Instrument.CO2().Value, 
                (double) VerificationTest.Instrument.N2().Value,
                (double) GaugeTemp, 
                (double) GaugePressure.Value
            );

            return decimal.Round((decimal)super.SuperFactor, 4);
        }
    }
}