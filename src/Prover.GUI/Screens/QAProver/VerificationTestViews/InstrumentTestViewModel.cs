using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    //public class InstrumentTestViewModel : ViewModelBase
    //{
    //    public InstrumentTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, Instrument instrument) 
    //        : base(screenManager, eventAggregator)
    //    {
    //        Instrument = instrument;

    //        //TODO
    //        foreach (var x in Instrument.VerificationTests.OrderBy(v => v.TestNumber))
    //        {
    //            var item = ScreenManager.ResolveViewModel<VerificationSetViewModel>();
    //            item.VerificationTest = x;
    //            TestViews.Add(item);
    //        }
    //    }

    //    public Instrument Instrument { get; }

    //    #region Views

    //    public InstrumentInfoViewModel SiteInformationItem { get; set; }

    //    public ObservableCollection<VerificationSetViewModel> TestViews { get; set; } =
    //        new ObservableCollection<VerificationSetViewModel>();

    //    public VolumeTestViewModel VolumeInformationItem { get; set; }

    //    #endregion
    //}
}