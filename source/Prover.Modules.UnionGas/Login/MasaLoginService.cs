using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.MasaWebService;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas.Login
{
    /// <summary>
    ///     Defines the <see cref="MasaLoginService" />
    /// </summary>
    public class MasaLoginService : LoginServiceBase<EmployeeDTO>, IDisposable
    {
        /// <summary>
        ///     Defines the _log
        /// </summary>
        private readonly ILogger _log;

        /// <summary>
        ///     Defines the _webService
        /// </summary>
        private readonly IUserService<EmployeeDTO> _webService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MasaLoginService" /> class.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="employeeService"></param>
        public MasaLoginService(ILogger<MasaLoginService> log, IUserService<EmployeeDTO> employeeService)
        {
            _log = log ?? NullLogger<MasaLoginService>.Instance;
            _webService = employeeService;
        }

        public override async Task<string> GetLoginDetails()
        {
            return await MessageInteractions.GetInputString.Handle("Employee number:");
        }

        public override async Task<bool> Login(string username, string password = null)
        {
            User = await _webService.GetUser(username);

            LoggedInSubject.OnNext(User != null);

            return !User?.Id.IsNullOrEmpty() ?? false;
        }

        protected override string UserId => User?.Id;
    }
}