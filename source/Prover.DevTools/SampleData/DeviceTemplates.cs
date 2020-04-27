using Devices.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.UI.Desktop.Dialogs;
using Prover.UI.Desktop.ViewModels.Verifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Prover.DevTools.SampleData
{
	public class DeviceTemplates : DialogViewModel, IDevToolsMenuItem
	{
		public static readonly List<DeviceInstance> Devices = new List<DeviceInstance>();

		private static readonly string _folderPath = ".\\SampleData\\Devices";

		static DeviceTemplates()
		{
			//Task.Run(Initialize);
			foreach (var file in Directory.EnumerateFiles(_folderPath))
				Devices.Add(JsonConvert.DeserializeObject<DeviceInstance>(File.ReadAllText(file)));
		}

		public DeviceTemplates(IScreenManager screenManager, IServiceProvider provider)
		{
			var loadFile = ReactiveCommand.CreateFromTask(async () =>
			{
				await screenManager.DialogManager.ShowViewModel(this);

				if (Result == DialogResult.Accepted)
				{
					var model = SelectedDeviceInstance.NewVerification();

					var testManager = ActivatorUtilities.CreateInstance<ManualTestManager>(provider, model.ToViewModel());

					await screenManager.ChangeView(testManager);
				}
			});
			Command = loadFile;
		}

		public ICollection<DeviceInstance> DevicesCollection { get; } = Devices.ToList();
		[Reactive] public DeviceInstance SelectedDeviceInstance { get; set; }

		/// <inheritdoc />
		public string Description { get; set; } = "Load test from template...";

		/// <inheritdoc />
		public ICommand Command { get; set; }
	}
}