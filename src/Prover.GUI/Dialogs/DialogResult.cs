namespace Prover.GUI.Dialogs
{
    //public enum Answer
    //{
    //    Yes,
    //    No,
    //    Ok,
    //    Cancel,
    //    Abort,
    //    Retry,
    //    Ignore
    //}

    //public class DialogResult<TResponse> : IResult
    //{
    //    private Func<IDialogViewModel<TResponse>> _locateVM =
    //        () => new DialogViewModel<TResponse>();

    //    public DialogResult(Dialog<TResponse> dialog)
    //    {
    //        Dialog = dialog;
    //    }

    //    public Dialog<TResponse> Dialog { get; private set; }

    //    public void Execute(CoroutineExecutionContext context)
    //    {
    //        IDialogViewModel<TResponse> vm = _locateVM();
    //        vm.Dialog = Dialog;
    //        Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowDialog(vm));
    //    }

    //    public event EventHandler<ResultCompletionEventArgs> Completed;

    //    public DialogResult<TResponse> In(IDialogViewModel<TResponse> dialogViewModel)
    //    {
    //        _locateVM = () => dialogViewModel;
    //        return this;
    //    }

    //    public DialogResult<TResponse> In<TDialogViewModel>()
    //        where TDialogViewModel : IDialogViewModel<TResponse>
    //    {
    //        _locateVM = () => IoC.Get<TDialogViewModel>();
    //        return this;
    //    }
    //}
}