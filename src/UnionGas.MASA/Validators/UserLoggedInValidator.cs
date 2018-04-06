using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prover.CommProtocol.Common;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Login;
using Prover.Core.Models.Instruments;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Validators
{
    public class UserLoggedInValidator : IValidator
    {
        private readonly ILoginService<EmployeeDTO> _loginService;

        public UserLoggedInValidator(ILoginService<EmployeeDTO> loginService)
        {
            _loginService = loginService;
        }

        public async Task<object> Validate(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            if (!string.IsNullOrEmpty(_loginService.User?.Id))
            {
                instrument.EmployeeId = _loginService.User.Id;
            }

            return !string.IsNullOrEmpty(_loginService.User?.Id);
        }
    }
}
