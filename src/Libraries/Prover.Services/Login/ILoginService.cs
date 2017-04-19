using System.Threading.Tasks;

namespace Prover.Services.Login
{
    public interface ILoginService<T>
        where T : class
    {
        T User { get; }
        Task<bool> Login(string username = null, string password = null);
        bool Logout();
    }
}