using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Wpf.ViewModels.Verifications;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications
{
    /// <summary>
    /// Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class TestDetailsView : ReactiveUserControl<TestDetailsViewModel>
    {
        public TestDetailsView()
        {
            InitializeComponent();
        }
    }
}
