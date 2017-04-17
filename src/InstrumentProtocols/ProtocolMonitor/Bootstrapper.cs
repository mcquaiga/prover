using System.Windows;
using Caliburn.Micro;
using ProtocolMonitor.ViewModel;

namespace ProtocolMonitor
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<SerialCommViewModel>();
            base.OnStartup(sender, e);
        }
    }
}