﻿using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using Prover.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public class PulseOutputTestViewModel : DeviationTestViewModel<PulseOutputItems.ChannelItems>
    {
        private readonly VolumeTestRunViewModelBase _volumeTest;

        public PulseOutputTestViewModel(PulseOutputItems.ChannelItems pulseChannelItems) : base(
            Global.PULSE_VARIANCE_THRESHOLD)
        {
            Items = pulseChannelItems;
        }

        public PulseOutputTestViewModel(PulseOutputItems.ChannelItems pulseChannelItems,
            VolumeTestRunViewModelBase volumeTest) : this(pulseChannelItems)
        {
            _volumeTest = volumeTest;

            this.WhenAnyValue(x => x._volumeTest.ActualValue)
                .Select(x => x.ToInt32())
                .ToPropertyEx(this, x => x.ActualValue)
                .DisposeWith(Cleanup);
        }
    }
}