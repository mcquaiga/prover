using ReactiveUI;

namespace Devices.Terminal.Wpf
{
    public class MainViewModel : ReactiveObject, IScreen
    {
        #region Constructors

        public MainViewModel()
        {
            Router = new RoutingState();
        }

        #endregion

        #region Properties

        public RoutingState Router { get; }

        #endregion
    }
}