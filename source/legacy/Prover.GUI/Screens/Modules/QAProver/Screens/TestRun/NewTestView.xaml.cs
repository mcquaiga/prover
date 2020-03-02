using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.TestRun
{
    /// <summary>
    ///     Interaction logic for InstrumentConnectView.xaml
    /// </summary>
    public partial class NewTestView : IViewFor<TestRunViewModel>
    {
        public NewTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                ViewModel = (TestRunViewModel)DataContext;

                d(this.WhenAnyValue(x => x.ViewModel.RefreshCommPortsCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());
            });

            
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestRunViewModel) value;
        }

        public TestRunViewModel ViewModel { get; set; }
    }
}