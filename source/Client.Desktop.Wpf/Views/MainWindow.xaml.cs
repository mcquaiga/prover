using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Prover.Application.Interactions;

namespace Client.Desktop.Wpf.Views
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
                this.OneWayBind(ViewModel, x => x.ScreenManager.DialogManager.DialogContent, x => x.DialogHost.DialogContent).DisposeWith(d);
                this.OneWayBind(ViewModel, x => x.ScreenManager.DialogManager.DialogContent, v => v.DialogHost.IsOpen, dialog => dialog != null).DisposeWith(d);

                this.OneWayBind(ViewModel, x => x.ToolbarItems, x => x.ToolbarItemsControl.ItemsSource).DisposeWith(d);

                this.BindCommand(ViewModel, x => x.NavigateBack, x => x.GoBackButton).DisposeWith(d);
                this.BindCommand(ViewModel, x => x.NavigateHome, x => x.GoHomeButton).DisposeWith(d);

            });

            RegisterNotificationInteractions();
        }

        private void RegisterNotificationInteractions()
        {
            NotificationInteractions.SnackBarMessage.RegisterHandler(async message =>
            {
                NotificationSnackBar.Message.Content = message.Input;
                NotificationSnackBar.IsActive = true;



                Observable.Timer(TimeSpan.FromSeconds(2))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        NotificationSnackBar.IsActive = false;
                        NotificationSnackBar.Message.Content = string.Empty;
                    });

                message.SetOutput(Unit.Default);
            });
        }
    }
}