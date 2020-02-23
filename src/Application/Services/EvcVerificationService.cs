using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Application.ViewModels;
using Devices.Core.Interfaces;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Builders;
using DynamicData;
using Shared.Interfaces;

namespace Application.Services
{
    public class EvcVerificationTestService
    {
        private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;

        private readonly SourceCache<EvcVerificationTest, Guid> _tests =
            new SourceCache<EvcVerificationTest, Guid>(k => k.Id);

        private CompositeDisposable _cleanup;


        public EvcVerificationTestService(IAsyncRepository<EvcVerificationTest> verificationRepository)
        {
            _verificationRepository = verificationRepository;

            VerificationTests = _tests.Connect().RemoveKey().AsObservableList();

            var dbQuery = GetTests()
                .Subscribe(test => { _tests.Edit(innerCache => { innerCache.AddOrUpdate(test); }); });

            _cleanup = new CompositeDisposable(dbQuery);
        }

        public IObservableList<EvcVerificationTest> VerificationTests { get; }

        public async Task<EvcVerificationTest> AddOrUpdateVerificationTest(EvcVerificationTest evcVerificationTest)
        {
            await _verificationRepository.AddAsync(evcVerificationTest);

            return evcVerificationTest;
        }

        public IObservable<EvcVerificationTest> AllNotExported()
        {
            return _verificationRepository.List(test => test.ExportedDateTime == null);
        }

        public EvcVerificationTestCreator Factory(DeviceInstance device) =>
            new EvcVerificationTestCreator(device, AddOrUpdateVerificationTest);

        private IObservable<EvcVerificationTest> GetTests(Expression<Func<EvcVerificationTest, bool>> predicate = null)
        {
            return _verificationRepository.List();
        }

        //private IObservable<IChangeSet<EvcVerificationTest, Guid>> GetTestChangeSetObservable()
        //{
        //    return ObservableChangeSet.Create<EvcVerificationTest, Guid>(cache =>
        //    {

        //    })
        //}
    }

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

            if (correctionTest.Temperature != null)
                builder.BuildTemperatureTest(correctionTest.Temperature.Items, correctionTest.Temperature.Gauge);

            if (correctionTest.Pressure != null)
                builder.BuildPressureTest(correctionTest.Pressure.Items, correctionTest.Pressure.Gauge,
                    correctionTest.Pressure.AtmosphericGauge);

            if (correctionTest.SuperFactor != null)
                builder.BuildSuperFactorTest(correctionTest.SuperFactor.Items);

            if (correctionTest.Volume != null)
                builder.BuildVolumeTest(correctionTest.Volume.StartValues, correctionTest.Volume.EndValues,
                    correctionTest.Volume.AppliedInput);

            builder.Commit();
        }

        //protected void CreateTestPoint(int level, IEnumerable<ItemValue> beforeValues, IEnumerable<ItemValue> afterValues)
        //{
        //     _evcBuilder.TestPointFactory().CreateNew(level, correctionTest.BeforeValues, correctionTest.AfterValues);
        //}
    }
}