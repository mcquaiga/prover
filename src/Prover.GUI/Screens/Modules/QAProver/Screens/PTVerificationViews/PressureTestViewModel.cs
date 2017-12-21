using System;
using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core;
using Prover.Core.Settings;
using Prover.GUI.Events;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class AtmosphericGaugePressureUpdateMessage
    {
        public PressureTestViewModel Sender { get; }
        public decimal? Value { get; }

        public AtmosphericGaugePressureUpdateMessage(PressureTestViewModel sender, decimal? value)
        {
            Sender = sender;
            Value = value;
        }
    }

    public class PressureTestViewModel : TestRunViewModelBase<Core.Models.Instruments.PressureTest>,
        IHandle<AtmosphericGaugePressureUpdateMessage>
    {
        private readonly ISettingsService _settingsService;

        public PressureTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.PressureTest testRun, ISettingsService settingsService) : base(screenManager, eventAggregator, testRun)
        {
            _settingsService = settingsService;
            GaugePressure = TestRun.GasGauge;
            AtmosphericGauge = TestRun.AtmosphericGauge;
            //_gaugePressure = TestRun.GasGauge;
            //_atmosphericGauge = TestRun.AtmosphericGauge;

            if (ShowAbsolute)
            {
                if (_settingsService.SharedSettingsInstance.TestSettings.UpdateAbsolutePressure)
                {
                    LockGaugePressure = false;
                    this.WhenAnyValue(x => x.GaugePressure, x => x.AtmosphericGauge,
                            (g, a) => g + (a ?? 0))
                        .ToProperty(this, x => x.AbsolutePressure, out _absolutePressure);
                }
                else
                {
                    LockGaugePressure = true;
                    _absolutePressure = ObservableAsPropertyHelper<decimal?>.Default(TestRun.GasPressure);
                    this.WhenAnyValue(x => x.AtmosphericGauge, x => x.AbsolutePressure,
                            (atm, abs) => abs - (atm ?? 0))
                        .Subscribe(x => GaugePressure = x);
                }

                this.WhenAnyValue(x => x.AtmosphericGauge)
                    .Subscribe(x =>
                    {
                        TestRun.AtmosphericGauge = x;
                        EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(TestRun.VerificationTest));

                        if (TestRun.VerificationTest.TestNumber == 0)
                            EventAggregator.PublishOnUIThread(new AtmosphericGaugePressureUpdateMessage(this, x));
                    });
            }
            else
            {
                _absolutePressure = ObservableAsPropertyHelper<decimal?>.Default(TestRun.GasPressure);
            }

            this.WhenAnyValue(x => x.GaugePressure)
                .Subscribe(x => TestRun.GasGauge = x);
            //.Subscribe(x =>
            //{
            //    TestRun.AtmosphericGauge = x;
            //    EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(TestRun.VerificationTest));
            //});

            GaugePressure = TestRun.GasGauge;
            AtmosphericGauge = TestRun.AtmosphericGauge;
        }

        public bool LockGaugePressure { get; }
        private readonly ObservableAsPropertyHelper<decimal?> _absolutePressure;
        public decimal? AbsolutePressure => _absolutePressure.Value;
        private decimal? _gaugePressure;

        public decimal? GaugePressure
        {
            get => _gaugePressure;
            set
            {
                this.RaiseAndSetIfChanged(ref _gaugePressure, value);
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(TestRun.VerificationTest));
            }
        }

        public bool ShowAbsolute => TestRun.VerificationTest.Instrument.Transducer == TransducerType.Absolute;
        public bool ShowGaugeOnly => !ShowAbsolute;
        private decimal? _atmosphericGauge;

        public decimal? AtmosphericGauge
        {
            get => _atmosphericGauge;
            set => this.RaiseAndSetIfChanged(ref _atmosphericGauge, value);
        }

        public decimal? EvcGasPressure => TestRun.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue;
        public decimal? EvcFactor => TestRun.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue;

        public decimal? EvcAtmPressure =>
            TestRun.VerificationTest.Instrument.Items.GetItem(ItemCodes.Pressure.Atm).NumericValue;

        protected override void RaisePropertyChangeEvents()
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => TestRun.PercentError);
            NotifyOfPropertyChange(() => TestRun.HasPassed);
            NotifyOfPropertyChange(() => EvcGasPressure);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => EvcAtmPressure);
            NotifyOfPropertyChange(() => AtmosphericGauge);
            NotifyOfPropertyChange(() => PercentColour);
        }

        public void Handle(AtmosphericGaugePressureUpdateMessage message)
        {
            if (message.Sender != this)
                AtmosphericGauge = message.Value;
        }
    }
}