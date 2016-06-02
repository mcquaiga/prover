using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Messaging;
using Prover.CommProtocol.MiHoneywell.Items;

namespace Prover.CommProtocol.MiHoneywell.Messaging.Response
{
    internal class ResponseProcessors
    {
        public static ResponseProcessor<StatusResponseMessage>
            ResponseCode = new ResponseCodeProcessor();

        public static ResponseProcessor<ItemValue> ItemValue(ItemMetadata itemMetadata)
            => new ItemValueProcessor(itemMetadata);
    }

    internal class ItemValueProcessor : ResponseProcessor<ItemValue>
    {
        public ItemValueProcessor(ItemMetadata itemMetadata)
        {
            ItemMetadata = itemMetadata;
        }

        public ItemMetadata ItemMetadata { get; }

        public override IObservable<ItemValue> ResponseObservable(IObservable<char> source)
        {
            return Observable.Create<ItemValue>(observer =>
            {
                var valueChars = new List<char>();
                var checksumChars = new List<char>();

                Action emitPacket = () =>
                {
                    if (valueChars.Count > 0)
                    {
                        var valString = new string(valueChars.ToArray());
                        var checksum = new string(checksumChars.ToArray());

                        observer.OnNext(new MiItemValue(ItemMetadata, valString, checksum));

                        valueChars.Clear();
                        checksumChars.Clear();
                    }
                };

                var parsingItemValue = false;
                var parsingChecksum = false;

                return source.Subscribe(
                    c =>
                    {
                        if (parsingItemValue && char.IsNumber(c))
                        {
                            valueChars.Add(c);
                        }

                        if (parsingChecksum)
                        {
                            checksumChars.Add(c);
                        }

                        if (c.IsStartOfHandshake()) //Start of a new value
                        {
                            emitPacket();
                            parsingItemValue = true;
                        }

                        if (c.IsEndOfTransmission())
                        {
                            emitPacket();
                            parsingItemValue = false;
                            parsingChecksum = false;
                        }

                        if (c.IsEndOfText())
                        {
                            parsingItemValue = false;
                            parsingChecksum = true;
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

    internal class ResponseCodeProcessor : ResponseProcessor<StatusResponseMessage>
    {
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
                        var checksum = string.Concat(checksumChars.ToArray());
                        observer.OnNext(new StatusResponseMessage((ResponseCode) code, checksum));
                        codeChars.Clear();
                        checksumChars.Clear();
                    }
                };

                var parsingChecksum = false;
                var parsingResponseCode = false;

                return source.Subscribe(
                    c =>
                    {
                        if (c.IsAcknowledgement())
                        {
                            observer.OnNext(new StatusResponseMessage(ResponseCode.NoError, "0000"));
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
}