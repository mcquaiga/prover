namespace Prover.Client.WPF.Screens.QAProver
{
    public class InstrumentInfoViewModel : ViewModelBase, IHandle<VerificationTestEvent>
    {
        public InstrumentInfoViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        public string BasePressure
            =>
                $"{Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}"
        ;

        public string BaseTemperature => $"{Instrument.EvcBaseTemperature()} {Instrument.TemperatureUnits()}";

        public bool CommPortChecked
        {
            get { return Instrument.CommPortsPassed != null && Instrument.CommPortsPassed.Value; }
            set
            {
                Instrument.CommPortsPassed = value;
                Task.Run(() => QaTestManager?.SaveAsync());
                NotifyOfPropertyChange(() => Instrument);
            }
        }

        public string CorrectorType
        {
            get
            {
                switch (Instrument.CompositionType)
                {
                    case Core.Models.Instruments.CorrectorType.PTZ:
                        return "PTZ";
                    case Core.Models.Instruments.CorrectorType.P:
                        return "P";
                    default:
                        return "T";
                }
            }
        }

        public bool EventLogChecked
        {
            get { return Instrument.EventLogPassed != null && Instrument.EventLogPassed.Value; }
            set
            {
                Instrument.EventLogPassed = value;
                Task.Run(() => QaTestManager?.SaveAsync());
                NotifyOfPropertyChange(() => Instrument);
            }
        }

        public Instrument Instrument { get; set; }

        public string JobIdDisplay
            => !string.IsNullOrEmpty(Instrument.JobId) ? $"Job #{Instrument.JobId}" : string.Empty;

        public IQaRunTestManager QaTestManager { get; set; }

        public string TestDatePretty => $"{Instrument.TestDateTime:MMMM d, yyyy h:mm tt}";

        public void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => Instrument);
        }
    }
}