using System;
using System.Threading.Tasks;

namespace Prover.Shared.Interfaces
{
    public interface ILoginService<T>
        where T : class
    {
        Task<bool> Login(string username = null, string password = null);
        Task<bool> Logout();
        T User { get; }
        IObservable<bool> LoggedIn { get; }
    }


}