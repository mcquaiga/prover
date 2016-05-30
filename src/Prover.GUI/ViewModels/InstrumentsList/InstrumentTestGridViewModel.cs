using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments;
using Prover.GUI.ViewModels.InstrumentViews;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using Prover.Core.Storage;
using Prover.GUI.Reporting;
using Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentTestGridViewModel : InstrumentTestViewModel
    {
        public bool IsSelected { get; set; }

        public InstrumentTestGridViewModel(IUnityContainer container, Instrument instrument) : base(container, instrument)
        {
            SiteInformationItem = new InstrumentInfoViewModel(container, instrument);
            VolumeInformationItem = new VolumeTestViewModel(container, instrument.VolumeTest);
        }

        public void DisplayInstrumentReport()
        {
            var instrumentReport = new InstrumentGenerator(Instrument, _container);
            instrumentReport.Generate();
        }

        public async Task DeleteInstrument()
        {
            using (var store = new InstrumentStore(_container))
            {
                await store.Delete(Instrument);
            }
        }
    }
}
