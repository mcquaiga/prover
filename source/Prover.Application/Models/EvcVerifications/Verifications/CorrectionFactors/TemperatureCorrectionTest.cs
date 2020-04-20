using System;
using Devices.Communications.Status;
using Devices.Core.Items.ItemGroups;
using Prover.Calculations;
using Prover.Shared.Extensions;

namespace Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors
{
    public sealed class TemperatureCorrectionTest : CorrectionVerificationTest<TemperatureItems>
    {
        #region Public Properties

        private TemperatureCorrectionTest() { }
        //public TemperatureCorrectionTest(TemperatureItems items, decimal gaugeTemperature, decimal expectedValue, decimal actualValue, decimal percentError) 
        //    : base(items, expectedValue, actualValue, percentError)
        //{
        //    Gauge = gaugeTemperature;
        //    ActualValue = items.Factor;

        //    ExpectedValue = 
        //}

        public TemperatureCorrectionTest(TemperatureItems items, decimal gaugeTemperature)
        {
            Items = items;
            Gauge = gaugeTemperature;
            ActualValue = items.Factor;

            Update(Tolerances.TEMP_ERROR_TOLERANCE);
        }

       
        public decimal Gauge { get; set; }

        #endregion

        /// <inheritdoc />
        protected sealed override Func<ICorrectionCalculator> CalculatorFactory
            => () => new TemperatureCalculator(Items.Units, Items.Base, Gauge);
    }
}