using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views.TestsByJobNumber
{
    /// <summary>
    /// Interaction logic for TestsByJobNumberView.xaml
    /// </summary>
    public partial class TestsByJobNumberView
    {
        public TestsByJobNumberView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                //this.OneWayBind(ViewModel, vm => vm.JobNumbers, v => v.JobNumbersComboBox.ItemsSource).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.GetOpenJobNumbersCommand, v => v.SearchButton).DisposeWith(d);
                this.WhenAnyValue(x => x.ViewModel.GetOpenJobNumbersCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe()
                    .DisposeWith(d);                     
            });
        }

    }
}
