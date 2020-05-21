using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Prover.Application.ViewModels.Volume.Rotary
{
	public class RotaryUncorrectedVolumeTestViewModel : UncorrectedVolumeTestViewModel //, IUncorrectedVolumeTestViewModel
	{
		private readonly RotaryMeterItems _rotaryItems;

		public RotaryUncorrectedVolumeTestViewModel() : base(VolumeInputType.Rotary, null, null)
		{ }

		/// <inheritdoc />
		public RotaryUncorrectedVolumeTestViewModel(RotaryMeterItems rotaryItems, VolumeItems startValues, VolumeItems endValues)
				: base(VolumeInputType.Rotary, startValues, endValues)
		{
			//_rotaryItems = rotaryItems;
			DriveRate = rotaryItems.MeterType?.MeterDisplacement ?? (rotaryItems.MeterDisplacement != 0 ? rotaryItems.MeterDisplacement : -1);
		}

		/// <inheritdoc />
		public override decimal DriveRate { get; protected set; }
	}
}