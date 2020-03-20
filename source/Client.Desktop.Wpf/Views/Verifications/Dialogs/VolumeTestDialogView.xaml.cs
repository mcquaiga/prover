using Prover.Shared;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Client.Desktop.Wpf.Views.Verifications.Dialogs
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
                    .FirstOrDefault(p => p.Items.Units == PulseOutputUnitType.UncVol)?.Channel.ToString();

                ViewModel.PulseListenerService.PulseCountUpdates
                    .Where(p => p.Items.Units == PulseOutputUnitType.UncVol)
                    .Select(p => p.PulseCount.ToString())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, view => view.UncorrectedPulseValueTextBlock.Text)
                    .DisposeWith(d);

                CorrectedChannelNameTextBlock.Text = ViewModel.PulseListenerService.PulseChannels
                    .FirstOrDefault(p => p.Items.Units == PulseOutputUnitType.CorVol)?.Channel.ToString();

                ViewModel.PulseListenerService.PulseCountUpdates
                    .Where(p => p.Items.Units == PulseOutputUnitType.CorVol)
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
