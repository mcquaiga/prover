using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    ///     Interaction logic for InstrumentView.xaml
    /// </summary>
    public partial class VerificationGridView
    {
        public VerificationGridView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                TestDateTimeTextBlock.Text = $"{ViewModel.Test.TestDateTime:g}";

            });
        }

    }
}