using System;
using Caliburn.Micro;
using ReactiveUI;
using Prover.Core.VerificationTests;
using System.Reactive.Subjects;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class SaveTestEvent
    {
        
    }

    public class FrequencyTestViewModel : TestRunViewModelBase<Core.Models.Instruments.FrequencyTest>
    {
        private readonly IQaRunTestManager _testRunManager;

        public FrequencyTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, Core.Models.Instruments.FrequencyTest testRun
            , ISubject<VerificationTest> changeObservable, IQaRunTestManager testRunManager = null) : base(screenManager, eventAggregator, testRun, changeObservable)
        {
            _testRunManager = testRunManager;
            if (_testRunManager != null)
            {
                PreTestCommand = ReactiveCommand.CreateFromTask(_testRunManager.DownloadPreVolumeTest);
                PostTestCommand = ReactiveCommand.CreateFromTask(_testRunManager.DownloadPostVolumeTest);
            }

            _mainRotorPulses = testRun.MainRotorPulseCount;
            _senseRotorPulses = testRun.SenseRotorPulseCount;
            _mechanicalOutputFactor = testRun.MechanicalOutputFactor;

            this.WhenAnyValue(x => x.MainRotorPulses, x => x.SenseRotorPulses, x => x.MechanicalOutputFactor)
                .Subscribe(x =>
                {
                    TestRun.MainRotorPulseCount = x.Item1 ?? 0;
                    TestRun.SenseRotorPulseCount = x.Item2 ?? 0;
                    TestRun.MechanicalOutputFactor = x.Item3 ?? 0;
                    ChangedEvent.OnNext(TestRun.VerificationTest);                    
                });
        }

        #region Properties
        
        private ReactiveCommand _preTestCommand;
        public ReactiveCommand PreTestCommand
        {
            get
            {
                return _preTestCommand;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _preTestCommand, value);
            }
        }

        private ReactiveCommand _postTestCommand;
        public ReactiveCommand PostTestCommand
        {
            get
            {
                return _postTestCommand;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _postTestCommand, value);
            }
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

        private decimal _adjustedStartReading;

        public decimal AdjustedStartReading
        {
            get { return _adjustedStartReading; }
            set { this.RaiseAndSetIfChanged(ref _adjustedStartReading, value); }
        }

        private decimal _adjustedEndReading;

        public decimal AdjustedEndReading
        {
            get { return _adjustedEndReading; }
            set { this.RaiseAndSetIfChanged(ref _adjustedEndReading, value); }
        }

        #endregion

        #region Methods


        #endregion

        protected sealed override void RaisePropertyChangeEvents()
        {
            AdjustedVolume = TestRun.AdjustedVolume();
            TestRun.RoundedAdjustedVolume();
            UnadjustedVolume = TestRun.UnadjustedVolume();
            EvcAdjustedVolume = TestRun.TibAdjustedVolume() ?? 0;
            EvcUnadjustedVolume = TestRun.EvcUnadjustedVolume() ?? 0;

            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => TestRun.PercentError);
        }
    }
}
