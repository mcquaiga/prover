using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.Core.Exports;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Services;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;
using ReactiveUI.Legacy;
using ReactiveCommand = ReactiveUI.ReactiveCommand;
using System.IO;
using System.Diagnostics;

namespace Prover.GUI.Modules.ClientManager.Screens.CsvExporter
{
    public class ExportToCsvViewModel : ViewModelBase, IWindowSettings
    {
        public ExportToCsvViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, 
            IExportCertificate exporter, ICertificateService certificateService) : base(screenManager, eventAggregator)
        {
            ExportCommand = ReactiveCommand.CreateFromTask<Client>(async client =>
            {
                var csvFiles = await exporter.Export(Client, SelectedFromCertificate.Number, SelectedToCertificate.Number);

                if (csvFiles.Any())
                {
                    var fileDir = new FileInfo(csvFiles.First()).Directory;
                    Process.Start(fileDir.FullName);
                }
            });
            ExportCommand.ThrownExceptions
                .Subscribe(ex => MessageBox.Show(ex.Message));

            GetCertificatesCommand = ReactiveCommand.CreateFromObservable<Client, Certificate>(
                client => certificateService.GetAllCertificates(client).ToObservable());
            GetCertificatesCommand
                .Subscribe(c => ClientCertificates.Add(c));
            FromCertificates = ClientCertificates.CreateDerivedCollection(x => x);
            ToCertificates = ClientCertificates.CreateDerivedCollection(x => x);

            this.WhenAnyValue(x => x.Client)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(client =>
                {
                    ClientCertificates.Clear();
                })
                .Where(c => c != null)
                .InvokeCommand(GetCertificatesCommand);
        }

        #region Properties
        private Client _client;
        public Client Client
        {
            get { return _client; }
            set { this.RaiseAndSetIfChanged(ref _client, value); }
        }

        private ReactiveList<Certificate> _clientCertificates = new ReactiveList<Certificate>() {ChangeTrackingEnabled = true};
        public ReactiveList<Certificate> ClientCertificates
        {
            get { return _clientCertificates; }
            set { this.RaiseAndSetIfChanged(ref _clientCertificates, value); }
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
            get { return _selectedFromCertificate; }
            set { this.RaiseAndSetIfChanged(ref _selectedFromCertificate, value); }
        }

        private Certificate _selectedToCertificate;
        public Certificate SelectedToCertificate
        {
            get { return _selectedToCertificate; }
            set { this.RaiseAndSetIfChanged(ref _selectedToCertificate, value); }
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

    }
}
