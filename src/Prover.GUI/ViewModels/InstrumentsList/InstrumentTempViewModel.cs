using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentTempViewModel : ReactiveScreen
    {
        private readonly ObservableCollection<InstrumentTempTestViewModel> _testViewModels;
        public Temperature Temperature { get; set; }
        public InstrumentTempViewModel(Temperature temperature)
        {
            Temperature = temperature;
            _testViewModels = new ObservableCollection<InstrumentTempTestViewModel>();
            Temperature.Tests.ForEach(t => _testViewModels.Add(new InstrumentTempTestViewModel(t)));
        }

        public List<InstrumentTempTestViewModel> Tests {
            get
            {
                return (from t in _testViewModels
                        orderby t.Test.TestLevel
                        select t).ToList();

            }
        }
    }
}
