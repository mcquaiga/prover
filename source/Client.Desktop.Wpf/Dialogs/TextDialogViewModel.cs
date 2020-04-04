namespace Client.Desktop.Wpf.Dialogs
{
    public class TextDialogViewModel : DialogViewModel
    {
        public TextDialogViewModel(string message, string title = "")
        {
            Message = message;
            Title = title;
        }
    }
}