using Devices.Terminal.Wpf.Controls;
using ReactiveUI;
using Splat;

namespace Devices.Terminal.Wpf
{
    public class MainViewModel : ReactiveObject, IScreen
    {
        public MainViewModel()
        {
            Router = new RoutingState();

            SettingsViewModel = new SettingsViewModel();
        }

        public RoutingState Router { get; }
        public SettingsViewModel SettingsViewModel { get; }
    }
}