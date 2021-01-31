using Devices.Core.Items.ItemGroups;
using Prover.Calculations;
using Prover.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Prover.Application.ViewModels.Volume
{
	public interface IUncorrectedVolumeTestViewModel
	{
		VolumeInputType DriveInputType { get; set; }
		decimal AppliedInput { get; set; }
		decimal UncorrectedInputVolume { get; }
		decimal DriveRate { get; }
		VolumeItems StartValues { get; set; }
		VolumeItems EndValues { get; set; }
		decimal StartReading { get; set; }
		decimal EndReading { get; set; }
		decimal Multiplier { get; set; }
		PulseOutputTestViewModel PulseOutputTest { get; set; }
		decimal PassTolerance { get; }
		decimal ExpectedValue { get; }
		decimal ActualValue { get; }
		decimal PercentError { get; }
		bool Verified { get; }
		IObservable<bool> VerifiedObservable { get; }
	}

	public class UncorrectedVolumeTestViewModel : VolumeTestRunViewModelBase, IDeviceStartAndEndValues<VolumeItems>, IUncorrectedVolumeTestViewModel
	{
		private const decimal Tolerance = Tolerances.UNCOR_ERROR_THRESHOLD;

		public UncorrectedVolumeTestViewModel(VolumeInputType inputType, VolumeItems startValues, VolumeItems endValues, decimal? driveRate = null)
			: base(Tolerance, startValues, endValues)
		{
			DriveInputType = inputType;



			Multiplier = startValues.UncorrectedMultiplier;

			DriveRate = driveRate ?? startValues.DriveRate;
		}

		public VolumeInputType DriveInputType { get; set; }

		[Reactive] public decimal AppliedInput { get; set; }

		public extern decimal UncorrectedInputVolume { [ObservableAsProperty] get; }

		protected override void Dispose(bool isDisposing)
		{
		}

		public virtual decimal DriveRate { get; protected set; }

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			this.WhenAnyValue(x => x.StartReading, x => x.EndReading, (start, end) => VolumeCalculator.TotalVolume(start, end, StartValues.UncorrectedMultiplier))
				.ToPropertyEx(this, x => x.ActualValue)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.AppliedInput)
				.Select(_ => VolumeCalculator.TrueUncorrected(DriveRate, AppliedInput))
				.ToPropertyEx(this, x => x.ExpectedValue)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.ExpectedValue)
				.ToPropertyEx(this, x => x.UncorrectedInputVolume)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.StartValues)
				.Subscribe(v => StartReading = v.UncorrectedReading)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.EndValues)
				.Subscribe(v => EndReading = v.UncorrectedReading)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.StartReading)
				.Subscribe(startReading => StartValues.UncorrectedReading = startReading)
				.DisposeWith(cleanup);

			this.WhenAnyValue(x => x.EndReading)
				.Subscribe(endReading => EndValues.UncorrectedReading = endReading)
				.DisposeWith(cleanup);
		}
	}
}