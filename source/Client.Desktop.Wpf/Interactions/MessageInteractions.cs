using System.Reactive;
using ReactiveUI;

namespace Client.Desktop.Wpf.Interactions
{
    public static class MessageInteractions
    {
        public static Interaction<string, Unit> ShowMessage { get; } = new Interaction<string, Unit>();
        public static Interaction<string, Unit> ShowError { get; } = new Interaction<string, Unit>();

        public static Interaction<string, bool> ShowYesNo { get; } = new Interaction<string, bool>();
        public static Interaction<string, bool> ShowQuestion { get; } = new Interaction<string, bool>();
    }

    public static class NotificationInteractions
    {
        public static Interaction<string, Unit> SnackBarMessage { get; } = new Interaction<string, Unit>();
    }
}