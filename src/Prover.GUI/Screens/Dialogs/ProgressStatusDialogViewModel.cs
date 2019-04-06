namespace Prover.GUI.Screens.Dialogs
{
    using Prover.Core.Models.Instruments;
    using Prover.Core.VerificationTests.Events;
    using PubSub.Extension;
    using ReactiveUI;
    using System;
    using System.Reactive;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ProgressStatusDialogViewModel" />
    /// </summary>
    public class ProgressStatusDialogViewModel : DialogViewModel
    {
        #region Constants

        /// <summary>
        /// Defines the LiveReadStatusView
        /// </summary>
        private const string LiveReadStatusView = "LiveReadStatusView";

        /// <summary>
        /// Defines the StatusProgressBarView
        /// </summary>
        private const string StatusProgressBarView = "StatusAndProgressBarView";

        /// <summary>
        /// Defines the VolumeTestStatusView
        /// </summary>
        private const string VolumeTestStatusView = "VolumeTestStatusView";

        #endregion

        #region Fields

        /// <summary>
        /// Defines the _contentItem
        /// </summary>
        private string _contentItem;

        /// <summary>
        /// Defines the _corPulseCount
        /// </summary>
        private int _corPulseCount;

        /// <summary>
        /// Defines the _headerText
        /// </summary>
        private string _headerText;

        /// <summary>
        /// Defines the _liveReadStatus
        /// </summary>
        private LiveReadStatusEvent _liveReadStatus;

        /// <summary>
        /// Defines the _statusProgress
        /// </summary>
        private long _statusProgress;

        /// <summary>
        /// Defines the _statusText
        /// </summary>
        private string _statusText;

        /// <summary>
        /// Defines the _uncorPulseCount
        /// </summary>
        private int _uncorPulseCount;

        /// <summary>
        /// Defines the _volumeTest
        /// </summary>
        private VolumeTest _volumeTest;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressStatusDialogViewModel"/> class.
        /// </summary>
        /// <param name="headerText">The headerText<see cref="string"/></param>
        /// <param name="taskFunc">The taskFunc<see cref="Func{IObserver{string}, CancellationToken, Task}"/></param>
        public ProgressStatusDialogViewModel(string headerText,
            Func<IObserver<string>, CancellationToken, Task> taskFunc)
        {
            ContentItem = StatusProgressBarView;
            HeaderText = headerText;

            CancellationTokenSource = new CancellationTokenSource();
            var statusObserver = Observer.Create<string>(s => StatusText = s);
            TaskCommand = ReactiveCommand.CreateFromTask(() => taskFunc(statusObserver, CancellationTokenSource.Token)
                .ContinueWith(_ => TryClose(true)));

            TaskCommand.IsExecuting
                .Subscribe(x => ShowDialog = x);

            CancelCommand = ReactiveCommand.Create(() =>
                {
                    StatusText = "Cancelling...";
                    CancellationTokenSource?.Cancel();
                },
                TaskCommand.IsExecuting);

            ShowDialog = true;

            this.Subscribe<LiveReadStatusEvent>(e =>
            {
                ContentItem = LiveReadStatusView;
                HeaderText = e.HeaderMessage;
                LiveReadStatus = e;
            });

            this.Subscribe<VolumeTestStatusEvent>(e =>
            {
                ContentItem = VolumeTestStatusView;
                HeaderText = e.HeaderText;
                VolumeTest = e.VolumeTest;
                UncorPulseCount = VolumeTest.UncPulseCount;
                CorPulseCount = VolumeTest.CorPulseCount;
            });

            this.Subscribe<VerificationTestEvent>(e =>
            {
                ContentItem = StatusProgressBarView;
                StatusText = e.Message;
            });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CancelCommand
        /// </summary>
        public ReactiveCommand CancelCommand { get; }

        /// <summary>
        /// Gets or sets the ContentItem
        /// </summary>
        public string ContentItem { get => _contentItem; set => this.RaiseAndSetIfChanged(ref _contentItem, value); }

        /// <summary>
        /// Gets or sets the CorPulseCount
        /// </summary>
        public int CorPulseCount { get => _corPulseCount; set => this.RaiseAndSetIfChanged(ref _corPulseCount, value); }

        /// <summary>
        /// Gets or sets the HeaderText
        /// </summary>
        public string HeaderText { get => _headerText; set => this.RaiseAndSetIfChanged(ref _headerText, value); }

        /// <summary>
        /// Gets or sets the LiveReadStatus
        /// </summary>
        public LiveReadStatusEvent LiveReadStatus { get => _liveReadStatus; set => this.RaiseAndSetIfChanged(ref _liveReadStatus, value); }

        /// <summary>
        /// Gets or sets the StatusProgress
        /// </summary>
        public long StatusProgress
        {
            get
            {
                return _statusProgress;
            }

            set => this.RaiseAndSetIfChanged(ref _statusProgress, value);
        }

        /// <summary>
        /// Gets or sets the StatusText
        /// </summary>
        public string StatusText { get => _statusText; set => this.RaiseAndSetIfChanged(ref _statusText, value); }

        /// <summary>
        /// Gets or sets the UncorPulseCount
        /// </summary>
        public int UncorPulseCount { get => _uncorPulseCount; set => this.RaiseAndSetIfChanged(ref _uncorPulseCount, value); }

        /// <summary>
        /// Gets or sets the VolumeTest
        /// </summary>
        public VolumeTest VolumeTest { get => _volumeTest; set => this.RaiseAndSetIfChanged(ref _volumeTest, value); }

        #endregion

        #region Methods

        /// <summary>
        /// The Dispose
        /// </summary>
        public override void Dispose()
        {
            CancelCommand?.Dispose();
            TaskCommand?.Dispose();
            CancellationTokenSource?.Dispose();
        }

        #endregion
    }
}
