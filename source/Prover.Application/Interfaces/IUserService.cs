using System.Threading.Tasks;

namespace Prover.Application.Interfaces
{
    public interface IUserService<TUser>
    {
        Task<TUser> GetUser(string userId);
    }
}