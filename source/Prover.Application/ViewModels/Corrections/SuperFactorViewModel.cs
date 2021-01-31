﻿using Devices.Core.Items.ItemGroups;
using Prover.Calculations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Prover.Application.ViewModels.Corrections
{
	public sealed class SuperFactorViewModel : CorrectionTestViewModel<SuperFactorItems>
	{
		private const decimal Tolerance = Tolerances.SUPER_FACTOR_TOLERANCE;


		public SuperFactorViewModel(SuperFactorItems items) : base(items, Tolerance)
		{

		}

		public SuperFactorViewModel(SuperFactorItems items, TemperatureFactorViewModel temperature, PressureFactorViewModel pressure)
			: this(items)
		{
			Temperature = temperature;
			Pressure = pressure;

			//Setup(Temperature, Pressure);
		}

		public extern decimal SquaredFactor { [ObservableAsProperty] get; }

		[Reactive] public TemperatureFactorViewModel Temperature { get; protected set; }
		[Reactive] public PressureFactorViewModel Pressure { get; protected set; }

		protected override Func<ICorrectionCalculator> CalculatorFactory
			=> () => new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, Temperature.Gauge, Pressure.Gauge);

		public void Setup(TemperatureFactorViewModel tempViewModel, PressureFactorViewModel pressureViewModel)
		{
			Temperature = tempViewModel;
			Pressure = pressureViewModel;
		}

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			this.WhenAnyValue(x => x.Pressure.Items)
				.Where(i => i != null)
				.Select(i => i.UnsqrFactor)
				.ToPropertyEx(this, x => x.ActualValue, Pressure.Items.UnsqrFactor)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.Temperature.Gauge, x => x.Pressure.Gauge, x => x.Pressure.AtmosphericGauge)
				.Select(_ => Unit.Default)
				.InvokeCommand(UpdateFactor)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.ExpectedValue)
				.Select(Calculators.SquaredFactor)
				.ToPropertyEx(this, x => x.SquaredFactor)
				.DisposeWith(cleanup);
		}
	}
}