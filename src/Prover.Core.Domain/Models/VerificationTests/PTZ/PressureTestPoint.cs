using System;
using Prover.Domain.Models.Instruments.Items;

namespace Prover.Domain.Models.VerificationTests.PTZ
{
    public class PressureTestPoint
    {
        public PressureTestPoint(decimal gasGauge, IPressureItems pressureItems)
        {
            EvcItems = pressureItems;
            GaugePressure = gasGauge;
            AtmosphericGauge = 0;
        }

        public IPressureItems EvcItems { get; set; }

        public decimal GasPressure
        {
            get
            {
                if (EvcItems.TransducerType == "Absolute")
                    return GaugePressure + AtmosphericGauge;
                return GaugePressure + EvcItems.AtmPressure;
            }
        }

        public decimal GaugePressure { get; private set; }
        public decimal AtmosphericGauge { get; private set; }

        public decimal? PercentError
            => ActualFactor != 0 ? Math.Round((EvcItems.Factor - ActualFactor) / ActualFactor * 100, 2)
                : default(decimal?);

        public decimal ActualFactor
            => EvcItems.Base != 0 ? decimal.Round(GasPressure / EvcItems.Base, 4) : 0.0m;

        public void SetGaugeValues(decimal gasGauge, decimal atmGauge)
        {
            GaugePressure = gasGauge;
            AtmosphericGauge = atmGauge;
        }
    }
}