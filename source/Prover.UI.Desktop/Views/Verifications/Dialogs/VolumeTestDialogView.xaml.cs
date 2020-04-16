using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Prover.Shared;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Verifications.Dialogs
{
    /// <summary>
    /// Interaction logic for SessionDialogView.xaml
    /// </summary>
    public partial class VolumeTestDialogView : IDisposable
    {
        public VolumeTestDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                UncorrectedTargetValueTextBlock.Text = ViewModel.TargetUncorrectedPulses.ToString();
                UncorrectedChannelNameTextBlock.Text = ViewModel.PulseListenerService.PulseChannels
                    .FirstOrDefault(p => p.Items.ChannelType == PulseOutputType.UncVol)?.Channel.ToString();

                ViewModel.PulseListenerService.PulseCountUpdates
                    .Where(p => p.Items.ChannelType == PulseOutputType.UncVol)
                    .Select(p => p.PulseCount.ToString())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, view => view.UncorrectedPulseValueTextBlock.Text)
                    .DisposeWith(d);

                CorrectedChannelNameTextBlock.Text = ViewModel.PulseListenerService.PulseChannels
                    .FirstOrDefault(p => p.Items.ChannelType == PulseOutputType.CorVol)?.Channel.ToString();

                ViewModel.PulseListenerService.PulseCountUpdates
                    .Where(p => p.Items.ChannelType == PulseOutputType.CorVol)
                    .Select(p => p.PulseCount.ToString())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, view => view.CorrectedPulseValueTextBlock.Text)
                    .DisposeWith(d);
            });
        }


        public void Dispose()
        {

        }
    }
}
