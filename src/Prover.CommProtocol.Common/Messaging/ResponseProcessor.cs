using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Fluent;
using Prover.CommProtocol.Common.Extensions;

namespace Prover.CommProtocol.Common.Messaging
{
    internal class ResponseProcessors
    {
        public static ResponseProcessor<string>
            MessageProcessor = new PacketProcessor();
    }

    public abstract class ResponseProcessor<T>
    {
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        public abstract IObservable<T> ResponseObservable(Subject<char> source);
    }

    public class PacketProcessor : ResponseProcessor<string>
    {
        public override IObservable<string> ResponseObservable(Subject<char> source)
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
