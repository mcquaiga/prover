using System;
using Prover.InstrumentProtocol.Core.Models.Instrument.Items;
using Prover.Shared.Domain;
using Prover.Shared.Enums;

namespace Prover.Domain.Model.Verifications
{
    public class TemperatureTest : Entity, ITemperatureItems
    {
        private const double MetericTempCorrection = 273.15d;
        private const double TempCorrection = 459.67d;

        public TemperatureTest() : base(Guid.NewGuid())
        {
            GaugeTemperature = 0;
        }

        public double Base { get; set; }
        public double Factor { get; set; }
        public double GasTemperature { get; set; }

        public double GaugeTemperature { get; set; }
        public TemperatureUnits Units { get; set; }

        public double CalculatedFactor()
        {
            double result;
            switch (Units)
            {
                case TemperatureUnits.C:
                    result = (MetericTempCorrection + Base) / (GaugeTemperature + MetericTempCorrection);
                    break;
                case TemperatureUnits.F:
                    result = (TempCorrection + Base) / (GaugeTemperature + TempCorrection);
                    break;
                case TemperatureUnits.K:
                    throw new NotImplementedException("Kelvin units");
                case TemperatureUnits.R:
                    throw new NotImplementedException("Rankine units");
                default:
                    result = 0d;
                    break;
            }

            return Math.Round(result, 4);
        }

        public double? PercentError()
        {
            var actualFactor = CalculatedFactor();
            if (actualFactor == 0) return default(double?);

            return Math.Round((Factor - actualFactor) / actualFactor * 100, 2);
        }
    }
}