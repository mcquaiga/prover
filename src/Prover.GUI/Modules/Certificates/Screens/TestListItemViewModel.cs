using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.GUI.Modules.Certificates.Common;
using Prover.GUI.Reports;
using Prover.GUI.Screens;
using ReactiveUI;

namespace Prover.GUI.Modules.Certificates.Screens
{
    public class TestListItemViewModel : ViewModelBase
    {
        private readonly InstrumentReportGenerator _instrumentReportGenerator;
        private readonly TestRunService _testRunService;

        public TestListItemViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            TestRunService testRunService, InstrumentReportGenerator instrumentReportGenerator)
            : base(screenManager, eventAggregator)
        {
            _testRunService = testRunService;
            _instrumentReportGenerator = instrumentReportGenerator;

            ArchiveTestCommand = ReactiveCommand.CreateFromTask(ArchiveTest);

            ViewQaTestReportCommand = ReactiveCommand.CreateFromTask(DisplayInstrumentReport);

            AddTestToCertificate = ReactiveCommand.Create(() => IsSelected = true);
        }

        #region Properties

        private bool _isDisplayed = true;

        public bool IsDisplayed
        {
            get => _isDisplayed;
            set => this.RaiseAndSetIfChanged(ref _isDisplayed, value);
        }

        private ReactiveCommand _addTestToCertificate;

        public ReactiveCommand AddTestToCertificate
        {
            get => _addTestToCertificate;
            set => this.RaiseAndSetIfChanged(ref _addTestToCertificate, value);
        }

        private VerificationViewModel _verificationViewModel;

        public VerificationViewModel VerificationView
        {
            get => _verificationViewModel;
            set
            {
                this.RaiseAndSetIfChanged(ref _verificationViewModel, value);
                Instrument = _verificationViewModel?.Instrument;
            }
        }

        private Instrument _instrument;

        public Instrument Instrument
        {
            get => _instrument;
            set => this.RaiseAndSetIfChanged(ref _instrument, value);
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        private ReactiveCommand _viewQaTestReportCommand;

        public ReactiveCommand ViewQaTestReportCommand
        {
            get => _viewQaTestReportCommand;
            set => this.RaiseAndSetIfChanged(ref _viewQaTestReportCommand, value);
        }

        private ReactiveCommand _archiveTestCommand;

        public ReactiveCommand ArchiveTestCommand
        {
            get => _archiveTestCommand;
            set => this.RaiseAndSetIfChanged(ref _archiveTestCommand, value);
        }

        private bool _isArchived;

        public bool IsArchived
        {
            get => _isArchived;
            set => this.RaiseAndSetIfChanged(ref _isArchived, value);
        }

        #endregion

        public void Initialize(Instrument instrument, IObservable<Predicate<Instrument>> filterObservable)
        {
            var vvm = new VerificationViewModel(instrument);
            VerificationView = vvm;

            filterObservable.Subscribe(p => { IsDisplayed = p(Instrument); });
        }

        public async Task DisplayInstrumentReport()
        {
            await _instrumentReportGenerator.GenerateAndViewReport(VerificationView.Instrument);
        }

        public async Task ArchiveTest()
        {
            await _testRunService.ArchiveTest(VerificationView.Instrument);
            IsArchived = true;
        }

        public override void Dispose()
        {
            VerificationView = null;
            base.Dispose();
        }
    }
}