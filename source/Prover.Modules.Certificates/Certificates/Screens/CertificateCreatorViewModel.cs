﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.Core.Exports;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Settings;
using Prover.GUI.Screens.Modules.Certificates.Reports;
using Prover.GUI.Screens.Modules.ClientManager.Screens.CsvExporter;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.Certificates.Screens
{
    public class CertificateCreatorViewModel : ViewModelBase, IDisposable
    {
        public List<string> VerificationType => Enum.GetNames(typeof(VerificationTypeEnum)).ToList();
        private readonly IClientService _clientService;
        private readonly ICertificateService _certificateService;
        private readonly ISettingsService _settingsService;
        private readonly Client _allClient = new Client {Id = Guid.Empty, Name = "(No client)"};

        public CertificateCreatorViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IClientService clientService, ICertificateService certificateService,
            ExportToCsvViewModel exportToCsvViewModel, ISettingsService settingsService)
            : base(screenManager, eventAggregator)
        {
            ExportToCsvViewModel = exportToCsvViewModel;
            
            _clientService = clientService;
            _certificateService = certificateService;
            _settingsService = settingsService;

            LoadInstrumentsCommand = ReactiveCommand.CreateFromObservable<Client, Instrument>(
                client =>
                {
                    return _certificateService
                        .GetInstrumentsWithNoCertificate(client.Id)
                        .OrderBy(x => x.SerialNumber)
                        .ToObservable()
                        .DefaultIfEmpty(null);
                });

            LoadInstrumentsCommand
                .ThrownExceptions
                .Subscribe(ex => Console.WriteLine(ex.Message));

            LoadInstrumentsCommand
                .Where(i => i != null)
                .Subscribe(i =>
                {
                    var item = ScreenManager.ResolveViewModel<TestListItemViewModel>();
                    item.Initialize(i, FilterObservable);
                    RootResults.Add(item);
                });

            FilterObservable = new Subject<Predicate<Instrument>>();
            UpdateFilterCommand = ReactiveCommand.Create<string>(filter =>
            {
                FilterObservable
                    .OnNext(x => filter == "All"
                                 || string.IsNullOrEmpty(filter)
                                 || filter == "Passed" && x.HasPassed
                                 || filter == "Failed" && !x.HasPassed);
            });

            ResultFilteredItems = RootResults.CreateDerivedCollection(
                x => x,
                x => x.IsDisplayed,
                (x, y) =>
                {
                    var sn = x.Instrument.SerialNumber.CompareTo(y.Instrument.SerialNumber);

                    if (sn == 0)
                    {
                         return x.Instrument.TestDateTime.CompareTo(y.Instrument.TestDateTime);
                    }

                    return sn;
                });                   

            ResultFilteredItems.ChangeTrackingEnabled = true;

            ResultFilteredItems.ItemChanged
                .Where(x => x.PropertyName == "IsArchived" && x.Sender.IsArchived)
                .Subscribe(x => RootResults.Remove(x.Sender));

            ResultFilteredItems.ItemChanged.Where(x => x.PropertyName == "IsSelected")
                .Select(x => ResultFilteredItems.Count(c => c.IsSelected))
                .Merge(RootResults.CountChanged.Select(x => 0))
                .ToProperty(this, x => x.SelectedCount, out _selectedCount);

            ResultFilteredItems.CountChanged
                .ToProperty(this, x => x.VisibleCount, out _visibleCount);

            LoadInstrumentsCommand.IsExecuting
                .ToProperty(this, x => x.ShowLoadingIndicator, out _showLoadingIndicator);

            ResultFilteredItems.CountChanged
                .Select(x => x > 0)
                .ToProperty(this, x => x.ShowTestViewListBox, out _showTestViewListBox);

            this.WhenAnyValue(x => x.ShowTestViewListBox)
                .CombineLatest(this.WhenAnyValue(x => x.ShowLoadingIndicator),
                    (list, loading) => !list && !loading)
                .ToProperty(this, x => x.DisplayHelpBlankState, out _displayHelpBlankState);
            BlankStateText = "Select a customer from the dropdown";
            LoadInstrumentsCommand
                .Take(1)
                .Subscribe(x => BlankStateText = "No tests found");

            LoadClientsCommand = ReactiveCommand.CreateFromObservable(LoadClients);
            LoadClientsCommand
                .Where(c => c != null)
                .Subscribe(c => Clients.Add(c));

            FetchNextCertificateNumberCommand = ReactiveCommand.Create(_certificateService.GetNextCertificateNumber);
            FetchNextCertificateNumberCommand
                .Subscribe(x => NextCertificateNumber = x);
            FetchNextCertificateNumberCommand.ThrownExceptions
                .Subscribe(ex => Log.Error(ex));

            Certificates = new ReactiveList<Certificate>() { ChangeTrackingEnabled = true };
            FetchExistingCertificatesCommand = ReactiveCommand.CreateFromObservable<Certificate>(() =>
                _certificateService.GetAllCertificates()
                    .ToObservable()
                    .DefaultIfEmpty(null));
            FetchExistingCertificatesCommand.ThrownExceptions
                .Subscribe(ex => Log.Error(ex));
            FetchExistingCertificatesCommand
                .Where(x => x != null)
                .Subscribe(x => Certificates.Add(x));

            ExistingClientCertificates = Certificates.CreateDerivedCollection(
                c => c,
                c => SelectedClient != null && c.ClientId == SelectedClient.Id,
                (x, y) => x.Number.CompareTo(y.Number));

            ExportToCsvViewModel.SetClientCertificatesList(ExistingClientCertificates);

            var canExecutePrintCommand = this.WhenAnyValue(x => x.SelectedExistingClientCertificate)
                .Select(c => c != null);
            PrintExistingCertificateCommand =
                ReactiveCommand.Create<Certificate>(PrintExistingCertificate, canExecutePrintCommand);

            var selectedClientChange = this.WhenAnyValue(x => x.SelectedClient)
                .Do(c =>
                {
                    RootResults.Clear();
                })
                .Where(c => c != null);

            selectedClientChange
                .ObserveOn(RxApp.MainThreadScheduler)
                .Delay(TimeSpan.FromMilliseconds(50))
                .Subscribe(client =>
                {
                    LoadInstrumentsCommand.Execute(client).Wait();                      
                });

            selectedClientChange                    
                .Subscribe(client =>
                {
                    ExistingClientCertificates = Certificates.CreateDerivedCollection(
                        c => c,
                        c => client != null && c.ClientId == client.Id,
                        (x, y) => x.Number.CompareTo(y.Number));

                    ExportToCsvViewModel.SetClientCertificatesList(ExistingClientCertificates);
                });

            GetTestedByCommand =
                ReactiveCommand.CreateFromObservable(() => _certificateService.GetDistinctTestedBy().ToObservable());
            GetTestedByCommand
                .Where(t => t != null)
                .Subscribe(tb => TestedBy.Add(tb));

            GetTestedByCommand
                .ThrownExceptions
                .Subscribe(ex => Log.Error(ex));

            var canCreateCertificate = this.WhenAnyValue(x => x.TestedBy, x => x.SelectedVerificationType, x => x.SelectedClient, x => x.SelectedMeasurementApparatus,
                (testedBy, vt, client, measurement) => !string.IsNullOrEmpty(SelectedTestedBy) &&
                                          !string.IsNullOrEmpty(SelectedVerificationType) && 
                                          SelectedMeasurementApparatus != null &&
                                          client != null &&
                                          client.Id != Guid.Empty);
            CreateCertificateCommand = ReactiveCommand.CreateFromTask(CreateCertificate, canCreateCertificate);

            MeasurementApparatuses = _settingsService.Shared.CertificateSettings.MeasurementApparatuses;
        }

        #region Commands 

        public ReactiveCommand<Client, Instrument> LoadInstrumentsCommand { get; set; }
        public ReactiveCommand<Certificate, Unit> PrintExistingCertificateCommand { get; set; }
        public ReactiveCommand<Unit, long> FetchNextCertificateNumberCommand { get; set; }
        public ReactiveCommand<Unit, Client> LoadClientsCommand { get; protected set; }
        public ReactiveCommand<Unit, Certificate> FetchExistingCertificatesCommand { get; set; }
        public ReactiveCommand CreateCertificateCommand { get; set; }
        public ReactiveCommand<Unit, string> GetTestedByCommand { get; set; }

        #endregion

        #region Fody Properties

        //public extern ReactiveList<Instrument> RootClientInstruments { [ObservableAsProperty] get; }
        //public extern ReactiveList<CreateVerificationViewModel> RootResults { [ObservableAsProperty] get; }
        //public extern bool ShowLoadingIndicator { [ObservableAsProperty] get; }
        //public extern bool ShowTestViewListBox { [ObservableAsProperty] get; }
        //public extern long NextCertificateNumber { [ObservableAsProperty] get; }      
        //public extern IEnumerable<Certificate> ExistingClientCertificates { [ObservableAsProperty] get; }
        //public extern IEnumerable<Client> Clients { [ObservableAsProperty] get; }

        //[Reactive]
        //public Certificate SelectedExistingClientCertificate { get; set; }

        //[Reactive]
        //public string SelectedVerificationType { get; set; }

        //[Reactive]
        //public Client SelectedClient { get; set; }

        //[Reactive]
        //public string TestedBy { get; set; }

        //[Reactive]
        //public ExportToCsvViewModel ExportToCsvViewModel { get; set; }      

        //[Reactive]
        //public IReactiveDerivedList<CreateVerificationViewModel> ResultFilteredItems { get; set; }

        #endregion

        #region Properties

        public ReactiveCommand<string, Unit> UpdateFilterCommand { get; set; }
        public Subject<Predicate<Instrument>> FilterObservable { get; set; }

        private ReactiveList<TestListItemViewModel> _rootResults =
            new ReactiveList<TestListItemViewModel> {ChangeTrackingEnabled = true};

        public ReactiveList<TestListItemViewModel> RootResults
        {
            get => _rootResults;
            set => this.RaiseAndSetIfChanged(ref _rootResults, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _showLoadingIndicator;
        public bool ShowLoadingIndicator => _showLoadingIndicator.Value;
        private readonly ObservableAsPropertyHelper<bool> _showTestViewListBox;
        public bool ShowTestViewListBox => _showTestViewListBox.Value;
        private readonly ObservableAsPropertyHelper<bool> _displayHelpBlankState;
        public bool DisplayHelpBlankState => _displayHelpBlankState.Value;
        private string _blankStateText;

        public string BlankStateText
        {
            get => _blankStateText;
            set => this.RaiseAndSetIfChanged(ref _blankStateText, value);
        }

        private long _nextCertificateNumber;
        public long NextCertificateNumber
        {
            get => _nextCertificateNumber;
            set => this.RaiseAndSetIfChanged(ref _nextCertificateNumber, value);
        }

        private ReactiveList<Certificate> _certificates;
        public ReactiveList<Certificate> Certificates
        {
            get => _certificates;
            set => this.RaiseAndSetIfChanged(ref _certificates, value);
        }

        private IReactiveDerivedList<Certificate> _existingClientCertificates;
        public IReactiveDerivedList<Certificate> ExistingClientCertificates
        {
            get => _existingClientCertificates;
            set => this.RaiseAndSetIfChanged(ref _existingClientCertificates, value);
        }

        private Certificate _selectedExistingClientCertificate;
        public Certificate SelectedExistingClientCertificate
        {
            get => _selectedExistingClientCertificate;
            set => this.RaiseAndSetIfChanged(ref _selectedExistingClientCertificate, value);
        }

        private Client _selectedClient;

        public Client SelectedClient
        {
            get => _selectedClient;
            set => this.RaiseAndSetIfChanged(ref _selectedClient, value);
        }

        private string _selectedTestedBy;

        public string SelectedTestedBy
        {
            get => _selectedTestedBy;
            set => this.RaiseAndSetIfChanged(ref _selectedTestedBy, value);
        }

        private ReactiveList<string> _testedBy = new ReactiveList<string> {ChangeTrackingEnabled = true};

        public ReactiveList<string> TestedBy
        {
            get => _testedBy;
            set => this.RaiseAndSetIfChanged(ref _testedBy, value);
        }

        private List<CertificateSettings.MeasurementApparatus> _measurementApparatus;
        public List<CertificateSettings.MeasurementApparatus> MeasurementApparatuses
        {
            get => _measurementApparatus;
            set => this.RaiseAndSetIfChanged(ref _measurementApparatus, value);
        }

        private CertificateSettings.MeasurementApparatus _selectedMeasurementApparatus;
        public CertificateSettings.MeasurementApparatus SelectedMeasurementApparatus
        {
            get => _selectedMeasurementApparatus;
            set => this.RaiseAndSetIfChanged(ref _selectedMeasurementApparatus, value);
        }

        private ReactiveList<Client> _clients =
            new ReactiveList<Client> {new Client {Id = Guid.Empty, Name = "(No Client)"}};

        public ReactiveList<Client> Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref _clients, value);
        }

        private string _selectedVerificationType;

        public string SelectedVerificationType
        {
            get => _selectedVerificationType;
            set => this.RaiseAndSetIfChanged(ref _selectedVerificationType, value);
        }

        private IReactiveDerivedList<TestListItemViewModel> _resultFilteredItems;

        public IReactiveDerivedList<TestListItemViewModel> ResultFilteredItems
        {
            get => _resultFilteredItems;
            set => this.RaiseAndSetIfChanged(ref _resultFilteredItems, value);
        }

        public ExportToCsvViewModel ExportToCsvViewModel { get; set; }
        private readonly ObservableAsPropertyHelper<int> _visibleCount;
        public int VisibleCount => _visibleCount.Value;
        private readonly ObservableAsPropertyHelper<int> _selectedCount;
        public int SelectedCount => _selectedCount.Value;

        #endregion

        #region Private Functions

        private IObservable<Client> LoadClients()
        {
            return _clientService.GetActiveClients()
                .ToObservable()
                .DefaultIfEmpty(null);
        }

        private async Task CreateCertificate()
        {
            var items = ResultFilteredItems.Where(x => x.IsSelected).ToList();
            var instruments = items.Select(i => i.VerificationView.Instrument).ToList();

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

            var certificate = await _certificateService.CreateCertificate(
                NextCertificateNumber,
                SelectedTestedBy,
                SelectedVerificationType,
                SelectedMeasurementApparatus,
                instruments);

            CertificateGenerator.GenerateXps(certificate);
            Certificates.Add(certificate);
            NextCertificateNumber++;
            RootResults.RemoveAll(items);
        }

        private void PrintExistingCertificate(Certificate certificate)
        {
            if (certificate == null) return;

            var cert = _certificateService.GetCertificate(certificate.Number);

            if (cert == null)
            {
                MessageBox.Show("No certificate found.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CertificateGenerator.GenerateXps(cert);
        }

        #endregion

        public override void Dispose()
        {
            foreach (var viewModel in RootResults)
                viewModel.Dispose();
            RootResults?.Clear();
            ResultFilteredItems?.Dispose();
            LoadInstrumentsCommand?.Dispose();
            CreateCertificateCommand?.Dispose();
        }
    }
}