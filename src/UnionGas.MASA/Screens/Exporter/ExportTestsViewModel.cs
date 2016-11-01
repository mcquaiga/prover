using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;

namespace UnionGas.MASA.Screens.Exporter
{
    public class ExportTestsViewModel : ViewModelBase, IHandle<DataStorageChangeEvent>
    {
        private readonly IExportTestRun _exportTestRun;
        private readonly IInstrumentStore<Instrument> _instrumentStore;

        public ExportTestsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IExportTestRun exportTestRun,
            IInstrumentStore<Instrument> instrumentStore) : base(screenManager, eventAggregator)
        {
            _exportTestRun = exportTestRun;
            _instrumentStore = instrumentStore;
            GetInstrumentsByCertificateId(null);
        }

        public List<string> VerificationTypes => new List<string> {"Verification", "Re-Verification"};

        public DateTime CreatedDateTime { get; set; }
        public string VerificationType { get; set; }
        public string TestedBy { get; set; }

        public int InstrumentCount
        {
            get { return InstrumentItems.Count(x => x.IsSelected); }
        }

        public ObservableCollection<QaTestRunGridViewModel> InstrumentItems { get; set; } =
            new ObservableCollection<QaTestRunGridViewModel>();

        public void Handle(DataStorageChangeEvent message)
        {
            NotifyOfPropertyChange(() => InstrumentItems);
        }

        public void OneWeekFilter()
        {
            GetInstrumentsWithNoCertificateLastWeek();
        }

        public void OneMonthFilter()
        {
            GetInstrumentsWithNoCertificateLastMonth();
        }

        public void ResetFilter()
        {
            GetInstrumentsByCertificateId(null);
        }

        public async Task<IEnumerable<Instrument>> GetSelectedInstruments()
        {
            return await Task.Run(() => InstrumentItems.Where(x => x.IsSelected).Select(i => i.Instrument));
        }

        public async Task ExportQARuns()
        {
            await _exportTestRun.Export(await GetSelectedInstruments());
        }

        public void GetInstrumentsByCertificateId(Guid? certificateGuid)
        {
            GetInstrumentVerificationTests(x => x.CertificateId == certificateGuid);
        }

        public void GetInstrumentsWithNoCertificateLastMonth()
        {
            var dateFilter = DateTime.Now.AddDays(-30);
            GetInstrumentVerificationTests(x => (x.CertificateId == null) && (x.TestDateTime >= dateFilter));
        }

        public void GetInstrumentsWithNoCertificateLastWeek()
        {
            var dateFilter = DateTime.Now.AddDays(-7);
            GetInstrumentVerificationTests(x => (x.CertificateId == null) && (x.TestDateTime >= dateFilter));
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
    }
}