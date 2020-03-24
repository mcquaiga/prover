using System.Threading.Tasks;

namespace Prover.Shared.Interfaces
{
    public interface ILoginService<T>
        where T : class
    {
        Task<bool> GetLoginDetails();
        Task<bool> Login(string username = null, string password = null);
        bool Logout();
        T User { get; }
        bool IsLoggedIn { get; }
    }


}