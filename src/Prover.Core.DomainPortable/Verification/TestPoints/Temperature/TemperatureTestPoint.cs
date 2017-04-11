using System;
using Prover.Core.DomainPortable.Instrument.Items;

namespace Prover.Domain.Verification.TestPoints.Temperature
{
    public class TemperatureTestPoint : Entity<Guid>
    {
        private const decimal MetericTempCorrection = 273.15m;
        private const decimal TempCorrection = 459.67m;

        public TemperatureTestPoint(decimal gaugeTemperature, ITemperatureItems temperatureItems)
            : base(Guid.NewGuid())
        {
            GaugeTemperature = gaugeTemperature;
            EvcItems = temperatureItems;
        }

        public TemperatureTestPoint(Guid id, ITemperatureItems evcItems, decimal gaugeTemperature)
            : base(id)
        {
            EvcItems = evcItems;
            GaugeTemperature = gaugeTemperature;
        }

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

        public ITemperatureItems EvcItems { get; set; }
        public decimal GaugeTemperature { get; set; }

        public decimal? PercentError
            =>
                ActualFactor != 0
                    ? Math.Round((EvcItems.Factor - ActualFactor) / ActualFactor * 100, 2)
                    : default(decimal?);
    }
}