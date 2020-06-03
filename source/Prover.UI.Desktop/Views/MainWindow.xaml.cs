using Prover.Application.Interactions;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData.Binding;
using MaterialDesignThemes.Wpf;

namespace Prover.UI.Desktop.Views {
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	[SingleInstanceView]
	public partial class MainWindow {
		public MainWindow() {
			InitializeComponent();

			this.WhenActivated(d => {
				this.OneWayBind(ViewModel, x => x.AppTitle, x => x.MainWindowView.Title).DisposeWith(d);
				this.OneWayBind(ViewModel, x => x.ScreenManager.Router, x => x.RoutedViewHost.Router).DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.ToolbarManager, v => v.MenuToolbarViewHost.ViewModel).DisposeWith(d);

				this.Bind(ViewModel, vm => vm.MessageQueue, x => x.NotificationSnackBar.MessageQueue).DisposeWith(d);

				this.WhenAnyValue(x => x.ViewModel.NavigateHome)
					.ObserveOn(RxApp.MainThreadScheduler)
					.SelectMany(x => x.Execute())
					.SubscribeOn(RxApp.MainThreadScheduler)
					.Subscribe();

			});
			RegisterNotificationInteractions();
		}

		public SnackbarMessageQueue PersistentMessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromMinutes(5));

		private void RegisterNotificationInteractions() {

			Notifications.SnackBarMessage.RegisterHandler(async message => {
				ViewModel.MessageQueue.Enqueue(message.Input);

				message.SetOutput(Unit.Default);
				await Task.CompletedTask;
			});

			PersistentNotificationSnackBar.MessageQueue = PersistentMessageQueue;

			Notifications.PersistentMessage.RegisterHandler(message => {

				PersistentMessageQueue.Enqueue(message.Input);
				//PersistentNotificationSnackBar.IsActive = true;
				//PersistentNotificationSnackBar.Message.ActionCommand = ReactiveCommand.Create(() => PersistentNotificationSnackBar.IsActive = false);

				message.SetOutput(Unit.Default);
				return Task.CompletedTask;
			});

			Notifications.SnackBarUpdates.RegisterHandler(async context => {
				var oldMessageQueue = ViewModel.MessageQueue;

				ViewModel.MessageQueue = null;

				NotificationSnackBar.IsActive = true;

				var msgUpdates = context.Input
										//.Throttle(TimeSpan.FromMilliseconds(150))
										.ObserveOn(RxApp.MainThreadScheduler)
					   .Subscribe(
							msg => NotificationSnackBar.Message.Content = msg,
							() => {
								NotificationSnackBar.IsActive = false;
								NotificationSnackBar.Message.Content = string.Empty;
								NotificationSnackBar.MessageQueue = oldMessageQueue;
							});



				//Observable.Timer(TimeSpan.FromSeconds(2))
				//          .ObserveOn(RxApp.MainThreadScheduler)
				//          .Subscribe(_ =>
				//          {
				//              NotificationSnackBar.IsActive = false;
				//              NotificationSnackBar.Message.Content = string.Empty;
				//          });

				//var disposer = new SerialDisposable();

				//var oldMessageQueue = ViewModel.MessageQueue;

				//var messageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(500));
				//ViewModel.MessageQueue = messageQueue;
				////NotificationSnackBar.MessageQueue = messageQueue;

				//var msgUpdates = context.Input
				//       .Throttle(TimeSpan.FromMilliseconds(150))
				//       .ObserveOn(RxApp.MainThreadScheduler)
				//       .Subscribe(
				//            msg => messageQueue.Enqueue(msg),
				//            () => disposer.Dispose());

				//disposer.Disposable = Disposable.Create(() =>
				//{
				//    ViewModel.MessageQueue = oldMessageQueue;
				//    messageQueue.Dispose();
				//    msgUpdates.Dispose();

				//});

				//ViewModel.MessageQueue.Enqueue(message.Input);

				context.SetOutput(Unit.Default);
				await Task.CompletedTask;
			});
		}
	}
}