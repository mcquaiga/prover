using Caliburn.Micro;
using Prover.GUI.Common.Events;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
{
    public class SaveTestEvent
    {
        
    }

    public class FrequencyTestViewModel : TestRunViewModelBase<Core.Models.Instruments.FrequencyTest>
    {
        private readonly IQaRunTestManager _testRunManager;

        public FrequencyTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, Core.Models.Instruments.FrequencyTest testRun
            , IQaRunTestManager testRunManager = null) : base(screenManager, eventAggregator, testRun)
        {
            _testRunManager = testRunManager;
            PreTestCommand = ReactiveCommand.CreateFromTask(_testRunManager.DownloadPreVolumeTest);
            PostTestCommand = ReactiveCommand.CreateFromTask(_testRunManager.DownloadPostVolumeTest);

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

        #endregion

        #region Methods

     
        #endregion

        protected sealed override void RaisePropertyChangeEvents()
        {
            AdjustedVolume = TestRun.AdjustedVolume();
            TestRun.RoundedAdjustedVolume();
            UnadjustedVolume = TestRun.UnadjustedVolume();
            EvcAdjustedVolume = TestRun.EvcAdjustedVolume() ?? 0;
            EvcUnadjustedVolume = TestRun.EvcUnadjustedVolume() ?? 0;

            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => TestRun.PercentError);
        }
    }
}
