using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Core.Models.Instruments.DriveTypes;

namespace Prover.Core.DriveTypes.Tests
{
    [TestClass()]
    public class EnergyTests
    {

        [TestMethod()]
        public void VerifyEnergyCalculationTherms()
        {
            var corrected = (580m - 388m) * 100;
            var energy = new Energy(Energy.Units.Therms, 388m, 580m, 1000.00m, corrected);

            Assert.IsTrue(energy.HasPassed);
        }

        [TestMethod()]
        public void VerifyEnergyCalculationTherms2()
        {
            var corrected = (200m - 100m) * 100;
            var energy = new Energy(Energy.Units.Therms, 100m, 200m, 1000.00m, corrected);

            Assert.IsTrue(energy.HasPassed);
        }
    }
}