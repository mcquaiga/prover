using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas.Login
{
    //internal interface ILoginService<TUser> : ILoginService<string, TUser>
    //    where TUser : class
    //{

    //}

    /// <summary>
    ///     Defines the <see cref="MasaLoginService" />
    /// </summary>
    public class MasaLoginService : LoginServiceBase<EmployeeDTO>, IDisposable
    {
        private readonly IRepository<string, EmployeeDTO> _employeeCache;

        private readonly List<EmployeeDTO> _employees = new List<EmployeeDTO>();

        /// <summary>
        ///     Defines the _log
        /// </summary>
        private readonly ILogger<MasaLoginService> _log;

        /// <summary>
        ///     Defines the _webService
        /// </summary>
        private readonly IUserService<EmployeeDTO> _webService;

        public MasaLoginService(ILogger<MasaLoginService> log, IUserService<EmployeeDTO> employeeService,
            IRepository<string, EmployeeDTO> employeeCache)
        {
            _log = log ?? NullLogger<MasaLoginService>.Instance;
            _webService = employeeService;
            _employeeCache = employeeCache;
        }

        protected override string UserId => User?.Id;

        /// <inheritdoc />
        public override async Task<string> GetDisplayName<T>(T id)
        {
            if (string.IsNullOrEmpty(id?.ToString())) return string.Empty;

            var employee = await GetUserDetails(id.ToString());
            return employee.EmployeeName;
        }

        public override async Task<string> GetLoginDetails() =>
            await MessageInteractions.GetInputString.Handle("Employee number:");

        /// <inheritdoc />
        public override async Task<EmployeeDTO> GetUserDetails<TId>(TId id)
        {
            _log.LogDebug($"Getting user details for employee #{id.ToString()}");
            var idString = id.ToString();
            if (User?.Id == idString)
                return User;

            return GetUsers().FirstOrDefault(u => u.EmployeeNbr == idString) 
                ?? await _webService.GetUser(idString);
        }

        public override IEnumerable<EmployeeDTO> GetUsers()
        {
            _log.LogDebug($"Getting employees.");
            if (_employees.Any() == false)
                _employees.AddRange(_employeeCache?.GetAll());

            return _employees;
        }

        public override async Task<bool> Login(string username, string password = null)
        {
            User = await _webService.GetUser(username);

            LoggedInSubject.OnNext(User != null);

            _employeeCache.Add(User);
            _employees.Add(User);

            return !User?.Id.IsNullOrEmpty() ?? false;
        }
    }
}