using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Interop;
using Client.Desktop.Wpf.Interactions;
using Client.Desktop.Wpf.ViewModels;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using Client.Desktop.Wpf.Views.Dialogs;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [SingleInstanceView]
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                // Bind the view model router to RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router).DisposeWith(d);
                this.OneWayBind(ViewModel, x => x.DialogManager.DialogContent, x => x.DialogHost.DialogContent).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DialogManager.DialogContent, v => v.DialogHost.IsOpen, dialog => dialog != null).DisposeWith(d);

                this.BindCommand(ViewModel, x => x.NavigateBack, x => x.GoBackButton).DisposeWith(d);

                this.BindCommand(ViewModel, x => x.NavigateHome, x => x.GoHomeButton).DisposeWith(d);

                this.BindCommand(ViewModel, x => x.ShowTestDialog, x => x.ShowDialogButton).DisposeWith(d);
            });

            MessageInteractions.ShowMessage.RegisterHandler(async i =>
                {
                    await ViewModel.DialogManager.ShowMessage(i.Input, "Message");
                    i.SetOutput(Unit.Default);
                });

            MessageInteractions.ShowYesNo.RegisterHandler(async i =>
            {
                var answer = false;

                var view = new TextDialogView {MessageText = {Text = i.Input}, TitleText = {Text = "Adam"}};
                var model = new TextDialogViewModel(i.Input, "Adam");
                view.ViewModel = model;

                DialogHost.DialogContent = view;
                DialogHost.IsOpen = true;

                void callback(object sender, RoutedEventArgs args)
                {
                    answer = view.Answer ?? false;
                    DialogHost.DialogContent = null;
                    DialogHost.IsOpen = false;
                };
                view.OkButton.Click += callback;
                view.CancelButton.Click += callback;
                
                i.SetOutput(answer);
            });

            NotificationInteractions.SnackBarMessage.RegisterHandler(async message =>
                {
                    NotificationSnackBar.Message.Content = message.Input;
                    NotificationSnackBar.IsActive = true;

                    await Task.Delay(TimeSpan.FromSeconds(3));

                    NotificationSnackBar.IsActive = false;
                    NotificationSnackBar.Message.Content = string.Empty;

                    message.SetOutput(Unit.Default);
                });
        }
    }
}