namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public interface IDInOutBoard
    {
        decimal PulseTiming { get; set; }
        void Dispose();
        int ReadInput();
        void StartMotor();
        void StopMotor();
    }
}