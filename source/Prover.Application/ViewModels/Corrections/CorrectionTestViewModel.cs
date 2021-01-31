using Prover.Calculations;
using Prover.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Items.ItemGroups;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Corrections
{
	public abstract class VerifyViewModel : ViewModelWithIdBase
	{

	}


	public interface IAssertExpectedActual
	{
		decimal PassTolerance { get; }
		decimal ExpectedValue { get; }
		decimal ActualValue { get; }
		decimal PercentError { get; }
		bool Verified { get; }
	}

	public interface IDeviceItemsGroup<T>
		where T : class
	{
		T Items { get; set; }
	}

	public abstract class VarianceTestViewModel : VerificationViewModel, IAssertExpectedActual
	{
		protected VarianceTestViewModel()
		{
		}

		protected VarianceTestViewModel(decimal passTolerance)
		{
			PassTolerance = passTolerance;


		}

		[Reactive] public decimal PassTolerance { get; protected set; }
		public extern decimal ExpectedValue { [ObservableAsProperty] get; }
		public extern decimal ActualValue { [ObservableAsProperty] get; }
		public extern decimal PercentError { [ObservableAsProperty] get; }

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			VerifiedObservable = this.WhenAnyValue(x => x.PercentError).Select(p => p.IsBetween(PassTolerance));

			this.WhenAnyValue(x => x.ExpectedValue, x => x.ActualValue, Calculators.PercentDeviation)
				.ToPropertyEx(this, x => x.PercentError, 100m, true)
				.DisposeWith(Cleanup);
		}
	}

	public abstract class DeviationTestViewModel<T> : ViewModelWithIdBase, IAssertVerification, IDeviceItemsGroup<T>
		where T : class
	{
		protected DeviationTestViewModel()
		{
		}

		protected DeviationTestViewModel(int passTolerance)
		{
			PassTolerance = passTolerance;


		}

		[Reactive] public T Items { get; set; }
		[Reactive] public int PassTolerance { get; protected set; }
		[Reactive] public int ActualValue { get; set; }
		public extern int ExpectedValue { [ObservableAsProperty] get; }
		public extern int Deviation { [ObservableAsProperty] get; }
		public extern bool Verified { [ObservableAsProperty] get; }

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			this.WhenAnyValue(x => x.ExpectedValue, x => x.ActualValue, Calculators.Deviation)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Select(x => ActualValue == 0 && ExpectedValue == 0 ? 100 : x)
				.ToPropertyEx(this, x => x.Deviation, 100)
				.DisposeWith(Cleanup);

			this.WhenAnyValue(x => x.Deviation)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Select(p => p.IsBetween(PassTolerance))
				.ToPropertyEx(this, x => x.Verified)
				.DisposeWith(Cleanup);
		}
	}

	public abstract class ItemVarianceTestViewModel<T> : VarianceTestViewModel, IDeviceItemsGroup<T>
		where T : class
	{
		protected ItemVarianceTestViewModel(T items, decimal passTolerance) : base(passTolerance) => Items = items;

		protected ItemVarianceTestViewModel()
		{
		}

		[Reactive] public T Items { get; set; }


	}

	public abstract class CorrectionTestViewModel<T> : ItemVarianceTestViewModel<T>
		where T : class
	{
		protected ReactiveCommand<Unit, decimal> UpdateFactor;
		private CompositeDisposable _cleanup = new CompositeDisposable();

		protected CorrectionTestViewModel()
		{
		}

		protected CorrectionTestViewModel(T items, decimal passTolerance) : base(items, passTolerance)
		{


		}

		protected abstract Func<ICorrectionCalculator> CalculatorFactory { get; }

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			UpdateFactor = ReactiveCommand.Create<Unit, decimal>(_ => CalculatorFactory.Invoke().CalculateFactor())
										  .DisposeWith(cleanup);

			UpdateFactor.ToPropertyEx(this, x => x.ExpectedValue)
						.DisposeWith(cleanup);

			UpdateFactor
					.DisposeWith(cleanup);
		}

		/// <inheritdoc />
		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
				_cleanup.Dispose();
		}

		//public IItemGroup TestItems { get; set; }
	}

	//public class CorrectionVerificationsViewModel : ViewModelBase
	//{
	//	public ICollection<VerificationTestPointViewModel> VerificationTests { get; set; } = new List<VerificationTestPointViewModel>();

	//	internal CorrectionVerificationsViewModel RefreshTests(ICollection<VerificationTestPointViewModel> testViewModels)
	//	{
	//		VerificationTests = testViewModels;
	//		return this;
	//	}


	//}

}