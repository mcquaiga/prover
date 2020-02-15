using System;
using System.Windows.Controls;
using System.Windows.Forms;
using MvvmDialogs;
using ReactiveUI;

namespace Client.Wpf.Screens
{
    public class ReactiveDialog<T> : ReactiveWindow<T>, IWindow 
        where T : class
    {
        private object _dataContext;

        public object DataContext
        {
            get => ViewModel;

            set => ViewModel =(T)value;
        }

        public bool? DialogResult { get; set; }
        public ContentControl Owner { get; set; }
    }
}