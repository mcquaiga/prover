using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.Verifications.CustomActions;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Prover.Modules.UnionGas.VerificationActions
{
    internal class MasaVerificationActions : IDisposable
    {
        private readonly MeterInventoryNumberValidator _companyNumberValidator;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly VerificationActivator<EvcVerificationViewModel> _testActivator;
        private readonly ILogger<MasaVerificationActions> _logger;
        private readonly ILoginService<EmployeeDTO> _loginService;
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private SerialDisposable _disposer = new SerialDisposable();

        public MasaVerificationActions(
            MeterInventoryNumberValidator companyNumberValidator,
            ILoginService<EmployeeDTO> loginService,
            IDeviceSessionManager deviceManager,
            VerificationActivator<EvcVerificationViewModel> testActivator,
            ILogger<MasaVerificationActions> logger = null)
        {
            _companyNumberValidator = companyNumberValidator;
            _loginService = loginService;
            _deviceManager = deviceManager;
            _testActivator = testActivator;
            _logger = logger ?? NullLogger<MasaVerificationActions>.Instance;

            testActivator.AddActivationBlock(async evc => await OnInitialize(evc));
            //testActivator.AddActivationBlock(async evc => await On(evc));
        }

        public void Dispose()
        {
            _cleanup?.Dispose();
            _disposer.Disposable = Disposable.Empty;
        }

        public async Task OnInitialize(EvcVerificationViewModel verification)
        {
            var tasks = new List<Task>();
            var loggedIn = Observable.Empty<bool>();

            if (!_loginService.IsSignedOn)
            {
                await _loginService.Login();
            }

            _disposer.Disposable = 
                _loginService
                    .LoggedIn
                    .Subscribe(x => verification.EmployeeId = _loginService.User?.Id);

            var meterDto = await _companyNumberValidator.ValidateInventoryNumber(verification, updateDeviceItemValue: true);
           
            verification.JobId = meterDto?.JobNumber.ToString();
            verification.EmployeeId = _loginService.User?.Id;
        }

        public async Task OnComplete(EvcVerificationViewModel verification)
        {
            await Task.CompletedTask;
            _disposer.Disposable = Disposable.Empty;
        }
    }
}