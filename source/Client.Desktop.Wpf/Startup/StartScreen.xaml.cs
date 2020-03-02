using System;
using System.Windows;

namespace Client.Desktop.Wpf.Startup
{
    /// <summary>
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : Window, IDisposable
    {
        public StartScreen()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
        }
    }
}