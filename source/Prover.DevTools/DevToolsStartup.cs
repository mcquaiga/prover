using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interactions;
using Prover.DevTools.Hardware;
using Prover.DevTools.Importer;
using Prover.DevTools.SampleData;
using Prover.DevTools.Storage;
using Prover.Shared;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Extensions;
using System;

namespace Prover.DevTools
{
    public class DevToolsStartup : IConfigureModule
    {
        /// <inheritdoc />
        private void AddServices(HostBuilderContext builder, IServiceCollection services)
        {
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
        public void ConfigureServices(HostBuilderContext builder, IServiceCollection services)
        {
            AddServices(builder, services);
        }

        /// <inheritdoc />

        /// <inheritdoc />
        public void ConfigureAppConfiguration(HostBuilderContext builder, IConfigurationBuilder config)
        {
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
