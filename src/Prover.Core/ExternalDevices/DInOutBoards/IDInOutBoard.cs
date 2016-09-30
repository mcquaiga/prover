using System.Threading.Tasks;

namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public interface IDInOutBoard
    {
        void Dispose();
        Task<int> ReadInput();
        void StartMotor();
        void StopMotor();
    }
}