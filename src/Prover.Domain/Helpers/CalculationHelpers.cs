using System;

namespace Prover.Domain.Model.Helpers
{
    public class CalculationHelpers
    {
        public static double? CalculatePercentError(double calculatedValue, double actualValue)
        {
            if (calculatedValue == 0) return default(double?);

            var error = (actualValue - calculatedValue) / calculatedValue * 100;

            return Math.Round(error, 2);
        }
    }
}