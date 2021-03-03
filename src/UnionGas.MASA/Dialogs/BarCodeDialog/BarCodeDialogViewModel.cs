using Caliburn.Micro;
using Prover.GUI.Screens;

namespace UnionGas.MASA.Dialogs.BarCodeDialog {

	public class BarCodeDialogViewModel : ViewModelBase {

		#region Constructors

		public BarCodeDialogViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
			: base(screenManager, eventAggregator) {
			BarCodeNumber = string.Empty;
		}

		#endregion

		#region Properties

		public string BarCodeNumber { get; set; }

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