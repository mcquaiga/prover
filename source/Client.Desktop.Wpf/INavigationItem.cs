using System.Reactive;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Desktop.Wpf
{
    public interface INavigationItem
    {
        ReactiveCommand<Unit, Unit> Navigate { get; }
        PackIconKind IconKind { get; }
        bool IsHome { get; }
    }

    //public class NavigationBar
    //{
    //    public static INavigationItem HomeItem(IScreenManager screenManager) => new HomeNavigationItem(screenManager);
    //    public static INavigationItem BackItem(IScreenManager screenManager) => new BackNavigationItem(screenManager);

    //    public static IEnumerable<INavigationItem> GetItems() => new INavigationItem[] { };  //{new HomeNavigationItem(), new BackNavigationItem()};

    //    public class HomeNavigationItem : INavigationItem
    //    {
    //        private readonly IScreenManager _screenManager;

    //        public HomeNavigationItem(IScreenManager screenManager)
    //        {
    //            _screenManager = screenManager;
    //        }

    //        public ReactiveCommand<Unit, Unit> Navigate =>
    //            ReactiveCommand.CreateFromObservable(() =>
    //            {
    //                _screenManager.GoHome();
    //                return Observable.Return(Unit.Default);
    //            });

    //        public PackIconKind IconKind => PackIconKind.Home;
    //        public bool IsHome => true;
    //    }

    //    public class BackNavigationItem : INavigationItem
    //    {
    //        private IScreenManager _screenManager;

    //        public BackNavigationItem(IScreenManager screenManager)
    //        {
    //            _screenManager = screenManager;
    //        }

    //        public ReactiveCommand<Unit, Unit> Navigate => _screenManager.Router.NavigateBack;
             

    //        public PackIconKind IconKind => PackIconKind.ArrowBack;
    //        public bool IsHome => false;
    //    }
    //}
    
}