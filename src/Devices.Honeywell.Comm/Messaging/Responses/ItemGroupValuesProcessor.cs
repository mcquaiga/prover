using Devices.Communications.Extensions;
using Devices.Communications.IO;
using Devices.Communications.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Devices.Honeywell.Comm.Messaging.Responses
{
    internal class ItemGroupValuesProcessor : ResponseProcessor<ItemGroupResponseMessage>
    {
        #region Constructors

        public ItemGroupValuesProcessor(IEnumerable<int> itemNumbers)
        {
            ItemNumbers = itemNumbers as int[] ?? itemNumbers.ToArray();

            if (ItemNumbers.Count() > 15)
                throw new ArgumentOutOfRangeException($"{nameof(itemNumbers)} can only have 15 items max");

            ItemValues = ItemNumbers.ToDictionary(x => x, y => string.Empty);
        }

        #endregion

        #region Properties

        public IEnumerable<int> ItemNumbers { get; set; }

        #endregion

        #region Methods

        public override IObservable<ItemGroupResponseMessage> ResponseObservable(IObservable<char> source)
        {
            return Observable.Create<ItemGroupResponseMessage>(observer =>
            {
                var currentItemNumber = ItemNumbers.GetEnumerator();
                var valueChars = new List<char>();
                var checksumChars = new List<char>();

                void EmitPacket()
                {
                    var checksum = new string(checksumChars.ToArray());

                    observer.OnNext(new ItemGroupResponseMessage(ItemValues, checksum));
                    checksumChars.Clear();
                }

                void AddValue()
                {
                    if (valueChars.Any())
                    {
                        currentItemNumber.MoveNext();
                        var raw = new string(valueChars.ToArray());
                        ItemValues[currentItemNumber.Current] = raw.ScrubInvalidCharacters();

                        valueChars.Clear();
                    }
                }

                var parsingItemValue = false;
                var parsingChecksum = false;

                //[SOH]12345678,11111111[ETX]ABCD[EOT]
                return source.Subscribe(
                    c =>
                    {
                        switch (c)
                        {
                            case ControlCharacters.SOH:
                                parsingItemValue = true;
                                break;

                            case ControlCharacters.ETX:
                                AddValue();
                                parsingItemValue = false;
                                parsingChecksum = true;
                                break;

                            case ControlCharacters.EOT:
                                EmitPacket();
                                parsingItemValue = false;
                                parsingChecksum = false;
                                break;

                            case ControlCharacters.Comma:
                                AddValue();
                                break;

                            default:
                                {
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
                        EmitPacket();
                        observer.OnCompleted();
                    });
            });
        }

        #endregion

        private Dictionary<int, string> ItemValues { get; }
    }
}