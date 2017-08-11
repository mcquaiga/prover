using ReactiveUI;

namespace Prover.GUI.Modules.ClientManager.Screens.CsvTemplates
{
    /// <summary>
    /// Interaction logic for ClientCsvTemplatesView.xaml
    /// </summary>
    public partial class ClientCsvTemplatesView : IViewFor<ClientCsvTemplatesViewModel>
    {
        public ClientCsvTemplatesView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(ViewModel = (ClientCsvTemplatesViewModel) DataContext);                
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ClientCsvTemplatesViewModel) value; }
        }

        public ClientCsvTemplatesViewModel ViewModel { get; set; }
    }
}
