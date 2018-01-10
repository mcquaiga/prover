using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Prover.CommProtocol.Common.IO
{
    public interface ICommPort : IDisposable
    {
        IConnectableObservable<char> DataReceivedObservable { get; }
        ISubject<string> DataSentObservable { get; }
        string Name { get; }
        bool IsOpen();
        Task Open(CancellationToken ct);
        Task Close();
        Task Send(string data);
    }

    //public abstract class CommPort : ICommPort
    //{
    //    protected const int OpenPortTimeoutMs = 5000;
    //    protected const int ReadWriteTimeoutMs = 200;
    //    protected readonly Logger Log = LogManager.GetCurrentClassLogger();

    //    //public abstract IConnectableObservable<char> DataReceivedObservable { get; }
    //    //public abstract ISubject<string> DataSentObservable { get; }

    //    //public abstract string Name { get; }

    //    //public abstract bool IsOpen();
    //    //public abstract Task Open(CancellationToken ct);
    //    //public abstract Task Close();
    //    //public abstract Task Send(string data);

    //    //public abstract void Dispose();
    //}

    public static class ControlCharacters
    {
        public const char SOH = (char) 1;
        public const char STX = (char) 2;
        public const char ETX = (char) 3;
        public const char EOT = (char) 4;
        public const char ENQ = (char) 5;
        public const char ACK = (char) 6;
        public const char CR = (char) 13;
        public const char NAK = (char) 21;
        public const char RS = (char) 30;
        public const char Comma = (char) 44;

        public static List<char> All => new List<char>
        {
            SOH,
            STX,
            ETX,
            EOT,
            ENQ,
            ACK,
            CR,
            NAK,
            RS
        };

        public static string Prettify(string msg)
        {
            msg = msg.Replace(SOH.FormatChar(), "[SOH]");
            msg = msg.Replace(STX.FormatChar(), "[STX]");
            msg = msg.Replace(ETX.FormatChar(), "[ETX]");
            msg = msg.Replace(EOT.FormatChar(), "[EOT]");
            msg = msg.Replace(ENQ.FormatChar(), "[ENQ]");
            msg = msg.Replace(ACK.FormatChar(), "[ACK]");
            msg = msg.Replace(CR.FormatChar(), "[CR]");
            msg = msg.Replace(NAK.FormatChar(), "[NAK]");
            msg = msg.Replace(RS.FormatChar(), "[RS]");
            return msg;
        }

        private static string FormatChar(this char c) => new string(new[] {c});
    }

    public static class ControlCharacterExtensions
    {
        public static bool IsEndOfTransmission(this char c) => c == ControlCharacters.EOT;

        /// <summary>
        ///     Checks is the char value is equal to SOH
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsStartOfHandshake(this char c) => c == ControlCharacters.SOH;

        public static bool IsStartOfText(this char c) => c == ControlCharacters.STX;

        public static bool IsEndOfText(this char c) => c == ControlCharacters.ETX;

        public static bool IsAcknowledgement(this char c) => c == ControlCharacters.ACK;
    }
}