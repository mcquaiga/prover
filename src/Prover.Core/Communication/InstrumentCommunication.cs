using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Prover.Core.Storage;
using Prover.SerialProtocol;
using Prover.Core.Models.Instruments;
using System.Windows;
using Prover.Core.Models;

namespace Prover.Core.Communication
{
    public class InstrumentCommunicator
    {
        private readonly ICommPort _commPort;
        private miSerialProtocolClass _miSerial;
        private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public InstrumentCommunicator(ICommPort commPort, InstrumentType instrumentType)
        {
            _commPort = commPort;

            switch (instrumentType)
            {
                case InstrumentType.Ec300:
                    _miSerial = new EC300Class(commPort);
                    break;
                default:
                    _miSerial = new MiniMaxClass(commPort);
                    break;
            }

            IsConnected = false;
        }

        public InstrumentCommunicator(ICommPort commPort, Instrument instrument) : this(commPort, instrument.Type) { }

        public async Task<Dictionary<int, string>> DownloadItemsAsync(IEnumerable<ItemDetails> itemsToDownload, bool disconnectAfter = true)
        {
            await Connect();

            var result = await Task.Run(()=> DownloadItems(itemsToDownload));

            if (disconnectAfter)
                await Disconnect();

            return result;
        }

        public async Task DownloadItemsAsync(InstrumentItems itemsToDownload, bool disconnectAfter = true)
        {
            itemsToDownload.InstrumentValues = await DownloadItemsAsync(itemsToDownload.Items);
        }

        private Dictionary<int, string> DownloadItems(IEnumerable<ItemDetails> itemsToDownload)
        {
            var myItems = _miSerial.RG((from i in itemsToDownload select i.Number).ToList());
            return myItems;
        }

        public bool IsConnected { get; private set; }

        public async Task Connect()
        {
            int tryCount = 0;
            do
            {
                try
                {
                    if (!IsConnected)
                        await Task.Run(() => _miSerial.Connect()).ContinueWith(taskResult =>
                        {
                            if (!taskResult.IsFaulted)
                                IsConnected = true;
                            else
                                IsConnected = false;
                        });
                                   
                }
                catch (AggregateException ae)
                {
                    ae.Handle((x) =>
                    {
                        if (x is CommExceptions)
                        {
                            _log.Error("An error occured connecting to instrumnet.", x);
                            return true;
                        }

                        return false;                        
                    });
                    tryCount++;
                    IsConnected = false;
                }
            } while (!IsConnected && tryCount < 10);

            if (tryCount >= 10)
            {
                _log.Error("Could not connect to instrument.");
                throw new InstrumentCommunicationException(InstrumentErrorsEnum.TooManyRetransmissionsError);
            }
        }

        public async Task Disconnect()
        {
            if (IsConnected)
                await Task.Run(()=> _miSerial.Disconnect());

            IsConnected = false;
        }

        public async Task<decimal> LiveReadItem(int itemNumber)
        {
            return await Task.Run(async () =>
            {
                await Connect();
                return Convert.ToDecimal(_miSerial.LR(itemNumber));
            });
        }
        
    }
}
