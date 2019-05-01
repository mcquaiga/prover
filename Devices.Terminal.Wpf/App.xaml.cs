using System.Windows;

namespace Devices.Terminal.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constructors

        protected override void OnStartup(StartupEventArgs e)
        {
            Bootstrapper.Start();
        }

        #endregion
    }
}