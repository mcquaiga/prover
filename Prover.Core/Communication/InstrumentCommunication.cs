using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using miSerialProtocol;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Communication
{
    public static class InstrumentCommunication
    {
        public static Dictionary<int, string> DownloadItemsAsync(ICommPort commPort, Instrument instrument, IEnumerable<Item> itemsToDownload )
        {
            miSerialProtocolClass miSerial = null;
            switch (instrument.Type)
            {
                case InstrumentType.MiniMax:
                    miSerial = new miSerialProtocol.MiniMaxClass(commPort);
                    break;
                case InstrumentType.Ec300:
                    miSerial = new miSerialProtocol.EC300Class(commPort);
                    break;
            }

            miSerial.Connect();
            var myItems = miSerial.RG((from i in itemsToDownload select i.Number).ToList());
            miSerial.Disconnect();
            return myItems;
        }  
    }
}
