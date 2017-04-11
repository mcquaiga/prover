using System;
using Prover.Domain.Instrument.Items;
using Prover.Shared.Domain;

namespace Prover.Domain.Verification.TestPoints.Pressure
{
    public class PressureTestPoint : TestPointBase<IPressureItems>
    {
        public double GaugePressure { get; private set; }
        public double AtmosphericGauge { get; private set; }

        public PressureTestPoint() : base(Guid.NewGuid()) { }

        public PressureTestPoint(Guid id, IPressureItems evcItems, double gaugePressure, double? atmosphericGauge)
            : base(id, evcItems)
        {
            GaugePressure = gaugePressure;
            AtmosphericGauge = atmosphericGauge ?? 0;
            EvcItems = evcItems;
        }

        public double ActualFactor => EvcItems.Base != 0 ? Math.Round(GasPressure / EvcItems.Base, 4) : 0.0d;

        public double GasPressure
        {
            get
            {
                if (EvcItems.TransducerType == "Absolute")
                    return GaugePressure + AtmosphericGauge;
                return GaugePressure + EvcItems.AtmPressure;
            }
        }

        public double? PercentError
            => ActualFactor != 0
                ? Math.Round((EvcItems.Factor - ActualFactor) / ActualFactor * 100, 2)
                : default(double?);

        public void SetGaugeValues(double gasGauge, double atmGauge)
        {
            GaugePressure = gasGauge;
            AtmosphericGauge = atmGauge;
        }
    }
}