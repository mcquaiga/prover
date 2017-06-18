using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Exports;
using Prover.Core.Models.Clients;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Controls;
using ReactiveUI;


namespace Prover.GUI.Modules.Clients.Screens.Clients
{
    public class ClientViewModel : ViewModelBase
    {
        private readonly IProverStore<Core.Models.Clients.Client> _clientStore;
        public ClientViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Core.Models.Clients.Client> clientStore, Core.Models.Clients.Client client = null)
            : base(screenManager, eventAggregator)
        {
            _clientStore = clientStore;
            _client = client;                  

            EditCommand = ReactiveCommand.CreateFromTask(Edit);

            var canSave = this.WhenAnyValue(c => c.Client, c => !string.IsNullOrEmpty(c.Name));
            SaveCommand = ReactiveCommand.CreateFromTask(Save, canSave);

            GoBackCommand = ReactiveCommand.CreateFromTask(GoBack);

            GoToCsvTemplateManager = ReactiveCommand.CreateFromTask(OpenCsvTemplateEditor);

            InstrumentTypes = new List<InstrumentType>(Instruments.GetAll().ToList());                         

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
        }

        private async Task OpenCsvTemplateEditor()
        {
            var csvEditorViewModel = IoC.Get<ClientCsvTemplatesViewModel>();
            var result = ScreenManager.ShowDialog(csvEditorViewModel);

            if (result.HasValue && result.Value)
            {
                var csv = new ClientCsvTemplate(_client)
                {
                    VerificationType = (VerificationTypEnum) Enum.Parse(typeof(VerificationTypEnum),
                        csvEditorViewModel.SelectedVerificationType),
                    CsvTemplate = csvEditorViewModel.CsvTemplate,
                };

                _client.CsvTemplates.Add(csv);
            }
        }

        public async Task Edit()
        {
            await ScreenManager.ChangeScreen(this);
            SelectedInstrumentType = InstrumentTypes.First();
            //SelectedItemFileType = ClientItemType.Reset;
        }

        private async Task GoBack()
        {
            await ScreenManager.ChangeScreen<ClientManagerViewModel>();
        }

        private async Task Save()
        {
            await _clientStore.UpsertAsync(Client);  
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

        private ReactiveCommand _goToCsvExporter;

        public ReactiveCommand GoToCsvExporter
        {
            get { return _goToCsvExporter; }
            set { this.RaiseAndSetIfChanged(ref _goToCsvExporter, value); }
        }

        private ReactiveCommand _goToCsvTemplateManager;

        public ReactiveCommand GoToCsvTemplateManager
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