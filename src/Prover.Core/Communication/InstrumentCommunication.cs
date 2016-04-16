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
using Caliburn.Micro;
using Prover.Core.Events;

namespace Prover.Core.Communication
{
    public class InstrumentCommunicator
    {
        private readonly ICommPort _commPort;
        private miSerialProtocolClass _miSerial;
        private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private IEventAggregator _eventAggregator;
        private int maxConnectAttempts = 10;

        public InstrumentCommunicator(IEventAggregator eventAggregator, ICommPort commPort, InstrumentType instrumentType)
        {
            _commPort = commPort;
            _eventAggregator = eventAggregator;
            
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

        public InstrumentCommunicator(IEventAggregator eventAggregator, ICommPort commPort, Instrument instrument) : this(eventAggregator, commPort, instrument.Type) { }

        public async Task<Dictionary<int, string>> DownloadItemsAsync(IEnumerable<ItemDetail> itemsToDownload, bool disconnectAfter = true)
        {
            await Connect();

            var result = await Task.Run(()=> DownloadItems(itemsToDownload));

            if (disconnectAfter)
                await Disconnect();

            return result;
        }

        public async Task<string> DownloadItem(int itemNumber, bool disconnectAfter = true)
        {
            await Connect();

            var result = await Task.Run(() => _miSerial.RD(itemNumber));

            if (disconnectAfter)
                await Disconnect();

            return Convert.ToString(result);
        }

        public async Task WriteItem(int itemNumber, string value, bool disconnectAfter = true)
        {
            await Connect();

            await Task.Run(() => _miSerial.WD(itemNumber, value));

            if (disconnectAfter)
                await Disconnect();
        }

        public async Task DownloadItemsAsync(InstrumentItems itemsToDownload, bool disconnectAfter = true)
        {
            itemsToDownload.InstrumentValues = await DownloadItemsAsync(itemsToDownload.Items, disconnectAfter);
        }

        private Dictionary<int, string> DownloadItems(IEnumerable<ItemDetail> itemsToDownload)
        {
            var myItems = _miSerial.RG((from i in itemsToDownload select i.Number).ToList());
            return myItems;
        }

        public bool IsConnected { get; private set; }

        public async Task Connect()
        {
            var tryCount = 0;

            do
            {
                try
                {
                    if (!IsConnected)
                    {
                        //await _eventAggregator.PublishOnUIThreadAsync(new InstrumentConnectionEvent(InstrumentConnectionEvent.Status.Connecting, tryCount + 1, maxConnectAttempts));
                        await Task.Run(() => _miSerial.Connect()).ContinueWith(taskResult =>
                        {
                            if (!taskResult.IsFaulted)
                                IsConnected = true;
                            else
                                IsConnected = false;
                        });
                    }                                   
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
            } while (!IsConnected && tryCount < maxConnectAttempts);

            if (tryCount >= maxConnectAttempts)
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
