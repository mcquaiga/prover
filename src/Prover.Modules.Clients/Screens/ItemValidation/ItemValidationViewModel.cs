using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.Modules.Clients.Validators;
using ReactiveUI;

namespace Prover.Modules.Clients.Screens.ItemValidation
{
    public class ItemValidationViewModel : ViewModelBase
    {
        public ItemVerificationManager ItemVerificationManager { get; set; }

        public ItemValidationViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
            SkipCommand = ReactiveCommand.Create(() =>
            {
                TryClose(false);
            });

            UpdateCommand = ReactiveCommand.Create(() =>
            {
                TryClose(true);
            });
        }

        public Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> InvalidItems => ItemVerificationManager.InvalidInstrumentValues;

        #region Commands
        private ReactiveCommand _skipCommand;
        public ReactiveCommand SkipCommand
        {
            get { return _skipCommand; }
            set { this.RaiseAndSetIfChanged(ref _skipCommand, value); }
        }

        private ReactiveCommand _updateCommand;
        public ReactiveCommand UpdateCommand
        {
            get { return _updateCommand; }
            set { this.RaiseAndSetIfChanged(ref _updateCommand, value); }
        }

        #endregion
    }
}
