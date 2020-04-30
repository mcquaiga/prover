using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.Models;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.Modules.UnionGas.Login
{
    //internal interface ILoginService<TUser> : ILoginService<string, TUser>
    //    where TUser : class
    //{

    //}

    /// <summary>
    ///     Defines the <see cref="MasaLoginService" />
    /// </summary>
    public class MasaLoginService : LoginServiceBase<Employee>, IDisposable
    {
        private readonly IRepository<Employee> _employeeCache;

        private readonly List<Employee> _employees = new List<Employee>();

        /// <summary>
        ///     Defines the _log
        /// </summary>
        private readonly ILogger<MasaLoginService> _log;

        /// <summary>
        ///     Defines the _webService
        /// </summary>
        private readonly IUserService<EmployeeDTO> _webService;

        public MasaLoginService(ILogger<MasaLoginService> log, IUserService<EmployeeDTO> employeeService,
            IRepository<Employee> employeeCache)
        {
            _log = log ?? NullLogger<MasaLoginService>.Instance;
            _webService = employeeService;
            _employeeCache = employeeCache;
        }

        protected override string UserId => User?.UserId;

        /// <inheritdoc />
        public override async Task<string> GetDisplayName<T>(T id)
        {
            if (string.IsNullOrEmpty(id?.ToString())) return string.Empty;

            var employee = await GetUserDetails(id.ToString());
            return employee.UserName;
        }

        public override async Task<string> GetLoginDetails() =>
            await Messages.GetInputString.Handle("Employee number:");

        /// <inheritdoc />
        public override async Task<Employee> GetUserDetails<TId>(TId id)
        {
            _log.LogDebug($"Getting user details for employee #{id.ToString()}");

            var idString = id.ToString();
            if (User?.UserId == idString)
                return User;

            return GetUsers().FirstOrDefault(u => u.UserId == idString);
        }

        public override IEnumerable<Employee> GetUsers()
        {
            _log.LogDebug($"Getting employees.");

            if (_employees.Any() == false)
                _employees.AddRange(_employeeCache?.GetAll());

            return _employees;
        }

        public override async Task<bool> Login(string username, string password = null)
        {
            if (string.IsNullOrEmpty(username)) await Login();

            var employeeDto = await _webService.GetUser(username);

            if (employeeDto != null)
            {
                User = new Employee()
                {
                    UserId = employeeDto.Id,
                    UserName = employeeDto.EmployeeName //_employeeCache.GetAll().Where(e => e.UserId == employeeDto.Id)
                };

                LoggedInSubject.OnNext(User != null);

                _employeeCache.Add(User);
                _employees.Add(User);
            }

            return !User?.UserId.IsNullOrEmpty() ?? false;
        }


    }
}