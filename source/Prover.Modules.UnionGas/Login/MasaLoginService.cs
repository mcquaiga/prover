using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas.Login
{
    /// <summary>
    ///     Defines the <see cref="MasaLoginService" />
    /// </summary>
    public class MasaLoginService : ILoginService<EmployeeDTO>
    {
        private readonly ISubject<bool> _isLoggedIn = new Subject<bool>();

        /// <summary>
        ///     Defines the _log
        /// </summary>
        private readonly ILogger _log;

        /// <summary>
        ///     Defines the _webService
        /// </summary>
        private readonly DCRWebServiceSoap _webService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MasaLoginService" /> class.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="dcrWebService"></param>
        public MasaLoginService(ILogger<MasaLoginService> log, DCRWebServiceSoap dcrWebService)
        {
            _log = log ?? NullLogger<MasaLoginService>.Instance;
            _webService = dcrWebService;
        }

        public IObservable<bool> LoggedIn => _isLoggedIn;

        public EmployeeDTO User { get; private set; }


        public async Task<bool> Login(string username, string password = null)
        {
            User = null;

            if (!string.IsNullOrEmpty(username))
            {
                _log.LogDebug($"Getting employee with #{username} from MASA.");

                var employeeRequest = new GetEmployeeRequest(new GetEmployeeRequestBody(username));
                var response = await _webService.GetEmployeeAsync(employeeRequest);

                User = response.Body.GetEmployeeResult;
            }

            _isLoggedIn.OnNext(User != null);

            return !string.IsNullOrEmpty(User?.Id);
        }

        /// <summary>
        ///     The Logout
        /// </summary>
        /// <returns>The <see cref="bool" /></returns>
        public async Task<bool> Logout()
        {
            await Task.CompletedTask;

            User = null;
            _isLoggedIn.OnNext(false);
            return true;
        }

        /// <summary>
        /// Defines the _eventAggregator
        /// </summary>
    }
}