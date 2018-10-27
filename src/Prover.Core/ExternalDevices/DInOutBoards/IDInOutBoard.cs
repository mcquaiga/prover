using System.Threading.Tasks;

namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public interface IDInOutBoard
    {
        decimal PulseTiming { get; set; }
        void Dispose();
        Task<int> ReadInput();
        void StartMotor();
        void StopMotor();
    }
}