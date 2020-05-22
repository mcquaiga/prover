using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Items.DriveTypes;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume.Rotary
{
	public sealed class RotaryMeterTestViewModel : ItemVarianceTestViewModel<RotaryMeterItems>
	{
		public RotaryMeterTestViewModel(RotaryMeterItems rotaryItems) : base(rotaryItems,
			Tolerances.METER_DIS_ERROR_THRESHOLD)
		{
			Items = rotaryItems;


		}

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);
			this.WhenAnyValue(x => x.Items)
				.Where(x => x != null)
				.Select(x => x.MeterDisplacement != 0 ? x.MeterDisplacement : x.MeterType.MeterDisplacement ?? 0)
				.ToPropertyEx(this, x => x.ActualValue)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.Items)
				.Where(x => x != null)
				.Select(x => x.MeterType.MeterDisplacement ?? 0)
				.ToPropertyEx(this, x => x.ExpectedValue)
				.DisposeWith(cleanup);
		}
	}
}