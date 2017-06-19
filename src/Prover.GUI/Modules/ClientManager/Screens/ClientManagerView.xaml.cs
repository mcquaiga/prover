using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Modules.ClientManager.Screens
{
    /// <summary>
    ///     Interaction logic for ClientManager.xaml
    /// </summary>
    public partial class ClientManagerView : UserControl, IViewFor<ClientManagerViewModel>
    {
        public ClientManagerView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ClientManagerViewModel) value; }
        }

        public ClientManagerViewModel ViewModel { get; set; }
    }
}