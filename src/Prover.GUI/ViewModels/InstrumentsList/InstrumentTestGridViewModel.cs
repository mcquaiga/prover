using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments;
using Prover.GUI.ViewModels.InstrumentViews;
using Microsoft.Practices.Unity;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentTestGridViewModel : InstrumentTestViewModel
    {
        public string HasPassed => true ? "PASS" : "FAIL";

        public bool IsSelected { get; set; }

        public InstrumentTestGridViewModel(IUnityContainer container, Instrument instrument) : base(container, instrument)
        {
            SiteInformationItem = new InstrumentInfoViewModel(container, instrument);
            VolumeInformationItem = new VolumeVerificationViewModel(container, instrument);
        }
    }
}
