using System.Windows;
using System.Windows.Controls;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public partial class VerificationTestView : UserControl
    {
        public VerificationTestView()
        {
            InitializeComponent();
            Loaded += ControlLoaded;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            var my = sender.ToString();
        }
    }
}