using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.Certificates.Common;
using Prover.GUI.Modules.Certificates.Reports;
using Prover.GUI.Modules.ClientManager.Screens.CsvExporter;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.GUI.Modules.Certificates.Screens
{
    public class CertificateCreatorViewModel : ViewModelBase, IDisposable
    {

        public List<string> VerificationType => new List<string>
        {
            "New",
            "Re-Verified"
        };
  
        private readonly IClientStore _clientStore;
        private readonly ICertificateService _certificateService;
        private readonly Client _allClient = new Client {Id = Guid.Empty, Name = "(No client)"};

        public CertificateCreatorViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IClientStore clientStore, ICertificateService certificateService, ExportToCsvViewModel exportToCsvViewModel)
            : base(screenManager, eventAggregator)
        {
            ExportToCsvViewModel = exportToCsvViewModel;
            _clientStore = clientStore;
            _certificateService = certificateService;

            var canCreateCertificate = this.WhenAnyValue(x => x.TestedBy, x => x.SelectedVerificationType, x => x.SelectedClient, 
                (testedBy, vt, client) 
                    => !string.IsNullOrEmpty(testedBy) && !string.IsNullOrEmpty(SelectedVerificationType) && client != null && client.Id != Guid.Empty);
            CreateCertificateCommand = ReactiveCommand.CreateFromTask(CreateCertificate, canCreateCertificate);

            ExecuteTestSearch = ReactiveCommand.CreateFromTask<Guid?, ReactiveList<CreateVerificationViewModel>>(GetInstrumentsWithNoCertificate);
            ExecuteTestSearch.ToPropertyEx(this, x => x.RootResults, new ReactiveList<CreateVerificationViewModel>());

            UpdateFilter = ReactiveCommand.Create<string>(filter =>
            {
                ResultFilteredItems = RootResults.CreateDerivedCollection(x => x,
                    x => filter == "All" || string.IsNullOrEmpty(filter) || (filter == "Passed" && x.Instrument.HasPassed) || (filter == "Failed" && !x.Instrument.HasPassed),
                    (x, y) => x.Instrument.TestDateTime.CompareTo(y.Instrument.TestDateTime));
            });

            this.WhenAnyValue(x => x.RootResults)
                .Select(s => "All")
                .InvokeCommand(UpdateFilter);

            LoadClientsCommand = ReactiveCommand.CreateFromTask(LoadClients);
            LoadClientsCommand
                .ToPropertyEx(this, x => x.Clients, new List<Client>());

            FetchNextCertificateNumberCommand = ReactiveCommand.CreateFromTask(_certificateService.GetNextCertificateNumber);
            FetchNextCertificateNumberCommand
                .ToPropertyEx(this, x => x.NextCertificateNumber);

            FetchExistingClientCertificatesCommand =
                ReactiveCommand.CreateFromTask<Client, IEnumerable<Certificate>>(_certificateService.GetAllCertificates);
            FetchExistingClientCertificatesCommand
                .ToPropertyEx(this, x => x.ExistingClientCertificates, new List<Certificate>());

            var canExecutePrintCommand = this.WhenAnyValue(x => x.SelectedExistingClientCertificate)
                .Select(c => c != null);
            PrintExistingCertificateCommand =
                ReactiveCommand.CreateFromTask<Certificate>(PrintExistingCertificate, canExecutePrintCommand);

            var selectedClientChange = this.WhenAnyValue(x => x.SelectedClient);            
            selectedClientChange
                .Where(c => c != null)
                .Select(x => x.Id)
                .Delay(TimeSpan.FromMilliseconds(100))
                .InvokeCommand(ExecuteTestSearch);

            selectedClientChange
                .Where(c => c != null)
                .InvokeCommand(FetchExistingClientCertificatesCommand);

            selectedClientChange
                .Where(c => c != null)
                .Subscribe(c => ExportToCsvViewModel.Client = c);

            var searchIsExecuting = this.WhenAnyObservable(x => x.ExecuteTestSearch.IsExecuting);
            selectedClientChange
                .CombineLatest(searchIsExecuting, (client, search) => client != null && search)
                .ToPropertyEx(this, x => x.ShowLoadingIndicator);

            this.WhenAnyValue(x => x.ResultFilteredItems)
                .CombineLatest(searchIsExecuting, (results, exec) => results != null && results.Any() && !exec)
                .ToPropertyEx(this, x => x.ShowTestViewListBox);

            searchIsExecuting
                .Where(x => !x)
                .CombineLatest(this.WhenAnyValue(x => x.ResultFilteredItems, results => results != null && !results.Any()),
                    (searchExecuting, clients) => clients && !searchExecuting)
                .ToPropertyEx(this, x => x.DisplayHelpBlankState, true);

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


        #region Commands

        public ReactiveCommand<Certificate, Unit> PrintExistingCertificateCommand { get; set; }

        public ReactiveCommand<Unit, long> FetchNextCertificateNumberCommand { get; set; }

        public ReactiveCommand<Unit, List<Client>> LoadClientsCommand { get; protected set; }

        public ReactiveCommand<Guid?, ReactiveList<CreateVerificationViewModel>> ExecuteTestSearch { get; }

        public ReactiveCommand<string, IEnumerable<CreateVerificationViewModel>> FilterResultsCommand { get; private set; }

        public ReactiveCommand<Client, IEnumerable<Certificate>> FetchExistingClientCertificatesCommand { get; set; }

        public ReactiveCommand CreateCertificateCommand { get; set; }

        public ReactiveCommand<string, Unit> UpdateFilter { get; set; }

        #endregion

        #region Properties        
        public extern ReactiveList<CreateVerificationViewModel> RootResults { [ObservableAsProperty] get; }
        public extern bool DisplayHelpBlankState { [ObservableAsProperty] get; }
        public extern bool ShowLoadingIndicator { [ObservableAsProperty] get; }
        public extern bool ShowTestViewListBox { [ObservableAsProperty] get; }
        public extern long NextCertificateNumber { [ObservableAsProperty] get; }      
        public extern IEnumerable<Certificate> ExistingClientCertificates { [ObservableAsProperty] get; }
        public extern IEnumerable<Client> Clients { [ObservableAsProperty] get; }
      
        [Reactive]
        public Certificate SelectedExistingClientCertificate { get; set; }
        
        [Reactive]
        public string SelectedVerificationType { get; set; }

        [Reactive]
        public Client SelectedClient { get; set; }

        [Reactive]
        public string TestedBy { get; set; }

        [Reactive]
        public ExportToCsvViewModel ExportToCsvViewModel { get; set; }           

        [Reactive]
        public IReactiveDerivedList<CreateVerificationViewModel> ResultFilteredItems { get; set; }
       
        #endregion

        #region Private Functions
        private async Task<ReactiveList<CreateVerificationViewModel>> GetInstrumentsWithNoCertificate(Guid? clientId = null)
        {
            var results = new ReactiveList<CreateVerificationViewModel>();
            var instruments = await _certificateService.GetInstrumentsWithNoCertificate(clientId);

            foreach (var i in instruments)
            {
                var vvm = new VerificationViewModel(i);
                var item = ScreenManager.ResolveViewModel<CreateVerificationViewModel>();
                item.VerificationView = vvm;
                results.Add(item);
            }
            return results;
        }       

        private async Task<List<Client>> LoadClients()
        {
            var clients = new List<Client> { _allClient };
            clients.AddRange(await _clientStore.GetAll());
            return clients;
        }

        private async Task CreateCertificate()
        {
            var instruments = ResultFilteredItems.Where(x => x.IsSelected)
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

            var certificate =
                await _certificateService.CreateCertificate(TestedBy, SelectedVerificationType, instruments);

            var search = ExecuteTestSearch.Execute(SelectedClient?.Id);

            CertificateGenerator.GenerateXps(certificate);
            await FetchNextCertificateNumberCommand.Execute();
            await search;
        }

        private async Task PrintExistingCertificate(Certificate certificate)
        {
            if (certificate == null) return;

            var cert = await _certificateService.GetCertificate(certificate.Number);

            if (cert == null)
            {
                MessageBox.Show("No certificate found.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CertificateGenerator.GenerateXps(cert);
        }
        #endregion

        public void Dispose()
        {
            ResultFilteredItems?.Dispose();
            ExecuteTestSearch?.Dispose();
            CreateCertificateCommand?.Dispose();
        }
    }
}