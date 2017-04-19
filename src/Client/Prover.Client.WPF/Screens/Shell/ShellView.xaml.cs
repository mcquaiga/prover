using System.Windows;
using Prover.Client.Framework.Screens.Shell;
using ReactiveUI;

namespace Prover.Client.WPF.Screens.Shell
{
    /// <summary>
    ///     Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : IViewFor<ShellViewModel>
    {
        public ShellView()
        {
            InitializeComponent();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            Style = (Style) FindResource(typeof(Window));
        }

        public ShellViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ShellViewModel) value; }
        }
    }
}