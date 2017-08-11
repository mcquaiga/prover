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
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ClientDetailsViewModel) value; }
        }

        public ClientDetailsViewModel ViewModel { get; set; }

     
    }
}