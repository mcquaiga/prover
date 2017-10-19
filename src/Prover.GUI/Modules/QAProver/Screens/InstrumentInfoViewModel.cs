﻿using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Enums;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using ReactiveUI;

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
            $"{decimal.Round(Instrument.Items.GetItem(ItemCodes.Pressure.Range).NumericValue, 0)} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}";

        public string BaseTemperature => $"{Instrument.EvcBaseTemperature()} {Instrument.TemperatureUnits()}";

        public string TestDatePretty => $"{Instrument.TestDateTime:d}";

        public string JobIdDisplay
            => !string.IsNullOrEmpty(Instrument.JobId) ? $"Job #{Instrument.JobId}" : string.Empty;

        public bool DisplayEventLogCommPortView => Instrument.VolumeTest.DriveType is MechanicalDrive;

        public bool EventLogChecked
        {
            get => Instrument.EventLogPassed != null && Instrument.EventLogPassed.Value;
            set
            {
                Instrument.EventLogPassed = value;
                NotifyOfPropertyChange(() => Instrument);
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public bool CommPortChecked
        {
            get => Instrument.CommPortsPassed != null && Instrument.CommPortsPassed.Value;
            set
            {
                Instrument.CommPortsPassed = value;
                NotifyOfPropertyChange(() => Instrument);
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public string DriveRate => Instrument.Items.GetItem(98).Description;

        public IQaRunTestManager QaTestManager { get; set; }

        public void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => Instrument);
        }
    }
}