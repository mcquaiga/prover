using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Exports;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.QAProver.Screens;
using ReactiveUI;

namespace Prover.GUI.Modules.Clients.Screens.Clients
{
    public class ClientCsvTemplatesViewModel : ViewModelBase
    {
        public ClientCsvTemplatesViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
            OkCommand = ReactiveCommand.Create(() => { TryClose(true); });
            CancelCommand = ReactiveCommand.Create(() => { TryClose(false); });
        }

        #region Properties

        private InstrumentType _selectedInstrumentType;
        public InstrumentType SelectedInstrumentType
        {
            get { return _selectedInstrumentType; }
            set { this.RaiseAndSetIfChanged(ref _selectedInstrumentType, value); }
        }
        
        public List<InstrumentType> InstrumentTypes => Instruments.GetAll().ToList();

        private string _selectedVerificationType;
        public string SelectedVerificationType
        {
            get { return _selectedVerificationType; }
            set { this.RaiseAndSetIfChanged(ref _selectedVerificationType, value); }
        }

        public List<string> VerificationTypes => Enum.GetNames(typeof(VerificationTypEnum)).ToList();

        public List<string> FieldList => new ExportFields().GetPropertyNames().OrderBy(x => x).ToList();

        private string _csvTemplate;
        public string CsvTemplate
        {
            get { return _csvTemplate; }
            set { this.RaiseAndSetIfChanged(ref _csvTemplate, value); }
        }

        #endregion

        #region Commands
        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        #endregion

        #region Private       
        #endregion
    }
}
