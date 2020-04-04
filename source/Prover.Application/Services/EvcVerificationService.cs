using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.Extensions;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Domain.EvcVerifications.Builders;
using Prover.Shared.Interfaces;

namespace Prover.Application.Services
{
    public class EvcVerificationTestService
    {
        private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;

        private IObservableCache<EvcVerificationTest, Guid> _tests =
            new SourceCache<EvcVerificationTest, Guid>(k => k.Id);

        private CompositeDisposable _cleanup;

        public EvcVerificationTestService(IAsyncRepository<EvcVerificationTest> verificationRepository)
        {
            _verificationRepository = verificationRepository;
            VerificationTests = _tests;
        }

        public IObservableCache<EvcVerificationTest, Guid> VerificationTests { get; private set; }

        public IObservableCache<EvcVerificationTest, Guid> FetchTests()
        {
            var tests = GetTests().Publish();

            VerificationTests = tests.AsObservableCache();

            _cleanup = new CompositeDisposable(VerificationTests, tests.Connect());

            return VerificationTests;
        }

        private IObservable<IChangeSet<EvcVerificationTest, Guid>> GetTests(Expression<Func<EvcVerificationTest, bool>> predicate = null)
        {
            return _verificationRepository.List().ToObservableChangeSet(t => t.Id);
        }
    }

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