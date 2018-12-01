using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.InstrumentInfo;

namespace Prover.GUI.Screens.QAProver
{
    public class InstrumentInfoViewModel : ViewModelBase, IHandle<VerificationTestEvent>
    {
        public InstrumentInfoViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        public void Initialize(Instrument instrument, ITestRunManager qaRunTestManager)
        {
            Instrument = instrument;
            QaTestManager = qaRunTestManager;

            if (Instrument.InstrumentType.Name == "TOC")
            {
                TocInfoItem = new TocInfoViewModel(Instrument);
            }
        }

        public Instrument Instrument { get; set; }
        public TocInfoViewModel TocInfoItem { get; set; }

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

        public string BasePressure =>
            $"{decimal.Round(Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue, 2)} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}";

        public string PressureRange =>
            $"{decimal.Round(Instrument.Items.GetItem(ItemCodes.Pressure.Range).NumericValue, 0)} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}";

        public string BaseTemperature => $"{Instrument.EvcBaseTemperature()} {Instrument.TemperatureUnits()}";
        public string TestDatePretty => $"{Instrument.TestDateTime:g}"; //MMMM d, yyyy h:mm tt

        public string MeterIndexDescription => Instrument.MeterIndexDescription();

        public string JobIdDisplay =>
            !string.IsNullOrEmpty(Instrument.JobId) ? $"Job #{Instrument.JobId}" : string.Empty;

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

        public bool DisplayEventLogCommPortView => Instrument.InstrumentType == Instruments.MiniAt;

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

        public ITestRunManager QaTestManager { get; set; }

        public void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => Instrument);
        }
    }
}