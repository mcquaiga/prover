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
using ReactiveUI;

namespace Prover.Modules.CertificatesUi.Screens.Certificates
{
    public class CertificateViewModel : ViewModelBase, IHandle<DataStorageChangeEvent>
    {
        private readonly IExportTestRun _exportTestRun;
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IProverStore<Certificate> _certificateStore;

        public CertificateViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, 
            IProverStore<Instrument> instrumentStore, IProverStore<Certificate> certificateStore, IExportTestRun exportTestRun = null) : base(screenManager, eventAggregator)
        {
            _exportTestRun = exportTestRun;
            _instrumentStore = instrumentStore;
            _certificateStore = certificateStore;
            GetInstrumentsWithNoExportDate();
            
            var canExportAllPassed = this.WhenAnyValue(x => x.PassedInstrumentTests, 
                (passed) => passed.Any());          
            ExportAllPassedQaRunsCommand = ReactiveCommand.CreateFromTask(ExportAllPassedQaRuns, canExportAllPassed);
        }

        public ObservableCollection<QaTestRunGridViewModel> InstrumentItems { get; set; } =
            new ObservableCollection<QaTestRunGridViewModel>();

        public IEnumerable<Instrument> PassedInstrumentTests => 
            InstrumentItems.Where(x => x.Instrument.HasPassed && !string.IsNullOrEmpty(x.Instrument.JobId) && !string.IsNullOrEmpty(x.Instrument.EmployeeId))
                .Select(i => i.Instrument);

        public ReactiveCommand ExportAllPassedQaRunsCommand { get; set; }

        public async Task ExportAllPassedQaRuns()
        {
            await _exportTestRun.Export(PassedInstrumentTests);
        }

        private void GetInstrumentsWithNoExportDate()
        {
            GetInstrumentVerificationTests(x => x.ExportedDateTime == null && x.ArchivedDateTime == null);
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

        public void CreateCertificate()
        {
            var selectedInstruments = InstrumentItems.Where(x => x.IsSelected).ToList();

            if (selectedInstruments.Count() > 8)
            {
                MessageBox.Show("Maximum 8 instruments allowed per certificate.");
                return;
            }

            if (!selectedInstruments.Any())
            {
                MessageBox.Show("Please select at least one instrument.");
                return;
            }

            //if (VerificationType == null || TestedBy == null)
            //{
            //    MessageBox.Show("Please enter a tested by and verificate type.");
            //    return;
            //}

            var instruments = InstrumentItems.Where(x => x.IsSelected).Select(i => i.Instrument).ToList();
            var certificate = CreateCertificate("adam", "Verified", instruments);

            GetInstrumentsWithNoExportDate();

            CertificateGenerator(certificate);
        }

        public void Handle(DataStorageChangeEvent message)
        {
            GetInstrumentsWithNoExportDate();
        }

        private async Task<Certificate> CreateCertificate(string testedBy, string verificationType, List<Instrument> instruments)
        {
            var number = _certificateStore.Query().DefaultIfEmpty().Max(x => x.Number) + 1;

            var certificate = new Certificate
            {
                CreatedDateTime = DateTime.Now,
                VerificationType = verificationType,
                TestedBy = testedBy,
                Number = number,
                Instruments = new Collection<Instrument>()
            };

            instruments.ForEach(i =>
            {
                i.CertificateId = certificate.Id;
                i.Certificate = certificate;
                certificate.Instruments.Add(i);
            });

            await _certificateStore.UpsertAsync(certificate);
            return new Certificate();
        }
    }
}