using Caliburn.Micro;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.Certificates.Common;
using Prover.GUI.Modules.Certificates.Reports;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prover.Core.Exports;
using Prover.Core.Shared.Extensions;
using Prover.Core.Models.Certificates;

namespace Prover.GUI.Modules.Certificates.Screens
{
    public class CertificateCreatorViewModel : ViewModelBase, IDisposable
    {
        public List<string> VerificationType => new List<string>
        {
            "New",
            "Re-Verified"
        };

        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IClientStore _clientStore;
        private readonly ICertificateStore _certificateStore;
        private readonly IExportCertificate _certificateExporter;
        private readonly Client _allClient = new Client {Id = Guid.Empty, Name = "(No client)"};

        public CertificateCreatorViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Instrument> instrumentStore, IClientStore clientStore,
            ICertificateStore certificateStore, IExportCertificate certificateExporter)
            : base(screenManager, eventAggregator)
        {
            _instrumentStore = instrumentStore;
            _clientStore = clientStore;
            _certificateStore = certificateStore;
            _certificateExporter = certificateExporter;

            var canCreateCertificate = this.WhenAnyValue(x => x.TestedBy, x => x.SelectedVerificationType,
                x => x.SelectedClient,
                (testedBy, vt, client)
                    => !string.IsNullOrEmpty(testedBy) && !string.IsNullOrEmpty(SelectedVerificationType) &&
                       (client != null && client.Id != Guid.Empty));
            CreateCertificateCommand = ReactiveCommand.CreateFromTask(CreateCertificate, canCreateCertificate);

            ExecuteTestSearch = ReactiveCommand.CreateFromTask<Guid?, IEnumerable<CreateVerificationViewModel>>(GetInstrumentsWithNoCertificate);

            var searchIsExecuting = this.WhenAnyObservable(x => x.ExecuteTestSearch.IsExecuting);

            _searchResults = ExecuteTestSearch               
                .Delay(TimeSpan.FromMilliseconds(150))
                .ToProperty(this, x => x.SearchResults, new List<CreateVerificationViewModel>());

            LoadClientsCommand = ReactiveCommand.CreateFromTask(LoadClients);
            _clientsList = LoadClientsCommand.ToProperty(this, x => x.Clients, new List<Client>());

            FetchNextCertificateNumberCommand =
                ReactiveCommand.CreateFromTask(_certificateStore.GetNextCertificateNumber);
            _nextCertificateNumber = FetchNextCertificateNumberCommand.ToProperty(this, x => x.NextCertificateNumber);

            var selectedClientChange = this.WhenAnyValue(x => x.SelectedClient);

            _showLoadingIndicator = selectedClientChange
                .Select(c => c != null)
                .ToProperty(this, x => x.ShowLoadingIndicator);
            
            _showTestViewListBox = selectedClientChange
                .Select(c => false)
                .ToProperty(this, x => x.ShowTestViewListBox);
            
            _displayHelpBlankState = selectedClientChange
                .Select( c=> false)
                .ToProperty(this, x => x.DisplayHelpBlankState);

            selectedClientChange
                .Where(c => c != null)
                .Select(x => x.Id)
                .Delay(TimeSpan.FromMilliseconds(100))
                .InvokeCommand(ExecuteTestSearch);

            _showLoadingIndicator = selectedClientChange
                .CombineLatest(searchIsExecuting, (client, search) => client != null && search)
                .ToProperty(this, x => x.ShowLoadingIndicator);

            _showTestViewListBox = this.WhenAnyValue(x => x.SearchResults)
                .CombineLatest(searchIsExecuting, (results, exec) => results.Any() && !exec)
                .ToProperty(this, x => x.ShowTestViewListBox);

            _displayHelpBlankState = searchIsExecuting
                .Where(x => !x)
                .CombineLatest(this.WhenAnyValue(x => x.SearchResults, results => !results.Any()), 
                    (searchExecuting, clients) => clients && !searchExecuting)
                .ToProperty(this, x => x.DisplayHelpBlankState, true);

            var canExecutePrintCommand = this.WhenAnyValue(x => x.ExistingCertificateNumber, s => s.HasValue);

            PrintExistingCertificateCommand =
                ReactiveCommand.CreateFromTask<long?>(PrintExistingCerificate, canExecutePrintCommand);


            #region unused code

            //Instruments.ItemChanged
            //    .Where(x => x.PropertyName == "IsSelected" && (SelectedClient == null || !Instruments.Any(i => i.IsSelected)))
            //    .Select(v => v.Sender)
            //    .Subscribe(model =>
            //    {
            //        var selectedInstrument = model.Instrument;
            //        SelectedClient = model.Instrument.Client;
            //        GetInstrumentsWithNoCertificate(SelectedClient?.Id);
            //        Instruments.First(x => x.Instrument.Id == selectedInstrument.Id).IsSelected = true;
            //    });

            //Instruments.ItemChanged
            //    .Where(x => x.PropertyName == "IsSelected" && !Instruments.Any(i => i.IsSelected))
            //    .Select(v => v.Sender)
            //    .Subscribe(async model =>
            //    {
            //        SelectedClient = null;
            //        await GetInstrumentsWithNoCertificate();
            //    });

            #endregion
        }

