using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Prover.Shared.Interfaces;

namespace Prover.Application.Services
{
    public class User
    {
        public string EmployeeName { get; set; }
        public string EmployeeNbr { get; set; }
        public string Id { get; set; }
    }

    public class LocalLoginService<TUser> : ILoginService<TUser>
        where TUser : class, new()
    {
        private readonly ISubject<bool> _isLoggedIn = new Subject<bool>();
        private readonly Func<ICollection<TUser>, string, TUser> _lookupFunc;


        public LocalLoginService(ICollection<TUser> users, Func<ICollection<TUser>, string, TUser> lookupFunc)
        {
            _lookupFunc = lookupFunc;
            _users = users;
        }

        private readonly ICollection<TUser> _users;

        public TUser User { get; private set; }
        public IObservable<bool> LoggedIn => _isLoggedIn;

        public async Task<bool> Login(string username = null, string password = null)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            User = _lookupFunc.Invoke(_users, username);

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