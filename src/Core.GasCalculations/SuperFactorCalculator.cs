using System;
using Core.GasCalculations.ZFactor;
using Devices.Core.Items.ItemGroups;
using Shared.Extensions;

namespace Core.GasCalculations
{
    public class SuperFactorCalculator : ICorrectionCalculator
    {
        private readonly ZFactorCalc _zCalc;

        public SuperFactorCalculator(decimal co2, decimal n2, decimal specGr, decimal gaugeTempF,
            decimal gaugePressurePsi)
        {
            _zCalc = new ZFactorCalc(co2, n2, specGr, gaugeTempF, gaugePressurePsi);
        }

        #region Public Properties

        public decimal SquaredFactor()
            => Calculators.SquaredFactor(CalculateFactor());
        #endregion

        #region Public Methods

        public decimal CalculateFactor()
        {
            return Round.Factor(_zCalc.SuperFactor);
        }

        

        #endregion
    }
}