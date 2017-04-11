using System;
using Prover.Domain.Instrument.Items;
using Prover.Shared.Domain;
using Prover.Shared.Enums;

namespace Prover.Domain.Verification.TestPoints.Temperature
{
    public class TemperatureTestPoint : TestPointBase<ITemperatureItems>
    {
        private const double MetericTempCorrection = 273.15d;
        private const double TempCorrection = 459.67d;
    
        public double GaugeTemperature { get; set; }

        public TemperatureTestPoint() : base(Guid.NewGuid(), null)
        {
            
        }

        public TemperatureTestPoint(double gaugeTemperature, ITemperatureItems evcItems)
            : base(Guid.NewGuid(), evcItems)
        {
            GaugeTemperature = gaugeTemperature;
            EvcItems = evcItems;
        }

        public TemperatureTestPoint(Guid id, ITemperatureItems evcItems, double gaugeTemperature)
            : base(id, evcItems)
        {
            EvcItems = evcItems;
            GaugeTemperature = gaugeTemperature;
        }

        public double? PercentError
        {
            get
            {
                if (ActualFactor == 0) return default(double?);

                return Math.Round((EvcItems.Factor - ActualFactor) / ActualFactor * 100, 2);
            }
        }

        public double ActualFactor
        {
            get
            {
                double result;
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
                        result = 0d;
                        break;
                }

                return Math.Round(result, 4);
            }
        }
       
    }
}