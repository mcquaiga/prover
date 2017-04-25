using System.Reactive.Linq;
using System.Windows.Controls;
using ReactiveUI;
using System;

namespace UnionGas.MASA.Screens.Exporter
{
    /// <summary>
    ///     Interaction logic for CreateCertificateView.xaml
    /// </summary>
    public partial class ExportTestsView : UserControl, IViewFor<ExportTestsViewModel>
    {
        public ExportTestsView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                ViewModel = (ExportTestsViewModel)DataContext;
                d(this.WhenAnyValue(x => x.ViewModel.ExecuteTestSearch)
                    .SelectMany(x => x.Execute())
                    .Subscribe());
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ExportTestsViewModel) value; }
        }

        public ExportTestsViewModel ViewModel { get; set; }
    }
}