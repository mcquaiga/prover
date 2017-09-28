using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.GUI.Common.Events;
using ReactiveUI;

namespace Prover.GUI.Common.Screens.Dialogs
{
    public static partial class DialogDisplayHelpers
    {        
        public static ReactiveCommand ProgressStatusDialogCommand(IEventAggregator eventAggregator, string headerText,
            Func<IObserver<string>, CancellationToken, Task> taskFunc)
        {
            return ReactiveCommand.Create(() =>
            {
                ProgressStatusDialogMessage(eventAggregator, headerText, taskFunc);
            });
        }

        public static void ProgressStatusDialogMessage(IEventAggregator eventAggregator, string headerText, Func<IObserver<string>, CancellationToken, Task> taskFunc)
        {
            var message = new DialogDisplayEvent(new ProgressStatusDialogViewModel(headerText, taskFunc));
            eventAggregator.PublishOnUIThreadAsync(message);
        }
    }
}
