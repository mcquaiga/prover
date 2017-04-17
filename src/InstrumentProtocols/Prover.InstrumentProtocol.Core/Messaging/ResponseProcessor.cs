using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NLog;
using Prover.InstrumentProtocol.Core.IO;

namespace Prover.InstrumentProtocol.Core.Messaging
{
    internal class ResponseProcessors
    {
        public static ResponseProcessor<string>
            MessageProcessor = new PacketProcessor();
    }

    public abstract class ResponseProcessor<T>
    {
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        public abstract IObservable<T> ResponseObservable(IObservable<char> source);
    }

    public class PacketProcessor : ResponseProcessor<string>
    {
        public override IObservable<string> ResponseObservable(IObservable<char> source)
        {
            return Observable.Create<string>(observer =>
            {
                var msgChars = new List<char>();

                Action emitPacket = () =>
                {
                    if (msgChars.Count > 0)
                    {
                        var msg = string.Concat(msgChars.ToArray());

                        observer.OnNext(msg);
                        msgChars.Clear();
                    }
                };

                return source.Subscribe(
                    c =>
                    {
                        if (c.IsStartOfHandshake()) emitPacket();

                        msgChars.Add(c);

                        if (c.IsAcknowledgement())
                            emitPacket();

                        if (c.IsEndOfTransmission())
                            emitPacket();
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
}