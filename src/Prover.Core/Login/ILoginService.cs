using System.Threading.Tasks;

namespace Prover.Core.Login
{
    public interface ILoginService<T>
        where T : class
    {
        Task<bool> Login(string username = null, string password = null);
        T User { get; }
    }
}