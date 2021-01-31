using Devices.Core.Items.ItemGroups;
using Prover.Application.Interfaces;
using Prover.Calculations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Disposables;

namespace Prover.Application.ViewModels.Volume
{
	public class CorrectedVolumeTestViewModel : VolumeTestRunViewModelBase
	{
		private readonly ICalculateTrueCorrectedFactor _trueCorrectedFactor;
		private const decimal Tolerance = Tolerances.COR_ERROR_THRESHOLD;

		public CorrectedVolumeTestViewModel(IUncorrectedVolumeTestViewModel uncorrected,
			ICalculateTrueCorrectedFactor trueCorrectedFactor, VolumeItems startValues, VolumeItems endValues)
			: base(Tolerance, startValues, endValues)
		{
			_trueCorrectedFactor = trueCorrectedFactor;
			Uncorrected = uncorrected;

			Multiplier = startValues.CorrectedMultiplier;
		}

		[Reactive] public IUncorrectedVolumeTestViewModel Uncorrected { get; protected set; }

		public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			this.WhenAnyValue(x => x.StartReading, x => x.EndReading, (start, end) => VolumeCalculator.TotalVolume(start, end, StartValues.CorrectedMultiplier))
				.ToPropertyEx(this, x => x.ActualValue)
				.DisposeWith(cleanup);

			_trueCorrectedFactor.TrueCorrectedObservable
								.ToPropertyEx(this, x => x.TotalCorrectionFactor, _trueCorrectedFactor.TotalCorrectionFactor, true)
								.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.Uncorrected.UncorrectedInputVolume, x => x.TotalCorrectionFactor, (input, factor) => VolumeCalculator.TrueCorrected(factor, input))
				.ToPropertyEx(this, x => x.ExpectedValue)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.StartValues)
				.Subscribe(v => StartReading = v.CorrectedReading)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.EndValues)
				.Subscribe(v => EndReading = v.CorrectedReading)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.StartReading)
				.Subscribe(startReading => StartValues.CorrectedReading = startReading)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.EndReading)
				.Subscribe(endReading => EndValues.CorrectedReading = endReading)
				.DisposeWith(cleanup);
		}
	}
}