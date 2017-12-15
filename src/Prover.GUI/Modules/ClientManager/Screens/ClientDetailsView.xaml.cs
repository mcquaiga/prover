using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Modules.ClientManager.Screens
{
    /// <summary>
    ///     Interaction logic for Client.xaml
    /// </summary>
    public partial class ClientDetailsView : UserControl, IViewFor<ClientDetailsViewModel>
    {
        public ClientDetailsView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(ViewModel = (ClientDetailsViewModel) DataContext);

                d(this.WhenAnyValue(x => x.ViewModel.SwitchToDetailsContextCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ClientDetailsViewModel) value;
        }

        public ClientDetailsViewModel ViewModel { get; set; }
    }
}