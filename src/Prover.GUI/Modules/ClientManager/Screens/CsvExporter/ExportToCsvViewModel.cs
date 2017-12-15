using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using Caliburn.Micro;
using Prover.Core.Exports;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Services;
using Prover.GUI.Screens;
using ReactiveUI;
using MessageBox = System.Windows.MessageBox;

namespace Prover.GUI.Modules.ClientManager.Screens.CsvExporter
{
    public class ExportToCsvViewModel : ViewModelBase, IWindowSettings, IDisposable
    {
        public ExportToCsvViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IExportCertificate exporter, ICertificateService certificateService) : base(screenManager, eventAggregator)
        {
            ExportCommand = ReactiveCommand.CreateFromTask<Client>(async client =>
            {
                var exportPath = GetExportDirectory();
                var csvFiles = await exporter.Export(Client, SelectedFromCertificate.Number,
                    SelectedToCertificate.Number, exportPath);

                if (csvFiles.Any())
                {
                    var fileDir = new FileInfo(csvFiles.First()).Directory;
                    if (fileDir != null)
                        Process.Start(fileDir.FullName);
                }
            });

            ExportCommand.ThrownExceptions
                .Subscribe(ex => { MessageBox.Show(ex.Message); });

            GetCertificatesCommand = ReactiveCommand.CreateFromObservable<Client, Certificate>(
                client => certificateService.GetAllCertificates(client).ToObservable());
            GetCertificatesCommand
                .Subscribe(c => ClientCertificates.Add(c));

            FromCertificates = ClientCertificates.CreateDerivedCollection(x => x);
            ToCertificates = ClientCertificates.CreateDerivedCollection(x => x);

            this.WhenAnyValue(x => x.Client)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(c => { ClientCertificates.Clear(); })
                .Where(c => c != null)
                .InvokeCommand(GetCertificatesCommand);
        }

        private static string GetExportDirectory()
        {
            var exportPath = string.Empty;
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    exportPath = fbd.SelectedPath;
            }

            return exportPath;
        }

        #region Properties

        public CsvTemplateNotFoundException NotFoundException { get; private set; }
        private Client _client;

        public Client Client
        {
            get => _client;
            set => this.RaiseAndSetIfChanged(ref _client, value);
        }

        //private readonly ObservableAsPropertyHelper<ReactiveList<Certificate>> _clientCertificates;
        //public ReactiveList<Certificate> ClientCertificates => _clientCertificates.Value;
        private ReactiveList<Certificate> _clientCertificates =
            new ReactiveList<Certificate> {ChangeTrackingEnabled = true};

        public ReactiveList<Certificate> ClientCertificates
        {
            get => _clientCertificates;
            set => this.RaiseAndSetIfChanged(ref _clientCertificates, value);
        }

        private IReactiveDerivedList<Certificate> _fromCertificates;

        public IReactiveDerivedList<Certificate> FromCertificates
        {
            get => _fromCertificates;
            set => this.RaiseAndSetIfChanged(ref _fromCertificates, value);
        }

        private IReactiveDerivedList<Certificate> _toCertificates;

        public IReactiveDerivedList<Certificate> ToCertificates
        {
            get => _toCertificates;
            set => this.RaiseAndSetIfChanged(ref _toCertificates, value);
        }

        private Certificate _selectedFromCertificate;

        public Certificate SelectedFromCertificate
        {
            get => _selectedFromCertificate;
            set => this.RaiseAndSetIfChanged(ref _selectedFromCertificate, value);
        }

        private Certificate _selectedToCertificate;

        public Certificate SelectedToCertificate
        {
            get => _selectedToCertificate;
            set => this.RaiseAndSetIfChanged(ref _selectedToCertificate, value);
        }

        #endregion

        #region Commands

        public ReactiveCommand<Client, Unit> ExportCommand { get; }
        public ReactiveCommand<Client, Certificate> GetCertificatesCommand { get; set; }

        #endregion

        public dynamic WindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.MinWidth = 300;
                settings.Title = "Export Certificates to CSVs";
                return settings;
            }
        }

        public override void Dispose()
        {
            _fromCertificates?.Dispose();
            _toCertificates?.Dispose();
            ExportCommand?.Dispose();
            GetCertificatesCommand?.Dispose();
        }
    }
}