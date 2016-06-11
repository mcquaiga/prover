using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.GUI.Screens.VerificationTest;
using Prover.GUI.ViewModels;

namespace Prover.GUI.Screens.Shell
{
    public class MainMenuViewModel : ReactiveScreen
    {
        private readonly IUnityContainer _container;

        public MainMenuViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public async Task NewTestButton()
        {
            await ScreenManager.Change(_container, new StartTestViewModel(_container));
        }

        public async Task ExportRunButton()
        {
            await ScreenManager.Change(_container, new CreateCertificateViewModel(_container));
        }

        public async Task RawInstrumentAccessButton()
        {
            await ScreenManager.Change(_container, new InstrumentAccessViewModel(_container));
        }

        //public void ConnectToInstrument()
        //{
        //    ProgressDialogView.Execute(_container, "Connecting to Instrument...",
        //    (bw) =>
        //    {
        //        var commPort = Communications.CreateCommPortObject(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);
        //        var instrComm = new InstrumentCommunicator(_container.Resolve<IEventAggregator>(), commPort, InstrumentType.MiniMax);
        //        Task.Run(() => instrComm.Connect());
        //    });
        //}
    }
}
