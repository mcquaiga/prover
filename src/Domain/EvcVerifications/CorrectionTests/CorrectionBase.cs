using System;
using Core.GasCalculations;
using Devices.Core.Interfaces;
using Domain.Interfaces;
using Shared.Domain;
using Shared.Extensions;

namespace Domain.EvcVerifications.CorrectionTests
{
    public interface IVerificationTest : IAssertPassFail
    {
        //void Update();
    }

    public class BaseVerificationTest : BaseEntity, IVerificationTest
    {
        public virtual bool HasPassed()
        {
            return false;
        }
    }

    public interface ICalculateCorrectionFactor
    {
        #region Public Methods

        void CalculateFactors();

        #endregion
    }

    public enum CorrectionFactorTestType
    {
        Super,
        Pressure,
        Temperature
    }


    public partial class CorrectionTest : VerificationTestBaseEntity
    {
        public CorrectionTest(CorrectionFactorTestType testType, decimal actualFactor, decimal expectedFactor)
        {
            TestType = testType;
            ActualFactor = actualFactor;
            ExpectedFactor = expectedFactor;
        }

        public CorrectionTest(CorrectionFactorTestType testType, decimal actualFactor)
        {
            TestType = testType;
            ActualFactor = actualFactor;
        }

        private CorrectionTest()
        {
        }

        #region Public Properties
        public CorrectionFactorTestType TestType { get; }

        public decimal ExpectedFactor { get; set; }

        public decimal ActualFactor { get; set; }

        #endregion

        #region Public Methods

        public override bool HasPassed()
        {
            return Variance(ExpectedFactor, ActualFactor).IsBetween(1);
        }

        public virtual CorrectionTest GetValue()
        {
            return this;
        }

        public virtual void SetValues(decimal actualFactor, decimal expectedFactor)
        {
            ActualFactor = actualFactor;
            ExpectedFactor = expectedFactor;

        }
        #endregion
    }

    public partial class CorrectionTest
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

        public static CorrectionTest Create(CorrectionFactorTestType testType, decimal actualFactor, decimal expectedFactor)
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

        public static CorrectionTestCalculatorDecorator CreateWithCalculator(CorrectionFactorTestType testType,
            ICorrectionCalculator calculator,
            decimal actualFactor)
        {
            var test = new CorrectionTest(testType, actualFactor);
            var testCalc = new CorrectionTestCalculatorDecorator(calculator, test);
            testCalc.CalculateFactors();
           
            return testCalc;
        }

        public static CorrectionTestCalculatorDecorator CreateWithCalculator(CorrectionFactorTestType testType,
            ICorrectionCalculator calculator,
            decimal actualFactor, decimal gauge)
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

            correctionTest.ActualFactor = actualFactor;
            correctionTest.CalculateFactors();
            return correctionTest;
        }

        #endregion
    }

    public class CorrectionTestWithGauge : CorrectionTest
    {
        public CorrectionTestWithGauge(CorrectionFactorTestType testType, decimal actualFactor, decimal expectedFactor, decimal gauge) 
            : base(testType, actualFactor, expectedFactor)
        {
            Gauge = gauge;
        }

        public CorrectionTestWithGauge(CorrectionFactorTestType testType, decimal actualFactor, decimal gauge) : base(testType, actualFactor)
        {
            Gauge = gauge;
        }

        #region Public Properties

        public decimal Gauge { get; set; }

        #endregion
    }

    public class PressureFactorTest : CorrectionTestWithGauge
    {
        protected PressureFactorTest(CorrectionFactorTestType testType, decimal actualFactor, decimal gauge) : base(
            testType, actualFactor, gauge)
        {
        }
    }

    public abstract class CorrectionTestDecorator : CorrectionTest
    {
        protected CorrectionTest CorrectionTest;

        protected CorrectionTestDecorator(CorrectionTest correctionTest) : base(correctionTest.TestType,
            correctionTest.ActualFactor)
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
            ExpectedFactor = Calculator.CalculateFactor();

            SetValues(ActualFactor, ExpectedFactor);
        }

        public override void SetValues(decimal actualFactor, decimal expectedFactor)
        {
            base.SetValues(actualFactor, expectedFactor);
            CorrectionTest.SetValues(actualFactor, expectedFactor);
        }

        public override CorrectionTest GetValue()
        {
            return CorrectionTest;
        }

        #endregion
    }
}