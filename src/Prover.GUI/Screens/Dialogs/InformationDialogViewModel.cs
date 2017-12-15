using System.Reactive;
using ReactiveUI;

namespace Prover.GUI.Screens.Dialogs
{
    public class InformationDialogViewModel : DialogViewModel
    {
        public InformationDialogViewModel(string displayText)
        {
            DisplayText = displayText;

            DoneCommand = ReactiveCommand.Create(() => ShowDialog = false);

            ShowDialog = true;
        }

        private string _displayText;
        public string DisplayText
        {
            get => _displayText;
            set => this.RaiseAndSetIfChanged(ref _displayText, value);
        }

        public ReactiveCommand DoneCommand { get; }

        public override void Dispose()
        {
            TaskCommand?.Dispose();
        }
    }
}