//using Core.GasCalculations;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using Shared;

//namespace Tests.Core.GasCalculations
//{
//    [TestClass()]
//    public class PressureCalculatorTests
//    {
//        [TestInitialize]
//        public void Initialize()
//        {
//            //PressureValues = new Mock<PressureItems>();

//            //PressureValues.Setup(p => p.AtmosphericPressure).Returns(14.7300m);
//            //PressureValues.Setup(p => p.Base).Returns(14.7300m);
//            //PressureValues.Setup(p => p.Factor).Returns(6.4311m);
//            //PressureValues.Setup(p => p.GasPressure).Returns(80.0m);
//            //PressureValues.Setup(p => p.Range).Returns(100);
//            //PressureValues.Setup(p => p.TransducerType).Returns(PressureTransducerType.Gauge);
//            //PressureValues.Setup(p => p.UnitType).Returns(PressureUnitType.PSIG);
//            //PressureValues.Setup(p => p.UnsqrFactor).Returns(1.0076m);
//        }

//        public Mock<PressureItems> PressureValues
//        { get; set; }


//        [TestMethod()]
//        public void PressureCalculatorTest()
//        {
//            Assert.IsTrue(true);
//        }

//        [TestMethod()]
//        public void GasPressureGaugeTransducerTest()
//        {
//            //var gauge = 80.0m;
//            //var atm = 14.73m;
//            //var expected = gauge + atm;

//            //var mock = new Mock<PressureItems>();
//            //mock.Setup(p => p.TransducerType).Returns(PressureTransducerType.Gauge);

//            //var calc = mock.Object.GetCalculator(gauge, atm);
//            //Assert.IsTrue(calc.GasPressure == expected);
//        }

//        [TestMethod()]
//        public void GasPressureAbsoluteTransducerTest()
//        {
//            var gauge = 80.0m;
//            var atm = 14.73m;
//            var expected = gauge;

//            var mock = new Mock<PressureItems>();
//            mock.Setup(p => p.TransducerType).Returns(PressureTransducerType.Absolute);

//            var calc = mock.Object.GetCalculator(gauge, atm);
//            Assert.IsTrue(calc.GasPressure == expected);
//        }

//        [TestMethod()]
//        public void CalculateFactorTest()
//        {
//            var gauge = 80.0m;
//            var atm = 14.73m;
//            var expected = 6.4311m;
//            var calc = PressureValues.Object.GetCalculator(gauge, atm);
//            Assert.IsTrue(calc.CalculateFactor() == expected);
//        }

//        [TestMethod()]
//        public void GetGaugePressureTest()
//        {
//            var range = 100;
//            var percent = .80m;
//            var expected = 80;

//            var gauge = PressureCalculator.GetGaugePressure(range, percent);

//            Assert.IsTrue(gauge == expected);
//        }
//    }
//}