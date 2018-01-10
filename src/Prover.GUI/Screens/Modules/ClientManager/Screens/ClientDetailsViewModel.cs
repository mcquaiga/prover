using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Clients;
using Prover.Core.Services;
using Prover.GUI.Controls;
using Prover.GUI.Screens.Modules.ClientManager.Screens.CsvExporter;
using Prover.GUI.Screens.Modules.ClientManager.Screens.CsvTemplates;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.ClientManager.Screens
{
    public class ClientDetailsViewModel : ViewModelBase
    {
        private readonly IClientService _clientService;

        public ClientDetailsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IClientService clientService, Client client = null)
            : base(screenManager, eventAggregator)
        {
            _clientService = clientService;
            _client = client;

            EditCommand = ReactiveCommand.Create(Edit);
            ArchiveCommand = ReactiveCommand.CreateFromTask(ArchiveClient);

            var canSave = this.WhenAnyValue(c => c.Client, c => !string.IsNullOrEmpty(c.Name));
            SaveCommand = ReactiveCommand.CreateFromTask(Save, canSave);

            GoBackCommand = ReactiveCommand.CreateFromTask(GoBack);

            GoToCsvExporter = ReactiveCommand.Create<Client>(OpenCsvExporter);

            GoToCsvTemplateManager = ReactiveCommand.Create<ClientCsvTemplate>(OpenCsvTemplateEditor);

            SwitchToDetailsContextCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Client != null)
                    Client = await _clientService.GetById(Client.Id);

                IsDirty = true;
                InstrumentTypes = new ReactiveList<InstrumentType>(HoneywellInstrumentTypes.GetAll());

                var instrumentSelected = this.WhenAnyValue(x => x.SelectedInstrumentType).Where(x => x != null);

                VerifyItemList = new ItemsFileViewModel(_client, ClientItemType.Verify, instrumentSelected,
                    "These items will be compared to the instruments values at the beginning of the test");

                ResetItemList = new ItemsFileViewModel(_client, ClientItemType.Reset, instrumentSelected,
                    "These items will be written to the instrument after the volume test is completed.");

                ClientCsvTemplates = new ReactiveList<ClientCsvTemplate>(Client.CsvTemplates);
                DeleteCsvTemplateCommand = ReactiveCommand.CreateFromTask<ClientCsvTemplate>(async csv =>
                {
                    if (await _clientService.DeleteCsvTemplate(csv))
                        ClientCsvTemplates.Remove(csv);
                });

                DeleteCsvTemplateCommand.ThrownExceptions
                    .Subscribe(ex => Log.Error(ex));
            });
        }

        #region Commands

        public ReactiveCommand<ClientCsvTemplate, Unit> DeleteCsvTemplateCommand { get; set; }
        public ReactiveCommand ArchiveCommand { get; set; }
        private ReactiveCommand<Unit, Unit> _saveCommand;

        public ReactiveCommand<Unit, Unit> SaveCommand
        {
            get => _saveCommand;
            set => this.RaiseAndSetIfChanged(ref _saveCommand, value);
        }

        private ReactiveCommand _editCommand;

        public ReactiveCommand EditCommand
        {
            get => _editCommand;
            set => this.RaiseAndSetIfChanged(ref _editCommand, value);
        }

        private ReactiveCommand _updateItemListCommand;

        public ReactiveCommand UpdateItemListCommand
        {
            get => _updateItemListCommand;
            set => this.RaiseAndSetIfChanged(ref _updateItemListCommand, value);
        }

        private ReactiveCommand _goBackCommand;

        public ReactiveCommand GoBackCommand
        {
            get => _goBackCommand;
            set => this.RaiseAndSetIfChanged(ref _goBackCommand, value);
        }

        private ReactiveCommand _deleteRowCommand;

        public ReactiveCommand DeleteRowCommand
        {
            get => _deleteRowCommand;
            set => this.RaiseAndSetIfChanged(ref _deleteRowCommand, value);
        }

        private ReactiveCommand _addItemCommand;

        public ReactiveCommand AddItemCommand
        {
            get => _addItemCommand;
            set => this.RaiseAndSetIfChanged(ref _addItemCommand, value);
        }

        private ReactiveCommand<Client, Unit> _goToCsvExporter;

        public ReactiveCommand<Client, Unit> GoToCsvExporter
        {
            get => _goToCsvExporter;
            set => this.RaiseAndSetIfChanged(ref _goToCsvExporter, value);
        }

        private ReactiveCommand<ClientCsvTemplate, Unit> _goToCsvTemplateManager;

        public ReactiveCommand<ClientCsvTemplate, Unit> GoToCsvTemplateManager
        {
            get => _goToCsvTemplateManager;
            set => this.RaiseAndSetIfChanged(ref _goToCsvTemplateManager, value);
        }

        private ReactiveCommand<Unit, Unit> _switchToDetailsContextCommand;

        public ReactiveCommand<Unit, Unit> SwitchToDetailsContextCommand
        {
            get => _switchToDetailsContextCommand;
            set => this.RaiseAndSetIfChanged(ref _switchToDetailsContextCommand, value);
        }

        #endregion

        #region Properties   

        private bool _isRemoved;

        public bool IsRemoved
        {
            get => _isRemoved;
            set => this.RaiseAndSetIfChanged(ref _isRemoved, value);
        }

        private ReactiveList<InstrumentType> _instrumentTypes;

        public ReactiveList<InstrumentType> InstrumentTypes
        {
            get => _instrumentTypes;
            set => this.RaiseAndSetIfChanged(ref _instrumentTypes, value);
        }

        private InstrumentType _selecedInstrumentType;

        public InstrumentType SelectedInstrumentType
        {
            get => _selecedInstrumentType;
            set => this.RaiseAndSetIfChanged(ref _selecedInstrumentType, value);
        }

        public List<ClientItemType> ItemFileTypesList
            => Enum.GetValues(typeof(ClientItemType)).Cast<ClientItemType>().ToList();

        private Client _client;

        public Client Client
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

        private ReactiveList<ClientCsvTemplate> _clientCsvTemplates =
            new ReactiveList<ClientCsvTemplate> {ChangeTrackingEnabled = true};

        public ReactiveList<ClientCsvTemplate> ClientCsvTemplates
        {
            get => _clientCsvTemplates;
            set => this.RaiseAndSetIfChanged(ref _clientCsvTemplates, value);
        }

        #endregion

        #region Public Functions

        public void Edit()
        {
            ScreenManager.ChangeScreen(this);
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
                if (csvEditorViewModel.ClientCsvTemplate.Client == null)
                {
                    csvEditorViewModel.ClientCsvTemplate.Client = _client;
                    _client.CsvTemplates.Add(csvEditorViewModel.ClientCsvTemplate);
                }

                using (ClientCsvTemplates.SuppressChangeNotifications())
                {
                    ClientCsvTemplates.Clear();
                    ClientCsvTemplates.AddRange(Client.CsvTemplates.ToList());
                }
            }
        }

        private async Task ArchiveClient()
        {
            await _clientService.ArchiveClient(Client);
            IsRemoved = true;
        }

        private void OpenCsvExporter(Client client)
        {
            var exporter = IoC.Get<ExportToCsvViewModel>();
            exporter.Client = _client;

            ScreenManager.ShowDialog(exporter);
        }

        private async Task GoBack()
        {
            await Save();
            ScreenManager.ChangeScreen<ClientManagerViewModel>();
        }

        private async Task Save()
        {
            await _clientService.Save(Client);
            IsDirty = false;
        }

        public bool IsDirty { get; set; }

        #endregion

        public override void CanClose(Action<bool> callback)
        {
            if (IsDirty)
            {
                MessageBoxResult result;
                result = MessageBox.Show($"Save changes?", "Save", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    Save().ConfigureAwait(false);

                if (result == MessageBoxResult.Cancel)
                {
                    callback(false);
                    return;
                }
            }
            callback(true);
        }

        protected override void OnDeactivate(bool close)
        {
            Save().ContinueWith(_ => { base.OnDeactivate(close); });
        }
    }
}