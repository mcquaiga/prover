using System.Threading.Tasks;

namespace Prover.Core.Login
{
    public interface ILoginService<T>
        where T : class
    {
        Task<bool> GetLoginDetails();
        Task<bool> Login(string username = null, string password = null);
        Task<bool> Logout();
        T User { get; }
        bool IsLoggedIn { get; }
    }
}