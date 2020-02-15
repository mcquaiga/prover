using System;
using System.Threading.Tasks;
using Client.Wpf.ViewModels;
using Client.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace Client.Wpf
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
                //if (TabablzControl.GetIsClosingAsPartOfDragOperation(window)) return;

                (((MainWindow)sender).DataContext as IDisposable)?.Dispose();
            };

            return window;
        }
    }

    public interface IWindowFactory
    {
        MainWindow Create(bool showMenu = true);
    }
}