        private async Task PrintExistingCerificate(long? certificateNumber)
        {
            if (!certificateNumber.HasValue) return;

            var cert = await _certificateStore.GetCertificate(certificateNumber.Value);

            if (cert == null)
            {
                MessageBox.Show("No certificate found.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CertificateGenerator.GenerateXps(cert);
        }

        public ReactiveCommand<long?, Unit> PrintExistingCertificateCommand { get; set; }

        public ReactiveCommand<Unit, long> FetchNextCertificateNumberCommand { get; set; }

        private async Task<List<Client>> LoadClients()
        {
            var clients = new List<Client> {_allClient};
            clients.AddRange(await _clientStore.GetAll());
            return clients;
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<CreateVerificationViewModel>> _searchResults;
        public IEnumerable<CreateVerificationViewModel> SearchResults => _searchResults.Value;

        public ReactiveCommand<Unit, List<Client>> LoadClientsCommand { get; protected set; }

        public ReactiveCommand<Guid?, IEnumerable<CreateVerificationViewModel>> ExecuteTestSearch
        {
            get;
            protected set;
        }

        public ReactiveCommand CreateCertificateCommand { get; set; }

        private readonly ObservableAsPropertyHelper<bool> _displayHelpBlankState;
        public bool DisplayHelpBlankState => _displayHelpBlankState.Value;

        private readonly ObservableAsPropertyHelper<bool> _showLoadingIndicator;
        public bool ShowLoadingIndicator => _showLoadingIndicator.Value;

        private readonly ObservableAsPropertyHelper<bool> _showTestViewListBox;
        public bool ShowTestViewListBox => _showTestViewListBox.Value;

        private readonly ObservableAsPropertyHelper<long> _nextCertificateNumber;
        public long NextCertificateNumber => _nextCertificateNumber.Value;

        private long? _existingCertificateNumber;

        public long? ExistingCertificateNumber
        {
            get { return _existingCertificateNumber; }
            set { this.RaiseAndSetIfChanged(ref _existingCertificateNumber, value); }
        }

        private Client _selectedClient;

        public Client SelectedClient
        {
            get { return _selectedClient; }
            set { this.RaiseAndSetIfChanged(ref _selectedClient, value); }
        }

        private string _testedBy;

        public string TestedBy
        {
            get { return _testedBy; }
            set { this.RaiseAndSetIfChanged(ref _testedBy, value); }
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<Client>> _clientsList;
        public IEnumerable<Client> Clients => _clientsList.Value;

        private string _selectedVerificationType;

        public string SelectedVerificationType
        {
            get { return _selectedVerificationType; }
            set { this.RaiseAndSetIfChanged(ref _selectedVerificationType, value); }
        }

        private async Task<IEnumerable<CreateVerificationViewModel>> GetInstrumentsWithNoCertificate(
            Guid? clientId = null)
        {
            var results = new List<CreateVerificationViewModel>();
            var instruments = await _certificateStore.GetInstrumentsWithNoCertificate(clientId);

            return await Task.Run(() =>
            {
                foreach (var i in instruments)
                {
                    var vvm = new VerificationViewModel(i);
                    var item = ScreenManager.ResolveViewModel<CreateVerificationViewModel>();
                    item.VerificationView = vvm;
                    results.Add(item);
                }
                return results;
            });            
        }

     
        private async Task CreateCertificate()
        {
            var instruments = SearchResults.Where(x => x.IsSelected)
                .Select(i => i.VerificationView.Instrument)
                .ToList();

            if (instruments.Count > 8)
            {
                MessageBox.Show("Maximum 5 instruments allowed per certificate.");
                return;
            }

            if (!instruments.Any())
            {
                MessageBox.Show("Please select at least one instrument.");
                return;
            }

            if (instruments.Select(x => x.ClientId).Distinct().Count() > 1)
                MessageBox.Show("Cannot have multiple clients on the same certificate.");

            var certificate = await _certificateStore.CreateCertificate(TestedBy, SelectedVerificationType, instruments);
            await ExportCertificate(certificate);

            var search = ExecuteTestSearch.Execute(SelectedClient?.Id);

            CertificateGenerator.GenerateXps(certificate);
            await FetchNextCertificateNumberCommand.Execute();
            await search;
        }

        private async Task ExportCertificate(Certificate certificate)
        {
            var instruments = certificate.Instruments;
            var client = instruments.First().Client;

            if (client.CreateCertificateCsvFile)
            {
                await _certificateExporter.Export(instruments);
            }
        }

        public void Dispose()
        {
            _searchResults?.Dispose();
            ExecuteTestSearch?.Dispose();
            CreateCertificateCommand?.Dispose();
        }
    }
}