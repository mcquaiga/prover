using System.Reactive;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.Application.Interactions
{
    public static class MessageInteractions
    {

        public static Interaction<IDialogViewModel, Unit> ShowDialog { get; } 
            = new Interaction<IDialogViewModel, Unit>();

        public static Interaction<string, Unit> ShowMessage { get; } = new Interaction<string, Unit>();

        public static Interaction<string, Unit> ShowError { get; } = new Interaction<string, Unit>();

        public static Interaction<string, bool> ShowYesNo { get; } = new Interaction<string, bool>();

        public static Interaction<string, bool> ShowQuestion { get; } = new Interaction<string, bool>();

        public static Interaction<string, string> GetInputString { get; } = new Interaction<string, string>();

        public static Interaction<string, object> GetInput { get; } = new Interaction<string, object>();

        public static Interaction<string, decimal> GetInputNumber { get; } = new Interaction<string, decimal>();

        public static Interaction<string, int> GetInputInteger { get; } = new Interaction<string, int>();

        //public static Interaction<string, decimal> GetInputDecimal { get; } = new Interaction<string, decimal>();

        public static Interaction<string, string> OpenFileDialog { get; } = new Interaction<string, string>();

        //public static Interaction<string, T> GetInput<T>() => new Interaction<string, T>();

    }

    public static class NotificationInteractions
    {
        public static Interaction<string, Unit> SnackBarMessage { get; } = new Interaction<string, Unit>();
    }
}