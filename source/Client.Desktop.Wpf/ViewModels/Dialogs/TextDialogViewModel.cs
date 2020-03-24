using Client.Desktop.Wpf.Dialogs;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Dialogs
{
    public class TextDialogViewModel : DialogViewModel
    {
        public TextDialogViewModel(string message, string title = "")
        {
            Message = message;
            Title = title;
        }

        [Reactive] public string Message { get; set; }
        [Reactive] public string Title { get; set; }
    }
}