using System.Reactive;
using System.Reactive.Disposables;
using Prover.Application.Interactions;
using ReactiveUI;

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
                this.OneWayBind(ViewModel, x => x.ToolbarItems, x => x.ToolbarItemsControl.ItemsSource).DisposeWith(d);
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
            NotificationInteractions.SnackBarMessage.RegisterHandler(message =>
            {
                ViewModel.MessageQueue.Enqueue(message.Input);

                //NotificationSnackBar.Message.Content = message.Input;
                //NotificationSnackBar.IsActive = true;

                //Observable.Timer(TimeSpan.FromSeconds(2))
                //          .ObserveOn(RxApp.MainThreadScheduler)
                //          .Subscribe(_ =>
                //          {
                //              NotificationSnackBar.IsActive = false;
                //              NotificationSnackBar.Message.Content = string.Empty;
                //          });

                message.SetOutput(Unit.Default);
            });
        }
    }
}