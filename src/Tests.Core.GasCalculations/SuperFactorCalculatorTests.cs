using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.Core.GasCalculations
{
    [TestClass()]
    public class SuperFactorCalculatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Items = new Mock<ISuperFactorItems>();

            Items.Setup(p => p.Co2).Returns(0.6320m); //55
            Items.Setup(p => p.N2).Returns(0.8190m); //54
            Items.Setup(p => p.SpecGr).Returns(0.5773m); //53
            //","53":"  0.5773","54":"  0.8190","55":"  0.6320"
        }

        public Mock<ISuperFactorItems> Items
        { get; set; }

        
        [TestMethod()]
        public void CalculateTest()
        {
            var gaugeP = 80m;
            var gaugeT = 32m;
            var expected = 1.0082m;

            var calc = Items.Object.Calculate(gaugeT, gaugeP);
            Assert.AreEqual(expected, calc.CalculateFactor());
        }

        [TestMethod()]
        public void CalculateSquaredFactorTest()
        {
            var gaugeP = 80m;
            var gaugeT = 32m;
            var expected = 1.0165m;

            var calc = Items.Object.Calculate(gaugeT, gaugeP);
            var super = calc.CalculateFactor();
            Assert.AreEqual(expected, calc.SquaredFactor());
        }
    }
}