using Devices.Communications.IO;
using Devices.Communications.Messaging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Devices.Honeywell.Comm.Messaging.Responses
{
    internal class ResponseCodeProcessor : ResponseProcessor<StatusResponseMessage>
    {
        #region Methods

        public override IObservable<StatusResponseMessage> ResponseObservable(IObservable<char> source)
        {
            return Observable.Create<StatusResponseMessage>(observer =>
            {
                var codeChars = new List<char>();
                var checksumChars = new List<char>();

                Action emitPacket = () =>
                {
                    if (codeChars.Count > 0)
                    {
                        var code = int.Parse(string.Concat(codeChars.ToArray()));
                        codeChars.Clear();

                        var checksum = string.Concat(checksumChars.ToArray());
                        checksumChars.Clear();

                        observer.OnNext(new StatusResponseMessage((ResponseCode)code, checksum));
                    }
                };

                var parsingChecksum = false;
                var parsingResponseCode = false;

                return source.Subscribe(
                    c =>
                    {
                        switch (c)
                        {
                            case ControlCharacters.ACK:
                                observer.OnNext(new StatusResponseMessage(ResponseCode.NoError, "0000"));
                                break;

                            case ControlCharacters.SOH:
                                emitPacket();
                                parsingResponseCode = true;
                                break;

                            case ControlCharacters.ETX:
                                parsingResponseCode = false;
                                parsingChecksum = true;
                                break;

                            case ControlCharacters.EOT:
                                emitPacket();
                                parsingChecksum = false;
                                parsingResponseCode = false;
                                break;

                            default:
                                {
                                    if (parsingResponseCode)
                                        codeChars.Add(c);

                                    if (parsingChecksum)
                                        checksumChars.Add(c);
                                    break;
                                }
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

        #endregion
    }
}