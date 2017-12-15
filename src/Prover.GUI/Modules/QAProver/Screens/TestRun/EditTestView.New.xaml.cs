using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.TestRun
{
    /// <summary>
    ///     Interaction logic for QATestRunView.xaml
    /// </summary>
    public partial class EditTestViewNew : UserControl, IViewFor<TestRunViewModel>
    {
        public EditTestViewNew()
        {
            InitializeComponent();

            this.WhenActivated(d => { d(ViewModel = (TestRunViewModel) DataContext); });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestRunViewModel) value;
        }

        public TestRunViewModel ViewModel { get; set; }
    }
}