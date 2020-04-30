using Devices.Core.Items.ItemGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Shared;

namespace Prover.Calculations.Tests
{
    [TestClass]
    public class TemperatureCalculatorTests
    {
        #region Public Properties

        public Mock<TemperatureItems> TempItems { get; set; }

        #endregion

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            TempItems = new Mock<TemperatureItems>();

            TempItems.Setup(p => p.Units).Returns(TemperatureUnitType.F);
            TempItems.Setup(p => p.Base).Returns(60m);
            TempItems.Setup(p => p.Factor).Returns(0.9458m);
            TempItems.Setup(p => p.GasTemperature).Returns(89.79m);
        }

        [TestMethod]
        public void AssertTempFactorCalculatorTest()
        {
            var gauge = 90m;
            var expected = 0.9454m;
            var items = TempItems.Object;
            var calc = new TemperatureCalculator(items.Units, items.Base, gauge); 
            Assert.AreEqual(expected, calc.CalculateFactor());
        }


        #endregion
    }
}