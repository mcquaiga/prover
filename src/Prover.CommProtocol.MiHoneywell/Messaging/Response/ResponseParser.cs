using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Extensions;
using Prover.CommProtocol.Common.Messaging;

namespace Prover.CommProtocol.MiHoneywell.Messaging.Response
{
    internal class ResponseProcessors
    {
        public static ResponseProcessor<string>
            ItemValue = new ItemValueProcessor();
        
        public static ResponseProcessor<Tuple<ResponseCode, string>>
            ResponseCode = new ResponseCodeProcessor();

        public static ResponseProcessor<bool>
            Acknowledgment = new AcknowledgementProcessor();
    }

    internal class ItemValueProcessor : ResponseProcessor<string>
    {
        public override IObservable<string> ResponseObservable(IObservable<char> source)
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
    }

    internal class ResponseCodeProcessor : ResponseProcessor<Tuple<ResponseCode, string>>
    {
        public override IObservable<Tuple<ResponseCode, string>> ResponseObservable(IObservable<char> source)
        {
            return Observable.Create<Tuple<ResponseCode, string>>(observer =>
            {
                var codeChars = new List<char>();
                var checksumChars = new List<char>();

                Action emitPacket = () =>
                {
                    if (codeChars.Count > 0)
                    {
                        var code = int.Parse(string.Concat(codeChars.ToArray()));
                        var checksum = string.Concat(checksumChars.ToArray());

                        observer.OnNext(new Tuple<ResponseCode, string>((ResponseCode)code, checksum));

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
    }

    internal class AcknowledgementProcessor : ResponseProcessor<bool>
    {
        public override IObservable<bool> ResponseObservable(IObservable<char> source)
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
    }
}
