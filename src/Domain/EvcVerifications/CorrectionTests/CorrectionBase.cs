using System;
using System.Runtime.Serialization;
using Core.GasCalculations;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Domain.Interfaces;
using Shared.Domain;
using Shared.Extensions;

namespace Domain.EvcVerifications.CorrectionTests
{
    public interface IVerificationTest : IAssertPassFail
    {
    }

    public enum CorrectionFactorTestType
    {
        Super,
        Pressure,
        Temperature,
        CorrectedVolume,
        UncorrectedVolume,
        Energy
    }

    public class CorrectionTest : BaseEntity, IVerificationTest
    {
        public CorrectionTest(CorrectionFactorTestType testType, decimal actualFactor, decimal expectedFactor)
        {
            TestType = testType;
            ActualValue = actualFactor;
            ExpectedValue = expectedFactor;
        }

        public CorrectionTest(CorrectionFactorTestType testType, decimal actualFactor)
        {
            TestType = testType;
            ActualValue = actualFactor;
        }

        public CorrectionTest(CorrectionFactorTestType testType)
        {
            TestType = testType;
        }

        private CorrectionTest()
        {
        }

        #region Public Properties

        public CorrectionFactorTestType TestType { get; }

        public decimal ExpectedValue { get; set; }

        public decimal ActualValue { get; set; }

        public virtual decimal PassTolerance => Global.TEMP_ERROR_TOLERANCE;

        #endregion

        #region Public Methods

        public virtual CorrectionTest GetValue()
        {
            return this;
        }

        public virtual void SetValues(decimal actualFactor, decimal expectedFactor)
        {
            ActualValue = actualFactor;
            ExpectedValue = expectedFactor;
        }

        public virtual decimal Variance(decimal expectedValue, decimal actualValue)
        {
            return expectedValue == 0
                ? 100m
                : Round.Factor((actualValue - expectedValue) / expectedValue * 100);
        }

        public bool HasPassed()
        {
            return Variance(ExpectedValue, ActualValue).IsBetween(PassTolerance);
        }

        #endregion
    }

    public static class CorrectionFactory
    {
        #region Public Methods

        public static CorrectionTest Create(CorrectionFactorTestType testType, ICorrectionCalculator calculator,
            decimal actualFactor)
        {
            var test = new CorrectionTest(testType, actualFactor);
            var testCalc = new CorrectionTestCalculatorDecorator(calculator, test);
            testCalc.CalculateFactors();
            return test;
            //return testCalc;
        }

        public static CorrectionTest Create(CorrectionFactorTestType testType, decimal actualFactor,
            decimal expectedFactor)
        {
            return new CorrectionTest(testType, actualFactor, expectedFactor);
        }

        public static CorrectionTest Create(CorrectionFactorTestType testType, ICorrectionCalculator calculator,
            decimal actualFactor, decimal gauge)
        {
            var test = new CorrectionTestWithGauge(testType, actualFactor, gauge);
            var testCalc = new CorrectionTestCalculatorDecorator(calculator, test);
            testCalc.CalculateFactors();
            return test;
            //return testCalc;
        }

        public static CorrectionTest Create(CorrectionFactorTestType testType, ICorrectionCalculator calculator,
            decimal actualFactor, decimal gauge, Func<IItemGroup, ICorrectionCalculator> calculatorFactory)
        {
            var test = new CorrectionTestWithGauge(testType, actualFactor, gauge);
            var testCalc = new CorrectionTestCalculatorDecorator(calculator, test, calculatorFactory);
            testCalc.CalculateFactors();
            return test;
            //return testCalc;
        }

        public static CorrectionTestCalculatorDecorator CreateWithCalculator(CorrectionFactorTestType testType, ICorrectionCalculator calculator, decimal actualFactor)
        {
            var test = new CorrectionTest(testType, actualFactor);
            var testCalc = new CorrectionTestCalculatorDecorator(calculator, test);
            testCalc.CalculateFactors();

            return testCalc;
        }

        public static CorrectionTestCalculatorDecorator CreateWithCalculator(CorrectionFactorTestType testType, ICorrectionCalculator calculator, decimal actualFactor, decimal gauge)
        {
            var test = new CorrectionTestWithGauge(testType, actualFactor, gauge);
            var testCalc = new CorrectionTestCalculatorDecorator(calculator, test);
            testCalc.CalculateFactors();
            return testCalc;
        }

        public static CorrectionTest Update<TItem>(CorrectionTestCalculatorDecorator correctionTest, TItem itemValues,
            decimal actualFactor, decimal? gauge = null)
            where TItem : IItemGroup
        {
            //correctionTest.Calculator = correctionTest.CalculatorFactory.Invoke(itemValues);

            correctionTest.ActualValue = actualFactor;
            correctionTest.CalculateFactors();
            return correctionTest;
        }

        public abstract class CorrectionTestDecorator : CorrectionTest
        {
            protected CorrectionTest CorrectionTest;

            protected CorrectionTestDecorator(CorrectionTest correctionTest) : base(correctionTest.TestType,
                correctionTest.ActualValue)
            {
                CorrectionTest = correctionTest;
            }
        }

        public class CorrectionTestCalculatorDecorator : CorrectionTestDecorator, ICalculateCorrectionFactor
        {
            public CorrectionTestCalculatorDecorator(ICorrectionCalculator calculator, CorrectionTest correctionTest,
                Func<IItemGroup, ICorrectionCalculator> calculatorFactory = null) :
                base(correctionTest)
            {
                Calculator = calculator;
                BaseType = correctionTest.GetType();
            }

            #region Public Properties

            public ICorrectionCalculator Calculator { get; set; }

            public Type BaseType { get; }

            #endregion

            #region Public Methods

            public void CalculateFactors()
            {
                ExpectedValue = Calculator.CalculateFactor();

                SetValues(ActualValue, ExpectedValue);
            }

            public override CorrectionTest GetValue()
            {
                return CorrectionTest;
            }

            public override void SetValues(decimal actualFactor, decimal expectedFactor)
            {
                base.SetValues(actualFactor, expectedFactor);
                CorrectionTest.SetValues(actualFactor, expectedFactor);
            }

            #endregion
        }
        #endregion
    }

    public class CorrectionTestWithGauge : CorrectionTest
    {
        public CorrectionTestWithGauge(CorrectionFactorTestType testType, decimal actualFactor, decimal expectedFactor,
            decimal gauge)
            : base(testType, actualFactor, expectedFactor)
        {
            Gauge = gauge;
        }

        public CorrectionTestWithGauge(CorrectionFactorTestType testType, decimal actualFactor, decimal gauge) : base(
            testType, actualFactor)
        {
            Gauge = gauge;
        }

        #region Public Properties

        public decimal Gauge { get; set; }

        #endregion
    }

    public class CorrectionTestWithItems<T> : CorrectionTest
        where T : IItemGroup
    {
        protected CorrectionTestWithItems(CorrectionFactorTestType testType, T items) : base(testType)
        {

        }

        public T Values { get; set; }
    }

    public class PressureCorrectionTest : CorrectionTestWithItems<IPressureItems>
    {
        protected PressureCorrectionTest(CorrectionFactorTestType testType, IPressureItems items) : base(testType, items)
        {

        }

        public decimal Gauge { get; set; }
        public decimal AtmGauge { get; set; }
    }

    public class TemperatureCorrectionTest : CorrectionTestWithItems<ITemperatureItems>
    {
        protected TemperatureCorrectionTest(CorrectionFactorTestType testType, ITemperatureItems items) : base(testType, items)
        {
        }

        public decimal Gauge { get; set; }
    }
  
}