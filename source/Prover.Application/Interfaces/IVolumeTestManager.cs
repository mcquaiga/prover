using System.Threading.Tasks;

namespace Prover.Application.Interfaces
{
    public interface IVolumeTestManager
    {
        Task RunFinishActions();
        Task RunStartActions();
    }
}