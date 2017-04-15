namespace Prover.Domain.Verification.TestPoints
{
    //public class TestPoint : Entity<Guid>
    //{
    //    private static readonly Dictionary<TestLevel, double> PressureGauges = new Dictionary<TestLevel, double>
    //    {
    //        {TestLevel.Level1, 0.80},
    //        {TestLevel.Level2, 0.50},
    //        {TestLevel.Level3, 0.20}
    //    };

    //    private static readonly Dictionary<TestLevel, int> TempGauges = new Dictionary<TestLevel, int>
    //    {
    //        {TestLevel.Level1, 32},
    //        {TestLevel.Level2, 60},
    //        {TestLevel.Level3, 90}
    //    };

    //    public TestPoint() : base(Guid.NewGuid())
    //    {
    //    }

    //    public TestPoint(IInstrument instrument, TestLevel level) : base(Guid.NewGuid())
    //    {
    //        Level = level;
    //        Pressure = CreatePressureTest(level, instrument);
    //        Temperature = CreateTemperatureTest(level, instrument);
    //        Volume = CreateVolumeTest(level, instrument);
    //    }

    //    public TestLevel Level { get; protected set; }
    //    public PressureTestPoint Pressure { get; protected set; }
    //    public SuperFactorTestPoint SuperFactor { get; protected set; }
    //    public TemperatureTestPoint Temperature { get; protected set; }

    //    public TestRun.TestRun TestRun { get; set; }
    //    public VolumeTestPoint Volume { get; set; }

    //    internal void Update(IInstrument instrument, IPressureItems pressureTestItems,
    //        ITemperatureItems temperatureTestItems, IVolumeItems volumePreTestItems, IVolumeItems volumePostTestItems)
    //    {
    //        if (Pressure != null)
    //            Pressure.EvcItems = pressureTestItems;

    //        if (Temperature != null)
    //            Temperature.EvcItems = temperatureTestItems;

    //        //if (Volume != null)
    //        //    Volume.Update(volumePreTestItems, volumePostTestItems, Volume.AppliedInput, Temperature?.ActualFactor, Pressure?.ActualFactor, SuperFactor?.ActualFactor);

    //        if (instrument.CorrectorType == EvcCorrectorType.PTZ)
    //            SuperFactor = new SuperFactorTestPoint(instrument.SuperFactorItems, Temperature.GaugeTemperature,
    //                Pressure.GasPressure, Pressure.EvcItems.UnsqrFactor);
    //    }

    //    private static PressureTestPoint CreatePressureTest(TestLevel level, IInstrument instrument)
    //    {
    //        PressureTestPoint pressure = null;
    //        if (instrument.CorrectorType == EvcCorrectorType.P || instrument.CorrectorType == EvcCorrectorType.PTZ)
    //        {
    //            pressure = new PressureTestPoint
    //            {
    //                EvcItems = instrument.PressureItems
    //            };

    //            pressure.SetGaugeValues(PressureGauges[level] * instrument.PressureItems.Range, 0);
    //        }

    //        return pressure;
    //    }

    //    private static TemperatureTestPoint CreateTemperatureTest(TestLevel level, IInstrument instrument)
    //    {
    //        TemperatureTestPoint temperature = null;
    //        if (instrument.CorrectorType == EvcCorrectorType.T || instrument.CorrectorType == EvcCorrectorType.PTZ)
    //            temperature = new TemperatureTestPoint(
    //                TempGauges[level],
    //                instrument.TemperatureItems);
    //        return temperature;
    //    }

    //    private static VolumeTestPoint CreateVolumeTest(TestLevel level, IInstrument instrument)
    //    {
    //        VolumeTestPoint volume = null;
    //        if (level == TestLevel.Level1)
    //            volume = new VolumeTestPoint(instrument.VolumeItems);

    //        return volume;
    //    }
    //}
}