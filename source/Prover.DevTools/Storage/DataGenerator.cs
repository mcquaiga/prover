using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Devices.Core.Interfaces;
using DynamicData.Kernel;
using Microsoft.Extensions.Logging;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using Prover.Modules.DevTools.SampleData;
using Prover.Shared.Storage.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.DevTools.Storage
{
	internal class DataGenerator : ViewModelBase, IDevToolsMenuItem
	{
		private readonly IAsyncRepository<EvcVerificationTest> _repository;
		private CompositeDisposable _cleanup;

		public DataGenerator(IAsyncRepository<EvcVerificationTest> repository)
		{
			_repository = repository;
			Command = ReactiveCommand.Create(StartStopGenerator);
		}

		/// <inheritdoc />
		[Reactive] public string Description { get; set; } = "Start data generator";

		/// <inheritdoc />
		[Reactive] public ICommand Command { get; set; }

		private void StartStopGenerator()
		{
			if (_cleanup == null || _cleanup.IsDisposed)
			{
				_cleanup = new CompositeDisposable();
				StartGenerator().DisposeWith(_cleanup);
				Description = "Stop data generator";
				return;
			}

			if (_cleanup != null && !_cleanup.IsDisposed)
			{
				StopGenerator();
				Description = "Start data generator";
				return;
			}
		}

		private void StopGenerator()
		{
			_cleanup?.Dispose();
			Logger.LogDebug("Stopped data generator...");
		}

		private IDisposable StartGenerator()
		{
			Logger.LogDebug("Starting data generator...");

			var numOfDeviceSamples = DeviceTemplates.Devices.Count;
			var random = new Random(DateTime.Now.Millisecond * DateTime.UtcNow.Hour);

			return ThreadPoolScheduler.Instance.ScheduleRecurringAction(TimeSpan.FromSeconds(random.Next(0, 5)), async () =>
			{
				var test = CreateVerification();
				await _repository.UpsertAsync(test);

				Logger.LogDebug($" Generated verification with test date: {test.TestDateTime:g}");

				await VerificationEvents.OnSave.Publish(test);
			});

			EvcVerificationTest CreateVerification()
			{
				var device = GetRandomDevice();

				var test = device.NewVerification();
				SetTestDateTimes(test);

				return test;
			}


			DeviceInstance GetRandomDevice() => DeviceTemplates.Devices[random.Next(0, numOfDeviceSamples - 1)];

			void SetTestDateTimes(EvcVerificationTest test)
			{
				test.TestDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(0, 30)));
				test.SubmittedDateTime = test.TestDateTime.AddSeconds(random.Next(300, 720));
			}


		}
	}
}