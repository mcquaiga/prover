namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public interface IDInOutBoard
    {
        void Dispose();
        int ReadInput();
        void StartMotor();
        void StopMotor();
    }
}