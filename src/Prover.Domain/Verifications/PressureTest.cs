using System;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using Prover.Core.Shared.Domain;
using Prover.Core.Shared.Enums;

namespace Prover.Domain.Model.Verifications
{
    public class PressureTest : EntityWithId, IPressureItems
    {
        public PressureTest() : base(Guid.NewGuid())
        {
        }

        public double AtmosphericPressure { get; set; }
        public double Base { get; set; }
        public double Factor { get; set; }
        public double GasPressure { get; set; }
        public double GaugePressure { get; set; }
        public int Range { get; set; }
        public PressureTransducerType TransducerType { get; set; }

        public PressureUnits Units { get; set; }
        public double UnsqrFactor { get; set; }

        public double CalculatedFactor()
        {
            var gasPressure = GaugePressure + AtmosphericPressure;

            return Math.Round(gasPressure / Base, 4);
        }

        public double? PercentError()
        {
            var actualFactor = CalculatedFactor();
            if (actualFactor == 0) return null;

            var error = (Factor - actualFactor) / actualFactor;
            return Math.Round(error * 100, 2);
        }
    }
}