using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews.VerificationSet
{
    /// <summary>
    ///     Interaction logic for PTVerificationSet.xaml
    /// </summary>
    public partial class CardNew : IViewFor<VerificationSetViewModel>
    {
        public CardNew()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(ViewModel = (VerificationSetViewModel)DataContext);

                d(this.BindCommand(ViewModel, vm => vm.RunTestCommand, v => v.RunTestButton));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (VerificationSetViewModel) value; }
        }

        public VerificationSetViewModel ViewModel { get; set; }
    }
}