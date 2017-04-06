using System;
using Prover.Domain.Models.Instruments.Items;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.VerificationTests.PTZ
{
    public class TemperatureTestPoint
    {
        private const decimal TempCorrection = 459.67m;
        private const decimal MetericTempCorrection = 273.15m;

        public TemperatureTestPoint(decimal gaugeTemperature, ITemperatureItems temperatureItems)
        {
            GaugeTemperature = gaugeTemperature;
            EvcItems = temperatureItems;
        }

        public ITemperatureItems EvcItems { get; set; }

        public decimal GaugeTemperature { get; set; }

        public decimal ActualFactor
        {
            get
            {
                decimal result;
                switch (EvcItems.Units)
                {
                    case TemperatureUnits.C:
                        result = (MetericTempCorrection + EvcItems.Base) / (GaugeTemperature + MetericTempCorrection);
                        break;
                    case TemperatureUnits.F:
                        result = (TempCorrection + EvcItems.Base) / (GaugeTemperature + TempCorrection);
                        break;
                    case TemperatureUnits.K:
                        throw new NotImplementedException("Kelvin units");
                    case TemperatureUnits.R:
                        throw new NotImplementedException("Rankine units");
                    default:
                        result = 0m;
                        break;
                }

                return Math.Round(result, 4);
            }
        }

        public decimal? PercentError
            =>
                ActualFactor != 0
                    ? Math.Round((EvcItems.Factor - ActualFactor) / ActualFactor * 100, 2)
                    : default(decimal?);
    }
}