using Prover.Application.Interactions;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.UI.Desktop.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [SingleInstanceView]
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, x => x.AppTitle, x => x.MainWindowView.Title).DisposeWith(d);
                this.OneWayBind(ViewModel, x => x.ScreenManager.Router, x => x.RoutedViewHost.Router).DisposeWith(d);
                this.OneWayBind(ViewModel, x => x.ToolbarItems, x => x.ToolbarItemsControl.ItemsSource,
                            value => value.OrderBy(x => x.SortOrder)).DisposeWith(d);

                this.OneWayBind(ViewModel, x => x.ScreenManager.ToolbarItems, x => x.ToolbarActionItemsControl.ItemsSource).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.MessageQueue, x => x.NotificationSnackBar.MessageQueue).DisposeWith(d);

                this.BindCommand(ViewModel, x => x.NavigateBack, x => x.GoBackButton).DisposeWith(d);
                this.BindCommand(ViewModel, x => x.NavigateHome, x => x.GoHomeButton).DisposeWith(d);


                //this.BindCommand(ViewModel, x => x., x => x.GoHomeButton).DisposeWith(d);
                //this.BindCommand(ViewModel, x => x.OpenDialog, x => x.ShowDialogButton).DisposeWith(d);

            });
            RegisterNotificationInteractions();

        }

        private void RegisterNotificationInteractions()
        {
            Notifications.SnackBarMessage.RegisterHandler(async message =>
            {
                ViewModel.MessageQueue.Enqueue(message.Input);

                message.SetOutput(Unit.Default);
                await Task.CompletedTask;
            });

            Notifications.SnackBarUpdates.RegisterHandler(async context =>
            {
                var oldMessageQueue = ViewModel.MessageQueue;

                ViewModel.MessageQueue = null;

                NotificationSnackBar.IsActive = true;

                var msgUpdates = context.Input
                                        //.Throttle(TimeSpan.FromMilliseconds(150))
                                        .ObserveOn(RxApp.MainThreadScheduler)
                       .Subscribe(
                            msg => NotificationSnackBar.Message.Content = msg,
                            () =>
                            {
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