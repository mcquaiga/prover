using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Modules.Clients.Screens.Clients
{
    /// <summary>
    /// Interaction logic for ClientCsvTemplatesView.xaml
    /// </summary>
    public partial class ClientCsvTemplatesView : UserControl, IViewFor<ClientCsvTemplatesViewModel>
    {
        public ClientCsvTemplatesView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ClientCsvTemplatesViewModel) value; }
        }

        public ClientCsvTemplatesViewModel ViewModel { get; set; }
    }
}
