using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace UnionGas.MASA.Screens.Exporter
{
    /// <summary>
    ///     Interaction logic for CreateCertificateView.xaml
    /// </summary>
    public partial class ExportTestsView : IViewFor<ExportTestsViewModel>
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
            set { ViewModel = (ExportTestsViewModel)value; }
        }

        public ExportTestsViewModel ViewModel { get; set; }
    }
}