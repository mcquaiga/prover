﻿using System.Reactive.Disposables;
using Client.Desktop.Wpf.ViewModels;
using ReactiveUI;

namespace Client.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
 
            this.WhenActivated(disposables =>
            {
                // Bind the view model router to RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                    .DisposeWith(disposables);
           
                this.BindCommand(ViewModel, x => x.GoBack, x => x.GoBackButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, x => x.DialogContent, x => x.DialogHost.DialogContent)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.DialogViewOpen, v => v.DialogHost.IsOpen);

                //this.OneWayBind(ViewModel, x => x.DialogManager.DialogView, x => x.DialogHost.DialogContent)
                //    .DisposeWith(disposables);

                //this.OneWayBind(ViewModel, vm => vm.DialogManager.IsOpen, v => v.DialogHost.IsOpen);

                //this.BindCommand(ViewModel, x => x.ShowTestDialog, x => x.ShowDialogButton)
                //    .DisposeWith(disposables);
                
            });
        }

        private void GoBackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
