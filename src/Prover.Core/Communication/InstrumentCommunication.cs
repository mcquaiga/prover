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

namespace Prover.Core.Communication
{
    public class InstrumentCommunication
    {
        private readonly Instrument _instrument;
        private readonly ICommPort _commPort;
        private miSerialProtocolClass _miSerial;
        private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public InstrumentCommunication(ICommPort commPort, Instrument instrument)
        {
            _commPort = commPort;
            _instrument = instrument;
         
            switch (_instrument.Type)
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

        public async Task<Dictionary<int, string>> DownloadItemsAsync(IEnumerable<ItemsBase.Item> itemsToDownload )
        {
            await Connect();
            return await Task.Run(()=> DownloadItems(itemsToDownload));
        }

        public bool IsConnected { get; set; }

        public async Task Connect()
        {
            int tryCount = 0;
            do
            {
                try
                {
                    if (!IsConnected)
                        await Task.Run(() => _miSerial.Connect());
                    IsConnected = true;
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
                _miSerial.Dispose();
                _log.Error("Could not connect to instrument.");
                throw new InstrumentCommunicationException(InstrumentErrorsEnum.TooManyRetransmissionsError);
            }
        }

        public async Task Disconnect()
        {
            await Task.Run(()=> _miSerial.Disconnect());
            IsConnected = false;
        }

        public async Task<Dictionary<int, string>> DownloadItems(IEnumerable<ItemsBase.Item> itemsToDownload)
        {
            if (!IsConnected) await Connect();
            var myItems = _miSerial.RG((from i in itemsToDownload select i.Number).ToList());
            return myItems;
        }

        public async Task<double> LiveReadItem(int itemNumber)
        {
            return await Task.Run(async () =>
            {
                if (!IsConnected) await Connect();
                return Convert.ToDouble(_miSerial.LR(itemNumber));
            });
        }
        
    }
}
