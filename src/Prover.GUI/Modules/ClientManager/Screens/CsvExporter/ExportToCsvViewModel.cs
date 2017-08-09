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

namespace Prover.GUI.Modules.ClientManager.Screens.CsvExporter
{
    public class ExportToCsvViewModel : ViewModelBase, IWindowSettings
    {
        private readonly IExportCertificate _exporter;
        private readonly ICertificateService _certificateService;

        public ExportToCsvViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, 
            IExportCertificate exporter, ICertificateService certificateService) : base(screenManager, eventAggregator)
        {
            _exporter = exporter;
            _certificateService = certificateService;

            ExportCommand = ReactiveCommand.CreateFromTask<Client>(ExportCertificates);

            GetCertificatesCommand = 
                ReactiveCommand.CreateFromTask<Client, IEnumerable<Certificate>>(_certificateService.GetAllCertificates);

            this.WhenAnyValue(x => x.Client)
                .Where(c => c != null)
                .InvokeCommand(GetCertificatesCommand);

            //GetCertificatesCommand
            //    .Select(c => c.ToList())
            //    .Subscribe(certs =>
            //    {                    
            //        FromCertificates = certs;
            //        ToCertificates = certs;
            //    });

            _fromCertificates = GetCertificatesCommand
                .ToProperty(this, x => x.FromCertificates, new List<Certificate>());
            _toCertificates = GetCertificatesCommand
                .ToProperty(this, x => x.ToCertificates, new List<Certificate>());
        }

        #region Properties
        private Client _client;
        public Client Client
        {
            get { return _client; }
            set { this.RaiseAndSetIfChanged(ref _client, value); }
        }

        private IEnumerable<Certificate> _clientCertificates;
        public IEnumerable<Certificate> ClientCertificates
        {
            get { return _clientCertificates; }
            set { this.RaiseAndSetIfChanged(ref _clientCertificates, value); }
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<Certificate>> _fromCertificates;
        public IEnumerable<Certificate> FromCertificates => _fromCertificates.Value;     

        private Certificate _selectedFromCertificate;
        public Certificate SelectedFromCertificate
        {
            get { return _selectedFromCertificate; }
            set { this.RaiseAndSetIfChanged(ref _selectedFromCertificate, value); }
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<Certificate>> _toCertificates;
        public IEnumerable<Certificate> ToCertificates => _toCertificates.Value;    

        private Certificate _selectedToCertificate;
        public Certificate SelectedToCertificate
        {
            get { return _selectedToCertificate; }
            set { this.RaiseAndSetIfChanged(ref _selectedToCertificate, value); }
        }
        #endregion

        #region Commands

        public ReactiveCommand<Client, Unit> ExportCommand { get; }
        public ReactiveCommand<Client, IEnumerable<Certificate>> GetCertificatesCommand { get; set; }       

        #endregion

        #region Private Functions
        private async Task ExportCertificates(Client client)
        {
            await _exporter.Export(Client, SelectedFromCertificate.Number, SelectedToCertificate.Number);
        }
       
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
