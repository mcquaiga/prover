using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Exports;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;
using ReactiveUI.Legacy;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Prover.GUI.Modules.ClientManager.Screens.CsvExporter
{
    public class ExportToCsvViewModel : ViewModelBase
    {
        private readonly IExportCertificate _exporter;
        private readonly IProverStore<Certificate> _certificateStore;

        public ExportToCsvViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, 
            IExportCertificate exporter, IProverStore<Certificate> certificateStore) : base(screenManager, eventAggregator)
        {
            _exporter = exporter;
            _certificateStore = certificateStore;

            ExportCommand = ReactiveCommand.CreateFromTask(ExportCertificates);
            GetCertificateNumbersCommand = ReactiveCommand.CreateFromTask<Client, List<long>>(GetCertificateNumbers);

            this.WhenAnyValue(x => x.Client)
                .Where(c => c != null)
                .InvokeCommand(GetCertificateNumbersCommand);

            _fromCertificateNumbers = GetCertificateNumbersCommand
                .ToProperty(this, x => x.FromCertificateNumbers);
            _toCertificateNumbers = GetCertificateNumbersCommand
                .ToProperty(this, x => x.ToCertificateNumbers);
        }

        #region Properties
        private Core.Models.Clients.Client _client;

        public Core.Models.Clients.Client Client
        {
            get { return _client; }
            set { this.RaiseAndSetIfChanged(ref _client, value); }
        }

        private readonly ObservableAsPropertyHelper<List<long>> _fromCertificateNumbers;
        public List<long> FromCertificateNumbers => _fromCertificateNumbers.Value;        

        private long _selectedFromCertificateNumber;

        public long SelectedFromCertificateNumber
        {
            get { return _selectedFromCertificateNumber; }
            set { this.RaiseAndSetIfChanged(ref _selectedFromCertificateNumber, value); }
        }

        private readonly ObservableAsPropertyHelper<List<long>> _toCertificateNumbers;
        public List<long> ToCertificateNumbers => _toCertificateNumbers.Value;


        private long _selectedToCertificateNumber;

        public long SelectedToCertificateNumber
        {
            get { return _selectedToCertificateNumber; }
            set { this.RaiseAndSetIfChanged(ref _selectedToCertificateNumber, value); }
        }
        #endregion

        #region Commands

        public ReactiveCommand ExportCommand { get; }

        public ReactiveCommand<Client, List<long>> GetCertificateNumbersCommand { get; }

        #endregion

        #region Private Functions
        private async Task ExportCertificates()
        {
            await _exporter.Export(Client, SelectedFromCertificateNumber, SelectedToCertificateNumber);
        }

        private async Task<List<long>> GetCertificateNumbers(Client client)
        {
            var results = await _certificateStore.Query()
                .Where(certs => certs.ClientId == client.Id)
                .OrderBy(c => c.Number)
                .Select(c => c.Number)
                .ToListAsync();

            return results;
        }
        #endregion
    }
}
