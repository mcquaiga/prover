using System;
using Caliburn.Micro;
using Prover.GUI.Events;
using ReactiveUI;
using System.Threading.Tasks;
using Prover.Core.VerificationTests;
using System.Reactive.Subjects;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class FrequencyTestViewModel : TestRunViewModelBase<Core.Models.Instruments.FrequencyTest>
    {
        public decimal AdjustedEndReading
        {
            get { return _adjustedEndReading; }
            set { this.RaiseAndSetIfChanged(ref _adjustedEndReading, value); }
        }

        public decimal AdjustedStartReading
        {
            get { return _adjustedStartReading; }
            set { this.RaiseAndSetIfChanged(ref _adjustedStartReading, value); }
        }

        public decimal AdjustedVolume
        {
            get { return _adjustedVolume; }
            set { this.RaiseAndSetIfChanged(ref _adjustedVolume, value); }
        }

        public decimal EvcAdjustedVolume
        {
            get { return _evcAdjustedVolume; }
            set { this.RaiseAndSetIfChanged(ref _evcAdjustedVolume, value); }
        }

        public decimal EvcUnadjustedVolume
        {
            get { return _evcUnadjustedVolume; }
            set { this.RaiseAndSetIfChanged(ref _evcUnadjustedVolume, value); }
        }

        public long? MainRotorPulses
        {
            get { return _mainRotorPulses; }
            set { this.RaiseAndSetIfChanged(ref _mainRotorPulses, value); }
        }

        public long? MechanicalOutputFactor
        {
            get { return _mechanicalOutputFactor; }
            set { this.RaiseAndSetIfChanged(ref _mechanicalOutputFactor, value); }
        }

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

        public long? SenseRotorPulses
        {
            get { return _senseRotorPulses; }
            set { this.RaiseAndSetIfChanged(ref _senseRotorPulses, value); }
        }

        public decimal UnadjustedVolume
        {
            get { return _unadjustedVolume; }
            set { this.RaiseAndSetIfChanged(ref _unadjustedVolume, value); }
        }

        public FrequencyTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, Core.Models.Instruments.FrequencyTest testRun
            , ISubject<VerificationTest> changeObservable, IQaRunTestManager testRunManager = null) : base(screenManager, eventAggregator, testRun, changeObservable)
        {
            _testRunManager = testRunManager;
            if (_testRunManager != null)
            {
                PreTestCommand = ReactiveCommand.CreateFromTask(_testRunManager.DownloadPreVolumeTest);
                PostTestCommand = ReactiveCommand.CreateFromTask(_testRunManager.DownloadPostVolumeTest);
            }

            MainRotorPulses = testRun.MainRotorPulseCount;
            SenseRotorPulses = testRun.SenseRotorPulseCount;
            MechanicalOutputFactor = testRun.MechanicalOutputFactor;

            this.WhenAnyValue(x => x.MainRotorPulses, x => x.SenseRotorPulses, x => x.MechanicalOutputFactor)
                .Subscribe(x =>
                {
                    TestRun.MainRotorPulseCount = x.Item1 ?? 0;
                    TestRun.SenseRotorPulseCount = x.Item2 ?? 0;
                    TestRun.MechanicalOutputFactor = x.Item3 ?? 0;
                    ChangedEvent.OnNext(TestRun.VerificationTest);
                });
        }

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

        private readonly IQaRunTestManager _testRunManager;

        private decimal _adjustedEndReading;
        private decimal _adjustedStartReading;
        private decimal _adjustedVolume;
        private decimal _evcAdjustedVolume;
        private decimal _evcUnadjustedVolume;
        private long? _mainRotorPulses;
        private long? _mechanicalOutputFactor;
        private ReactiveCommand _postTestCommand;
        private ReactiveCommand _preTestCommand;
        private long? _senseRotorPulses;
        private decimal _unadjustedVolume;
    }

    public class SaveTestEvent
    {
    }
}