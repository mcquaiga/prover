using System.Threading.Tasks;

namespace Prover.Core.Login
{
    public interface ILoginService
    {
        Task<object> Login(string username = null, string password = null);
    }
}