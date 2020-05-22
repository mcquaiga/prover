using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Items.ItemGroups;
using Prover.Calculations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Corrections
{
	public sealed class TemperatureFactorViewModel : CorrectionTestViewModel<TemperatureItems>
	{
		private const decimal Tolerance = Tolerances.TEMP_ERROR_TOLERANCE;

		[Reactive] public decimal Gauge { get; set; }

		public TemperatureFactorViewModel(TemperatureItems items, decimal gauge) : base(items, Tolerance)
		{
			Gauge = gauge;


		}

		protected override Func<ICorrectionCalculator> CalculatorFactory
			=> () => new TemperatureCalculator(Items.Units, Items.Base, Gauge);

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			this.WhenAnyValue(x => x.Items)
				.Select(i => i.Factor)
				.ToPropertyEx(this, x => x.ActualValue, Items.Factor)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.Gauge)
				.Select(i => Unit.Default)
				.InvokeCommand(UpdateFactor)
				.DisposeWith(cleanup);
		}
	}
}