﻿//      *********    DO NOT MODIFY THIS FILE     *********
//      This file is regenerated by a design tool. Making
//      changes to this file can cause errors.
namespace Expression.Blend.SampleData.MainMenuDataSource
{
    using System; 
    using System.ComponentModel;

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
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainMenuDataSource()
        {
            try
            {
                Uri resourceUri = new Uri("/Prover.GUI.Common;component/SampleData/MainMenuDataSource/MainMenuDataSource.xaml", UriKind.RelativeOrAbsolute);
                System.Windows.Application.LoadComponent(this, resourceUri);
            }
            catch
            {
            }
        }

        private AppMainMenus _AppMainMenus = new AppMainMenus();

        public AppMainMenus AppMainMenus
        {
            get
            {
                return this._AppMainMenus;
            }
        }
    }

    public class AppMainMenusItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _AppTitle = string.Empty;

        public string AppTitle
        {
            get
            {
                return this._AppTitle;
            }

            set
            {
                if (this._AppTitle != value)
                {
                    this._AppTitle = value;
                    this.OnPropertyChanged("AppTitle");
                }
            }
        }

        private string _IconSource = string.Empty;

        public string IconSource
        {
            get
            {
                return this._IconSource;
            }

            set
            {
                if (this._IconSource != value)
                {
                    this._IconSource = value;
                    this.OnPropertyChanged("IconSource");
                }
            }
        }
    }

    public class AppMainMenus : System.Collections.ObjectModel.ObservableCollection<AppMainMenusItem>
    { 
    }
#endif
}