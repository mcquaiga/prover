using Prover.Application.Interfaces;
using ReactiveUI;
using System;
using System.Reactive;

namespace Prover.Application.Interactions {
	public static class Messages {
		public static Interaction<IDialogViewModel, Unit> ShowDialog { get; } = new Interaction<IDialogViewModel, Unit>();

		public static Interaction<string, Unit> ShowMessage { get; } = new Interaction<string, Unit>();

		public static Interaction<string, Unit> ShowError { get; } = new Interaction<string, Unit>();

		public static Interaction<string, bool> ShowYesNo { get; } = new Interaction<string, bool>();

		public static Interaction<string, bool> ShowQuestion { get; } = new Interaction<string, bool>();

		public static Interaction<string, string> GetInputString { get; } = new Interaction<string, string>();

		public static Interaction<string, object> GetInput { get; } = new Interaction<string, object>();

		public static Interaction<string, decimal> GetInputNumber { get; } = new Interaction<string, decimal>();

		public static Interaction<string, int> GetInputInteger { get; } = new Interaction<string, int>();

		public static Interaction<string, string> OpenFileDialog { get; } = new Interaction<string, string>();
	}

	public static class Exceptions {
		public static Interaction<string, Unit> Error { get; } = Messages.ShowError;
		public static Interaction<string, Unit> Warning { get; } = Messages.ShowError;
		public static Interaction<string, Unit> Critical { get; } = Messages.ShowError;
	}

	public static class Notifications {

		public static Interaction<string, Unit> SnackBarMessage { get; } = new Interaction<string, Unit>();
		public static Interaction<IObservable<string>, Unit> SnackBarUpdates { get; } = new Interaction<IObservable<string>, Unit>();

		public static Interaction<string, Unit> ActionMessage { get; } = new Interaction<string, Unit>();
		public static Interaction<string, Unit> PersistentMessage { get; } = new Interaction<string, Unit>();
	}
}