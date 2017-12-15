using ReactiveUI;

namespace Prover.GUI.Modules.ClientManager.Screens.CsvExporter
{
    /// <summary>
    /// Interaction logic for ExportToCsvView.xaml
    /// </summary>
    public partial class ExportToCsvView : IViewFor<ExportToCsvViewModel>
    {
        public ExportToCsvView()
        {
            InitializeComponent();

            this.WhenActivated(d => { d(ViewModel = (ExportToCsvViewModel) DataContext); });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ExportToCsvViewModel) value;
        }

        public ExportToCsvViewModel ViewModel { get; set; }
    }
}