using Devices.Communications.IO;

namespace Devices.Communications.Extensions
{
    //public abstract class CommPort : ICommPort
    //{
    //    protected const int OpenPortTimeoutMs = 5000;
    //    protected const int ReadWriteTimeoutMs = 200;
    //    protected readonly Logger Log = LogManager.GetCurrentClassLogger();

    // //public abstract IConnectableObservable<char> DataReceivedObservable { get; } //public
    // abstract ISubject<string> DataSentObservable { get; }

    // //public abstract string Name { get; }

    // //public abstract bool IsOpen(); //public abstract Task Open(CancellationToken ct); //public
    // abstract Task Close(); //public abstract Task Send(string data);

    //    //public abstract void Dispose();
    //}

    public static class ControlCharacterExtensions
    {
        #region Public Methods

        public static bool IsAcknowledgement(this char c) => c == ControlCharacters.ACK;

        public static bool IsEndOfText(this char c) => c == ControlCharacters.ETX;

        public static bool IsEndOfTransmission(this char c) => c == ControlCharacters.EOT;

        /// <summary>
        /// Checks is the char value is equal to SOH
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsStartOfHandshake(this char c) => c == ControlCharacters.SOH;

        public static bool IsStartOfText(this char c) => c == ControlCharacters.STX;

        #endregion Public Methods
    }
}