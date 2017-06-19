using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Exports;
using Prover.Core.Models.Clients;
using Prover.Core.Shared.Enums;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Controls;
using Prover.GUI.Modules.ClientManager.Screens.CsvExporter;
using Prover.GUI.Modules.ClientManager.Screens.CsvTemplates;
using ReactiveUI;

namespace Prover.GUI.Modules.ClientManager.Screens
{
    public class ClientDetailsViewModel : ViewModelBase
    {
        private readonly IProverStore<Core.Models.Clients.Client> _clientStore;

        public ClientDetailsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Core.Models.Clients.Client> clientStore, Core.Models.Clients.Client client = null)
            : base(screenManager, eventAggregator)
        {
            _clientStore = clientStore;
            _client = client;                  

            EditCommand = ReactiveCommand.CreateFromTask(Edit);

            var canSave = this.WhenAnyValue(c => c.Client, c => !string.IsNullOrEmpty(c.Name));
            SaveCommand = ReactiveCommand.CreateFromTask(Save, canSave);

            GoBackCommand = ReactiveCommand.CreateFromTask(GoBack);

            GoToCsvExporter = ReactiveCommand.Create<Core.Models.Clients.Client>(OpenCsvExporter);
            GoToCsvTemplateManager = ReactiveCommand.Create<ClientCsvTemplate>(OpenCsvTemplateEditor);

            InstrumentTypes = new List<InstrumentType>(HoneywellInstrumentTypes.GetAll().ToList());                         

            VerifyItemList = new ItemsFileViewModel(_client, ClientItemType.Verify, "These items will be compared to the instruments values at the beginning of the test");
            this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Where(x => x != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(VerifyItemList.UpdateListItems);

            ResetItemList = new ItemsFileViewModel(_client, ClientItemType.Reset, "These items will be written to the instrument after the volume test is completed.");
            this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Where(x => x != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(ResetItemList.UpdateListItems);

            ClientCsvTemplates.AddRange(Client.CsvTemplates.ToList());
        }

        public async Task Edit()
        {
            await ScreenManager.ChangeScreen(this);
            SelectedInstrumentType = InstrumentTypes.First();
            //SelectedItemFileType = ClientItemType.Reset;
        }

        #region Commands

        private ReactiveCommand<Unit, Unit> _saveCommand;

        public ReactiveCommand<Unit, Unit> SaveCommand
        {
            get { return _saveCommand; }
            set { this.RaiseAndSetIfChanged(ref _saveCommand, value); }
        }

        private ReactiveCommand _editCommand;

        public ReactiveCommand EditCommand
        {
            get { return _editCommand; }
            set { this.RaiseAndSetIfChanged(ref _editCommand, value); }
        }

        private ReactiveCommand _updateItemListCommand;

        public ReactiveCommand UpdateItemListCommand
        {
            get { return _updateItemListCommand; }
            set { this.RaiseAndSetIfChanged(ref _updateItemListCommand, value); }
        }

        private ReactiveCommand _goBackCommand;

        public ReactiveCommand GoBackCommand
        {
            get { return _goBackCommand; }
            set { this.RaiseAndSetIfChanged(ref _goBackCommand, value); }
        }

        private ReactiveCommand _deleteRowCommand;

        public ReactiveCommand DeleteRowCommand
        {
            get { return _deleteRowCommand; }
            set { this.RaiseAndSetIfChanged(ref _deleteRowCommand, value); }
        }

        private ReactiveCommand _addItemCommand;

        public ReactiveCommand AddItemCommand
        {
            get { return _addItemCommand; }
            set { this.RaiseAndSetIfChanged(ref _addItemCommand, value); }
        }

        private ReactiveCommand<Core.Models.Clients.Client, Unit> _goToCsvExporter;

        public ReactiveCommand<Core.Models.Clients.Client, Unit> GoToCsvExporter
        {
            get { return _goToCsvExporter; }
            set { this.RaiseAndSetIfChanged(ref _goToCsvExporter, value); }
        }

        private ReactiveCommand<ClientCsvTemplate, Unit> _goToCsvTemplateManager;

        public ReactiveCommand<ClientCsvTemplate, Unit> GoToCsvTemplateManager
        {
            get { return _goToCsvTemplateManager; }
            set { this.RaiseAndSetIfChanged(ref _goToCsvTemplateManager, value); }
        }
        #endregion

        #region Properties   

        private IEnumerable<InstrumentType> _instrumentTypes;

        public IEnumerable<InstrumentType> InstrumentTypes
        {
            get { return _instrumentTypes; }
            set { this.RaiseAndSetIfChanged(ref _instrumentTypes, value); }
        }

        private InstrumentType _selecedInstrumentType;
        public InstrumentType SelectedInstrumentType
        {
            get { return _selecedInstrumentType; }
            set { this.RaiseAndSetIfChanged(ref _selecedInstrumentType, value); }
        }

        public List<ClientItemType> ItemFileTypesList
            => Enum.GetValues(typeof(ClientItemType)).Cast<ClientItemType>().ToList();

        private Core.Models.Clients.Client _client;
        public Core.Models.Clients.Client Client
        {
            get => _client;
            set => this.RaiseAndSetIfChanged(ref _client, value);
        }

        private ItemsFileViewModel _resetItemList;
        public ItemsFileViewModel ResetItemList
        {
            get => _resetItemList;
            set => this.RaiseAndSetIfChanged(ref _resetItemList, value);
        }

        private ItemsFileViewModel _verifyItemList;
        public ItemsFileViewModel VerifyItemList
        {
            get => _verifyItemList;
            set => this.RaiseAndSetIfChanged(ref _verifyItemList, value);
        }

        private ReactiveList<ClientCsvTemplate> _clientCsvTemplates = new ReactiveList<ClientCsvTemplate>();
        public ReactiveList<ClientCsvTemplate> ClientCsvTemplates
        {
            get { return _clientCsvTemplates; }
            set { this.RaiseAndSetIfChanged(ref _clientCsvTemplates, value); }
        }

        #endregion

        #region Private Functions
        private void OpenCsvTemplateEditor(ClientCsvTemplate clientCsvTemplate = null)
        {
            var csvEditorViewModel = IoC.Get<ClientCsvTemplatesViewModel>();
            if (clientCsvTemplate != null)
                csvEditorViewModel.ClientCsvTemplate = clientCsvTemplate;

            var result = ScreenManager.ShowDialog(csvEditorViewModel);

            if (result.HasValue && result.Value)
            {
                if (clientCsvTemplate == null)
                {
                    clientCsvTemplate = new ClientCsvTemplate(_client)
                    {
                        VerificationType = (VerificationTypeEnum)Enum.Parse(typeof(VerificationTypeEnum), csvEditorViewModel.SelectedVerificationType),
                        InstrumentType = csvEditorViewModel.SelectedInstrumentType,
                        CorrectorType = (EvcCorrectorType)Enum.Parse(typeof(EvcCorrectorType), csvEditorViewModel.SelectedCorrectorType),
                        CsvTemplate = csvEditorViewModel.CsvTemplate
                    };
                    _client.CsvTemplates.Add(clientCsvTemplate);
                }
                else
                {
                    clientCsvTemplate.VerificationType = (VerificationTypeEnum)Enum.Parse(typeof(VerificationTypeEnum),
                        csvEditorViewModel.SelectedVerificationType);
                    clientCsvTemplate.InstrumentType = csvEditorViewModel.SelectedInstrumentType;
                    clientCsvTemplate.CorrectorType = (EvcCorrectorType)Enum.Parse(typeof(EvcCorrectorType),
                        csvEditorViewModel.SelectedCorrectorType);
                    clientCsvTemplate.CsvTemplate = csvEditorViewModel.CsvTemplate;
                }


                using (ClientCsvTemplates.SuppressChangeNotifications())
                {
                    ClientCsvTemplates.Clear();
                    ClientCsvTemplates.AddRange(Client.CsvTemplates.ToList());
                }
            }
        }

        private void OpenCsvExporter(Core.Models.Clients.Client client)
        {
            var exporter = IoC.Get<ExportToCsvViewModel>();
            exporter.Client = _client;

            var result = ScreenManager.ShowDialog(exporter);
            if (result.HasValue && result.Value)
            {

            }
        }

        private async Task GoBack()
        {
            await Save();
            await ScreenManager.ChangeScreen<ClientManagerViewModel>();
        }

        private async Task Save()
        {
            await _clientStore.UpsertAsync(Client);
        }
        #endregion
        protected override void OnDeactivate(bool close)
        {
            Save().ContinueWith(_ =>
            {
                base.OnDeactivate(close);
            });
            
        }
    }
}