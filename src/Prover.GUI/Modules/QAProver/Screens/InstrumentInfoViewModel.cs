using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Enums;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Modules.QAProver.Screens
{
    public class InstrumentInfoViewModel : ViewModelBase, IHandle<VerificationTestEvent>
    {
        public InstrumentInfoViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        public Instrument Instrument { get; set; }

        public string CorrectorType
        {
            get
            {
                switch (Instrument.CompositionType)
                {
                    case EvcCorrectorType.PTZ:
                        return "PTZ";
                    case EvcCorrectorType.P:
                        return "P";
                    default:
                        return "T";
                }
            }
        }

        public string BasePressure =>
            $"{decimal.Round(Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue, 2)} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}";

        public string AtmPressure =>
            $"{decimal.Round(Instrument.Items.GetItem(ItemCodes.Pressure.Atm).NumericValue, 2)} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}";

        public string PressureRange =>
            $"{Instrument.Items.GetItem(ItemCodes.Pressure.Range).RawValue.Trim()} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}";

        public string BaseTemperature => $"{Instrument.EvcBaseTemperature()} {Instrument.TemperatureUnits()}";

        public string TestDatePretty => $"{Instrument.TestDateTime:MMMM d, yyyy h:mm tt}";

        public string JobIdDisplay
            => !string.IsNullOrEmpty(Instrument.JobId) ? $"Job #{Instrument.JobId}" : string.Empty;

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

        public IQaRunTestManager QaTestManager { get; set; }

        public void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => Instrument);
        }
    }
}