using System;
using Devices.Core.Interfaces;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class SiteInformationViewModel : DeviceInfoViewModel
    {
        private readonly ILoginService _loginService;
        private readonly DateTime _startTestDate;
        public ReactiveCommand<string, string> GetUser;

        public SiteInformationViewModel(DeviceInstance device, EvcVerificationViewModel verificationViewModel,
                ILoginService loginService = null) : base(device)
        {
            _loginService = loginService;
            _startTestDate = DateTime.Now;

            Test = verificationViewModel;

            GetUser = ReactiveCommand.CreateFromTask<string, string>(async id =>
            {
                if (loginService == null)
                    return id;

                return await loginService.GetDisplayName(id);
            });

            this.WhenAnyValue(x => x.Test.EmployeeId)
                .InvokeCommand(GetUser);

            GetUser.ToPropertyEx(this, x => x.EmployeeName, Test.EmployeeId);
            //loginService?.GetDisplayName(Test.EmployeeId) ?? Test.EmployeeId);
        }

        public EvcVerificationViewModel Test { get; }

        public extern string EmployeeName { [ObservableAsProperty] get; }

        public DateTime TestDateTime => Test.TestDateTime ?? _startTestDate;
    }
}