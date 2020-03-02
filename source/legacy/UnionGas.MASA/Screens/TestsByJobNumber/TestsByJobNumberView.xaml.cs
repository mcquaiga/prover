using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace UnionGas.MASA.Screens.TestsByJobNumber
{
    /// <summary>
    /// Interaction logic for TestsByJobNumberView.xaml
    /// </summary>
    public partial class TestsByJobNumberView : UserControl, IViewFor<TestsByJobNumberViewModel>
    {
        public TestsByJobNumberView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                ViewModel = (TestsByJobNumberViewModel)DataContext;

                d(this.WhenAnyValue(x => x.ViewModel.GetOpenJobNumbersCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());                     
            });
        }

       object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TestsByJobNumberViewModel)value; }
        }

        public TestsByJobNumberViewModel ViewModel { get; set; }
    }
}
