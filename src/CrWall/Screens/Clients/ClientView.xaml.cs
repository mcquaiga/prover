using System.Windows.Controls;
using ReactiveUI;

namespace CrWall.Screens.Clients
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class ClientView : UserControl, IViewFor<ClientViewModel>
    {
        public ClientView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ClientViewModel) value; }
        }

        public ClientViewModel ViewModel { get; set; }
    }
}
