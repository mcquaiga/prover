using System.Windows.Controls;
using Client.Desktop.Wpf.Screens.Dialogs;
using ReactiveUI;

namespace Client.Desktop.Wpf.Screens
{
    public class ReactiveDialog<T> : ReactiveUserControl<T> 
        where T : DialogViewModel
    {
        public bool? DialogResult { get; set; }
        public ContentControl Owner { get; set; }
    }

    public class ReactiveDialog : ReactiveUserControl<DialogViewModel>
    {
        public bool? DialogResult { get; set; }
        public ContentControl Owner { get; set; }
    }
}