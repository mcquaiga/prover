using Devices.Core.Items.ItemGroups;
using Prover.Calculations;
using Prover.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Prover.Application.ViewModels.Corrections
{
	public sealed class PressureFactorViewModel : CorrectionTestViewModel<PressureItems>
	{
		private const decimal Tolerance = Tolerances.PRESSURE_ERROR_TOLERANCE;

		private PressureFactorViewModel() { }

		public PressureFactorViewModel(PressureItems items, decimal gauge, decimal atmosphericGauge) : base(items, Tolerance)
		{
			Gauge = gauge;

			if (Items.TransducerType == PressureTransducerType.Absolute)
			{
				AtmosphericGauge = atmosphericGauge;
			}

			_calculator = new PressureCalculator(Items, Gauge, AtmosphericGauge);

		}

		public extern decimal AbsoluteGauge { [ObservableAsProperty] get; }

		public bool ShowAbsolute => Items.TransducerType == PressureTransducerType.Absolute;

		[Reactive] public decimal Gauge { get; set; }
		[Reactive] public decimal AtmosphericGauge { get; set; }

		protected override Func<ICorrectionCalculator> CalculatorFactory => () => _calculator;

		public decimal GetTotalGauge()
		{
			return _calculator.GasPressure;
		}

		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			this.WhenAnyValue(x => x.Items)
				.Select(i => i.Factor)
				.ToPropertyEx(this, x => x.ActualValue, Items.Factor)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.Gauge, x => x.AtmosphericGauge,
				(g, atm) =>
				{
					_calculator.Gauge = g;
					_calculator.GaugeAtmospheric = atm;
					return Unit.Default;
				})
				.Throttle(TimeSpan.FromMilliseconds(100))
				.InvokeCommand(UpdateFactor)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.Gauge, x => x.AtmosphericGauge, (g, atm) => g + atm)
				.ToPropertyEx(this, x => x.AbsoluteGauge, deferSubscription: true, scheduler: RxApp.MainThreadScheduler)
				.DisposeWith(cleanup);
		}

		private readonly PressureCalculator _calculator;
		private PressureCalculator Calculator() => new PressureCalculator(Items, Gauge, AtmosphericGauge);
	}
}