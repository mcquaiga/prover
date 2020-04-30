using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Devices.Communications.IO;
using Devices.Communications.Messaging;

namespace Devices.Honeywell.Comm.Messaging.Responses
{
    internal class ItemValueProcessor : ResponseProcessor<ItemValueResponseMessage>
    {
        public ItemValueProcessor(int itemNumber)
        {
            ItemNumber = itemNumber;
        }

        public int ItemNumber { get; }

        public override IObservable<ItemValueResponseMessage> ResponseObservable(IObservable<char> source)
        {
            return Observable.Create<ItemValueResponseMessage>(observer =>
            {
                var numberChars = new List<char>();
                var valueChars = new List<char>();
                var checksumChars = new List<char>();

                Action emitPacket = () =>
                {
                    if (valueChars.Count > 0)
                    {
                        var numberString = new string(numberChars.ToArray());
                        var valString = new string(valueChars.ToArray());
                        var checksum = new string(checksumChars.ToArray());

                        observer.OnNext(new ItemValueResponseMessage(ItemNumber, valString, checksum));

                        numberChars.Clear();
                        valueChars.Clear();
                        checksumChars.Clear();
                    }
                };

                var parsingItemNumber = false;
                var parsingItemValue = false;
                var parsingChecksum = false;

                //[SOH]123[STX]12345678[ETX]ABCD[EOT]
                return source.Subscribe(
                    c =>
                    {
                        switch (c)
                        {
                            case ControlCharacters.SOH:
                                emitPacket();
                                parsingItemNumber = true;
                                break;
                            case ControlCharacters.STX:
                                parsingItemNumber = false;
                                parsingItemValue = true;
                                break;
                            case ControlCharacters.ETX:
                                parsingItemValue = false;
                                parsingChecksum = true;
                                break;
                            case ControlCharacters.EOT:
                                emitPacket();
                                parsingItemValue = false;
                                parsingChecksum = false;
                                parsingItemNumber = false;
                                break;
                            default:
                            {
                                if (parsingItemNumber)
                                    numberChars.Add(c);

                                if (parsingItemValue)
                                    valueChars.Add(c);

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
    }
}