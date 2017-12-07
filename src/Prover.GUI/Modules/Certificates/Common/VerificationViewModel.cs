using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Modules.Certificates.Common
{
    public class VerificationViewModel : ViewModelBase
    {
        public Instrument Instrument { get; set; }

        public int? RowNumber { get; set; }

        public VerificationViewModel(Instrument instrument)
        {
            Instrument = instrument;

            if (Instrument.IsLiveTemperature)
                Temperature = new TemperatureInfoViewModel(Instrument);

            if (Instrument.IsLivePressure)
                Pressure = new PressureInfoViewModel(Instrument);

            if (Instrument.IsLiveSuper)
                SuperFactor = new SuperFactorInfoViewModel(Instrument);

            Volume = new VolumeInfoViewModel(Instrument);

            VerificationTests = Instrument
                .VerificationTests
                .OrderBy(v => v.TestNumber)
                .Select(x => new VerificationTestViewModel(x))
                .ToList();
        }

        public SuperFactorInfoViewModel SuperFactor { get; protected set; }

        public VerificationViewModel(Instrument instrument, int rowNumber)
            : this(instrument)
        {
            RowNumber = rowNumber;
        }

        public string HasPassed => Instrument.HasPassed ? "PASS" : "FAIL";

        public string DateTimePretty => $"{Instrument.TestDateTime:g}";
        public string DatePretty => $"{Instrument.TestDateTime:d}";
        public string TimePretty => $"{Instrument.TestDateTime:t}";

        public TemperatureInfoViewModel Temperature { get; protected set; }

        public PressureInfoViewModel Pressure { get; protected set; }

        public VolumeInfoViewModel Volume { get; protected set; }

        public List<VerificationTestViewModel> VerificationTests { get; }            

        public class VerificationTestViewModel
        {
            public VerificationTest Test { get; }

            public VerificationTestViewModel(VerificationTest test)
            {
                Test = test;
            }

            public string Description => Test.TestLevel;
        }

        public bool ShowTemperature => Instrument.IsLiveTemperature;

        public bool ShowPressure => Instrument.IsLivePressure;

        public bool ShowSuperFactor => Instrument.IsLiveSuper;

        public VolumeTest VolumeTest => 
            Instrument.VerificationTests.FirstOrDefault(v => v.VolumeTest != null)?.VolumeTest;

        public string UncorrectedUnits => Instrument.Items.GetItem(92).Description;
        public string CorrectedUnits => Instrument.Items.GetItem(90).Description;

        public bool IsMechanicalDrive => !IsRotaryDrive;
        public MechanicalDrive MechanicalDriveInfo =>
            (MechanicalDrive) (Instrument?.VolumeTest?.DriveType is MechanicalDrive ? Instrument.VolumeTest.DriveType : null);

        public bool IsRotaryDrive => Instrument.DriveRateDescription().ToLower() == "rotary";
        public RotaryDrive RotaryMeterInfo => 
            (RotaryDrive) (Instrument?.VolumeTest?.DriveType is RotaryDrive ? Instrument.VolumeTest.DriveType : null);

        public class PressureInfoViewModel
        {
            private readonly Instrument _instrument;
            public PressureInfoViewModel(Instrument instrument)
            {
                _instrument = instrument;
            }

            public string UnitsAndTransducer => $"{Units}";

            public string Units => _instrument.Items.GetItem(87).Description;

            public decimal Base => decimal.Round(_instrument.Items.GetItem(13).NumericValue, 2);

            public string TransducerType => _instrument.Items.GetItem(112).Description;

            public string Range => $"{(int)_instrument.Items.GetItem(137).NumericValue} {UnitsAndTransducer}";
        }

        public class TemperatureInfoViewModel
        {
            private readonly Instrument _instrument;
            public TemperatureInfoViewModel(Instrument instrument)
            {
                _instrument = instrument;
            }

            public string Units => _instrument.Items.GetItem(89).Description;

            public decimal Base => _instrument.Items.GetItem(34).NumericValue;
        }

        public class SuperFactorInfoViewModel
        {
            private readonly Instrument _instrument;

            public SuperFactorInfoViewModel(Instrument instrument)
            {
                _instrument = instrument;
            }

            public decimal Co2 => _instrument.Items.GetItem(55).NumericValue;
            public decimal N2 => _instrument.Items.GetItem(54).NumericValue;
            public decimal SpecGr => _instrument.Items.GetItem(53).NumericValue;
        }

        public class VolumeInfoViewModel
        {
            private readonly Instrument _instrument;

            public VolumeInfoViewModel(Instrument instrument)
            {
                _instrument = instrument;
            }

            public decimal DriveRate => _instrument.Items.GetItem(98).NumericValue;

            public string DriveRateDescription
            {
                get
                {
                    if (_instrument.InstrumentType.Id == 12)
                        return "Rotary";

                    return _instrument.Items.GetItem(98).Description;
                }
            }

            public bool IsRotaryMeter => DriveRateDescription.ToLower() == "rotary";
        }

        public Client Client => Instrument.Client;
    }
}