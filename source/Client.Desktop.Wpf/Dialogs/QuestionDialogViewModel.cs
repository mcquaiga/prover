using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Dialogs
{
    public class QuestionDialogViewModel : DialogViewModel
    {
        public QuestionDialogViewModel(string message, string title = "")
        {
            Message = message;
            Title = title;
            Answer = false;

            SetResponse = ReactiveCommand.Create<bool>(yesNo => Answer = yesNo);

            SetResponse.InvokeCommand(CloseCommand);
        }

        [Reactive] public bool Answer { get; set; }

        [Reactive] public string Message { get; set; }
        [Reactive] public string Title { get; set; }

        public bool YesResponse => true;

        public bool NoResponse => false;

        public ReactiveCommand<bool, Unit> SetResponse { get; }
    }
}
