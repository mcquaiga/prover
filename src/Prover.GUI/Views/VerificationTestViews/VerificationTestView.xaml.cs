using System.Windows;
using System.Windows.Controls;

namespace Prover.GUI.Views.VerificationTestViews
{
    public partial class VerificationTestView : UserControl
    {
        public VerificationTestView()
        {
            InitializeComponent();
            Loaded += ControlLoaded;
        }

        void ControlLoaded(object sender, RoutedEventArgs e)
        {
            var my = sender.ToString();
        }
    }
}
