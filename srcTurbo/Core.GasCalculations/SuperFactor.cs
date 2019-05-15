using Core.GasCalculations.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GasCalculations
{
    public static class Supercompressibility
    {
        public static decimal Factor(decimal co2, decimal n2, decimal specGr, decimal gaugePressure, decimal gaugeTemp)
        {
            var zCalc = new ZFactor.ZFactorCalc(specGr, co2, n2, gaugeTemp, gaugePressure);
            return Round.Factor(zCalc.SuperFactor);
        }

        public static decimal FactorSquared(decimal factorValue)
                    => Round.Factor((decimal)Math.Pow((double)factorValue, 2d));
    }
}