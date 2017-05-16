using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Modules.Clients.Screens.Clients
{
    /// <summary>
    ///     Interaction logic for Client.xaml
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}