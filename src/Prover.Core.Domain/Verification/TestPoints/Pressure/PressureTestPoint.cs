using System;
using Prover.Domain.Instrument.Items;
using Prover.Shared.Domain;

namespace Prover.Domain.Verification.TestPoints.Pressure
{
    public class PressureTestPoint : Entity<Guid>
    {
        public decimal GaugePressure { get; private set; }
        public decimal AtmosphericGauge { get; private set; }
        public IPressureItems EvcItems { get; set; }

        public PressureTestPoint(decimal gasGauge, IPressureItems pressureItems)
            : base(Guid.NewGuid())
        {
            EvcItems = pressureItems;
            GaugePressure = gasGauge;
            AtmosphericGauge = 0;
        }

        public PressureTestPoint(Guid id, decimal gaugePressure, decimal atmosphericGauge, IPressureItems evcItems)
            : base(id)
        {
            GaugePressure = gaugePressure;
            AtmosphericGauge = atmosphericGauge;
            EvcItems = evcItems;
        }

        public decimal ActualFactor => EvcItems.Base != 0 ? decimal.Round(GasPressure / EvcItems.Base, 4) : 0.0m;

        public decimal GasPressure
        {
            get
            {
                if (EvcItems.TransducerType == "Absolute")
                    return GaugePressure + AtmosphericGauge;
                return GaugePressure + EvcItems.AtmPressure;
            }
        }

        public decimal? PercentError
            => ActualFactor != 0
                ? Math.Round((EvcItems.Factor - ActualFactor) / ActualFactor * 100, 2)
                : default(decimal?);

        public void SetGaugeValues(decimal gasGauge, decimal atmGauge)
        {
            GaugePressure = gasGauge;
            AtmosphericGauge = atmGauge;
        }

        protected override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}