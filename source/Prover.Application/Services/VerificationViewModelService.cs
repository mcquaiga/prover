using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Domain.EvcVerifications;

namespace Prover.Application.Services
{
    public class VerificationViewModelService
    {
        private readonly ISourceCache<EvcVerificationViewModel, Guid> _testsCache =
            new SourceCache<EvcVerificationViewModel, Guid>(k => k.Id);

        private readonly EvcVerificationTestService _testService;
        private readonly VerificationViewModelTestCreator _verificationTestCreator;

        public VerificationViewModelService(EvcVerificationTestService testService)
        {
            _testService = testService;
            _verificationTestCreator = new VerificationViewModelTestCreator();

            _testsCache.Connect();
        }

        public async Task<bool> AddOrUpdate(EvcVerificationViewModel viewModel)
        {
            var model = VerificationMapper.MapViewModelToModel(viewModel);

            await _testService.AddOrUpdateVerificationTest(model);

            return true;
        }

        public EvcVerificationTest CreateVerificationTestFromViewModel(EvcVerificationViewModel viewModel) =>
            VerificationMapper.MapViewModelToModel(viewModel);

        public async Task<EvcVerificationViewModel> GetTest(EvcVerificationTest verificationTest)
        {
            return await Task.Run(() => VerificationMapper.MapModelToViewModel(verificationTest));
        }

        public async Task<EvcVerificationViewModel> GetVerificationTest(
            EvcVerificationTest verificationTest)
        {
            return await Task.Run(() => VerificationMapper.MapModelToViewModel(verificationTest));
        }

        public async Task<ICollection<EvcVerificationViewModel>> GetVerificationTests(
            IEnumerable<EvcVerificationTest> verificationTests)
        {
            var evcTests = new ConcurrentBag<EvcVerificationViewModel>();

            foreach (var model in verificationTests) evcTests.Add(await GetVerificationTest(model));

            return evcTests.ToList();
        }

        public EvcVerificationViewModel NewTest(DeviceInstance device)
        {
            var testModel = new EvcVerificationTest(device);

            return _verificationTestCreator.BuildEvcVerificationViewModel(testModel);
        }
    }
}