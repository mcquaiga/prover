using System;
using System.ComponentModel;

namespace Expression.Blend.SampleData.MainMenuDataSource
{
// To significantly reduce the sample data footprint in your production application, you can set
// the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
    internal class MainMenuDataSource { }
#else

    public class MainMenuDataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainMenuDataSource()
        {
            try
            {
                var resourceUri =
                    new Uri("/Prover.GUI;component/DesignTime/SampleData/MainMenuDataSource/MainMenuDataSource.xaml",
                        UriKind.RelativeOrAbsolute);
                System.Windows.Application.LoadComponent(this, resourceUri);
            }
            catch
            {
            }
        }

        public AppMainMenus AppMainMenus { get; } = new AppMainMenus();
    }

    public class AppMainMenusItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _AppTitle = string.Empty;

        public string AppTitle
        {
            get => _AppTitle;
            set
            {
                if (_AppTitle != value)
                {
                    _AppTitle = value;
                    OnPropertyChanged("AppTitle");
                }
            }
        }

        private string _IconSource = string.Empty;

        public string IconSource
        {
            get => _IconSource;
            set
            {
                if (_IconSource != value)
                {
                    _IconSource = value;
                    OnPropertyChanged("IconSource");
                }
            }
        }
    }

    public class AppMainMenus : System.Collections.ObjectModel.ObservableCollection<AppMainMenusItem>
    {
    }
#endif
}