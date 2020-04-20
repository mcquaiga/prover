using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.Shared.Interfaces
{
    public interface IUser
    {
        string UserId { get; set; }
        string UserName { get; set; }
    }

    public interface ILoginService
    {
        Task<bool> Login(string username, string password = null);
        Task<bool> Login();
        
        IObservable<bool> SignOn();
        
        Task Logout();
        
        IObservable<bool> LoggedIn { get; }
        
        bool IsSignedOn { get; }
        
        IUser GetSignOnUser();

        Task<string> GetDisplayName<T>(T id);

    }

    public interface ILoginService<T> : ILoginService
        where T : IUser
    {
        T User { get; }
        Task<string> GetLoginDetails();
        IEnumerable<T> GetUsers();
        Task<T> GetUserDetails<TId>(TId id);
    }


}