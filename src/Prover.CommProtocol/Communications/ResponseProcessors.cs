using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Communications
{
    internal static class ResponseProcessors
    {
        private const int MaxPacketLength = 7;
        private static Dictionary<char, int> PacketLengthTable =
            new Dictionary<char, int> { { 'A', 5 }, { 'B', 6 }, { 'C', 7 } };

        internal static IObservable<string> GetResponseData(this IObservable<char> source)
        {
            return Observable.Create<string>(observer =>
            {
                var packet = new List<char>();

                Action emitPacket = () =>
                {
                    if (packet.Count > 0)
                    {
                        observer.OnNext(new string(packet.ToArray()));
                        packet.Clear();
                    }
                };

                return source.Subscribe(
                    c =>
                    {
                        if (c.IsStartOfHandshake())
                        {
                            emitPacket();
                        }

                        packet.Add(c);
                        if (c.IsEndOfTransmission())
                        {
                            emitPacket();
                        }
                    });
            });
        }

        internal static IObservable<bool> GetAcknowledgement(this IObservable<char> source)
        {
            return Observable.Create<bool>(observer =>
            {
                return source.Subscribe(c =>
                {
                    if (c.IsAcknowledgement())
                    {
                        observer.OnNext(true);
                    }
                });
            });
        }

        internal static IObservable<Tuple<int, string>> GetResponseCode(this IObservable<char> source)
        {
            return Observable.Create<Tuple<int, string>>(observer =>
            {
                var codeChars = new List<char>();
                var checksumChars = new List<char>();

                Action emitPacket = () =>
                {
                    if (codeChars.Count > 0)
                    {
                        var code = int.Parse(string.Concat(codeChars.ToArray()));
                        var checksum = string.Concat(checksumChars.ToArray());

                        observer.OnNext(new Tuple<int, string>(code, checksum));

                        codeChars.Clear();
                        checksumChars.Clear();
                    }
                };

                bool parsingChecksum = false;
                bool parsingResponseCode = false;

                return source.Subscribe(
                    c =>
                    {
                        if (c.IsAcknowledgement())
                        {

                        }

                        if (c.IsEndOfTransmission())
                        {
                            emitPacket();
                            parsingChecksum = false;
                            parsingResponseCode = false;
                        }

                        if (parsingResponseCode && char.IsNumber(c))
                        {
                            codeChars.Add(c);
                        }

                        if (parsingChecksum)
                        {
                            checksumChars.Add(c);
                        }                     

                        if (c.IsEndOfText())
                        {
                            parsingResponseCode = false;
                            parsingChecksum = true;
                        }

                        if (c.IsStartOfHandshake()) //Start of a new value
                        {
                            emitPacket();
                            parsingResponseCode = true;
                        }
                    },
                    observer.OnError,
                    () =>
                    {
                        emitPacket();
                        observer.OnCompleted();
                    });
            });
        }

        static bool IsEndOfTransmission(this char c)
        {
            return c == (char)CommChar.EOT;
        }

        static bool IsStartOfHandshake(this char c)
        {
            return c == (char)CommChar.SOH;
        }

        static bool IsEndOfText(this char c)
        {
            return c == (char)CommChar.ETX;
        }

        static bool IsAcknowledgement(this char c)
        {
            return c == (char)CommChar.ACK;
        }
}
}
