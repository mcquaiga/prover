using System.Windows.Media;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common.Events;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews
{
    public abstract class BaseTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        public virtual BaseVerificationTest Test { get; set; }

        public decimal? PercentError => Test?.PercentError;

        public Brush PercentColour
            =>
                Test == null || Test.HasPassed
                    ? Brushes.White
                    : (SolidColorBrush) new BrushConverter().ConvertFrom("#DC6156");

        public Brush PassColour => Test != null && Test.HasPassed ? Brushes.ForestGreen : Brushes.IndianRed;

        public string PassStatusIcon => Test != null && Test.HasPassed ? "pass" : "fail";

        public abstract void Handle(VerificationTestEvent message);
    }
}