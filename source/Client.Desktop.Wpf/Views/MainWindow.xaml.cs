﻿using ReactiveUI;
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
                this.OneWayBind(ViewModel, vm => vm.ScreenManager.DialogManager.DialogContent, v => v.DialogHost.IsOpen, dialog => dialog != null).DisposeWith(d);

                this.BindCommand(ViewModel, x => x.NavigateBack, x => x.GoBackButton).DisposeWith(d);
                this.BindCommand(ViewModel, x => x.NavigateHome, x => x.GoHomeButton).DisposeWith(d);
            });

            RegisterMessageInteractions();
        }
        
        private void RegisterMessageInteractions()
        {
            MessageInteractions.ShowMessage.RegisterHandler(async i =>
            {
                await ViewModel.ScreenManager.DialogManager.ShowMessage(i.Input, "Message");
                i.SetOutput(Unit.Default);
            });

            MessageInteractions.ShowYesNo.RegisterHandler(async i =>
            {
                var answer = await ViewModel.ScreenManager.DialogManager.ShowQuestion(i.Input);
                //var view = new TextDialogView {MessageText = {Text = i.Input}, TitleText = {Text = "Adam"}};
                
                //DialogHost.DialogContent = view;
                //DialogHost.IsOpen = true;

                //void callback(object sender, RoutedEventArgs args)
                //{
                //    answer = view.Answer ?? false;
                //    DialogHost.DialogContent = null;
                //    DialogHost.IsOpen = false;
                //};
                //view.OkButton.Click += callback;


                i.SetOutput(answer);
            });

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