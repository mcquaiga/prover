using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas.VerificationActions
{
    internal class MasaVerificationActions : IInitializeAction, ISubmitAction, IDisposable
    {
        private readonly MeterInventoryNumberValidator _companyNumberValidator;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly ILogger<MasaVerificationActions> _logger;
        private readonly ILoginService<EmployeeDTO> _loginService;
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private SerialDisposable _disposer = new SerialDisposable();

        public MasaVerificationActions(
            MeterInventoryNumberValidator companyNumberValidator,
            ILoginService<EmployeeDTO> loginService,
            IDeviceSessionManager deviceManager,
            ILogger<MasaVerificationActions> logger = null)
        {
            _companyNumberValidator = companyNumberValidator;
            _loginService = loginService;
            _deviceManager = deviceManager;
            _logger = logger ?? NullLogger<MasaVerificationActions>.Instance;
        }

        public void Dispose()
        {
            _cleanup?.Dispose();
            _disposer.Disposable = Disposable.Empty;
        }

        public async Task OnInitialize(EvcVerificationViewModel verification)
        {
            if (!_loginService.IsSignedOn)
                _loginService.SignOn();

            _disposer.Disposable = 
                _loginService.LoggedIn
                    .Subscribe(x => verification.EmployeeId = _loginService.User?.Id);

            var meterDto = await _companyNumberValidator.ValidateInventoryNumber(verification, updateDeviceItemValue: true);

            verification.JobId = meterDto?.JobNumber.ToString();
            verification.EmployeeId = _loginService.User?.Id;
        }

        public async Task OnSubmit(EvcVerificationViewModel verification)
        {
            await Task.CompletedTask;
            _disposer.Disposable = Disposable.Empty;
        }
    }
}