namespace Prover.UI.Desktop.Dialogs
{
    public class TextDialogViewModel : DialogViewModel
    {
        public TextDialogViewModel()
        {

        }

        public TextDialogViewModel(string message, string title = "")
        {
            Message = message;
            Title = title;
        }
    }
}