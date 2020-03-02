using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Windows.Forms;
using ReactiveUI;

namespace Client.Desktop.Wpf.Interactions
{
    public static class MessageInteractions
    {
        public static Interaction<string, Unit> ShowMessage { get; } = new Interaction<string, Unit>();
        public static Interaction<string, Unit> ShowError { get; } = new Interaction<string, Unit>();

        public static Interaction<string, bool> ShowYesNo { get; } = new Interaction<string, bool>();
    }

    public static class NotificationInteractions
    {
        public static Interaction<string, Unit> SnackBarMessage { get; } = new Interaction<string, Unit>();
    }

    public static class DeviceInteractions
    {
        public static Interaction<string, Unit> Connect { get; } = new Interaction<string, Unit>();
        public static Interaction<string, Unit> DownloadItems { get; } = new Interaction<string, Unit>();
        public static Interaction<string, Unit> LiveReadItems { get; } = new Interaction<string, Unit>();
    }
}
