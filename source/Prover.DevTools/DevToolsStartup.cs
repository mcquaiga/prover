using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interactions;
using Prover.Modules.DevTools.Hardware;
using Prover.Modules.DevTools.Importer;
using Prover.Modules.DevTools.SampleData;
using Prover.Modules.DevTools.Storage;
using Prover.Shared;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Extensions;
using System;
using Prover.Application.Interfaces;
using Prover.UI.Desktop.Controls;

namespace Prover.Modules.DevTools {
	public class DevToolsBootstrapper : IConfigureModule {
		/// <inheritdoc />
		private void AddServices(HostBuilderContext builder, IServiceCollection services) {
			services.AddSingleton<Func<PulseOutputChannel, IInputChannel>>(c => channel => SimulatedInputChannel.PulseInputSimulators[channel]);
			services.AddSingleton<Func<OutputChannelType, IOutputChannel>>(c => channel => SimulatedOutputChannel.OutputSimulators[channel]);
			services.AddViewsAndViewModels();
			services.AddSingleton<IDevToolsMenuItem, DataGenerator>();
			services.AddSingleton<IDevToolsMenuItem, DataImporter>();
			services.AddSingleton<IDevToolsMenuItem, DeviceTemplates>();
			services.AddSingleton<IToolbarItem, DevToolbarMenu>();

			//DevelopmentServices(services);
			//Task.Run(async () => await DeviceTemplates.Initialize());
		}

		//private void DevelopmentServices(IServiceCollection services)
		//{
		//    services.AddSingleton<DevelopmentWebService>();
		//    services.AddSingleton<IUserService<EmployeeDTO>>(c => c.GetRequiredService<DevelopmentWebService>());
		//    services.AddSingleton<IMeterService<MeterDTO>>(c => c.GetRequiredService<DevelopmentWebService>());
		//    services.AddSingleton<IExportService<QARunEvcTestResult>>(c => c.GetRequiredService<DevelopmentWebService>());
		//}

		/// <inheritdoc />
		public void ConfigureServices(HostBuilderContext builder, IServiceCollection services) {
			//if (builder.HostingEnvironment.IsDevelopment())
			AddServices(builder, services);
		}

		/// <inheritdoc />

		/// <inheritdoc />
		public void ConfigureAppConfiguration(HostBuilderContext builder, IConfigurationBuilder config) {
			//if (builder.HostingEnvironment.IsDevelopment())
			config.AddJsonFile("appsettings.DevTools.json");
		}
	}
}

/*
 *   <StackPanel Orientation="Vertical">
                <materialDesign:PopupBox HorizontalAlignment="Right" DockPanel.Dock="Right">

                    <StackPanel>

                  
                        <Button
                            Margin="15"
                            HorizontalAlignment="Right"
                            VerticalAlignment="Bottom"
                            Command="{Binding StartRotarySmokeTestCommand}"
                            Content="Run Rotary Smoke Test"
                            FontSize="14"
                            Style="{StaticResource MaterialDesignFlatButton}" />
                    </StackPanel>
                </materialDesign:PopupBox>
                <TextBlock
                    x:Name="FilePathTextBlock"
                    Height="Auto"
                    Style="{StaticResource MaterialDesignBody1TextBlock}" />
            </StackPanel>
 * 
 * 
 * */
