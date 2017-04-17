using System.Windows;
using System.Windows.Input;
using ProtocolMonitor.ViewModel;
using ReactiveUI;

namespace ProtocolMonitor.View
{
    /// <summary>
    ///     Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window, IViewFor<SerialCommViewModel>
    {
        public Shell()
        {
            ViewModel = new SerialCommViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public SerialCommViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SerialCommViewModel) value; }
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PART_HELP_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Window Metro Theme v1.0\nDesigned by Heiswayi Nrird (http://heiswayi.github.io) 2015.\nReleased under MIT license.");
        }

        private void PART_MAXIMIZE_RESTORE_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}