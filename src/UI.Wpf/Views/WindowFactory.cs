using System;
using Client.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Wpf.Views
{
    public class WindowFactory : IWindowFactory
    {
        private readonly IServiceProvider _resolver;

        public WindowFactory(IServiceProvider resolver)
        {
            _resolver = resolver;
        }

        public MainWindow Create(bool showMenu = true)
        {
            var window = new MainWindow();
            var model = _resolver.GetService<MainViewModel>();

            window.ViewModel = model;
            
            if (showMenu) 
                model.ShowMenu();

            
            window.Closing += (sender, e) =>
            {
                (((MainWindow)sender).ViewModel as IDisposable)?.Dispose();
            };

            return window;
        }
    }

    public interface IWindowFactory
    {
        MainWindow Create(bool showMenu = true);
    }
}
