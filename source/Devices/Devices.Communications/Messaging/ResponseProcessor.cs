using Devices.Communications.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Devices.Communications.Messaging
{
    public class LineProcessor : ResponseProcessor<string>
    {
        #region Public Methods

        public override IObservable<string> ResponseObservable(IObservable<char> source)
        {
            return Observable.Create<string>(observer =>
            {
                var msgChars = new List<char>();
                var cr = new char[2];

                void EmitPacket()
                {
                    if (msgChars.Count > 0)
                    {
                        var msg = string.Concat(msgChars.ToArray());

                        observer.OnNext(msg);
                        msgChars.Clear();
                    }
                }

                return source.Subscribe(
                    c =>
                    {
                        msgChars.Add(c);

                        if (c.Equals((char)13) || c == (char)10 || c.Equals('r') || c.Equals('n'))
                            EmitPacket();
                    },
                    observer.OnError,
                    () =>
                    {
                        EmitPacket();
                        observer.OnCompleted();
                    });
            });
        }

        #endregion Public Methods
    }

    public class PacketProcessor : ResponseProcessor<string>
    {
        #region Public Methods

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

        #endregion Public Methods
    }

    public abstract class ResponseProcessor<T>
    {
        #region Public Methods

        public abstract IObservable<T> ResponseObservable(IObservable<char> source);

        #endregion Public Methods
    }

    internal static class ResponseProcessors
    {
        #region Public Fields

        public static ResponseProcessor<string>
            MessageProcessor = new PacketProcessor();

        #endregion Public Fields
    }
}