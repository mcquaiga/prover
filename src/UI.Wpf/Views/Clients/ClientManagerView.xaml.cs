﻿using System;
using System.Windows.Controls;
using Client.Wpf.ViewModels.Clients;
using ReactiveUI;

namespace Client.Wpf.Views.Clients
{
    /// <summary>
    ///     Interaction logic for ClientManager.xaml
    /// </summary>
    public partial class ClientManagerView : ReactiveUserControl<ClientManagerViewModel>
    {
        public ClientManagerView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                //ViewModel = (ClientManagerViewModel) DataContext;
                //d(this.WhenAnyValue(x => x.ViewModel.LoadClientsCommand)
                //    .SelectMany(x => x.Execute())
                //    .Subscribe());
            });
        }

    
    }
}