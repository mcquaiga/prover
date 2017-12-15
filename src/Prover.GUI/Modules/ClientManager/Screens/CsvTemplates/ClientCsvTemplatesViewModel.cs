using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Exports;
using Prover.Core.Extensions;
using Prover.Core.Models.Clients;
using Prover.Core.Shared.Enums;
using Prover.GUI.Screens;
using ReactiveUI;

namespace Prover.GUI.Modules.ClientManager.Screens.CsvTemplates
{
    public class ClientCsvTemplatesViewModel : ViewModelBase, IDisposable, IWindowSettings
    {
        public ClientCsvTemplatesViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(
            screenManager, eventAggregator)
        {
            ClientCsvTemplate = new ClientCsvTemplate();

            var canSave = this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Select(x => x != null && !string.IsNullOrEmpty(x.Name));
            OkCommand = ReactiveCommand.Create(() =>
            {
                ClientCsvTemplate.CsvTemplate = CsvTemplate;
                ClientCsvTemplate.InstrumentType = SelectedInstrumentType;

                ClientCsvTemplate.VerificationType = string.IsNullOrEmpty(SelectedVerificationType)
                    ? null
                    : (VerificationTypeEnum?) Enum.Parse(typeof(VerificationTypeEnum),
                        SelectedVerificationType);

                ClientCsvTemplate.CorrectorType = string.IsNullOrEmpty(SelectedCorrectorType)
                    ? null
                    : (EvcCorrectorType?) Enum.Parse(typeof(EvcCorrectorType), SelectedCorrectorType);

                ClientCsvTemplate.DriveType = string.IsNullOrEmpty(SelectedDriveType)
                    ? null
                    : (DriveTypeDescripter?) Enum.Parse(typeof(DriveTypeDescripter), SelectedDriveType);

                TryClose(true);
            }, canSave);
            CancelCommand = ReactiveCommand.Create(() => { TryClose(false); });

            AddFieldToTemplateCommand = ReactiveCommand.Create<string>(s =>
            {
                CsvTemplate = string.IsNullOrEmpty(CsvTemplate)
                    ? $"[{s}]"
                    : $"{CsvTemplate},[{s}]";
            });

            this.WhenAnyValue(x => x.ClientCsvTemplate)
                .Where(c => c != null)
                .Subscribe(y =>
                {
                    SelectedInstrumentType = y.InstrumentType;
                    SelectedCorrectorType = y.CorrectorTypeString;
                    SelectedVerificationType = y.VerificationTypeString;
                    SelectedDriveType = y.DriveTypeString;
                    CsvTemplate = y.CsvTemplate;
                });
        }

        #region Properties

        private ClientCsvTemplate _clientCsvTemplate;

        public ClientCsvTemplate ClientCsvTemplate
        {
            get => _clientCsvTemplate;
            set => this.RaiseAndSetIfChanged(ref _clientCsvTemplate, value);
        }

        // Instrument Types      

        public List<InstrumentType> InstrumentTypes => HoneywellInstrumentTypes.GetAll()
            .OrderBy(s => s.Name)
            .ToList();

        private InstrumentType _selectedInstrumentType;

        public InstrumentType SelectedInstrumentType
        {
            get => _selectedInstrumentType;
            set => this.RaiseAndSetIfChanged(ref _selectedInstrumentType, value);
        }

        // Verification Types
        public List<string> VerificationTypes => Enum.GetNames(typeof(VerificationTypeEnum))
            .AddEmptyItem(() => string.Empty)
            .OrderBy(s => s)
            .ToList();

        private string _selectedVerificationType;

        public string SelectedVerificationType
        {
            get => _selectedVerificationType;
            set => this.RaiseAndSetIfChanged(ref _selectedVerificationType, value);
        }

        public List<string> CorrectorTypes => Enum.GetNames(typeof(EvcCorrectorType))
            .AddEmptyItem(() => string.Empty)
            .OrderBy(s => s)
            .ToList();

        private string _selectedCorrectorType;

        public string SelectedCorrectorType
        {
            get => _selectedCorrectorType;
            set => this.RaiseAndSetIfChanged(ref _selectedCorrectorType, value);
        }

        public List<string> DriveTypes => Enum.GetNames(typeof(DriveTypeDescripter))
            .AddEmptyItem(() => string.Empty)
            .OrderBy(s => s)
            .ToList();

        private string _selectedDriveType;

        public string SelectedDriveType
        {
            get => _selectedDriveType;
            set => this.RaiseAndSetIfChanged(ref _selectedDriveType, value);
        }

        public List<string> FieldList
            => new ExportFields()
                .GetPropertyDescriptions()
                .Select(x => x)
                .OrderBy(x => x).ToList();

        private string _csvTemplate;

        public string CsvTemplate
        {
            get => _csvTemplate;
            set => this.RaiseAndSetIfChanged(ref _csvTemplate, value);
        }

        #endregion

        #region Commands

        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand CancelCommand { get; }
        public ReactiveCommand<string, Unit> AddFieldToTemplateCommand { get; }

        #endregion

        #region Private       

        #endregion

        public override void Dispose()
        {
            OkCommand?.Dispose();
            CancelCommand?.Dispose();
        }

        public dynamic WindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.CanResizeWithGrip;
                settings.Width = 1000;
                settings.SizeToContent = SizeToContent.Manual;
                settings.Title = "CSV Template Editor";
                return settings;
            }
        }
    }
}