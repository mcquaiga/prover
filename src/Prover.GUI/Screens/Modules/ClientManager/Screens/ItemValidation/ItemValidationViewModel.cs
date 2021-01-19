using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Modules.Clients.VerificationTestActions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Prover.GUI.Screens.Modules.ClientManager.Screens.ItemValidation {
	public class ItemValidationViewModel : ViewModelBase, IHandleInvalidItemVerification {
		public ItemValidationViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
			: base(screenManager, eventAggregator) {
			SkipCommand = ReactiveCommand.Create(() => { TryClose(false); });

			UpdateCommand = ReactiveCommand.Create(() => { TryClose(true); });
		}

		private Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> _invalidItems;

		public Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> InvalidItems
		{
			get => _invalidItems;
			set => this.RaiseAndSetIfChanged(ref _invalidItems, value);
		}

		#region Commands

		private ReactiveCommand _skipCommand;

		public ReactiveCommand SkipCommand
		{
			get => _skipCommand;
			set => this.RaiseAndSetIfChanged(ref _skipCommand, value);
		}

		private ReactiveCommand _updateCommand;

		public ReactiveCommand UpdateCommand
		{
			get => _updateCommand;
			set => this.RaiseAndSetIfChanged(ref _updateCommand, value);
		}

		public bool ShouldInvalidItemsBeChanged(Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> invalidItems) {
			if (invalidItems == null || !invalidItems.Any())
				return false;
			InvalidItems = invalidItems.OrderBy(i => i.Key.Number).ToDictionary(x => x.Key, y => y.Value);

			//show dialog
			bool? result = false;

			Application.Current.Dispatcher.Invoke(delegate {
				result = ScreenManager.ShowDialog(this);
			});

			return result.HasValue && result.Value;
		}

		#endregion
	}
}