namespace Prover.Application.Services
{
    //public class EvcVerificationTestService : IDisposable
    //{
    //    private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;
        
    //    private CompositeDisposable _cleanup;
    //    private readonly IConnectableObservable<IChangeSet<EvcVerificationTest, Guid>> _tests;

    //    public EvcVerificationTestService(IAsyncRepository<EvcVerificationTest> verificationRepository)
    //    {
    //        _verificationRepository = verificationRepository;

    //        _tests = GetTests().Publish();

    //        VerificationTests = _tests.AsObservableCache();
    //    }
        
    //    public IObservableCache<EvcVerificationTest, Guid> FetchTests()
    //    {
    //        if (_cleanup == null)
    //            _cleanup = new CompositeDisposable(VerificationTests, _tests.Connect());

    //        return VerificationTests;
    //    }
    //    private IObservableCache<EvcVerificationTest, Guid> VerificationTests { get; set; }
    //    private IObservable<IChangeSet<EvcVerificationTest, Guid>> GetTests(Expression<Func<EvcVerificationTest, bool>> predicate = null)
    //    {
    //        return _verificationRepository.Query().ToObservableChangeSet(t => t.Id);
    //    }

    //    public void Dispose()
    //    {
    //        _cleanup?.Dispose();
    //        VerificationTests?.Dispose();
    //    }
    //}

/*
    public class EvcVerificationTestCreator
    {
        private readonly Func<EvcVerificationTest, Task<EvcVerificationTest>> _callback;
        private readonly DeviceInstance _device;
        private EvcVerificationBuilder _evcBuilder;

        internal EvcVerificationTestCreator(DeviceInstance device,
            Func<EvcVerificationTest, Task<EvcVerificationTest>> callback)
        {
            _device = device;
            _callback = callback;
            _evcBuilder = new EvcVerificationBuilder(device);
        }

        public async Task<EvcVerificationTest> Create(IEnumerable<VerificationTestPointViewModel> testViewModels)
        {
            _evcBuilder = new EvcVerificationBuilder(_device);
            _evcBuilder.SetTestDateTime();

            testViewModels.ToList()
                .ForEach(test => CreateTestPoint(test.TestNumber, test));

            return await _callback.Invoke(_evcBuilder.GetEvcVerification());
        }

        public EvcVerificationTestCreator Create(IEnumerable<CorrectionTestDefinition> testDefinitions)
        {
            _evcBuilder = new EvcVerificationBuilder(_device);
            _evcBuilder.SetTestDateTime();

            return this;
        }

        public void CreateTestPoint(int level, VerificationTestPointViewModel correctionTest)
        {
            var builder = _evcBuilder.TestPointFactory().CreateNew(level);

            if (correctionTest.GetTemperatureTest() != null)
                builder.BuildTemperatureTest(correctionTest.GetTemperatureTest().Items, correctionTest.GetTemperatureTest().Gauge);

            if (correctionTest.GetPressureTest() != null)
                builder.BuildPressureTest(correctionTest.GetPressureTest().Items, correctionTest.GetPressureTest().Gauge,
                    correctionTest.GetPressureTest().AtmosphericGauge);

            if (correctionTest.GetSuperFactorTest() != null)
                builder.BuildSuperFactorTest(correctionTest.GetSuperFactorTest().Items);

            //if (correctionTest.GetVolumeTest() != null)
            //    builder.BuildVolumeTest(correctionTest.GetVolumeTest().StartValues, correctionTest.GetVolumeTest().EndValues,
            //        correctionTest.GetVolumeTest().Uncorrected.AppliedInput);

            builder.Commit();
        }

        //protected void CreateTestPoint(int level, IEnumerable<ItemValue> beforeValues, IEnumerable<ItemValue> afterValues)
        //{
        //     _evcBuilder.TestPointFactory().CreateNew(level, correctionTest.BeforeValues, correctionTest.AfterValues);
        //}
    }
*/
}