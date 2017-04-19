using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.Certificates.Reports;
using ReactiveUI;

namespace Prover.GUI.Modules.Certificates.Screens
{
    public class CertificateViewModel : ViewModelBase, IHandle<DataStorageChangeEvent>
    {
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly ICertificateStore _certificateStore;

        public CertificateViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Instrument> instrumentStore, ICertificateStore certificateStore) : base(screenManager, eventAggregator)
        {
            _instrumentStore = instrumentStore;
            _certificateStore = certificateStore;
            GetInstrumentsWithNoCertificate();

            var canCreateCertificate = this.WhenAnyValue(x => x.TestedBy, x => x.SelectedVerificationType,
                ((testedBy, vt) => !string.IsNullOrEmpty(testedBy) && !string.IsNullOrEmpty(SelectedVerificationType)));

            CreateCertificateCommand = ReactiveCommand.CreateFromTask(CreateCertificate, canCreateCertificate);
        }

        public ObservableCollection<QaTestRunGridViewModel> InstrumentItems { get; set; } =
            new ObservableCollection<QaTestRunGridViewModel>();

        private ReactiveList<QaTestRunGridViewModel> _selectedInstrumentItems;

        public ReactiveList<QaTestRunGridViewModel> SelectedInstrumentItems
        {
            get { return _selectedInstrumentItems; }
            set { this.RaiseAndSetIfChanged(ref _selectedInstrumentItems, value); }
        }

        public IEnumerable<Instrument> PassedInstrumentTests =>
            InstrumentItems.Where(
                    x =>
                        x.Instrument.HasPassed && !string.IsNullOrEmpty(x.Instrument.JobId) &&
                        !string.IsNullOrEmpty(x.Instrument.EmployeeId))
                .Select(i => i.Instrument);

        public ReactiveCommand CreateCertificateCommand { get; set; }

        private string _testedBy;

        public string TestedBy
        {
            get { return _testedBy; }
            set { this.RaiseAndSetIfChanged(ref _testedBy, value); }
        }

        public List<string> VerificationType => new List<string>
        {
            "Verification",
            "Re-verification"
        };

        private string _selectedVerificationType;

        public string SelectedVerificationType
        {
            get { return _selectedVerificationType; }
            set { this.RaiseAndSetIfChanged(ref _selectedVerificationType, value); }
        }

        private void GetInstrumentsWithNoCertificate()
        {
            GetInstrumentVerificationTests(x => x.CertificateId == null && x.ArchivedDateTime == null);
        }

        private void GetInstrumentVerificationTests(Func<Instrument, bool> whereFunc)
        {
            InstrumentItems.Clear();

            var instruments = _instrumentStore.Query()
                .Where(whereFunc)
                .OrderBy(i => i.TestDateTime)
                .ToList();

            foreach (var i in instruments)
            {
                var item = ScreenManager.ResolveViewModel<QaTestRunGridViewModel>();
                item.Instrument = i;
                InstrumentItems.Add(item);
            }

            NotifyOfPropertyChange(() => InstrumentItems);
        }

        private async Task CreateCertificate()
        {
            var instruments = InstrumentItems.Where(x => x.IsSelected).Select(i => i.Instrument).ToList();

            if (instruments.Count() > 8)
            {
                MessageBox.Show("Maximum 8 instruments allowed per certificate.");
                return;
            }

            if (!instruments.Any())
            {
                MessageBox.Show("Please select at least one instrument.");
                return;
            }


            var certificate = await CreateCertificate(TestedBy, SelectedVerificationType, instruments);

            GetInstrumentsWithNoCertificate();

            CertificateGenerator.GenerateXps(certificate);
        }

        public void Handle(DataStorageChangeEvent message)
        {
            GetInstrumentsWithNoCertificate();
        }

        private async Task<Certificate> CreateCertificate(string testedBy, string verificationType,
            List<Instrument> instruments)
        {
            var latestNumber = _certificateStore.Query()
                .Select(x => x.Number)
                .OrderByDescending(x => x)
                .FirstOrDefault();


            var certificate = new Certificate
            {
                CreatedDateTime = DateTime.Now,
                VerificationType = verificationType,
                TestedBy = testedBy,
                Number = latestNumber + 1,
                Instruments = new Collection<Instrument>()
            };

            instruments.ForEach(i =>
            {
                i.CertificateId = certificate.Id;
                i.Certificate = certificate;
                certificate.Instruments.Add(i);
            });

            await _certificateStore.UpsertAsync(certificate);
            return certificate;
        }
    }
}