using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using MaterialDesignThemes.Wpf;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace Prover.UI.Desktop.Dialogs {
	public class DialogViewModel : ViewModelBase, IDialogViewModel, IDisposable, IValidatableViewModel {
		protected readonly CancellationTokenSource CancellationTokenSource;

		public DialogViewModel(CancellationTokenSource cancellationTokenSource = null) {
			CancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
			Cancelled = CancellationTokenSource.Token;

			//ShowCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(true)).DisposeWith(Cleanup);
			CloseCommand = ReactiveCommand.CreateFromObservable(() => {
				Result = DialogResult.Accepted;
				return Observable.Return(Unit.Default);
			}, this.IsValid());

			CancelCommand = ReactiveCommand.CreateFromObservable(() => {
				Result = DialogResult.Cancelled;
				CancellationTokenSource.Cancel();
				return Observable.Return(Unit.Default);
			});

			CloseCommand.Merge(CancelCommand)
						.InvokeCommand(DialogHost.CloseDialogCommand)
						.DisposeWith(Cleanup);
		}

		public DialogViewModel() : this(new CancellationTokenSource()) {
		}

		public CancellationToken Cancelled { get; protected set; }

		public IObservable<Unit> Closed { get; set; }

		[Reactive] public string Title { get; set; }
		[Reactive] public string Message { get; set; }

		public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
		public DialogResult Result { get; protected set; } = DialogResult.Undetermined;
		public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }

		public ValidationContext ValidationContext { get; } = new ValidationContext();

		/// <inheritdoc />
		protected override void Dispose(bool isDisposing) {
			CloseCommand?.Dispose();
			CancelCommand?.Dispose();
			CancellationTokenSource?.Dispose();
			ValidationContext?.Dispose();
		}

	}
}