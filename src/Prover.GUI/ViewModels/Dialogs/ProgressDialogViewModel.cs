using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Prover.GUI.ViewModels.Dialogs
{
    public class ProgressDialogViewModel : ReactiveScreen
    {
        volatile bool _isBusy;
        BackgroundWorker _worker;

        public string Label { get; set; }        
        public string SubLabel { get; set; }

        internal ProgressDialogResult Result { get; private set; }

        public ProgressDialogViewModel(ProgressDialogSettings settings)
        {
            if (settings == null)
                settings = ProgressDialogSettings.WithLabelOnly;

            //if (settings.ShowSubLabel)
            //{
            //    top = 38;
            //    Height = 110;
            //    SubTextLabel.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    top = 22;
            //    Height = 100;
            //    SubTextLabel.Visibility = Visibility.Collapsed;
            //}

            //if (settings.ShowCancelButton)
            //{
            //    right = 74;
            //    CancelButton.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    right = 0;
            //    CancelButton.Visibility = Visibility.Collapsed;
            //}
        }

        internal ProgressDialogResult Execute(object operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            ProgressDialogResult result = null;

            _isBusy = true;

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _worker.DoWork +=
                (s, e) =>
                {
                    if (operation is Action)
                        ((Action)operation)();
                    else if (operation is Action<BackgroundWorker>)
                        ((Action<BackgroundWorker>)operation)(s as BackgroundWorker);
                    else if (operation is Action<BackgroundWorker, DoWorkEventArgs>)
                        ((Action<BackgroundWorker, DoWorkEventArgs>)operation)(s as BackgroundWorker, e);
                    else if (operation is Func<object>)
                        e.Result = ((Func<object>)operation)();
                    else if (operation is Func<BackgroundWorker, object>)
                        e.Result = ((Func<BackgroundWorker, object>)operation)(s as BackgroundWorker);
                    else if (operation is Func<BackgroundWorker, DoWorkEventArgs, object>)
                        e.Result = ((Func<BackgroundWorker, DoWorkEventArgs, object>)operation)(s as BackgroundWorker, e);
                    else
                        throw new InvalidOperationException("Operation type is not supoorted");
                };  

            _worker.RunWorkerCompleted +=
                (s, e) =>
                {
                    result = new ProgressDialogResult(e);
                  
                    _isBusy = false;
                    //Close();
                };

            _worker.ProgressChanged +=
                (s, e) =>
                {
                    if (!_worker.CancellationPending)
                    {
                        SubLabel = (e.UserState as string) ?? string.Empty;
                        //ProgressBar.Value = e.ProgressPercentage;
                    }
                };

            _worker.RunWorkerAsync();

            ShowDialog();

            return result;
        }

        void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (_worker != null && _worker.WorkerSupportsCancellation)
            {
                SubLabel = "Please wait while process will be cancelled...";
                CancelButtonEnabled = false;
                _worker.CancelAsync();
            }
        }

        public bool CancelButtonEnabled { get; set; }

        void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = _isBusy;
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Action operation)
        {
            return ExecuteInternal(container, label, (object)operation, null);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Action operation, ProgressDialogSettings settings)
        {
            return ExecuteInternal(container, label, (object)operation, settings);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Action<BackgroundWorker> operation)
        {
            return ExecuteInternal(container, label, (object)operation, null);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Action<BackgroundWorker> operation, ProgressDialogSettings settings)
        {
            return ExecuteInternal(container, label, (object)operation, settings);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Action<BackgroundWorker, DoWorkEventArgs> operation)
        {
            return ExecuteInternal(container, label, (object)operation, null);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Action<BackgroundWorker, DoWorkEventArgs> operation, ProgressDialogSettings settings)
        {
            return ExecuteInternal(container, label, (object)operation, settings);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Func<object> operationWithResult)
        {
            return ExecuteInternal(container, label, (object)operationWithResult, null);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Func<object> operationWithResult, ProgressDialogSettings settings)
        {
            return ExecuteInternal(container, label, (object)operationWithResult, settings);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Func<BackgroundWorker, object> operationWithResult)
        {
            return ExecuteInternal(container, label, (object)operationWithResult, null);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Func<BackgroundWorker, object> operationWithResult, ProgressDialogSettings settings)
        {
            return ExecuteInternal(container, label, (object)operationWithResult, settings);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Func<BackgroundWorker, DoWorkEventArgs, object> operationWithResult)
        {
            return ExecuteInternal(container, label, (object)operationWithResult, null);
        }

        internal static ProgressDialogResult Execute(IUnityContainer container, string label, Func<BackgroundWorker, DoWorkEventArgs, object> operationWithResult, ProgressDialogSettings settings)
        {
            return ExecuteInternal(container, label, (object)operationWithResult, settings);
        }

        internal static void Execute(IUnityContainer container, string label, Action operation, Action<ProgressDialogResult> successOperation, Action<ProgressDialogResult> failureOperation = null, Action<ProgressDialogResult> cancelledOperation = null)
        {
            ProgressDialogResult result = ExecuteInternal(container, label, operation, null);

            if (result.Cancelled && cancelledOperation != null)
                cancelledOperation(result);
            else if (result.OperationFailed && failureOperation != null)
                failureOperation(result);
            else if (successOperation != null)
                successOperation(result);
        }

        internal static ProgressDialogResult ExecuteInternal(IUnityContainer container, string label, object operation, ProgressDialogSettings settings)
        {
            var dialog = new ProgressDialogViewModel(settings);

            if (!string.IsNullOrEmpty(label))
                dialog.Label = label;

            return dialog.Execute(operation);
        }

        internal static bool CheckForPendingCancellation(BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (worker.WorkerSupportsCancellation && worker.CancellationPending)
                e.Cancel = true;

            return e.Cancel;
        }

        internal static void Report(BackgroundWorker worker, string message)
        {
            if (worker.WorkerReportsProgress)
                worker.ReportProgress(0, message);
        }

        internal static void Report(BackgroundWorker worker, string format, params object[] arg)
        {
            if (worker.WorkerReportsProgress)
                worker.ReportProgress(0, string.Format(format, arg));
        }

        internal static void Report(BackgroundWorker worker, int percentProgress, string message)
        {
            if (worker.WorkerReportsProgress)
                worker.ReportProgress(percentProgress, message);
        }

        internal static void Report(BackgroundWorker worker, int percentProgress, string format, params object[] arg)
        {
            if (worker.WorkerReportsProgress)
                worker.ReportProgress(percentProgress, string.Format(format, arg));
        }

        internal static bool ReportWithCancellationCheck(BackgroundWorker worker, DoWorkEventArgs e, string message)
        {
            if (CheckForPendingCancellation(worker, e))
                return true;

            if (worker.WorkerReportsProgress)
                worker.ReportProgress(0, message);

            return false;
        }

        internal static bool ReportWithCancellationCheck(BackgroundWorker worker, DoWorkEventArgs e, string format, params object[] arg)
        {
            if (CheckForPendingCancellation(worker, e))
                return true;

            if (worker.WorkerReportsProgress)
                worker.ReportProgress(0, string.Format(format, arg));

            return false;
        }

        internal static bool ReportWithCancellationCheck(BackgroundWorker worker, DoWorkEventArgs e, int percentProgress, string message)
        {
            if (CheckForPendingCancellation(worker, e))
                return true;

            if (worker.WorkerReportsProgress)
                worker.ReportProgress(percentProgress, message);

            return false;
        }

        internal static bool ReportWithCancellationCheck(BackgroundWorker worker, DoWorkEventArgs e, int percentProgress, string format, params object[] arg)
        {
            if (CheckForPendingCancellation(worker, e))
                return true;

            if (worker.WorkerReportsProgress)
                worker.ReportProgress(percentProgress, string.Format(format, arg));

            return false;
        }
    }

    public class ProgressDialogSettings
    {
        public static ProgressDialogSettings WithLabelOnly = new ProgressDialogSettings(false, false, true);
        public static ProgressDialogSettings WithSubLabel = new ProgressDialogSettings(true, false, true);
        public static ProgressDialogSettings WithSubLabelAndCancel = new ProgressDialogSettings(true, true, true);

        public bool ShowSubLabel { get; set; }
        public bool ShowCancelButton { get; set; }
        public bool ShowProgressBarIndeterminate { get; set; }

        public ProgressDialogSettings()
        {
            ShowSubLabel = false;
            ShowCancelButton = false;
            ShowProgressBarIndeterminate = true;
        }

        public ProgressDialogSettings(bool showSubLabel, bool showCancelButton, bool showProgressBarIndeterminate)
        {
            ShowSubLabel = showSubLabel;
            ShowCancelButton = showCancelButton;
            ShowProgressBarIndeterminate = showProgressBarIndeterminate;
        }
    }

    internal class ProgressDialogResult
    {
        public object Result { get; private set; }
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }

        public bool OperationFailed
        {
            get { return Error != null; }
        }

        public ProgressDialogResult(RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                Cancelled = true;
            else if (e.Error != null)
                Error = e.Error;
            else
                Result = e.Result;
        }
    }
 
}

