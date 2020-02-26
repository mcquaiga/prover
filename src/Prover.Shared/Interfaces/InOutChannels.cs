namespace Prover.Shared.Interfaces
{
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

    public interface IInputChannelFactory
    {
        IInputChannel CreateInputChannel(PulseOutputChannel pulseChannel);
    }

    public interface IOutputChannelFactory
    {
        
        IOutputChannel CreateOutputChannel(OutputChannelType channelType);
    }

    public enum OutputChannelType
    {
        Motor = 0,
        Tachometer = 1
    }
}