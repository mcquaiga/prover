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
    public class CreateVerificationViewModel : ViewModelBase
    {
        private readonly InstrumentReportGenerator _instrumentReportGenerator;
        private readonly IProverStore<Instrument> _instrumentStore;

        public CreateVerificationViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Instrument> instrumentStore, InstrumentReportGenerator instrumentReportGenerator)
            : base(screenManager, eventAggregator)
        {
            _instrumentStore = instrumentStore;
            _instrumentReportGenerator = instrumentReportGenerator;

            ArchiveTestCommand = ReactiveCommand.CreateFromTask(ArchiveTest);

            ViewQaTestReportCommand = ReactiveCommand.CreateFromTask(DisplayInstrumentReport);

            AddTestToCertificate = ReactiveCommand.Create(() => IsSelected = true);
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

        public async Task DisplayInstrumentReport()
        {
            await _instrumentReportGenerator.GenerateAndViewReport(VerificationView.Instrument);
        }

        private ReactiveCommand _archiveTestCommand;

        public ReactiveCommand ArchiveTestCommand
        {
            get { return _archiveTestCommand; }
            set { this.RaiseAndSetIfChanged(ref _archiveTestCommand, value); }
        }

        public async Task ArchiveTest()
        {
            await _instrumentStore.Delete(VerificationView.Instrument);
            await EventAggregator.PublishOnUIThreadAsync(new DataStorageChangeEvent());
        }
    }
}