namespace Prover.Application.ExternalDevices.DInOutBoards
{
    //public interface IDInOutBoard : IDisposable
    //{
    //    decimal PulseTiming { get; set; }
    //    void Dispose();
    //    int ReadInput();
    //    void StartMotor();
    //    void StopMotor();
    //}

    public interface IInputChannel
    {
        int GetValue();
    }

    public interface IOutputChannel
    {
        void OutputValue(short value);
        void SignalStart();
        void SignalStop();
    }

    public enum OutputChannelType
    {
        Motor = 0,
        Tachometer = 1
    }
}