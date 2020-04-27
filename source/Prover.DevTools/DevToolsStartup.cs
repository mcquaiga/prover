using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interactions;
using Prover.Application.ViewModels;
using Prover.DevTools.Hardware;
using Prover.DevTools.Importer;
using Prover.DevTools.SampleData;
using Prover.DevTools.Storage;
using Prover.Shared;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.Startup;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prover.DevTools
{
	public class DevToolsStartup : IConfigureModule
	{
		/// <inheritdoc />
		public void Configure(HostBuilderContext builder, IServiceCollection services)
		{
			services.AddSingleton<Func<PulseOutputChannel, IInputChannel>>(c => channel => SimulatedInputChannel.PulseInputSimulators[channel]);
			services.AddSingleton<Func<OutputChannelType, IOutputChannel>>(c => channel => SimulatedOutputChannel.OutputSimulators[channel]);

			services.AddViewsAndViewModels();

			services.AddSingleton<IDevToolsMenuItem, DataGenerator>();
			services.AddSingleton<IDevToolsMenuItem, DataImporter>();
			services.AddSingleton<IDevToolsMenuItem, DeviceTemplates>();

			services.AddSingleton<IToolbarItem, DevToolbarMenu>();

			//Task.Run(async () => await DeviceTemplates.Initialize());
		}

	}


	public class DevToolbarMenu : ViewModelBase, IToolbarItem
	{
		public DevToolbarMenu(IEnumerable<IDevToolsMenuItem> devMenuItems = null)
		{
			MenuItems = devMenuItems.ToList();
		}

		[Reactive] public ICollection<IDevToolsMenuItem> MenuItems { get; set; }

		/// <inheritdoc />
		public int SortOrder { get; } = 1;
	}
}
