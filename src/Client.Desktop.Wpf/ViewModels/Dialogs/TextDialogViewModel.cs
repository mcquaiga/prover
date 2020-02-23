using Client.Wpf.Screens.Dialogs;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Dialogs
{
    public class TextDialogViewModel : DialogViewModel
    {
        [Reactive] public string Message { get; set; }
        [Reactive] public string Title { get; set; }

        public TextDialogViewModel(string message, string title = "")
        {
            Message = message;
            Title = title;
        }
    }
}