using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentTempViewModel : ReactiveScreen
    {
        public Temperature Temperature;
        public InstrumentTempViewModel(Temperature temperature)
        {
            Temperature = temperature;
            Tests = new ObservableCollection<InstrumentTempTestViewModel>();
            Temperature.Tests.ForEach(t => Tests.Add(new InstrumentTempTestViewModel(t)));

            Tests.OrderByDescending(t => t.Test.TestLevel);
        }

        public ObservableCollection<InstrumentTempTestViewModel> Tests { get; set; }
    }
}
