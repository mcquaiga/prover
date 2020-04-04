using System;
using System.Threading.Tasks;

namespace Prover.Shared.Interfaces
{
    public interface ILoginService<out T>
        where T : class
    {
        Task<bool> Login(string username = null, string password = null);
        Task<bool> Login();
        Task Logout();
        T User { get; }
        IObservable<bool> LoggedIn { get; }
        bool IsSignedOn { get; }
        Task<string> GetLoginDetails();
    }


}