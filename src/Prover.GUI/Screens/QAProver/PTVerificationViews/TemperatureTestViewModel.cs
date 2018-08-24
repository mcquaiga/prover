using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.GUI.Common;
using TemperatureTestRun = Prover.Core.Models.Instruments.TemperatureTest;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
{
    public class TemperatureTestViewModel : TestRunViewModelBase<TemperatureTestRun>
    {
        public TemperatureTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            TemperatureTestRun temperatureTest)
            : base(screenManager, eventAggregator, temperatureTest)
        {
        }

        public double Gauge
        {
            get { return TestRun.Gauge; }
            set
            {
                TestRun.Gauge = value;
                //EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(TestRun.VerificationTest));
            }
        }

        public decimal? EvcReading => TestRun.Items?.GetItem(26).NumericValue;
        public decimal? EvcFactor => TestRun.Items?.GetItem(45).NumericValue;

        protected override void RaisePropertyChangeEvents()
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => PercentError);
            NotifyOfPropertyChange(() => Gauge);
            NotifyOfPropertyChange(() => EvcReading);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}