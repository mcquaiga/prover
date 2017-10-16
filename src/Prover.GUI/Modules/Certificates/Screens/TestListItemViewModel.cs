using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.Certificates.Common;
using Prover.GUI.Reports;
using ReactiveUI;

namespace Prover.GUI.Modules.Certificates.Screens
{
    public class TestListItemViewModel : ViewModelBase
    {
        private readonly InstrumentReportGenerator _instrumentReportGenerator;
        private readonly IProverStore<Instrument> _instrumentStore;

        public TestListItemViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Instrument> instrumentStore, InstrumentReportGenerator instrumentReportGenerator)
            : base(screenManager, eventAggregator)
        {
            _instrumentStore = instrumentStore;
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
            get { return _addTestToCertificate; }
            set { this.RaiseAndSetIfChanged(ref _addTestToCertificate, value); }
        }

        private VerificationViewModel _verificationViewModel;

        public VerificationViewModel VerificationView
        {
            get { return _verificationViewModel; }
            set
            {
                this.RaiseAndSetIfChanged(ref _verificationViewModel, value);
                Instrument = _verificationViewModel.Instrument;
            }
        }

        private Instrument _instrument;

        public Instrument Instrument
        {
            get { return _instrument; }
            set { this.RaiseAndSetIfChanged(ref _instrument, value); }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        private ReactiveCommand _viewQaTestReportCommand;

        public ReactiveCommand ViewQaTestReportCommand
        {
            get { return _viewQaTestReportCommand; }
            set { this.RaiseAndSetIfChanged(ref _viewQaTestReportCommand, value); }
        }

        private ReactiveCommand _archiveTestCommand;

        public ReactiveCommand ArchiveTestCommand
        {
            get { return _archiveTestCommand; }
            set { this.RaiseAndSetIfChanged(ref _archiveTestCommand, value); }
        }

        private bool _isArchived;

        public bool IsArchived
        {
            get => _isArchived;
            set => this.RaiseAndSetIfChanged(ref _isArchived, value);
        }

        #endregion

        public void SetFilter(IObservable<Predicate<Instrument>> filterObservable)
        {
            filterObservable.Subscribe(p =>
            {
                IsDisplayed = p(Instrument);
            });
        }

        public async Task DisplayInstrumentReport()
        {
            await _instrumentReportGenerator.GenerateAndViewReport(VerificationView.Instrument);
        }

        public async Task ArchiveTest()
        {
            await _instrumentStore.Delete(VerificationView.Instrument);
            IsArchived = true;
        }
    }
}