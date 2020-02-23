using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected IScreenManager ScreenManager { get; }

        protected ViewModelBase(IScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }

        //public virtual void Dispose()
        //{
        //}
    }
}