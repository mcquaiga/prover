using Prover.Application.Interfaces;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
// ReSharper disable CheckNamespace

namespace Prover.UI.Desktop.Dialogs
{
    public partial class DialogServiceManager
    {
        public void RegisterInteractions(IDialogServiceManager dialogManager)
        {
            Application.Interactions.Messages.ShowMessage.RegisterHandler(async i =>
            {
                await dialogManager.ShowMessage(i.Input, "Message");
                i.SetOutput(Unit.Default);
            });

            Application.Interactions.Messages.ShowYesNo.RegisterHandler(async i =>
            {
                var answer = await dialogManager.ShowQuestion(i.Input);
                i.SetOutput(answer);
            });

            Application.Interactions.Messages.ShowError.RegisterHandler(async i =>
            {
                await dialogManager.ShowMessage(i.Input, "Error");
                i.SetOutput(Unit.Default);
            });

            Application.Interactions.Messages.ShowDialog.RegisterHandler(async viewModel =>
            {
                await ShowViewModel(viewModel.Input);
                viewModel.SetOutput(Unit.Default);
            });

            Application.Interactions.Messages.GetInputString.RegisterHandler(async i =>
            {
                var answer = await dialogManager.ShowInputDialog<string>(i.Input);
                i.SetOutput(answer);
            });

            Application.Interactions.Messages.GetInput.RegisterHandler(async i =>
            {
                var answer = await dialogManager.ShowInputDialog<object>(i.Input);
                i.SetOutput(answer);
            });

            Application.Interactions.Messages.GetInputNumber.RegisterHandler(async i =>
            {
                await Observable.StartAsync(async () =>
                 {
                     var answer = await dialogManager.ShowInputDialog<decimal>(i.Input);
                     i.SetOutput(answer);
                 }, RxApp.MainThreadScheduler);
            });

            Application.Interactions.Messages.OpenFileDialog.RegisterHandler(async i =>
            {
                var fileOpen = new OpenFileDialog();
                if (fileOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    i.SetOutput(fileOpen.FileName);
                }

                return;
            });
        }


    }

}