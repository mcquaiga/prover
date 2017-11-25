using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews.FrequencyTest
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
