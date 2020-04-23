using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.Models;
using Prover.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.Modules.UnionGas.Verifications
{
    internal class MasaVerificationActions : IEventsSubscriber, IDisposable
    {
        private readonly MeterInventoryNumberValidator _companyNumberValidator;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly ILogger<MasaVerificationActions> _logger;
        private readonly ILoginService<Employee> _loginService;
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly SerialDisposable _disposer = new SerialDisposable();

        public MasaVerificationActions(
                ILogger<MasaVerificationActions> logger,
                MeterInventoryNumberValidator companyNumberValidator,
                ILoginService<Employee> loginService,
                IDeviceSessionManager deviceManager)
        {
            _logger = logger ?? NullLogger<MasaVerificationActions>.Instance;
            _companyNumberValidator = companyNumberValidator;
            _loginService = loginService;
            _deviceManager = deviceManager;

            VerificationEvents.OnInitialize.Subscribe(async e =>
            {
                await OnInitialize(e.Input);
            }).DisposeWith(_cleanup);

            VerificationEvents.OnSubmit.Subscribe(async e =>
            {
                //await OnSubmit(e.Input);
            }).DisposeWith(_cleanup);
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
                    .Subscribe(x => verification.EmployeeId = _loginService.User?.UserId);

            var meterDto = await _companyNumberValidator.ValidateInventoryNumber(verification, updateDeviceItemValue: true);

            verification.JobId = meterDto?.JobNumber.ToString();
            verification.EmployeeId = _loginService.User?.UserId;
        }

        public async Task OnSubmit(EvcVerificationViewModel verification)
        {
            _disposer.Disposable = Disposable.Empty;
            await Task.CompletedTask;
        }


    }
}