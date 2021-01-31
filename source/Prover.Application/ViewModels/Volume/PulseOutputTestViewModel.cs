using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using Prover.Calculations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
	public class PulseOutputTestViewModel : DeviationTestViewModel<PulseOutputItems.ChannelItems>
	{
		private readonly VolumeTestRunViewModelBase _volumeTest;
		private decimal _multiplier;

		public PulseOutputTestViewModel(PulseOutputItems.ChannelItems pulseChannelItems)
			: base(Tolerances.PULSE_VARIANCE_THRESHOLD)
		{
			Items = pulseChannelItems;
		}

		public PulseOutputTestViewModel(PulseOutputItems.ChannelItems pulseChannelItems, VolumeTestRunViewModelBase volumeTest)
			: this(pulseChannelItems)
		{
			_volumeTest = volumeTest;
			_multiplier = (Items as IVolumeUnits)?.Units.Multiplier ?? volumeTest.Multiplier;
		}

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			if (_volumeTest != null)
			{
				this.WhenAnyValue(x => x._volumeTest.ExpectedValue)
					.Select(expectedVolume => VolumeCalculator.PulseCount(expectedVolume, _multiplier))
					.ToPropertyEx(this, x => x.ExpectedValue)
					.DisposeWith(cleanup);
			}

		}
	}
}