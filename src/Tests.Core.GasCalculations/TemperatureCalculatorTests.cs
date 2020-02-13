using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Items.ItemGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.Core.GasCalculations
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
        public void TemperatureCalculatorTest()
        {
            var gauge = 90m;
            var expected = 0.9454m;

            var calc = TempItems.Object.GetCalculator(gauge);
            Assert.AreEqual(expected, calc.CalculateFactor());
        }
        

        #endregion
    }
}