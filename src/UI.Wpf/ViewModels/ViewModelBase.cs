using ReactiveUI;

namespace Client.Wpf.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected IScreenManager ScreenManager { get; }

        protected ViewModelBase(IScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }
    }
}