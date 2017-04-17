using System.Windows.Controls;
using ProtocolMonitor.ViewModel;
using ReactiveUI;

namespace ProtocolMonitor.View
{
    /// <summary>
    ///     Interaction logic for WriteDataView.xaml
    /// </summary>
    public partial class CommWindowView : UserControl, IViewFor<SerialCommViewModel>
    {
        public CommWindowView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SerialCommViewModel) value; }
        }

        public SerialCommViewModel ViewModel { get; set; }
    }
}