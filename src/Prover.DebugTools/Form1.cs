using System;
using System.Windows.Forms;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.Startup;
using Prover.Core.Storage;

namespace Prover.DebugTools
{
    public partial class Form1 : Form
    {
        public IUnityContainer Unity;

        public Form1()
        {
            InitializeComponent();
            var core = new CoreBootstrapper();
            Unity = core.Container;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var store = Unity.Resolve<IInstrumentStore<Instrument>>();
            ////var instr = new Instrument();
            ////instr.InstrumentValues = await InstrumentCommunication.DownloadItemsAsync(new SerialPort("COM3", BaudRateEnum.b38400), instr, ItemsBase.Item.LoadItems(InstrumentType.MiniMax));
            //store.UpsertAsync(instr);
        }
    }
}
