using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews.FrequencyTest
{
    /// <summary>
    /// Interaction logic for FrequencyTest.xaml
    /// </summary>
    public partial class CardNew : UserControl, IViewFor<FrequencyTestViewModel>
    {
        public CardNew()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (FrequencyTestViewModel) value; }
        }

        public FrequencyTestViewModel ViewModel { get; set; }
    }
}
