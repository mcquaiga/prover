using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.GUI.ViewModels.TemperatureViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.InstrumentReport
{
    public class InstrumentReportViewModel : ReactiveScreen
    {
        private readonly IUnityContainer _container;
        
        public Instrument Instrument { get; private set; }
        public SiteInformationViewModel SiteInformation { get; private set; }
        public TemperatureViewModel Temperature { get; private set; }
        public VolumeViewModel Volume { get; private set; }

        public InstrumentReportViewModel(IUnityContainer container, Instrument instrument)
        {
            _container = container;
            Instrument = instrument;
            SetupView();
        }

        private void SetupView()
        {
            SiteInformation = new SiteInformationViewModel(_container, Instrument);
            Temperature = new TemperatureViewModel(_container, Instrument, false, false);
            Volume = new VolumeViewModel(_container, Instrument, false);
        }
        
    }
}
