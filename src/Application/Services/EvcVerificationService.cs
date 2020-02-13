using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.ViewModels;
using AutoMapper;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Builders;
using Domain.EvcVerifications.Verifications.CorrectionFactors;
using Shared.Interfaces;

namespace Application.Services
{
    public class EvcVerificationTestService
    {
        private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;
        

        public EvcVerificationTestService(IAsyncRepository<EvcVerificationTest> verificationRepository)
        {
            _verificationRepository = verificationRepository;
            
        }

        public EvcVerificationTestCreator Factory(DeviceInstance device) => new EvcVerificationTestCreator(device, AddOrUpdateVerificationTest);

        public async Task<EvcVerificationTest> AddOrUpdateVerificationTest(EvcVerificationTest evcVerificationTest)
        {
            var existing = await _verificationRepository.GetAsync(evcVerificationTest.Id);
            
            if (existing == null)
            {
                await _verificationRepository.AddAsync(evcVerificationTest);
            }
            else
            {
                await _verificationRepository.UpdateAsync(evcVerificationTest);
            }

            return evcVerificationTest;
        }
    }

    public class EvcVerificationTestCreator
    {
        private readonly DeviceInstance _device;
        private readonly Func<EvcVerificationTest, Task<EvcVerificationTest>> _callback;
        private EvcVerificationBuilder _evcBuilder;

        internal EvcVerificationTestCreator(DeviceInstance device, Func<EvcVerificationTest, Task<EvcVerificationTest>> callback)
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
            var builder = _evcBuilder.TestPointFactory().CreateNew(level, correctionTest.BeforeValues, correctionTest.AfterValues);

            if (correctionTest.Temperature != null) 
                builder.BuildTemperatureTest(correctionTest.Temperature.Items, correctionTest.Temperature.Gauge);

            if (correctionTest.Pressure != null)
                builder.BuildPressureTest(correctionTest.Pressure.Items, correctionTest.Pressure.Gauge, correctionTest.Pressure.AtmosphericGauge);

            if (correctionTest.SuperFactor != null)
                builder.BuildSuperFactorTest(correctionTest.SuperFactor.Items);

            if (correctionTest.Volume != null)
                builder.BuildVolumeTest(correctionTest.Volume.StartItems, correctionTest.Volume.EndItems, correctionTest.Volume.AppliedInput);

            builder.Commit();
        }

        //protected void CreateTestPoint(int level, IEnumerable<ItemValue> beforeValues, IEnumerable<ItemValue> afterValues)
        //{
        //     _evcBuilder.TestPointFactory().CreateNew(level, correctionTest.BeforeValues, correctionTest.AfterValues);
        //}

    }
}
