namespace Prover.Domain.EvcVerifications.Verifications.CorrectionFactors
{
    public static class CorrectionTestFactory
    {
        #region Public Methods

        //public static VerificationTestEntity<IPressureItems> CreatePressureFactor(IPressureItems items, decimal gauge,
        //    decimal atmGauge)
        //{
        //    return new PressureCorrectionTest(items, 0, 0);
        //}

        //public static VerificationTestEntity<ISuperFactorItems> CreateSuperFactor(ISuperFactorItems items,
        //    TemperatureCorrectionTest temperature, PressureCorrectionTest pressure)
        //{
        //    return new SuperCorrectionTest(items, temperature, pressure);
        //}

        //public static TemperatureCorrectionTest CreateTemperatureTest(TemperatureItems items,
        //    decimal gauge)
        //{
        //    return new TemperatureCorrectionTest(items, gauge);
        //}

        //public static CorrectionTestCalculatorDecorator<T> WithCalculator<T>(this VerificationTestEntity<T> test, 
        //    Func<VerificationTestEntity<T>, ICorrectionCalculator> calc)
        //    where T : ItemGroup
        //{
        //    var r = new CorrectionTestCalculatorDecorator<T>(calc, test);
        //    r.CalculateFactors();
        //    return r;
        //}

        #endregion

        //public abstract class CorrectionTestDecorator<T> : VerificationTestEntity<T>
        //    where T : ItemGroup
        //{
        //    protected VerificationTestEntity<T> CorrectionTest;

        //    protected CorrectionTestDecorator(VerificationTestEntity<T> correctionTest) : base(correctionTest.TestType,
        //        correctionTest.Values)
        //    {
        //        CorrectionTest = correctionTest;
        //    }

        //    //public decimal ExpectedValue 
        //    //{ 
        //    //    get => CorrectionTest.ExpectedValue; 
        //    //    set => CorrectionTest.ExpectedValue = value;
        //    //}

        //    //public override decimal ActualValue { get => CorrectionTest.ActualValue; set => CorrectionTest.ActualValue = value; }
        //}

        //public class CorrectionTestCalculatorDecorator<T> : CorrectionTestDecorator<T>, ICalculateCorrectionFactor
        //    where T : ItemGroup
        //{
        //    public CorrectionTestCalculatorDecorator(Func<VerificationTestEntity<T>, ICorrectionCalculator> calculatorFunc,
        //        VerificationTestEntity<T> correctionTest) : base(correctionTest)
        //    {
        //        Calculator = calculatorFunc;
        //        BaseType = correctionTest.GetType();
        //    }

        //    #region Public Properties

        //    public Func<VerificationTestEntity<T>, ICorrectionCalculator> Calculator { get; set; }

        //    public Type BaseType { get; }

        //    public VerificationTestEntity<T> Value => CorrectionTest;

        //    #endregion

        //    #region Public Methods

        //    public void CalculateFactors()
        //    {
        //        ExpectedValue = Calculator.Invoke(CorrectionTest).CalculateFactor();
        //    }

        //    #endregion
        //}

        //private static Dictionary<VerificationTestType, Func<ItemGroup, VerificationTestEntity<ItemGroup>>> _calcFactory = new Dictionary<VerificationTestType, Func<ItemGroup, VerificationTestEntity<ItemGroup>>>
        //{
        //    { VerificationTestType.Pressure, CreatePressureFactor}
        //};
    }
}