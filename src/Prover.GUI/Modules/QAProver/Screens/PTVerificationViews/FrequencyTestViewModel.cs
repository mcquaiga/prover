using System;
using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public class SaveTestEvent
    {
        
    }

    public class FrequencyTestViewModel : TestRunViewModelBase<Core.Models.Instruments.FrequencyTest>
    {       
        public FrequencyTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, Core.Models.Instruments.FrequencyTest testRun) : base(screenManager, eventAggregator, testRun)
        {
            _mainRotorPulses = testRun.MainRotorPulseCount;
            _senseRotorPulses = testRun.SenseRotorPulseCount;
            _mechanicalOutputFactor = testRun.MechanicalOutputFactor;
            RaisePropertyChangeEvents();

            this.WhenAnyValue(x => x.MainRotorPulses, x => x.SenseRotorPulses, x => x.MechanicalOutputFactor)
                .Subscribe(x =>
                {
                    TestRun.MainRotorPulseCount = x.Item1 ?? 0;
                    TestRun.SenseRotorPulseCount = x.Item2 ?? 0;
                    TestRun.MechanicalOutputFactor = x.Item3 ?? 0;
                    eventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(TestRun.VerificationTest));
                    eventAggregator.PublishOnUIThread(new SaveTestEvent());
                });
        }

        private long? _mechanicalOutputFactor;
        public long? MechanicalOutputFactor
        {
            get { return _mechanicalOutputFactor; }
            set { this.RaiseAndSetIfChanged(ref _mechanicalOutputFactor, value); }
        }

        private long? _mainRotorPulses;
        public long? MainRotorPulses
        {
            get { return _mainRotorPulses; }
            set { this.RaiseAndSetIfChanged(ref _mainRotorPulses, value); }
        }

        private long? _senseRotorPulses;
        public long? SenseRotorPulses
        {
            get { return _senseRotorPulses; }
            set { this.RaiseAndSetIfChanged(ref _senseRotorPulses, value); }
        }

        private decimal _unadjustedVolume;

        public decimal UnadjustedVolume
        {
            get { return _unadjustedVolume; }
            set { this.RaiseAndSetIfChanged(ref _unadjustedVolume, value); }
        }

        private decimal _evcUnadjustedVolume;

        public decimal EvcUnadjustedVolume
        {
            get { return _evcUnadjustedVolume; }
            set { this.RaiseAndSetIfChanged(ref _evcUnadjustedVolume, value); }
        }

        private decimal _adjustedVolume;

        public decimal AdjustedVolume
        {
            get { return _adjustedVolume; }
            set { this.RaiseAndSetIfChanged(ref _adjustedVolume, value); }
        }

        private decimal _evcAdjustedVolume;

        public decimal EvcAdjustedVolume
        {
            get { return _evcAdjustedVolume; }
            set { this.RaiseAndSetIfChanged(ref _evcAdjustedVolume, value); }
        }

        protected sealed override void RaisePropertyChangeEvents()
        {
            AdjustedVolume = TestRun.AdjustedVolume();
            TestRun.RoundedAdjustedVolume();
            UnadjustedVolume = TestRun.UnadjustedVolume();
            EvcAdjustedVolume = TestRun.EvcAdjustedVolume() ?? 0;
            EvcUnadjustedVolume = TestRun.EvcUnadjustedVolume() ?? 0;
        }
    }
}
