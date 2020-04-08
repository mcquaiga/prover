using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.Shared.Interfaces
{
    public interface ILoginService
    {
        Task<bool> Login(string username, string password = null);
        Task<bool> Login();
        IObservable<bool> SignOn();
        Task Logout();
        IObservable<bool> LoggedIn { get; }
        bool IsSignedOn { get; }
    }

    public interface ILoginService<out T> : ILoginService
        where T : class
    {
        T User { get; }
        Task<string> GetLoginDetails();
        IEnumerable<T> GetUsers();
    }


}