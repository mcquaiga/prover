using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
