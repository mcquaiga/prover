using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews.FrequencyTest
{
    /// <summary>
    /// Interaction logic for FrequencyTest.xaml
    /// </summary>
    public partial class Card : UserControl, IViewFor<FrequencyTestViewModel>
    {
        public Card()
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
