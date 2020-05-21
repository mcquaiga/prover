using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using Devices.Core.Items;
using Prover.Application.Services.LiveReadCorrections;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.QATests.Dialogs
{
	/// <summary>
	///     Interaction logic for LiveReadDialogView.xaml
	/// </summary>
	public partial class LiveReadDialogView
	{
		public LiveReadDialogView()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				//ConfigTemp(d);
				//ConfigPressure(d);

				this.OneWayBind(ViewModel, vm => vm.LiveReadItems, v => v.LiveReadItemsControl.ItemsSource).DisposeWith(d);
			});
		}

		private void ConfigPressure(CompositeDisposable d)
		{
			var pressureItem = ViewModel.DeviceType.GetLivePressureItem();
			var pressureLive = ViewModel.LiveReadItems.FirstOrDefault(i => i.Item.Number == pressureItem?.Number);

			if (pressureItem != null && pressureLive != null)
			{
				PressureTargetValueTextBlock.Text = pressureLive.TargetValue.ToString(CultureInfo.CurrentCulture);
				pressureLive.StatusUpdates.Select(i => i.Value.DecimalValue()?.ToString() ?? "").ObserveOn(RxApp.MainThreadScheduler).BindTo(this, view => view.PressureValueTextBlock.Text).DisposeWith(d);

				pressureLive.StatusUpdates.Select(i => i.Stabilizer.Difference.ToString(CultureInfo.InvariantCulture))
							.ObserveOn(RxApp.MainThreadScheduler)
							.BindTo(this, view => view.PressureDifferenceTextBlock.Text)
							.DisposeWith(d);

				//pressureLive.StatusUpdates
				//    .Select(i => i.Stabilizer.Average.ToString(CultureInfo.InvariantCulture))
				//    .ObserveOn(RxApp.MainThreadScheduler)
				//    .BindTo(this, view => view.PressureAverageTextBlock.Text)
				//    .DisposeWith(d);

				pressureLive.StatusUpdates.Select(i => i.Stabilizer.Progress().ToString(CultureInfo.InvariantCulture))
							.ObserveOn(RxApp.MainThreadScheduler)
							.BindTo(this, view => view.PressureStableProgressBar.Value)
							.DisposeWith(d);
			}
			else
			{
				PressureStackPanelControl.Visibility = Visibility.Collapsed;
			}
		}

		private void ConfigTemp(CompositeDisposable d)
		{
			var tempItem = ViewModel.DeviceType.GetLiveTemperatureItem();
			var temperatureLive = ViewModel.LiveReadItems.FirstOrDefault(i => i.Item.Number == tempItem?.Number);

			if (tempItem != null && temperatureLive != null)
			{

				TemperatureTargetValueTextBlock.Text = temperatureLive.TargetValue.ToString(CultureInfo.InvariantCulture);

				//TemperatureStableProgressBar.Maximum = temperatureLive.Stabilizer.TargetThreshold.ToDouble();
				temperatureLive.StatusUpdates.Select(i => i.Value.DecimalValue()?.ToString() ?? "")
							   .ObserveOn(RxApp.MainThreadScheduler)
							   .BindTo(this, view => view.TemperatureValueTextBlock.Text)
							   .DisposeWith(d);

				temperatureLive.StatusUpdates.Select(i => i.Stabilizer.Difference.ToString(CultureInfo.InvariantCulture))
							   .Select(t => $"{t} / {temperatureLive.Stabilizer.TargetThreshold}")
							   .ObserveOn(RxApp.MainThreadScheduler)
							   .BindTo(this, view => view.TemperatureDifferenceTextBlock.Text)
							   .DisposeWith(d);

				temperatureLive.StatusUpdates.Select(i => i.Stabilizer.Average.ToString(CultureInfo.InvariantCulture))
							   .ObserveOn(RxApp.MainThreadScheduler)
							   .BindTo(this, view => view.TemperatureAverageTextBlock.Text)
							   .DisposeWith(d);
			}
			else
			{
				TemperatureStackPanelControl.Visibility = Visibility.Collapsed;
			}

			//temperatureLive.StatusUpdates
			//    .Select(i => i.Stabilizer.Progress().ToString(CultureInfo.InvariantCulture))
			//    .ObserveOn(RxApp.MainThreadScheduler)
			//    .BindTo(this, view => view.TemperatureStableProgressBar.Value)
			//    .DisposeWith(d);
		}

		//private void SetItemCard(ItemLiveReadStatus itemStatus)
		//{
		//    var card = (DataTemplate)FindResource("LiveReadCardTemplate");
		//    card.
		//}
	}
}