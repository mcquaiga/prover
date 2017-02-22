using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prover.Core.Tests
{
    [TestClass]
    public class Items
    {
        [TestMethod]
        public void LoadItems()
        {
            //var mylist = ItemLoader.LoadItems(InstrumentType.MiniMax);

            //foreach (var i in mylist)
            //{
            //    Console.WriteLine(i.ShortDescription);
            //}
        }

        [TestMethod]
        public void QueryInstruments()
        {
            //var boot = new CoreBootstrapper();
            ////var allInstr = new InstrumentStore();
            //var myinstr = allInstr.Query();
            //foreach (var i in myinstr)
            //{
            //    Console.WriteLine(i.SerialNumber);
            //}
        }

        [TestMethod]
        public void DownloadItems()
        {
            //var instr = new Instrument();
            //instr.InstrumentValues = await InstrumentCommunication.DownloadItemsAsync(new SerialPort("COM3", BaudRateEnum.b38400), instr, ItemsBase.Item.LoadItems(InstrumentType.MiniMax));
        }

    }
}
