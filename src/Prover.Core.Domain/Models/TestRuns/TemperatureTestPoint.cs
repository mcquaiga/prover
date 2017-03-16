using System;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.TestRuns
{
    public class TemperatureTestPoint
    {
        private const decimal TempCorrection = 459.67m;
        private const decimal MetericTempCorrection = 273.15m;

        public TemperatureTestPoint(decimal evcBase, TemperatureUnits units, decimal gaugeGas)
        {
            EvcBase = evcBase;
            Units = units;
            GaugeGas = gaugeGas;
        }

        public decimal EvcGas { get; set; }
        public decimal EvcFactor { get; set; }
        public decimal EvcBase { get; set; }

        public TemperatureUnits Units { get; set; }
        public decimal GaugeGas { get; set; }

        public decimal? PercentError => ActualFactor != 0 ? Math.Round((EvcFactor - ActualFactor) / ActualFactor * 100, 2) : default(decimal?);

        public decimal ActualFactor
        {
            get
            {
                var result = 0m;
                switch (Units)
                {
                    case TemperatureUnits.C:
                        result = (MetericTempCorrection + EvcBase) / (GaugeGas + MetericTempCorrection);
                        break;
                    
                    case TemperatureUnits.F:
                        result = (TempCorrection + EvcBase) / (GaugeGas + TempCorrection);
                        break;
                    case TemperatureUnits.K:
                        throw new NotImplementedException("Temperature calculations for Kelvin units");
                    case TemperatureUnits.R:
                        throw new NotImplementedException("Temperature calculations for Rankine units");
                }

                return Math.Round(result, 4);
            }
        }
    }
}