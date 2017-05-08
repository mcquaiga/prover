#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.Certificates.Common;
using Prover.GUI.Modules.Certificates.Reports;
using ReactiveUI;

namespace Prover.GUI.Modules.Certificates.Screens
{
    public class CertificateCreatorViewModel : ViewModelBase, IHandle<DataStorageChangeEvent>, IDisposable
    {
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IProverStore<Client> _clientStore;
        private readonly ICertificateStore _certificateStore;
        private readonly Client _allClient = new Client {Id = Guid.Empty, Name = "None"};

        public CertificateCreatorViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Instrument> instrumentStore, IProverStore<Client> clientStore,
            ICertificateStore certificateStore)
            : base(screenManager, eventAggregator)
        {
            _instrumentStore = instrumentStore;
            _clientStore = clientStore;
            _certificateStore = certificateStore;

            var canCreateCertificate = this.WhenAnyValue(x => x.TestedBy, x => x.SelectedVerificationType,
                (testedBy, vt) => !string.IsNullOrEmpty(testedBy) && !string.IsNullOrEmpty(SelectedVerificationType));
            CreateCertificateCommand = ReactiveCommand.CreateFromTask(CreateCertificate, canCreateCertificate);

            ExecuteTestSearch =
                ReactiveCommand.CreateFromTask<Guid?, IEnumerable<CreateVerificationViewModel>>(
                    GetInstrumentsWithNoCertificate);
            _searchResults = ExecuteTestSearch.ToProperty(this, x => x.SearchResults,
                new List<CreateVerificationViewModel>());

            LoadClientsCommand = ReactiveCommand.CreateFromTask(LoadClients);
            _clientsList = LoadClientsCommand.ToProperty(this, x => x.Clients, new List<Client>());

            var selectedClientChange = this.WhenAnyValue(x => x.SelectedClient);

            selectedClientChange
                .Where(c => c != null)
                .Select(x => x.Id)
                .InvokeCommand(ExecuteTestSearch);

            _displayHelpBlankState = selectedClientChange
                .Select(c => c == null)
                .ToProperty(this, x => x.DisplayHelpBlankState, true);

            _displayHelpBlankState = ExecuteTestSearch.IsExecuting
                .Select(y => !y && !SearchResults.Any())
                .ToProperty(this, x => x.DisplayHelpBlankState);

            _showLoadingIndicator = ExecuteTestSearch.IsExecuting
                .ToProperty(this, x => x.ShowLoadingIndicator);

            _showTestViewListBox = selectedClientChange
                .Select(x => x != null)
                .ToProperty(this, x => x.ShowTestViewListBox);

            _showTestViewListBox = ExecuteTestSearch.IsExecuting
                .Select(x => !x)
                .ToProperty(this, x => x.ShowTestViewListBox);


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

        private async Task<List<Client>> LoadClients()
        {
            return await Task.Run(() =>
            {
                var clients = new List<Client> {_allClient};

                clients.AddRange(_clientStore.Query()
                    .OrderBy(c => c.Name)
                    .ToList());
                return clients;
            });
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

        private string _testedBy;

        public string TestedBy
        {
            get => _testedBy;
            set => this.RaiseAndSetIfChanged(ref _testedBy, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _displayHelpBlankState;
        public bool DisplayHelpBlankState => _displayHelpBlankState.Value;

        private readonly ObservableAsPropertyHelper<bool> _showLoadingIndicator;
        public bool ShowLoadingIndicator => _showLoadingIndicator.Value;

        private readonly ObservableAsPropertyHelper<bool> _showTestViewListBox;
        public bool ShowTestViewListBox => _showTestViewListBox.Value;

        private Client _selectedClient;

        public Client SelectedClient
        {
            get => _selectedClient;
            set => this.RaiseAndSetIfChanged(ref _selectedClient, value);
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<Client>> _clientsList;
        public IEnumerable<Client> Clients => _clientsList.Value;

        public List<string> VerificationType => new List<string>
        {
            "Verification",
            "Re-verification"
        };

        private string _selectedVerificationType;

        public string SelectedVerificationType
        {
            get => _selectedVerificationType;
            set => this.RaiseAndSetIfChanged(ref _selectedVerificationType, value);
        }

        private async Task<IEnumerable<CreateVerificationViewModel>> GetInstrumentsWithNoCertificate(
            Guid? clientId = null)
        {
            clientId = clientId == Guid.Empty ? null : clientId;
            var tests = await Task.Run(() =>
            {
                var results = new List<CreateVerificationViewModel>();
                var instruments = GetInstruments(
                        x => x.CertificateId == null && x.ArchivedDateTime == null && x.ClientId == clientId)
                    .ToList();

                foreach (var i in instruments)
                {
                    var vvm = new VerificationViewModel(i);
                    var item = ScreenManager.ResolveViewModel<CreateVerificationViewModel>();
                    item.VerificationView = vvm;
                    results.Add(item);
                }
                return results;
            });

            return tests;
        }

        private IEnumerable<Instrument> GetInstruments(Func<Instrument, bool> whereFunc)
        {
            return _instrumentStore.Query().Where(whereFunc).OrderBy(i => i.TestDateTime);
        }

        private async Task CreateCertificate()
        {
            var instruments = SearchResults.Where(x => x.IsSelected)
                .Select(i => i.VerificationView.Instrument)
                .ToList();

            if (instruments.Count > 8)
            {
                MessageBox.Show("Maximum 8 instruments allowed per certificate.");
                return;
            }

            if (!instruments.Any())
            {
                MessageBox.Show("Please select at least one instrument.");
                return;
            }

            if (instruments.Select(x => x.ClientId).Distinct().Count() > 1)
                MessageBox.Show("Cannot have multiple clients on the same certificate.");



            var certificate = await CreateCertificate(TestedBy, SelectedVerificationType, instruments);

            var search = ExecuteTestSearch.Execute(SelectedClient?.Id);

            CertificateGenerator.GenerateXps(certificate);

            await search;
        }

        public void Handle(DataStorageChangeEvent message)
        {
            // GetInstrumentsWithNoCertificate();
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

        public void Dispose()
        {
            _searchResults?.Dispose();
            ExecuteTestSearch?.Dispose();
            CreateCertificateCommand?.Dispose();
        }
    }
}

#endregion