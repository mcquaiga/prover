using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Modules.Clients.VerificationTestActions;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.GUI.Modules.Clients.Screens.ItemValidation
{
    public class ItemValidationViewModel : ViewModelBase, IHandleInvalidItemVerification
    {
        public ItemValidationViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            SkipCommand = ReactiveCommand.Create(() => { TryClose(false); });

            UpdateCommand = ReactiveCommand.Create(() => { TryClose(true); });
        }

        public Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> InvalidItems { get; set; }

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

        public bool ShouldInvalidItemsBeChanged(Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> invalidItems)
        {
            if (invalidItems == null || !invalidItems.Any()) return false;

            //show dialog
            var result = ScreenManager.ShowDialog(this);
            return result.HasValue && result.Value;
        }

        #endregion
    }
}