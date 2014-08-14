using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.SerialProtocol;

namespace Prover.Core.Communication
{
    public class InstrumentManager
    {
        private readonly Instrument _instrument;
        public ICommPort CommPort { get; set; }

        public Instrument Instrument
        {
            get { return _instrument; }
        }

        public InstrumentManager()
        {
            _instrument = new Instrument();
        }

        public InstrumentManager(ICommPort commPort)
        {
            _instrument = new Instrument();
            CommPort = commPort;
        }

        public InstrumentManager(Instrument instrument, ICommPort commPort)
        {
            _instrument = instrument;
            CommPort = commPort;
        }

        public async void DownloadInstrumentItems()
        {
            _instrument.InstrumentValues = await InstrumentCommunication.DownloadItemsAsync(CommPort, _instrument, _instrument.Items);
        }

        public async void DownloadTemperatureItems()
        {
            _instrument.Temperature.InstrumentValues = await InstrumentCommunication.DownloadItemsAsync(CommPort, _instrument,_instrument.Temperature.Items);
        }

        public void Save()
        {
            var store = new InstrumentStore();
            store.Save(_instrument);
        }
    }
}
