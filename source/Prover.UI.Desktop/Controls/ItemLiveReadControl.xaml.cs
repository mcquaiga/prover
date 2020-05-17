using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace Prover.UI.Desktop.Controls
{
	/// <summary>
	/// Interaction logic for ItemLiveReadControl.xaml
	/// </summary>
	public partial class ItemLiveReadControl
	{
		public ItemLiveReadControl()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				var targetThreshold = ViewModel.Stabilizer.TargetThreshold;
				TargetValueTextBlock.Text = ViewModel.TargetValue.ToString(CultureInfo.InvariantCulture);
				//TemperatureStableProgressBar.Maximum = temperatureLive.Stabilizer.TargetThreshold.ToDouble();

				ViewModel.StatusUpdates
						 .Select(i => i.Value.DecimalValue()?.ToString() ?? "")
						 .ObserveOn(RxApp.MainThreadScheduler)
						 .BindTo(this, view => view.LiveItemValueTextBlock.Text)
						 .DisposeWith(d);

				ViewModel.StatusUpdates
						 .Select(i => i.Stabilizer.Difference)
						 .Select(t => $"{t} / {targetThreshold}")
						 .ObserveOn(RxApp.MainThreadScheduler)
						 .BindTo(this, view => view.DifferenceTextBlock.Text)
						 .DisposeWith(d);

				ViewModel.StatusUpdates
						 .Select(i => i.Stabilizer.Average.ToString(CultureInfo.InvariantCulture))
						 .ObserveOn(RxApp.MainThreadScheduler)
						 .BindTo(this, view => view.AverageTextBlock.Text)
						 .DisposeWith(d);

				//temperatureLive.StatusUpdates
				//    .Select(i => i.Stabilizer.Progress().ToString(CultureInfo.InvariantCulture))
				//    .ObserveOn(RxApp.MainThreadScheduler)
				//    .BindTo(this, view => view.TemperatureStableProgressBar.Value)
				//    .DisposeWith(d);

			});
		}
	}
}
