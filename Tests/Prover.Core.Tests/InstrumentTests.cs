using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.SerialProtocol;

namespace Prover.Core.Tests
{
    [TestClass]
    public class Items
    {
        [TestMethod]
        public void LoadItems()
        {
            var mylist = Item.LoadItems(InstrumentType.MiniMax);

            foreach (var i in mylist)
            {
                Console.WriteLine(i.ShortDescription);
            }
        }

        [TestMethod]
        public void DownloadItems()
        {
            var instr = new Instrument();
            InstrumentCommunication.DownloadItemsAsync(new SerialPort("COM3", BaudRateEnum.b38400), instr, Item.LoadItems(InstrumentType.MiniMax));

        }

    }
}
