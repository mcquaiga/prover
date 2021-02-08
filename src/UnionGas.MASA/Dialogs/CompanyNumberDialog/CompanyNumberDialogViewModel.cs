﻿using Caliburn.Micro;
using Prover.GUI.Screens;

namespace UnionGas.MASA.Dialogs.CompanyNumberDialog {

	public class CompanyNumberDialogViewModel : ViewModelBase {

		#region Constructors

		public CompanyNumberDialogViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
			: base(screenManager, eventAggregator) {
			CompanyNumber = string.Empty;
		}

		#endregion

		#region Properties

		public string CompanyNumber { get; set; }

		#endregion

		#region Methods

		public void Cancel() {
			TryClose(false);
		}

		public void Close() {
			TryClose(true);
		}

		#endregion
	}
}