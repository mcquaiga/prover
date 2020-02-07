using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.GUI.Events;
using ReactiveUI;

namespace Prover.GUI.Screens.Dialogs
{
    public static class DialogDisplayHelpers
    {
        public static ReactiveCommand ProgressStatusDialogCommand(IEventAggregator eventAggregator, string headerText,
            Func<IObserver<string>, CancellationToken, Task> taskFunc, IObservable<bool> canExecute = null)
        {
            return ReactiveCommand.Create(() => { ProgressStatusDialogMessage(eventAggregator, headerText, taskFunc); },
                canExecute);
        }

        public static void ProgressStatusDialogMessage(IEventAggregator eventAggregator, string headerText,
            Func<IObserver<string>, CancellationToken, Task> taskFunc)
        {
            var message = new DialogDisplayEvent(new ProgressStatusDialogViewModel(headerText, taskFunc));
            eventAggregator.PublishOnUIThreadAsync(message);
        }
    }
}