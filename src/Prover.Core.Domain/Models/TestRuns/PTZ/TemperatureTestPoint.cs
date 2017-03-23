using System;
using Prover.CommProtocol.Common.Instruments;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.TestRuns
{
    public class TemperatureTestPoint : ITemperatureItems
    {
        private const decimal TempCorrection = 459.67m;
        private const decimal MetericTempCorrection = 273.15m;

        public TemperatureTestPoint(decimal evcBase, TemperatureUnits units, decimal gaugeTemperature)
        {
            Base = evcBase;
            Units = units;
            GaugeTemperature = gaugeTemperature;
        }

        public decimal Base { get; }
        public decimal GasTemperature { get; set; }
        public decimal Factor { get; set; }

        public TemperatureUnits Units { get; set; }
        public decimal GaugeTemperature { get; set; }

        public decimal? PercentError => ActualFactor != 0 ? Math.Round((Factor - ActualFactor) / ActualFactor * 100, 2) : default(decimal?);

        public decimal ActualFactor
        {
            get
            {
                var result = 0m;
                switch (Units)
                {
                    case TemperatureUnits.C:
                        result = (MetericTempCorrection + Base) / (GaugeTemperature + MetericTempCorrection);
                        break;
                    
                    case TemperatureUnits.F:
                        result = (TempCorrection + Base) / (GaugeTemperature + TempCorrection);
                        break;
                    case TemperatureUnits.K:
                        throw new NotImplementedException("Temperature calculations for Kelvin units");
                    case TemperatureUnits.R:
                        throw new NotImplementedException("Temperature calculations for Rankine units");
                }

                return Math.Round(result, 4);
            }
        }

        public void Update(ITemperatureItems temperatureItems)
        {
            Factor = temperatureItems.Factor;
            GasTemperature = temperatureItems.GasTemperature;
        }
    }
}