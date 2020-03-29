using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas.Login
{
    public class LocalLoginService : ILoginService<EmployeeDTO>
    {
        private static readonly ICollection<EmployeeDTO> _employeeTest = new List<EmployeeDTO>
        {
            new EmployeeDTO {EmployeeName = "Adam McQuaig", EmployeeNbr = "123", Id = "1"},
            new EmployeeDTO {EmployeeName = "Tony", EmployeeNbr = "1234", Id = "2"},
            new EmployeeDTO {EmployeeName = "Glen", EmployeeNbr = "12345", Id = "3"},
            new EmployeeDTO {EmployeeName = "Kyle", EmployeeNbr = "123456", Id = "4"}
        };

        private readonly ISubject<bool> _isLoggedIn = new Subject<bool>();

        public EmployeeDTO User { get; private set; }
        public IObservable<bool> LoggedIn => _isLoggedIn;

        public async Task<bool> Login(string username = null, string password = null)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            User = _employeeTest.FirstOrDefault(e => e.EmployeeNbr == username);

            _isLoggedIn.OnNext(User != null);
            return User != null;
        }

        public async Task<bool> Logout()
        {
            await Task.CompletedTask;

            User = null;
            _isLoggedIn.OnNext(false);
            return true;
        }
    }
}