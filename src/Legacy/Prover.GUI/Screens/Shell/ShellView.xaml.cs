using System.Windows;
using ReactiveUI;

namespace Prover.GUI.Screens.Shell
{
    /// <summary>
    ///     Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IViewFor<ShellViewModel>
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