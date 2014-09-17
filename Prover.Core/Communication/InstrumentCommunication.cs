using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Prover.Core.Storage;
using Prover.SerialProtocol;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Communication
{
    public static class InstrumentCommunication
    {
        public static List<string> GetCommPortList()
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames().ToList();
            ports.Add("IrDA");
            return ports;
        }

        public static async Task<Dictionary<int, string>> DownloadItemsAsync(ICommPort commPort, Instrument instrument, IEnumerable<ItemsBase.Item> itemsToDownload )
        {
            return await Task.Run(()=> DownloadItems(commPort, instrument, itemsToDownload));
        }

        public static void Connect(ICommPort commPort, Instrument instrument, IEnumerable<ItemsBase.Item> itemsToDownload)
        {
            
        }

        public static Dictionary<int, string> DownloadItems(ICommPort commPort, Instrument instrument,
            IEnumerable<ItemsBase.Item> itemsToDownload)
        {
            miSerialProtocolClass miSerial = null;
            switch (instrument.Type)
            {
                case InstrumentType.Ec300:
                    miSerial = new EC300Class(commPort);
                    break;
                default:
                    miSerial = new MiniMaxClass(commPort);
                    break;
            }

            miSerial.Connect();
            var myItems = miSerial.RG((from i in itemsToDownload select i.Number).ToList());
            miSerial.Disconnect();
            return myItems;
        }

        public static ICommPort CreateCommPortObject(string commName, BaudRateEnum baudRate)
        {
            if (!GetCommPortList().Contains(commName)) return null;

            ICommPort commPort;
            if (commName == "IrDA")
            {
                commPort = new IrDAPort();
            }
            else
            {
                commPort = new SerialPort(commName, baudRate);
            }
            return commPort;
        }
    }
}
