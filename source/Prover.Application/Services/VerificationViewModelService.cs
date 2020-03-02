using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Services;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;

namespace Prover.Application.Services
{
    public class VerificationViewModelService
    {
/*
        private readonly IAsyncRepository<EvcVerificationTest> _evcVerificationRepository;
*/
        private readonly EvcVerificationTestService _testService;

        private readonly VerificationViewModelTestCreator _verificationTestCreator;

        private readonly ISourceCache<EvcVerificationViewModel, Guid> _testsCache = new SourceCache<EvcVerificationViewModel, Guid>(k => k.Id);

        //public IObservableCache<EvcVerificationViewModel, Guid> Connect() => _testsCache.Connect();

        public VerificationViewModelService(EvcVerificationTestService testService)
        {
       
            _testService = testService;
            _verificationTestCreator = new VerificationViewModelTestCreator();

            _testsCache.Connect();
        }

        public EvcVerificationTest CreateVerificationTestFromViewModel(EvcVerificationViewModel viewModel) => VerificationMapper.MapViewModelToModel(viewModel);

        public async Task<EvcVerificationViewModel> GetVerificationTest(
            EvcVerificationTest verificationTest)
        {
            return await Task.Run(() => VerificationMapper.MapModelToViewModel(verificationTest));
        }

        public async Task<ICollection<EvcVerificationViewModel>> GetVerificationTests(IEnumerable<EvcVerificationTest> verificationTests)
        {
            var evcTests = new ConcurrentBag<EvcVerificationViewModel>();

            foreach (var model in verificationTests)
            {
                evcTests.Add(await GetVerificationTest(model));
            }

            return evcTests.ToList();
        }

        public async Task<EvcVerificationViewModel> GetTest(EvcVerificationTest verificationTest)
        {
            return await Task.Run(() => VerificationMapper.MapModelToViewModel(verificationTest));
        }

        public EvcVerificationViewModel NewTest(DeviceInstance device)
        {
            var testModel = new EvcVerificationTest(device);

            var  testViewModel = VerificationMapper.MapModelToViewModel(testModel);

            return _verificationTestCreator.BuildEvcVerificationViewModel(testModel);
        }

        public async Task<bool> AddOrUpdate(EvcVerificationViewModel viewModel)
        {
            var model = VerificationMapper.MapViewModelToModel(viewModel);

            await _testService.AddOrUpdateVerificationTest(model);

            return true;
        }
    }
}